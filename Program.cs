using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "DEMO API",
        Description = "An ASP.NET Core Web API for managing ",
        TermsOfService = new Uri("https://api.talkofice.com/demoapi"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://api.talkofice.com/license")
        }
    });
});

var app = builder.Build();

app.UseHttpLogging(); //增加日志记录

string vPath = app.Configuration["virtualPath"];
app.UsePathBase(new PathString(vPath));
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            //根据访问地址，设置swagger服务路径
            swagger.Servers = new List<OpenApiServer> {
                new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{httpReq.Headers["X-Forwarded-Prefix"]}" },
                new OpenApiServer { Url = "https://api.talkofice.com/demoapi"}
            };
        });
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint($"{vPath}/swagger/v1/swagger.json", "v1");
        //若为string.Empty,则为根目录
        options.RoutePrefix = string.Empty; 
    });
//}

app.Use(async (context, next) =>
{
    await next();                
    Console.WriteLine("PATH:" + context.Request.Path);
    // if (context.Response.StatusCode == 404)
    // {    
    //     Console.WriteLine("PATH:" + context.Request.Path);                  
    //     await next();
    // }
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
