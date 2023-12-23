using Microsoft.EntityFrameworkCore;
using NewsBot.Data.NewsContext;
using NewsBot.Enums;
using NewsBot.Models.Entities;
using NewsBot.Models;
using NewsBot.Entities;

namespace FarzamNews.Utilities;

public class DataInitializer
{
    internal static void Initialize(NewsContext context, IConfiguration configuration)
    {
        context.Database.Migrate();
        InitData(context, configuration);
    }

    private static void InitData(NewsContext context, IConfiguration configuration)
    {
        if(!context.Users.Any(x=>x.UserType ==  UserType.Admin))
        {
            var user = configuration.GetSection("AdminInfo").Get<User>();
            if (user != null)
            {
                user.UserType = UserType.Admin;
                user.Username = "admin";
                context.Add(user);

                context.SaveChanges();
            }
        }
    }
}

