using Azure.Core;
using Data.Repositories;
using FarzamNews.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NewsBot.Common.Utilities;
using NewsBot.Data.NewsContext;
using NewsBot.Entities;
using NewsBot.Enums;
using NewsBot.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.WebSockets;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Utilities.KeyboardButtons;

namespace NewsBot.Controllers
{
    [Route("api/[controller]/[action]")]
    public class BotManagementController : Controller
    {
        private readonly TelegramBotClient _bot;
        private readonly IServiceScopeFactory moserviceScopeFactory;

        public BotManagementController(IServiceScopeFactory serviceScopeFactory, NewsContext db, IRepository<Entities.User> userRepo, TelegramBotClient telegramBot, IUserService user)
        {
            _bot = telegramBot;
            moserviceScopeFactory = serviceScopeFactory;
        }
        #region Webhook method
        //[HttpGet]
        //public async Task<ActionResult> SetWebhook()
        //{
        //    var allowedUpdates = new UpdateType[]
        //    {
        //        UpdateType.Message,
        //        UpdateType.CallbackQuery
        //    };

        //    await _bot.SetWebhookAsync(url: " https://errefer7lhhxpmiwcht7bm.hooks.webhookrelay.com", allowedUpdates: allowedUpdates);
        //    return Ok();
        //}

        //[HttpPost]
        //public async Task<ActionResult> ReceiveUpdate(object model)
        //{
        //    var update = JsonConvert.DeserializeObject<Update>(model.ToString());

        //    if (update is null)
        //        return Ok();

        //    if (update.Message != null)
        //    {
        //        var text = update.Message.Text;
        //        var chatId = update.Message.Chat.Id;

        //        if (text == DefaultContents.Start)
        //        {
        //            await _user.CheckUserBychatId(chatId, update, ActivityType.StartBot, CancellationToken.None);
        //            await _bot.SendTextMessageAsync(chatId, DefaultContents.WelcomeToBot, replyMarkup: Buttons.GenerateMainKeyboard());
        //        }
        //        else if (text == DefaultContents.Location)
        //        {
        //            await _user.CheckUserBychatId(chatId, update, ActivityType.GetLocation, CancellationToken.None);
        //            await _bot.SendVenueAsync(chatId, 45.87654, 56.76543, "دفتر مرکزی", "خیابان ایکس پلاک 2");
        //        }
        //        else if (text == DefaultContents.ContactUs)
        //        {
        //            await _user.CheckUserBychatId(chatId, update, ActivityType.GetContact, CancellationToken.None);
        //            await _bot.SendTextMessageAsync(chatId, DefaultContents.ContactUsMessage);
        //        }
        //        else if (text == DefaultContents.Money)
        //        {
        //            await _user.CheckUserBychatId(chatId, update, ActivityType.GetMoneyNews, CancellationToken.None);
        //            await _bot.SendTextMessageAsync(chatId, DefaultContents.MoneyMessage);
        //        }
        //        else if (text == DefaultContents.Profile)
        //        {
        //            var userId = await _user.CheckUserBychatId(chatId, update, ActivityType.Profile, CancellationToken.None);
        //            var user = _user.GetUserById(userId.Data, CancellationToken.None);
        //            await _bot.SendTextMessageAsync(chatId, string.Format(DefaultContents.Profile, user.FirstName, user.LastName ?? "___", user.Username ?? "___"), replyMarkup: Buttons.GenerateProfileKeyboard());

        //        }
        //        else if (text == DefaultContents.BackToMainMenu)
        //        {
        //            var userId = await _user.CheckUserBychatId(chatId, update, ActivityType.MainMenu, CancellationToken.None);
        //            var user = _user.GetUserById(userId.Data, CancellationToken.None);
        //            await _bot.SendTextMessageAsync(chatId, DefaultContents.BackToMainMenu, replyMarkup: Buttons.GenerateMainKeyboard());

        //        }
        //    }
        //    return Ok();
        //}
        #endregion

        [HttpGet]
        public async Task<ActionResult> SetBotConnection()
        {
            try
            {
                // configs for receiving options 
                var receivingOptions = new ReceiverOptions()
                {
                    AllowedUpdates = new UpdateType[]
                    {
                 UpdateType.Message,
                 UpdateType.CallbackQuery,
                 UpdateType.Poll,
                 UpdateType.PollAnswer
                    }
                };

                // Start receiving updates using long-polling
                _bot.StartReceiving(updateHandler: HandleUpdateAsync, pollingErrorHandler: HandleErrorAsync, receivingOptions);

                return Ok();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        private async Task<IActionResult> HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                if (update is null)
                    return BadRequest();

                using (var scope = moserviceScopeFactory.CreateScope())
                {
                    var _repoUser = scope.ServiceProvider.GetRequiredService<IRepository<Entities.User>>();
                    var _repoNews = scope.ServiceProvider.GetRequiredService<IRepository<Entities.News>>();
                    var _repoNewsKeyword = scope.ServiceProvider.GetRequiredService<IRepository<Entities.NewsKeyWord>>();
                    var _repoUserCollection = scope.ServiceProvider.GetRequiredService<IRepository<Entities.NewsUserCollection>>();
                    var _user = scope.ServiceProvider.GetRequiredService<IUserService>();

                    if (update.CallbackQuery is not null)
                    {
                        var text = update.CallbackQuery.Data;
                        var chatId = update.CallbackQuery.From.Id;
                        var user = await _repoUser.Table.Where(u => u.ChatId == chatId).FirstOrDefaultAsync();
                        if (user is null)
                        {
                            user = await _repoUser.AddAsync(new Entities.User()
                            {
                                ChatId = chatId,
                                FirstName = update.Message.From.FirstName,
                                LastName = update.Message.From.LastName,
                                Username = update.Message.From.Username,
                                UserType = Enums.UserType.Guest,
                                ParentId = null,
                                UserActivities = new List<Entities.UserActivity>()
                        {
                            new UserActivity(){ActivityType=ActivityType.StartBot}
                        }
                            }, cancellationToken);
                        }

                        var lastActivity = _user.LastActivity(user.Id);

                        if (text == "Canceled")
                        {
                            await botClient.SendTextMessageAsync(chatId, DefaultContents.PleaseRetry);
                        }
                        else if (text.StartsWith("Confirmed_"))
                        {
                            var value = text.Split("_")[1].ToString();

                            user.FirstName = value;
                            await _repoUser.UpdateAsync(user, cancellationToken);
                            await botClient.SendTextMessageAsync(chatId, DefaultContents.EditFirstNameDoneMessage, replyMarkup: Buttons.GenerateMainKeyboard(), cancellationToken: cancellationToken);
                        }
                        else if (text.StartsWith($"Save_"))
                        {
                            int newsId = 0;
                            if (int.TryParse(text.Split("_")[1], out newsId))
                            {
                                var news = await _repoNews.GetByIdAsync(cancellationToken, newsId);

                                if (news is null)
                                {
                                    await botClient.SendTextMessageAsync(chatId, DefaultContents.NewsNotFound);
                                    return Ok();
                                }

                                await _repoUserCollection.AddAsync(new NewsUserCollection()
                                {
                                    UserId = user.Id,
                                    NewsId = newsId
                                }, cancellationToken);

                                var newsKeywords = await _repoNewsKeyword.TableNoTracking.Where(n => n.NewsId == newsId).Include(n => n.KeyWord).ToListAsync();

                                string message = $"<b><i>{news.Title}</i></b>\n{news.Description}\n\n";

                                if (newsKeywords.Count > 0)
                                {
                                    foreach (var item in newsKeywords)
                                    {
                                        message += $"#{item.KeyWord.Title} ";
                                    }
                                    await _bot.EditMessageTextAsync(chatId, update.CallbackQuery.Message.MessageId, message, parseMode: ParseMode.Html, replyMarkup: Buttons.GenerateNewsKeyboard(newsId, true));
                                }
                                else
                                    await botClient.SendTextMessageAsync(chatId, DefaultContents.MessageIsNotValid);

                            }
                        }
                        else if (text.StartsWith($"UnSave_"))
                        {
                            int newsId = 0;
                            if (int.TryParse(text.Split("_")[1], out newsId))
                            {
                                var news = await _repoNews.GetByIdAsync(cancellationToken, newsId);

                                if (news is null)
                                {
                                    await botClient.SendTextMessageAsync(chatId, DefaultContents.NewsNotFound);
                                    return Ok();
                                }

                                var newsKeywords = await _repoNewsKeyword.TableNoTracking.Where(n => n.NewsId == newsId).Include(n => n.KeyWord).ToListAsync();
                                var obj = await _repoUserCollection.TableNoTracking.Where(n => n.UserId == user.Id && n.NewsId == newsId).FirstOrDefaultAsync();
                                if (obj is not null)
                                {
                                    await _repoUserCollection.DeleteAsync(obj, cancellationToken);
                                }
                                string message = $"<b><i>{news.Title}</i></b>\n{news.Description}\n\n";

                                if (newsKeywords.Count > 0)
                                {
                                    foreach (var item in newsKeywords)
                                    {
                                        message += $"#{item.KeyWord.Title} ";
                                    }


                                    await _bot.EditMessageTextAsync(chatId, update.CallbackQuery.Message.MessageId, message, parseMode: ParseMode.Html, replyMarkup: Buttons.GenerateNewsKeyboard(newsId, false));
                                }
                                else
                                    await botClient.SendTextMessageAsync(chatId, DefaultContents.MessageIsNotValid);

                            }
                        }
                        else if (text == "Dollar")
                        {
                            var message = await CurrencyApiParser.GetLatestPriceAsync("usd_buy");

                             await botClient.SendTextMessageAsync(chatId, message , cancellationToken: cancellationToken);
                        }
                        else if (text == "Derham")
                        {
                            var message = await CurrencyApiParser.GetLatestPriceAsync("dirham_dubai");

                             await botClient.SendTextMessageAsync(chatId, message , cancellationToken: cancellationToken);
                        }
                        else if (text == "Bahar")
                        {
                            var message = await CurrencyApiParser.GetLatestPriceAsync("bahar");

                             await botClient.SendTextMessageAsync(chatId, message , cancellationToken: cancellationToken);
                        }
                        else if (text == "Nim")
                        {
                            var message = await CurrencyApiParser.GetLatestPriceAsync("nim");

                             await botClient.SendTextMessageAsync(chatId, message , cancellationToken: cancellationToken);
                        }
                        else if (text == "Rob")
                        {
                            var message = await CurrencyApiParser.GetLatestPriceAsync("rob");

                             await botClient.SendTextMessageAsync(chatId, message , cancellationToken: cancellationToken);
                        }
                        else if (text == "18Ayar")
                        {
                            var message = await CurrencyApiParser.GetLatestPriceAsync("18ayar");

                             await botClient.SendTextMessageAsync(chatId, message , cancellationToken: cancellationToken);
                        }
                    }

                    if (update.Message is not null)
                    {

                        var text = update.Message.Text;
                        long chatId = update.Message.Chat.Id;
                        var user = await _repoUser.Table.Where(u => u.ChatId == chatId).FirstOrDefaultAsync(cancellationToken);

                        if (user is null)
                        {
                            user = await _repoUser.AddAsync(new Entities.User()
                            {
                                ChatId = chatId,
                                FirstName = update.Message.From.FirstName,
                                LastName = update.Message.From.LastName,
                                Username = update.Message.From.Username,
                                UserType = Enums.UserType.Guest,
                                ParentId = null,
                                UserActivities = new List<Entities.UserActivity>()
                        {
                            new UserActivity(){ActivityType=ActivityType.StartBot}
                        }
                            }, cancellationToken);
                        }

                        var lastActivity = _user.LastActivity(user.Id);

                        if (text == DefaultContents.Start)
                        {
                            await _user.AddActivityLog(user.Id, ActivityType.StartBot, cancellationToken);
                            await botClient.SendTextMessageAsync(chatId, DefaultContents.WelcomeToBot, replyMarkup: Buttons.GenerateMainKeyboard(), cancellationToken: cancellationToken);
                        }
                        else if (text == DefaultContents.Location)
                        {
                            await _user.AddActivityLog(user.Id, ActivityType.GetLocation, cancellationToken);
                            await botClient.SendVenueAsync(chatId, 45.87654, 56.76543, "دفتر مرکزی", "خیابان ایکس پلاک 2", cancellationToken: cancellationToken);
                        }
                        else if (text == DefaultContents.ContactUs)
                        {
                            await _user.AddActivityLog(user.Id, ActivityType.GetContact, cancellationToken);
                            await botClient.SendTextMessageAsync(chatId, DefaultContents.ContactUsMessage, cancellationToken: cancellationToken);
                        }
                        else if (text == DefaultContents.Money)
                        {
                            await _user.AddActivityLog(user.Id, ActivityType.GetMoneyNews, cancellationToken);
                            await botClient.SendTextMessageAsync(chatId, DefaultContents.MoneyMessage, cancellationToken: cancellationToken, replyMarkup: Buttons.GenerateCurrencyKeyboard());
                        }
                        else if (text == DefaultContents.Profile)
                        {
                            await _user.AddActivityLog(user.Id, ActivityType.Profile, cancellationToken);
                            await botClient.SendTextMessageAsync(chatId, string.Format(DefaultContents.ProfileDetail, user.FirstName, user.LastName ?? "___", user.Username ?? "___"), replyMarkup: Buttons.GenerateProfileKeyboard(), cancellationToken: cancellationToken);
                        }
                        else if (text == DefaultContents.BackToMainMenu)
                        {
                            await _user.AddActivityLog(user.Id, ActivityType.MainMenu, cancellationToken);
                            await botClient.SendTextMessageAsync(chatId, DefaultContents.BackToMainMenuMessage, replyMarkup: Buttons.GenerateMainKeyboard(), cancellationToken: cancellationToken);
                        }
                        else if (text == DefaultContents.EditFirstName)
                        {
                            await _user.AddActivityLog(user.Id, ActivityType.EditFirstName, cancellationToken);
                            await botClient.SendTextMessageAsync(chatId, DefaultContents.EditFirstNameMessage);
                        }
                        else if (text == DefaultContents.HeadOfNews)
                        {
                            await _user.AddActivityLog(user.Id, ActivityType.GetHeadOfNews, cancellationToken);
                            var today = DateTime.Now.Date;
                            var items = await _repoNews.TableNoTracking.Where(x => x.CreatedDate.Date == today).Select(x => new { x.Id, x.Title }).Take(10).ToListAsync();

                            var message = $"سرتیتر اخبار امروز {today.ToShortDateString()}\n";
                            for (int i = 0; i < items.Count(); i++)
                                message += $"{i + 1} - {items[i].Title}   /News_{items[i].Id}\n";

                            await _bot.SendTextMessageAsync(chatId, message);
                        }
                        else if (text == DefaultContents.Search)
                        {
                            await _user.AddActivityLog(user.Id, ActivityType.Search, cancellationToken);
                            await botClient.SendTextMessageAsync(chatId, DefaultContents.PleaseEnterYourText);
                        }
                        else if (text == DefaultContents.SavedNews)
                        {
                            var items = await _repoUserCollection.TableNoTracking.Where(x => x.UserId == user.Id).Include(x => x.News).Select(x => new { x.NewsId, x.News.Title }).Take(10).ToListAsync();

                            if (items.Count is 0)
                            {
                                await _bot.SendTextMessageAsync(chatId, DefaultContents.EmptySavedNews);
                            }
                            var message = $"اخبار ذخیره شده  \n";
                            for (int i = 0; i < items.Count(); i++)
                                message += $"{i + 1} - {items[i].Title}   /News_{items[i].NewsId}\n";

                            await _bot.SendTextMessageAsync(chatId, message);
                        }
                        else
                        {
                            if (text.StartsWith("/News_"))
                            {
                                int newsId = 0;
                                if (int.TryParse(text.Split("_")[1], out newsId))
                                {
                                    var news = await _repoNews.GetByIdAsync(cancellationToken, newsId);

                                    if (news is null)
                                    {
                                        await botClient.SendTextMessageAsync(chatId, DefaultContents.NewsNotFound);
                                        return Ok();
                                    }

                                    var newsKeywords = await _repoNewsKeyword.TableNoTracking.Where(n => n.NewsId == newsId).Include(n => n.KeyWord).ToListAsync();

                                    string message = $"<b><i>{news.Title}</i></b>\n{news.Description}\n\n";

                                    if (newsKeywords.Count > 0)
                                    {
                                        foreach (var item in newsKeywords)
                                        {
                                            message += $"#{item.KeyWord.Title} ";
                                        }

                                        bool isSaved = await _repoUserCollection.TableNoTracking.AnyAsync(u => u.UserId == user.Id && u.NewsId == newsId);

                                        await botClient.SendTextMessageAsync(chatId, message, parseMode: ParseMode.Html, replyMarkup: Buttons.GenerateNewsKeyboard(newsId, isSaved));
                                    }
                                    else
                                        await botClient.SendTextMessageAsync(chatId, DefaultContents.MessageIsNotValid);

                                }
                            }

                            else if (lastActivity.ActivityType is ActivityType.EditFirstName || lastActivity.ActivityType is ActivityType.GetEditFirstNameConfirmation)
                            {
                                await _user.AddActivityLog(user.Id, ActivityType.GetEditFirstNameConfirmation, cancellationToken);

                                await botClient.SendTextMessageAsync(chatId, DefaultContents.EditLastNameAlert, replyMarkup: Buttons.GenerateConfirmationKeyboard(text));
                            }
                            else if (lastActivity.ActivityType is ActivityType.Search)
                            {
                                await _user.AddActivityLog(user.Id, ActivityType.ShowSearchResult, cancellationToken);

                                var news = await _repoNews
                                          .TableNoTracking
                                          .Include(n => n.NewsKeyWords)
                                          .ThenInclude(n => n.KeyWord)
                                          .Where(n => n.Title.Contains(text) || n.NewsKeyWords.Any(z => z.KeyWord.Title == text))
                                          .ToListAsync();

                                if (news.Count is 0)
                                {
                                    await _bot.SendTextMessageAsync(chatId, DefaultContents.EmptySearchNews);
                                    return Ok();
                                }

                                var message = $"نتایج جستجو\n";
                                for (int i = 0; i < news.Count(); i++)
                                    message += $"{i + 1} - {news[i].Title}   /News_{news[i].Id}\n";

                                await _bot.SendTextMessageAsync(chatId, message);
                            }
                        }

                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
