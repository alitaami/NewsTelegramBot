using Data.Repositories;
using FarzamNews.Utilities;
using Microsoft.EntityFrameworkCore;
using NewsBot.Data.NewsContext;
using NewsBot.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<NewsContext>(options =>
    options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString("DbConnection")));

builder.Services.AddScoped<IRepository<News>, Repository<News>>();
builder.Services.AddScoped<IRepository<User>, Repository<User>>();
builder.Services.AddScoped<IRepository<NewsKeyWord>, Repository<NewsKeyWord>>();
builder.Services.AddScoped<IRepository<NewsUserCollection>, Repository<NewsUserCollection>>();
builder.Services.AddScoped<IRepository<UserActivity>, Repository<UserActivity>>();
builder.Services.AddScoped<IRepository<KeyWord>, Repository<KeyWord>>();
 
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

app.UseAuthorization();

app.MapControllers();

app.Run();
