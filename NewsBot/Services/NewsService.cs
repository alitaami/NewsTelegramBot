using Data.Repositories;
using Microsoft.Win32.SafeHandles;
using NewsBot.Common.Resources;
using NewsBot.Entities;
using NewsBot.Models.Base;
using NewsBot.Models.Entities;
using NewsBot.Models.ViewModels;
using NewsBot.Services.Interfaces;

namespace NewsBot.Services
{
    public class NewsService : ServiceBase<NewsService>, INewsService
    {
        private IRepository<News> _newsRepo;
        private IRepository<NewsKeyWord> _newskeyRepo;
        private IRepository<KeyWord> _keyRepo;
        public NewsService(ILogger<NewsService> logger, IRepository<News> newsRepo, IRepository<NewsKeyWord> newskeyRepo, IRepository<KeyWord> keyRepo) : base(logger)
        {
            _newsRepo = newsRepo;
            _newskeyRepo = newskeyRepo;
            _keyRepo = keyRepo;
        }

        public async Task<ServiceResult> DeleteNews(int id, CancellationToken cancellationToken)
        {
            try
            {
                var obj = await _newsRepo.GetByIdAsync(cancellationToken, id);

                if (obj is null)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.NotFound, null);///

                await _newsRepo.DeleteAsync(obj, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }

        public async Task<ServiceResult> GetNews(CancellationToken cancellationToken)
        {
            try
            {
                var res = _newsRepo.TableNoTracking.ToList();

                return Ok(res);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }


        public async Task<ServiceResult> GetNewsById(int id, CancellationToken cancellationToken)
        {
            try
            {
                var res = await _newsRepo.GetByIdAsync(cancellationToken, id);

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

        public async Task<ServiceResult> PostNews(NewsViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var data = await _newsRepo.AddAsync2(new News()
                {
                    MessageId = 0,
                    Title = model.Title,
                    Description = model.Description
                }
                , cancellationToken);

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }

        public async Task<ServiceResult> UpdateNews(NewsUpdateViewModel model, CancellationToken cancellationToken)
        {
            try
            {
                var obj = await _newsRepo.GetByIdAsync(cancellationToken, model.Id);

                obj.Title = model.Title;
                obj.Description = model.Description;

                await _newsRepo.UpdateAsync(obj, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
    }
}