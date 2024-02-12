using AutoMapper;
using Data.Repositories;
using NewsBot.Entities;
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
        IMapper _mapper;
        private readonly TelegramBotClient _bot;
        public UserService(
           TelegramBotClient bot, IMapper mapper, ILogger<UserService> logger, IRepository<Entities.User> Repo) : base(logger)
        {
            _bot = bot;
            _mapper = mapper;
            _Repo = Repo;
        }
        public async Task<ServiceResult> CheckUserBychatId(long chatId, Update update, CancellationToken cancellationToken)
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
