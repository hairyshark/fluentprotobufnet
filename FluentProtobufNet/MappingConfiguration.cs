
using ProtoBuf.Meta;

namespace FluentProtobufNet
{
    public class MappingConfiguration
    {
        readonly PersistenceModel _model;

        public MappingConfiguration(RuntimeTypeModel runtimeTypeModel)
        {
            _model = new PersistenceModel(runtimeTypeModel ?? TypeModel.Create());

            FluentMappings = new FluentMappingsContainer();
        }

        public FluentMappingsContainer FluentMappings { get; set; }

        public void Apply(Configuration cfg)
        {
            FluentMappings.Apply(_model);

            _model.Configure(cfg);

        }
    }
}