using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using demoapi.RedisClient;
using demoapi.MQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
    {
        options.AddPolicy("AnyOrigin", builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    }
);

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

    //Add Token
    options.AddSecurityDefinition("UserToken", new OpenApiSecurityScheme()
    {
        Description = "<a href='http://'>获取TOKEN</a>",
        Name = "UserToken",
        In = ParameterLocation.Header,
        Scheme = "string"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "UserToken"
            }
        }, new string[]{}
        }
    });
});

string RedisConnStr = builder.Configuration.GetConnectionString("RedisConnStr");
var RabbitMQConnSection = builder.Configuration.GetSection("RedisConnStr");
//Add redis cache
builder.Services.AddRedisClient(RedisConnStr);
//Add rabbitMQ 
builder.Services.Configure<RabbitMQConfig>(RabbitMQConnSection);
builder.Services.AddSingleton<RabbitMQHelper>();

var app = builder.Build();

string vPath = app.Configuration["virtualPath"];

// app.UseForwardedHeaders(new ForwardedHeadersOptions
// {
//     ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
// });

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
                new OpenApiServer { Url = "https://api.talkofice.com/demoapi"},
                new OpenApiServer { Url = "http://api.talkofice.com:30080/demoapi"}
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

//第一层中间件
app.Use(async (context, next) =>
{
    // if (context.Response.StatusCode == 404)
    // {
    //     //Console.WriteLine("404-PATH:" + context.Request.Headers);
    //     Console.WriteLine("404-PATH:" + context.Request.Path);
    //     Console.WriteLine("404-ContentType" + context.Response.ContentType);
    // }
    // else
    // {
    //     Console.WriteLine("PATH:" + context.Request.Path);
    //     Console.WriteLine("Resp StatusCode:" + context.Response.StatusCode);
    // }
    // Call the next delegate/middleware in the pipeline
    await next();
});


// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

//app.UseHttpsRedirection();

// global cors policy
app.UseCors("AnyOrigin");

app.UseAuthorization();

//自定义中间件（异常处理）
//app.UseMiddleware<demoapi.Middleware.MyExceptionMiddleware>();

app.MapControllers();

app.Run();
