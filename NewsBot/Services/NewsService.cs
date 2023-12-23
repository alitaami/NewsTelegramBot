using Data.Repositories;
using NewsBot.Common.Resources;
using NewsBot.Entities;
using NewsBot.Models.Base;
using NewsBot.Models.Entities;
using NewsBot.Services.Interfaces;

namespace NewsBot.Services
{
    public class NewsService : ServiceBase<NewsService>,INewsService
    {
        private IRepository<News> _newsRepo;
        private IRepository<NewsKeyWord> _newskeyRepo;
        private IRepository<KeyWord> _keyRepo;
        public NewsService(ILogger<NewsService> logger,IRepository<News>  newsRepo , IRepository<NewsKeyWord>  newskeyRepo, IRepository<KeyWord> keyRepo) :base(logger)
        {
            _newsRepo = newsRepo;
            _newskeyRepo = newskeyRepo;
            _keyRepo = keyRepo;
        }
        public async Task<ServiceResult> GetNewsById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var res = _newsRepo.GetByIdAsync(cancellationToken, id);
            
                if (res is null)
                return BadRequest(ErrorCodeEnum.BadRequest, Resource.NotFound, null);///

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
    }
}