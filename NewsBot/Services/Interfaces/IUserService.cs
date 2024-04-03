using NewsBot.Entities;
using NewsBot.Enums;
using NewsBot.Models.Base;
using Telegram.Bot.Types;

namespace NewsBot.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult> AddActivityLog(int userId, ActivityType type, CancellationToken cancellationToken);
        Entities.User GetUserById(object id, CancellationToken cancellationToken);
        UserActivity LastActivity(int userId);

    }
}
