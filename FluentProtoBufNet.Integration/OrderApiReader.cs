namespace TradeReaderService
{
    using Insight.Library.OTC.CoreBusinessObjects;
    using Insight.Library.OTC.DataMapper;

    /// <summary>
    /// The order api repository.
    /// </summary>
    public class OrderApiReader : ITradeReader
    {
        public TType Load<TType>(int id) where TType : class
        {
            var foo = new Execution(id);

            var dto = DataMapperFactory.Instance.DataMapper.MapExecution(foo);

            return dto as TType;
        }
    }
}