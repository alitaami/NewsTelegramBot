using AutoMapper;
using Data.Repositories;
using FarzamNews.Utilities;
using Microsoft.EntityFrameworkCore;
using NewsBot.Data.NewsContext;
using NewsBot.Entities;
using NewsBot.Mappings;
using NewsBot.Services;
using NewsBot.Services.Interfaces;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NewsContext>(options =>
    options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<INewsService, NewsService>();

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the TelegramBotClient
builder.Services.AddSingleton<TelegramBotClient>(provider =>
{
    var botToken = "6644956180:AAFN_eBw1BBknqz2UhjvTYxOMSm8d1NmH8w";
    return new TelegramBotClient(botToken);
});


var app = builder.Build();

var context = builder.Services.BuildServiceProvider().GetRequiredService<NewsContext>();
DataInitializer.Initialize(context, app.Configuration);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
 
app.MapControllers();

app.Run();
