using google.protobuf;
using ProtoBuf.CodeGenerator;

namespace FluentProtobufNet.Extraction
{
    using System;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.Xsl;

    using Configuration;
    using Mapping;
    using Sources;
    using ProtoBuf;
    using ProtoBuf.Meta;

    public class ModelBuilder<TAssemblyType, TTypeSource>
        where TAssemblyType : class
        where TTypeSource : ITypeSource
    {
        private bool built;

        private Configuration config;

        private IEnumerable<ModelDescriptor> descriptors;

        private RuntimeTypeModel model;

        public IEnumerable<ModelDescriptor> Descriptors
        {
            get
            {
                this.PreProcess();

                return this.descriptors;
            }
        }

        public string CodeGenSchema(string schema, string template, string defaultNamespace)
        {
            return ApplyTransform(schema, template, defaultNamespace);
        }

        public CompilerResults Compile<T>(T compiler, string code, string version, string keyFile = null, params string[] extraReferences)
            where T : CodeDomProvider
        {
            if (compiler == null)
            {
                compiler = (T)Activator.CreateInstance(typeof(T));
            }

            var path = Path.GetTempFileName();
            
            var refs = new List<string>
                                        {
                                            typeof(Uri).Assembly.Location, 
                                            typeof(XmlDocument).Assembly.Location, 
                                            typeof(Serializer).Assembly.Location,
                                            typeof(AssemblyVersionAttribute).Assembly.Location
                                        };

            if (extraReferences != null && extraReferences.Length > 0)
            {
                refs.AddRange(extraReferences);
            }

            var args = new CompilerParameters(refs.ToArray(), path, false);

            var sb = new StringBuilder();

            sb.Append(@"using System.Reflection;");
            sb.Append(string.Format(@"[assembly: AssemblyVersion(""{0}"")]", version));
            sb.Append(string.Format(@"[assembly: AssemblyFileVersion(""{0}"")]", version));

            if (!string.IsNullOrEmpty(keyFile))
            {
                sb.Append(string.Format(@"[assembly: AssemblyKeyFile(""{0}"")]", keyFile));
            }

            var assemblyFile = Path.GetTempFileName();
            File.WriteAllText(assemblyFile, sb.ToString());

            var codePath = Path.GetTempFileName();
            File.WriteAllText(codePath, code);

            string[] files = { assemblyFile, codePath };

            var results = compiler.CompileAssemblyFromFile(args, files);

            ShowErrors(results.Errors);

            if (results.Errors.Count <= 0)
            {
                return results;
            }

            foreach (CompilerError err in results.Errors)
            {
                Console.Error.WriteLine(err);
            }

            throw new InvalidOperationException(
                string.Format("{0} found {1} code errors errors", typeof(T).Name, results.Errors.Count));
        }

        public string ExportProto(string protoPath)
        {
            this.PreProcess();

            var proto = this.model.GetSchema(typeof(TAssemblyType));

            File.Delete(protoPath);

            File.WriteAllText(protoPath, proto);

            return proto;
        }

        public string ExportSchema(string outPath, params string[] dependencies)
        {
            this.PreProcess();

            this.ExportProto(outPath);

            var fileDescriptorSet = new FileDescriptorSet();

            var set = MergeProtoFile(outPath, fileDescriptorSet);

            set = dependencies.Aggregate(set, (current, dependency) => MergeProtoFile(dependency, current));

            var xser = new XmlSerializer(typeof(FileDescriptorSet));

            var settings = new XmlWriterSettings
                               {
                                   Indent = true,
                                   IndentChars = "  ",
                                   NewLineHandling = NewLineHandling.Entitize
                               };

            var sb = new StringBuilder();

            using (var writer = XmlWriter.Create(sb, settings))
            {
                xser.Serialize(writer, set);
            }

            var schema = sb.ToString();

            File.Delete(outPath + ".schema");

            File.WriteAllText(outPath + ".schema", schema);

            return schema;
        }

        public string GetIndex(string outPath, string defaultNamespace, params string[] dependencies)
        {
            this.PreProcess();

            this.ExportProto(outPath);

            var fileDescriptorSet = new FileDescriptorSet();

            var set = MergeProtoFile(outPath + ".proto", fileDescriptorSet);

            set = dependencies.Aggregate(set, (current, dependency) => MergeProtoFile(dependency, current));

            var cache = new List<Tuple<string, string, int, string>>();

            foreach (var descriptorProto in set.file.Select(file => file.message_type).SelectMany(types => types))
            {
                cache.AddRange(
                    descriptorProto.field.Select(
                        fieldDescriptorProto =>
                        new Tuple<string, string, int, string>(
                            descriptorProto.name,
                            fieldDescriptorProto.name,
                            fieldDescriptorProto.number,
                            fieldDescriptorProto.type.ToString())));
            }

            var sb = new StringBuilder();

            sb.AppendLine("// <auto-generated>");
            sb.AppendLine("//     This code was generated by a tool.");
            sb.AppendLine("//");
            sb.AppendLine("//     Changes to this file may cause incorrect behavior and will be lost if");
            sb.AppendLine("//     the code is regenerated.");
            sb.AppendLine("// </auto-generated>");

            sb.AppendLine(string.Format("namespace {0}", defaultNamespace));
            sb.AppendLine("{");
            sb.AppendLine("\tusing System;");
            sb.AppendLine("\tusing System.Collections.Generic;");
            sb.AppendLine(string.Empty);
            sb.AppendLine("\tpublic class Index");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tprivate readonly IDictionary<Type, List<Tuple<string, int>>> cache = new Dictionary<Type, List<Tuple<string, int>>>();");
            sb.AppendLine(string.Empty);
            sb.AppendLine("\t\tpublic Index()");
            sb.AppendLine("\t\t{");

            foreach (var groupBy in cache.GroupBy(c => c.Item1))
            {
                sb.AppendLine(string.Format("\t\t\tthis.cache.Add(typeof({0}), new List<Tuple<string, int>>());", groupBy.Key));
                
                sb.AppendLine(string.Empty);
                
                foreach (var tuple in groupBy)
                {
                    sb.Append("\t\t\t");
                    sb.Append(string.Format(@"this.cache[typeof({0})].Add(new Tuple<string, int>(""{1}"", {2}));", groupBy.Key, tuple.Item2, tuple.Item3));
                    sb.AppendLine();
                }

                sb.AppendLine(string.Empty);
            }   

            sb.AppendLine("\t\t}");
            sb.Append("\t}\n");
            sb.Append("}\n");

            return sb.ToString();
        }

        public ModelDescriptor Find(Type typeSource)
        {
            this.PreProcess();

            return this.Descriptors.FirstOrDefault(modelDescriptor => modelDescriptor.MetaType.Type == typeSource);
        }

        private static string ApplyTransform(string xml, string template, string defaultNamespace)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.CheckCharacters = false;

            StringBuilder sb = new StringBuilder();
            using (XmlReader reader = XmlReader.Create(new StringReader(xml)))
            using (TextWriter writer = new StringWriter(sb))
            {
                var xslt = new XslCompiledTransform();

                var xsltTemplate = Path.ChangeExtension(template, "xslt");

                if (!File.Exists(xsltTemplate))
                {
                    var localXslt = InputFileLoader.CombinePathFromAppRoot(xsltTemplate);
                    if (File.Exists(localXslt))
                    {
                        xsltTemplate = localXslt;
                    }
                }

                try
                {
                    xslt.Load(xsltTemplate);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Unable to load tranform: " + "FOO", ex);
                }

                var xsltOptions = new XsltArgumentList();

                xsltOptions.AddParam("detectMissing", string.Empty, "true");
                xsltOptions.RemoveParam("defaultNamespace", string.Empty);

                if (defaultNamespace != null)
                {
                    xsltOptions.AddParam("defaultNamespace", string.Empty, defaultNamespace);
                }

                xslt.Transform(reader, xsltOptions, writer);
            }

            return sb.ToString();
        }

        private static FileDescriptorSet MergeProtoFile(string outPath, FileDescriptorSet set)
        {
            var errors = new StringBuilder();

            var errorWriter = new StringWriter(errors);

            InputFileLoader.Merge(set, outPath, errorWriter);

            return set;
        }

        private static void ShowErrors(ICollection errors)
        {
            if (errors.Count <= 0)
            {
                return;
            }

            foreach (CompilerError err in errors)
            {
                Console.Error.Write(err.IsWarning ? "Warning: " : "Error: ");
                Console.Error.WriteLine(err.ErrorText);
            }
        }

        private void Build()
        {
            this.model = TypeModel.Create();

            this.config =
                Fluently.Configure()
                    .WithModel(this.model)
                    .WithIndexor(SeededIndexor.GetIndex)
                    .Mappings(m => { m.FluentMappings.AddFromAssemblySource<TAssemblyType, TTypeSource>(); })
                    .BuildConfiguration();

            this.descriptors =
                this.config.RuntimeTypeModel.GetTypes()
                    .Cast<object>()
                    .Select(incomingType => new ModelDescriptor((MetaType)incomingType, incomingType.GetIndex()));

            this.built = true;
        }

        private void PreProcess()
        {
            if (this.built)
            {
                return;
            }
            this.Build();
        }
    }
}
