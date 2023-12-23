using Data.Repositories;
using FarzamNews.Utilities;
using Microsoft.EntityFrameworkCore;
using NewsBot.Data.NewsContext;
using NewsBot.Entities;
using NewsBot.Services;
using NewsBot.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NewsContext>(options => 
    options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<INewsService,NewsService>();
 
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

var context = builder.Services.BuildServiceProvider().GetRequiredService<NewsContext>();
DataInitializer.Initialize(context,app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
