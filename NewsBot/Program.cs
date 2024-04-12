using AutoMapper;
using Data.Repositories;
using FarzamNews.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NewsBot.Data.NewsContext;
using NewsBot.Entities;
using NewsBot.Mappings;
using NewsBot.Services;
using NewsBot.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NLog;
using NLog.Web;
using System.Globalization;
using System.Text.Json.Serialization;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

#region SetupNlog
ILoggerFactory loggerFactory = new LoggerFactory();
LogManager.Setup().LoadConfigurationFromFile("nlog.config");
//loggerFactory.AddNLog().ConfigureNLog("nlog.config");
#if DEBUG
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();
builder.Logging.AddEventLog();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
#endif
builder.Logging.ClearProviders();
builder.Logging.AddNLogWeb();
builder.Host.UseNLog();
#endregion

// Set the DataDirectory for accessing DB file easily in a static way
string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
AppDomain.CurrentDomain.SetData("DataDirectory", baseDirectory);
 
#region AddMvcAndJsonOptions
builder.Services
                     .AddControllers()
                     .AddJsonOptions(options =>
                     {
                         options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                     })
                     .AddNewtonsoftJson(options =>
                     {
                         options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                         options.SerializerSettings.Converters.Add(new StringEnumConverter());
                         options.SerializerSettings.Culture = new CultureInfo("en");
                         options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                         options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                         options.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                         options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                         options.SerializerSettings.ContractResolver = new DefaultContractResolver
                         {
                             NamingStrategy = new CamelCaseNamingStrategy()
                         };
                         options.AllowInputFormatterExceptionMessages = true;
                     });
#endregion

//// Add services to the container.
//builder.Services.AddDbContext<NewsContext>(options =>
//    options.UseSqlServer(
//        connectionString: builder.Configuration.GetConnectionString("DbConnection")),
//        ServiceLifetime.Scoped); // Change ServiceLifetime.Transient to ServiceLifetime.Scoped

builder.Services.AddDbContext<NewsContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DbConnection")),
        ServiceLifetime.Scoped); // Change ServiceLifetime.Transient to ServiceLifetime.Scoped

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IUserService, UserService>();

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
    var botToken = "7125057765:AAF4UZKamrLrbTsA9aDqgLy_0pBLb8U5q7Y";
    return new TelegramBotClient(botToken);
});

builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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
