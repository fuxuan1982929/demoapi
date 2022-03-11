using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using demoapi.DAL.Accounting;
using demoapi.Infrastructure;
using demoapi.MQ;
using demoapi.RedisClient;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace demoapi;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public virtual IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services
        //.AddApplicationInsights(Configuration)
        .AddCustomMvc(Configuration)
        .AddHealthChecks(Configuration)
        //.AddCustomDbContext(Configuration)
        .AddCustomSwagger(Configuration)
        //.AddCustomIntegrations(Configuration)
        //.AddCustomConfiguration(Configuration)
        //.AddEventBus(Configuration)
        //.AddCustomAuthentication(Configuration);
        .AddSingleton<IHostedService, MyJobService>()
        .AddHostedService<RabbitListenerService>()
        // configure DI for DB
        .AddDbContext<accountingContext>(options =>
        {
            //设定DB
            options.UseMySql(Configuration.GetConnectionString("MySqlConnStr"), new MySqlServerVersion(new Version(5, 7, 27)));
        });

        //configure autofac
        var builder = new ContainerBuilder();

        builder.Populate(services);

        builder.RegisterModule(new MediatorModule());
        builder.RegisterModule(new ApplicationModule(Configuration.GetConnectionString("MySqlConnStr")));

        return new AutofacServiceProvider(builder.Build());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
    {
        //loggerFactory.AddAzureWebAppDiagnostics();
        //loggerFactory.AddApplicationInsights(app.ApplicationServices, LogLevel.Trace);

        var envDesc = Configuration["ENV"];
        var pathBase = Configuration["PATH_BASE"];

        loggerFactory.CreateLogger<Startup>().LogInformation(envDesc);
        loggerFactory.CreateLogger<Startup>().LogInformation("Using PATH BASE '{pathBase}'", pathBase);

        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }

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
                #endif
                };
            });
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"{pathBase}/swagger/v1/swagger.json", "v1");
            //若为string.Empty,则为根目录
            options.RoutePrefix = string.Empty;
        });

        app.UseRouting();
        app.UseCors("CorsPolicy");
        ConfigureAuth(app);

        app.UseEndpoints(endpoints =>
        {
            //endpoints.MapGrpcService<OrderingService>();
            //app.MapDefaultControllerRoute() 替代 app.MapControllerRoute(name: "default",pattern: "{controller=Home}/{action=Index}/{id?}");
            endpoints.MapDefaultControllerRoute();
            endpoints.MapControllers(); //绑定所有控制器

            // endpoints.MapGet("/_proto/", async ctx =>
            // {
            //     ctx.Response.ContentType = "text/plain";
            //     using var fs = new FileStream(Path.Combine(env.ContentRootPath, "Proto", "basket.proto"), FileMode.Open, FileAccess.Read);
            //     using var sr = new StreamReader(fs);
            //     while (!sr.EndOfStream)
            //     {
            //         var line = await sr.ReadLineAsync();
            //         if (line != "/* >>" || line != "<< */")
            //         {
            //             await ctx.Response.WriteAsync(line);
            //         }
            //     }
            // });

            endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
            {
                Predicate = r => r.Name.Contains("self")
            });
        });

        ConfigureEventBus(app);
    }

    private void ConfigureEventBus(IApplicationBuilder app)
    {
        //var eventBus = app.ApplicationServices.GetRequiredService<BuildingBlocks.EventBus.Abstractions.IEventBus>();

        // eventBus.Subscribe<UserCheckoutAcceptedIntegrationEvent, IIntegrationEventHandler<UserCheckoutAcceptedIntegrationEvent>>();
        // eventBus.Subscribe<GracePeriodConfirmedIntegrationEvent, IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>>();
        // eventBus.Subscribe<OrderStockConfirmedIntegrationEvent, IIntegrationEventHandler<OrderStockConfirmedIntegrationEvent>>();
        // eventBus.Subscribe<OrderStockRejectedIntegrationEvent, IIntegrationEventHandler<OrderStockRejectedIntegrationEvent>>();
        // eventBus.Subscribe<OrderPaymentFailedIntegrationEvent, IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>>();
        // eventBus.Subscribe<OrderPaymentSucceededIntegrationEvent, IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>>();
    }

    protected virtual void ConfigureAuth(IApplicationBuilder app)
    {
        //app.UseAuthentication();
        app.UseAuthorization();
    }
}

static class CustomExtensionsMethods
{
    public static IServiceCollection AddApplicationInsights(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddApplicationInsightsTelemetry(configuration);
        // services.AddApplicationInsightsKubernetesEnricher();
        return services;
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration)
    {
        // Add framework services.

        services.AddControllers(options =>
            {
                //options.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            // Added for functional tests
            //.AddApplicationPart(typeof(demoapi.Controllers.WeatherForecastController).Assembly)
            //.AddApplicationPart(typeof(demoapi.Controllers.MQTestController).Assembly)
            // Register your Web API controllers.           
            .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

        //Add redis cache
        string RedisConnStr = configuration.GetConnectionString("RedisConnStr");
        services.AddRedisClient(RedisConnStr);
        //add rabbitMQ
        services.Configure<RabbitMQConfig>(configuration.GetSection("RabbitMQConfig"));
        services.AddScoped<RabbitMQHelper>();

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                .SetIsOriginAllowed((host) => true)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });

        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

        // hcBuilder
        //     .AddSqlServer(
        //         configuration["ConnectionString"],
        //         name: "OrderingDB-check",
        //         tags: new string[] { "orderingdb" });

        // if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
        // {
        //     hcBuilder
        //         .AddAzureServiceBusTopic(
        //             configuration["EventBusConnection"],
        //             topicName: "eshop_event_bus",
        //             name: "ordering-servicebus-check",
        //             tags: new string[] { "servicebus" });
        // }
        // else
        // {
        //     hcBuilder
        //         .AddRabbitMQ(
        //             $"amqp://{configuration["EventBusConnection"]}",
        //             name: "ordering-rabbitmqbus-check",
        //             tags: new string[] { "rabbitmqbus" });
        // }

        return services;
    }

    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
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

        return services;
    }

    // public static IServiceCollection AddCustomIntegrations(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    //     services.AddTransient<IIdentityService, IdentityService>();
    //     services.AddTransient<Func<DbConnection, IIntegrationEventLogService>>(
    //         sp => (DbConnection c) => new IntegrationEventLogService(c));

    //     services.AddTransient<IOrderingIntegrationEventService, OrderingIntegrationEventService>();

    //     if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
    //     {
    //         services.AddSingleton<IServiceBusPersisterConnection>(sp =>
    //         {
    //             var serviceBusConnectionString = configuration["EventBusConnection"];

    //             var subscriptionClientName = configuration["SubscriptionClientName"];

    //             return new DefaultServiceBusPersisterConnection(serviceBusConnectionString);
    //         });
    //     }
    //     else
    //     {
    //         services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
    //         {
    //             var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();


    //             var factory = new ConnectionFactory()
    //             {
    //                 HostName = configuration["EventBusConnection"],
    //                 DispatchConsumersAsync = true
    //             };

    //             if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
    //             {
    //                 factory.UserName = configuration["EventBusUserName"];
    //             }

    //             if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
    //             {
    //                 factory.Password = configuration["EventBusPassword"];
    //             }

    //             var retryCount = 5;
    //             if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
    //             {
    //                 retryCount = int.Parse(configuration["EventBusRetryCount"]);
    //             }

    //             return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
    //         });
    //     }

    //     return services;
    // }

    // public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddOptions();
    //     services.Configure<OrderingSettings>(configuration);
    //     services.Configure<ApiBehaviorOptions>(options =>
    //     {
    //         options.InvalidModelStateResponseFactory = context =>
    //         {
    //             var problemDetails = new ValidationProblemDetails(context.ModelState)
    //             {
    //                 Instance = context.HttpContext.Request.Path,
    //                 Status = StatusCodes.Status400BadRequest,
    //                 Detail = "Please refer to the errors property for additional details."
    //             };

    //             return new BadRequestObjectResult(problemDetails)
    //             {
    //                 ContentTypes = { "application/problem+json", "application/problem+xml" }
    //             };
    //         };
    //     });

    //     return services;
    // }

    // public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    // {
    //     if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
    //     {
    //         services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
    //         {
    //             var serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
    //             var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
    //             var logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
    //             var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();
    //             string subscriptionName = configuration["SubscriptionClientName"];

    //             return new EventBusServiceBus(serviceBusPersisterConnection, logger,
    //                 eventBusSubcriptionsManager, iLifetimeScope, subscriptionName);
    //         });
    //     }
    //     else
    //     {
    //         services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
    //         {
    //             var subscriptionClientName = configuration["SubscriptionClientName"];
    //             var rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
    //             var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
    //             var logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
    //             var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

    //             var retryCount = 5;
    //             if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
    //             {
    //                 retryCount = int.Parse(configuration["EventBusRetryCount"]);
    //             }

    //             return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
    //         });
    //     }

    //     services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

    //     return services;
    // }



    // public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    // {
    //     // prevent from mapping "sub" claim to nameidentifier.
    //     JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

    //     var identityUrl = configuration.GetValue<string>("IdentityUrl");

    //     services.AddAuthentication(options =>
    //     {
    //         options.DefaultAuthenticateScheme = AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    //         options.DefaultChallengeScheme = AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;

    //     }).AddJwtBearer(options =>
    //     {
    //         options.Authority = identityUrl;
    //         options.RequireHttpsMetadata = false;
    //         options.Audience = "orders";
    //     });

    //     return services;
    // }
}

public class MyJobService : BackgroundService
{
    //启动5秒一次
    private int _timeoffset = 5000;

    public ILogger<MyJobService> _logger;
    public IConfiguration _configuration;

    public MyJobService(ILogger<MyJobService> logger, IConfiguration configuration)
    {
        this._logger = logger;
        this._configuration = configuration;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(_timeoffset, stoppingToken);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                //this._logger.LogInformation(DateTime.Now.ToString() + " MyJob。");
                //do
                await Task.Delay(this._timeoffset, stoppingToken);
            }
            this._logger.LogInformation(DateTime.Now.ToString() + "MyJob停止。");
        }
        catch (Exception ex)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                this._logger.LogInformation(DateTime.Now.ToString() + "MyJob异常:" + ex.Message + "," + ex.StackTrace);
            }
            else
            {
                this._logger.LogInformation(DateTime.Now.ToString() + "MyJob异常停止。");
            }
        }
    }
}

public class RabbitListenerService : BackgroundService
{
    private readonly ILogger<RabbitListenerService> _logger;
    private readonly RabbitMQHelper _MQHelper;

    public RabbitListenerService(ILogger<RabbitListenerService> logger, RabbitMQHelper MQHelper)
    {
        this._logger = logger;
        _MQHelper = MQHelper;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RabbitMQ消费者开始监听...");
        Register();
        return Task.CompletedTask;
    }

    public void Register()
    {
        _MQHelper.ReceiveNew("queue1", ProcessNew);
    }

    public bool ProcessNew(string message)
    {
        try
        {
            //接受message， 处理消息
            _logger.LogInformation("RabbitMQ > ProcessNew > message:" + message);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }
}

/*
EF生产ORM代码部分
https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql

##以下是安装最新版
dotnet tool install --global dotnet-ef

##以下是安装指定版
dotnet tool install --global dotnet-ef --version 3.1.1

##没有装的话，必须安装Design
dotnet add package Microsoft.EntityFrameworkCore.Design

##生产代码指令
dotnet ef dbcontext scaffold "server=ng.talkofice.com;port=30466;uid=root;pwd=Magic707Nicol^^();database=accounting" 
"Pomelo.EntityFrameworkCore.MySql" 
-o DAL/Accounting/Models 
--context-dir DAL/Accounting
-f

*/


//dotnet run --launch-profile "dev"
//dotnet run --launch-profile "prd"