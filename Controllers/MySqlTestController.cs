using demoapi.Attributes;
using demoapi.DAL.Accounting.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace demoapi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class MySqlTestController : ControllerBase
{
    [MyApi]
    [HttpGet()]
    public async Task<List<TblRecord>> GetData()
    {
        using var dbContext = new DAL.Accounting.accountingContext();
        var qry = from m in dbContext.TblRecords.AsNoTracking()
                  select m;

        return await qry.ToListAsync();
    }

 }
