using NewsBot.Enums;
using NewsBot.Models.Base;
using Telegram.Bot.Types;

namespace NewsBot.Services.Interfaces
{
    public interface IUserService
    {
        Task<ServiceResult> CheckUserBychatId(long chatId, Update update, ActivityType type, CancellationToken cancellationToken);
    }
}
