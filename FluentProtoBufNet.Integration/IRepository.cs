namespace TradeReaderService
{
    public interface IRepository<in TKey>
    {
        TType Load<TType>(TKey id) where TType : class;
    }
}