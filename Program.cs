var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpLogging(); //增加日志记录

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Use(async (context, next) =>
            {
                await next();                
                Console.WriteLine("PATH:" + context.Request.Path);
                if (context.Response.StatusCode == 404)
                {    
                    Console.WriteLine("PATH:" + context.Request.Path);                  
                    await next();
                }
            });

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
