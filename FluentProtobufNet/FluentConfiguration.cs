namespace FluentProtobufNet
{
    using System;
    using System.Collections.Generic;

    using ProtoBuf.Meta;

    public class FluentConfiguration
    {
        private readonly Configuration cfg;

        private readonly IDiagnosticLogger logger;

        readonly List<Action<MappingConfiguration>> _mappingsBuilders = new List<Action<MappingConfiguration>>();

        private RuntimeTypeModel runtimeTypeModel;

        private Func<object, int> indexor;

        internal FluentConfiguration()
            : this(new Configuration())
        {
        }

        internal FluentConfiguration(Configuration cfg)
        {
            this.cfg = cfg;
            this.logger = new NullDiagnosticsLogger();
        }

        internal Configuration Configuration
        {
            get
            {
                return this.cfg;
            }
        }

        public Configuration BuildConfiguration()
        {
            var mappingCfg = new MappingConfiguration(this.runtimeTypeModel, this.indexor);

            foreach (var builder in this._mappingsBuilders)
            {
                builder(mappingCfg);
            }

            mappingCfg.Apply(this.Configuration);

            return this.Configuration;
        }

        public FluentConfiguration Mappings(Action<MappingConfiguration> mappings)
        {
            this._mappingsBuilders.Add(mappings);
            return this;
        }

        public FluentConfiguration WithIndexor(Func<object, int> indexor)
        {
            this.indexor = indexor;
            return this;
        }

        public FluentConfiguration WithModel(RuntimeTypeModel model)
        {
            this.runtimeTypeModel = model;
            return this;
        }
    }
}