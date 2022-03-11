namespace demoapi.Application.Queries;

public record Record
{
    public string Id { get; init; } = "";
    public string UserId { get; init; } = "";
    public DateTime RecordDate { get; init; }
    public int AccountType { get; init; }
    public string TypeName {get; init;} = "";

}