using AutoMapper;
using Data.Repositories;
using Microsoft.Win32.SafeHandles;
using NewsBot.Common.Resources;
using NewsBot.Entities;
using NewsBot.Models.Base;
using NewsBot.Models.Entities;
using NewsBot.Models.ViewModels;
using NewsBot.Services.Interfaces;
using Telegram.Bot;
using static System.Net.Mime.MediaTypeNames;

namespace NewsBot.Services
{
    public class NewsService : ServiceBase<NewsService>, INewsService
    {
        private IRepository<News> _newsRepo;
        private IRepository<NewsKeyWord> _newskeyRepo;
        private IRepository<KeyWord> _keyRepo;
        IMapper _mapper;
        private readonly TelegramBotClient _bot;
        public NewsService(
           TelegramBotClient bot, IMapper mapper, ILogger<NewsService> logger, IRepository<News> newsRepo, IRepository<NewsKeyWord> newskeyRepo, IRepository<KeyWord> keyRepo) : base(logger)
        {
            _bot = bot;
            _mapper = mapper;
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

                var keywords = _newskeyRepo.TableNoTracking.Where(k => k.NewsId == id).ToList();

                foreach (var keyword in keywords)
                {
                    await _newskeyRepo.DeleteAsync(keyword, cancellationToken);
                }

                await _bot.DeleteMessageAsync("@NewsTestChannel1", obj.MessageId);

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
                var news = _mapper.Map<News>(model);

                string text = $"<b><i>{model.Title}</i></b>\n{model.Description}\n\n";

                if (model.KeyWords.Count > 0)
                {
                    // because it was null, when we mapped that 
                    news.NewsKeyWords = new List<NewsKeyWord>();

                    foreach (int id in model.KeyWords)
                    {
                        var keyWord = await _keyRepo.GetByIdAsync(cancellationToken, id);

                        if (keyWord is null)
                            continue;

                        text += $"{keyWord.Title}";
                        news.NewsKeyWords.Add(new NewsKeyWord
                        {
                            KeyWordId = keyWord.Id
                        });
                    }
                }

                var message =
                    await _bot.SendTextMessageAsync(chatId:"@NewsTestChannel1", text: text, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);

                news.MessageId = message.MessageId;

                var data = await _newsRepo.AddAsync2(
                    news
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
                var news = await _newsRepo.GetByIdAsync(cancellationToken, model.Id);

                if (news is null)
                    return BadRequest(ErrorCodeEnum.BadRequest, Resource.NotFound, null);///

                var keywords = _newskeyRepo.TableNoTracking.Where(k => k.NewsId == model.Id).ToList();
                string text = $"<b><i>{model.Title}</i></b>\n{model.Description}\n\n";

                _mapper.Map(model, news);

                if (model.KeyWords.Count > 0)
                {
                    foreach (var keyword in keywords)
                    {
                        await _newskeyRepo.DeleteAsync(keyword, cancellationToken);
                    }

                    // because it was null, when we mapped that 
                    news.NewsKeyWords = new List<NewsKeyWord>();

                    foreach (int id in model.KeyWords)
                    {
                        var keyWord = await _keyRepo.GetByIdAsync(cancellationToken, id);

                        if (keyWord is null)
                            continue;

                        text += $"{keyWord.Title}";
                        news.NewsKeyWords.Add(new NewsKeyWord
                        {
                            KeyWordId = keyWord.Id
                        });
                    }
                }

                await _bot.EditMessageTextAsync("@NewsTestChannel1", news.MessageId, text,parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                await _bot.SendTextMessageAsync("@NewsTestChannel1", "این خبر بروزرسانی شد",replyToMessageId:news.MessageId);

                await _newsRepo.UpdateAsync(news, cancellationToken);

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null, null);
                return InternalServerError(ErrorCodeEnum.InternalError, Resource.GeneralErrorTryAgain, null);
            }
        }
    }
}