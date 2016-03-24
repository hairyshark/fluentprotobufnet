namespace TradeReaderService
{
    using System;

    /// <summary>
    /// The TradeReader interface.
    /// </summary>
    public interface ITradeReader : IRepository<int>
    {
    }

    /// <summary>
    /// The TradeReader interface.
    /// </summary>
    public interface IVersionedTradeReader : IVersionedRepository<Guid, int>
    {
    }
}