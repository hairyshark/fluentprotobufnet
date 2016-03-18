namespace FluentProtobufNet
{
    using System;

    using ProtoBuf.Meta;

    public class MappingConfiguration
    {
        private readonly PersistenceModel _model;

        public MappingConfiguration(RuntimeTypeModel runtimeTypeModel, Func<object, int> indexor)
        {
            this._model = new PersistenceModel(runtimeTypeModel ?? TypeModel.Create(), indexor);

            this.FluentMappings = new FluentMappingsContainer();
        }

        public FluentMappingsContainer FluentMappings { get; set; }

        public void Apply(Configuration cfg)
        {
            this.FluentMappings.Apply(this._model);

            this._model.Configure(cfg);
        }
    }
}