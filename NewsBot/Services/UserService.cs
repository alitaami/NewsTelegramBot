using AutoMapper;
using Data.Repositories;
using NewsBot.Entities;
using NewsBot.Enums;
using NewsBot.Models.Base;
using NewsBot.Models.Entities;
using NewsBot.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NewsBot.Services
{
    public class UserService : ServiceBase<UserService>, IUserService
    {
        private IRepository<Entities.User> _Repo;
        private IRepository<Entities.UserActivity> _RepoAc;
        IMapper _mapper;
        private readonly TelegramBotClient _bot;
        public UserService(
           TelegramBotClient bot, IMapper mapper, ILogger<UserService> logger, IRepository<Entities.UserActivity> repoAc, IRepository<Entities.User> Repo) : base(logger)
        {
            _bot = bot;
            _mapper = mapper;
            _RepoAc = repoAc;
            _Repo = Repo;
        }
        public async Task<ServiceResult> CheckUserBychatId(long chatId, Update update, ActivityType type, CancellationToken cancellationToken)
        {
            try
            {
                var user = _Repo.TableNoTracking
                                .Where(u => u.ChatId == chatId)
                                .FirstOrDefault();

                if (user is null)
                {
                    user = await _Repo.AddAsync2(new Entities.User()
                    {
                        ChatId = chatId,
                        FirstName = update.Message.From.FirstName,
                        LastName = update.Message.From.LastName,
                        Username = update.Message.From.Username,
                        UserType = Enums.UserType.Guest,
                        ParentId = null
                    }, cancellationToken);
                    await _RepoAc.AddAsync2(new UserActivity
                    {
                        ActivityType = type,
                        UserId = user.Id
                    },cancellationToken);
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
