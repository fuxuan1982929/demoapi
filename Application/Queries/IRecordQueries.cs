namespace demoapi.Application.Queries;
public interface IRecordQueries
{
    Task<Record> GetRecordAsync(int id);

    // Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(Guid userId);

    // Task<IEnumerable<CardType>> GetCardTypesAsync();
}
