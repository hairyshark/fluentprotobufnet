namespace FluentProtobufNet.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using FluentProtobufNet.Extraction;
    using FluentProtobufNet.Mapping;
    using FluentProtobufNet.Sources;
    using FluentProtobufNet.Specification;
    using FluentProtobufNet.Tests.Basic;

    using Insight.Messaging.OTC.OrderAPI;
    using Insight.Messaging.OTC.OrderAPI.Data;
    using Insight.Messaging.OTC.OrderAPI.Data.Allocation;

    using Microsoft.CSharp;

    using NUnit.Framework;

    using ProtoBuf;

    [TestFixture]
    public class TestModelExtraction
    {
        private const string ProtoExtension = ".proto";

        [SetUp]
        public void Setup()
        {
            SeededIndexor.Reset();
        }

        private static string InternalTestCodeGen<TAssemblyType, TTypeSource>(
            ModelBuilder<TAssemblyType, TTypeSource> modelBuilder, 
            string outPath, 
            string template, 
            string defaultNameSpace,
            params string[] dependencies) where TAssemblyType : class where TTypeSource : ITypeSource
        {
            InternalTestBuildExtractProto(outPath + ProtoExtension, modelBuilder);

            var schema = modelBuilder.ExportSchema(outPath + ProtoExtension, dependencies);
            
            var code = modelBuilder.CodeGenSchema(schema, template, defaultNameSpace);

            Assert.IsNotNull(code);

            File.WriteAllText(outPath + ".cs", code);

            Console.WriteLine(code);

            return code;
        }

        private static void InternalTestBuildExtractProto<TAssemblyType, TTypeSource>(
            string ProtoPath, 
            ModelBuilder<TAssemblyType, TTypeSource> modelBuilder) where TAssemblyType : class
            where TTypeSource : ITypeSource
        {
            File.Delete(ProtoPath);

            Assert.IsFalse(File.Exists(ProtoPath));

            var schema = modelBuilder.ExportSchema(ProtoPath);

            Assert.IsTrue(File.Exists(ProtoPath));

            Assert.IsNotNull(schema);
        }

        [Test]
        public void TestBuildFileDescriptorsFromBasicTypes()
        {
            var modelBuilder = new ModelBuilder<Category, AssemblyTypeSource<NameSpaceSpecification<Category>>>();

            const string OutPath = @".\TestBuildFileDescriptorsFromBasicTypes";

            InternalTestBuildExtractProto(OutPath + ProtoExtension, modelBuilder);
        }

        [Test]
        public void TestBuildFileDescriptorsFromDataContractsInOrderApi()
        {
            var modelBuilder = new ModelBuilder<ExecutionNew, WcfAssemblyTypeSource<AnyDataContractSpecification>>();

            const string OutPath = @".\TestBuildFileDescriptorsFromDataContractsInOrderApi";

            InternalTestBuildExtractProto(OutPath + ProtoExtension, modelBuilder);
        }

        [Test]
        public void TestCodeGenSchemaAndCompileFromBasicTypes()
        {
            const string TemplateCSharp = "csharp";

            var modelBuilder = new ModelBuilder<Category, AssemblyTypeSource<NameSpaceSpecification<Category>>>();

            const string OutPath = @".\TestCodeGenSchemaFromBasicTypes";

            var code = InternalTestCodeGen(modelBuilder, OutPath, TemplateCSharp, "TestCodeGenSchemaFromBasicTypes", "bcl.proto");

            const string Version = "99.99.99.99";

            var results = modelBuilder.Compile<CSharpCodeProvider>(null, code: code, version: Version, keyFile: "testFluent.snk");

            Assert.IsNotNull(results);

            Assert.IsTrue(File.Exists(results.PathToAssembly));

            var assembly = Assembly.LoadFrom(results.PathToAssembly);

            Assert.AreEqual(Version, assembly.GetName().Version.ToString());

            Assert.IsTrue(assembly.IsFullyTrusted);

            File.Delete(OutPath + ".dll");

            File.Copy(results.PathToAssembly, OutPath + ".dll");
        }

        [Test]
        public void TestCodeGenSchemaAndCompileFromDataContractsInOrderApi()
        {
            const string TemplateCSharp = "csharp";

            var modelBuilder = new ModelBuilder<ExecutionNew, WcfAssemblyTypeSource<AnyDataContractSpecification>>();

            const string OutPath = @".\TestCodeGenSchemaFromDataContractsInOrderApi";

            var code = InternalTestCodeGen(modelBuilder, OutPath, TemplateCSharp, "TestCodeGenSchemaFromDataContractsInOrderApi", "bcl.proto");

            const string Version = "66.66.66.66";

            var results = modelBuilder.Compile<CSharpCodeProvider>(null, code: code, version: Version, keyFile: "testFluent.snk");

            Assert.IsNotNull(results);

            var assembly = Assembly.LoadFrom(results.PathToAssembly);

            Assert.AreEqual(Version, assembly.GetName().Version.ToString());

            File.Delete(OutPath + ".dll");

            File.Copy(results.PathToAssembly, OutPath + ".dll");
        }

        [Test]
        public void TestExportProtoFromBasicTypes()
        {
            var modelBuilder = new ModelBuilder<Category, AssemblyTypeSource<NameSpaceSpecification<Category>>>();

            const string OutPath = @".\TestExportProtoFromBasicTypes";

            var proto = modelBuilder.ExportProto(OutPath);

            Assert.IsNotNull(proto);

            Assert.IsTrue(File.Exists(OutPath + ProtoExtension));
        }

        [Test]
        public void TestExportSchemaFromBasicTypes()
        {
            var modelBuilder = new ModelBuilder<Category, AssemblyTypeSource<NameSpaceSpecification<Category>>>();

            const string OutPath = @".\TestExportProtoFromBasicTypes";

            var schema = modelBuilder.ExportSchema(OutPath + ProtoExtension, "bcl.proto");

            Assert.IsNotNull(schema);
        }

        [Test]
        public void TestExtractModelFromBasicTypes()
        {
            var modelBuilder = new ModelBuilder<Category, AssemblyTypeSource<NameSpaceSpecification<Category>>>();

            Assert.IsNotNull(modelBuilder.Descriptors);
            Assert.AreEqual(4, modelBuilder.Descriptors.Count());

            modelBuilder.EnsureExists(typeof(Category));
            modelBuilder.EnsureExists(typeof(CategoryWithDescription));
            modelBuilder.EnsureExists(typeof(CategoryThirdLevel));
            modelBuilder.EnsureExists(typeof(Item));
        }

        [Test]
        public void TestExtractModelFromDataContractsInOrderApi()
        {
            var modelBuilder = new ModelBuilder<ExecutionNew, WcfAssemblyTypeSource<AnyDataContractSpecification>>();

            Assert.IsNotNull(modelBuilder.Descriptors);
            Assert.AreEqual(154, modelBuilder.Descriptors.Count());

            modelBuilder.EnsureExists(typeof(Identifier));
            modelBuilder.EnsureExists(typeof(ExecutionCancel));
            modelBuilder.EnsureExists(typeof(ExecutionNew));
            modelBuilder.EnsureExists(typeof(ExecutionUnwind));
            modelBuilder.EnsureExists(typeof(AllocationBase));
            modelBuilder.EnsureExists(typeof(ContractAllocationBase));
            modelBuilder.EnsureExists(typeof(ContractUnwindAllocationBase));
            modelBuilder.EnsureExists(typeof(OrderBase));           // base for knowm types not decorated as a Datacontact
            modelBuilder.EnsureExists(typeof(OrderNew));            // subclass not decorated from OrderBase
            modelBuilder.EnsureExists(typeof(OrderUnwindSearch));   // subclass not decorated from OrderBase
            modelBuilder.EnsureExists(typeof(OrderCancel));         // subclass not decorated from OrderBase
            modelBuilder.EnsureExists(typeof(OrderAcknowledge));    // subclass not decorated from OrderBase
        }

        [Test]
        public void TestGenerateIndexFromBasicPropertyList()
        {
            var modelBuilder = new ModelBuilder<Category, AssemblyTypeSource<NameSpaceSpecification<Category>>>();

            const string OutPath = @".\TestGenerateIndexFromBasicPropertyList";

            var index = modelBuilder.GetIndex(OutPath, typeof(Category).Namespace + ".Index");

            Assert.IsNotNull(index);

            File.WriteAllText(OutPath + ".Index.cs", index);
        }

        [Test]
        public void TestGenerateIndexFromDataContractsInOrderApi()
        {
            var modelBuilder = new ModelBuilder<ExecutionNew, WcfAssemblyTypeSource<AnyDataContractSpecification>>();

            const string OutPath = @".\TestGenerateIndexFromDataContractsInOrderApi";

            var index = modelBuilder.GetIndex(OutPath, typeof(ExecutionNew).Namespace + ".Index");

            Assert.IsNotNull(index);

            File.WriteAllText(OutPath + ".Index.cs", index);
        }
    }
}
