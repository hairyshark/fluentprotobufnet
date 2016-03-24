namespace TradeReaderService
{
    public interface IVersionedRepository<in TKey, in TVersion>
    {
        TType Load<TType>(TKey id, TVersion version) where TType : class;
    }
}