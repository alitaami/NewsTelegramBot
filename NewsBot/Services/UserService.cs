using AutoMapper;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ServiceResult> AddActivityLog(int userId, ActivityType type, CancellationToken cancellationToken)
        {
            try
            {
                await _RepoAc.AddAsync(new UserActivity
                {
                    ActivityType = type,
                    UserId = userId
                }, cancellationToken);


                return Ok();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public Entities.User GetUserById(object id, CancellationToken cancellationToken)
        {
            var user = _Repo.TableNoTracking
                            .Where(u => u.Id == Convert.ToInt32(id))
                            .FirstOrDefault();
            return user;
        }

        public UserActivity LastActivity(int userId)
        {
            var activity = _RepoAc
                           .TableNoTracking
                           .Where(x=>x.UserId == userId)
                           .OrderByDescending(x=>x.Id)
                           .FirstAsync();

            return activity.Result;
        }
    }
}
