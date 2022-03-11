namespace demoapi.Application.Queries;

public class RecordQueries : IRecordQueries
{
    private string _connectionString = string.Empty;
    public RecordQueries(string conStr)
    {
        _connectionString = !string.IsNullOrWhiteSpace(conStr) ? conStr : throw new ArgumentNullException(nameof(conStr));
    }

    public Task<Record> GetRecordAsync(int id)
    {
        throw new Exception("test");
        //    using (var connection = new MySqlConnection(_connectionString))
        //     {
        //         connection.Open();

        //         var result = await connection.QueryAsync<dynamic>(
        //             @"select o.[Id] as ordernumber,o.OrderDate as date, o.Description as description,
        //                 o.Address_City as city, o.Address_Country as country, o.Address_State as state, o.Address_Street as street, o.Address_ZipCode as zipcode,
        //                 os.Name as status, 
        //                 oi.ProductName as productname, oi.Units as units, oi.UnitPrice as unitprice, oi.PictureUrl as pictureurl
        //                 FROM ordering.Orders o
        //                 LEFT JOIN ordering.Orderitems oi ON o.Id = oi.orderid 
        //                 LEFT JOIN ordering.orderstatus os on o.OrderStatusId = os.Id
        //                 WHERE o.Id=@id"
        //                 , new { id }
        //             );

        //         if (result.AsList().Count == 0)
        //             throw new KeyNotFoundException();

    }
}

