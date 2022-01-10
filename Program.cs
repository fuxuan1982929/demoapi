using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
    {
        options.AddPolicy("AnyOrigin", builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod();
        });
    }
);

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

string vPath = app.Configuration["virtualPath"];

app.UseHttpLogging(); //增加日志记录

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
#if DEBUG
                new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{httpReq.Headers["X-Forwarded-Prefix"]}" },
#else
                new OpenApiServer { Url = "https://api.talkofice.com/demoapi"}
#endif

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

    if (context.Response.StatusCode == 404)
    {
        //Console.WriteLine("404-PATH:" + context.Request.Headers);
        foreach(var i in context.Request.Headers)
        {           
            Console.WriteLine($"404-HEADER: KEY:{i.Key}, VALUE:{i.Value}");
        }
        
        Console.WriteLine("404-PATH:" + context.Request.Path);
        await next();
    }
    else
    {
        Console.WriteLine("PATH:" + context.Request.Path);
    }
});

app.UseHttpsRedirection();

// global cors policy
app.UseCors("AnyOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
