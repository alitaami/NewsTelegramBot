using Data.Repositories;
using FarzamNews.Utilities;
using Microsoft.AspNetCore.Mvc;
using NewsBot.Entities;
using NewsBot.Enums;
using NewsBot.Services.Interfaces;
using Newtonsoft.Json;
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
        private readonly IUserService _user;
        private readonly IRepository<Entities.User> _userRepo;
        public BotManagementController(IRepository<Entities.User> userRepo, TelegramBotClient telegramBot, IUserService user)
        {
            _bot = telegramBot;
            _user = user;
            _userRepo = userRepo;
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
         
        private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update is null)
                return;

            if (update.CallbackQuery is not null)
            {
                var text = update.CallbackQuery.Data;
                var chatId = update.CallbackQuery.From.Id;
                var user = _userRepo.Table.Where(u => u.ChatId == chatId).FirstOrDefault();
                if (user is null)
                {
                    user = await _userRepo.AddAsync2(new Entities.User()
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
                    await _userRepo.UpdateAsync(user,cancellationToken);
                }
            }

            if (update.Message is not null)
            {
                var text = update.Message.Text;
                var chatId = update.Message.Chat.Id;
                var user = _userRepo.TableNoTracking.Where(u => u.ChatId == chatId).FirstOrDefault();

                if (user is null)
                {
                    user = await _userRepo.AddAsync2(new Entities.User()
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
                    await botClient.SendTextMessageAsync(chatId, DefaultContents.MoneyMessage, cancellationToken: cancellationToken);
                }
                else if (text == DefaultContents.Profile)
                {
                    await _user.AddActivityLog(user.Id, ActivityType.Profile, cancellationToken);
                    await botClient.SendTextMessageAsync(chatId, string.Format(DefaultContents.Profile, user.FirstName, user.LastName ?? "___", user.Username ?? "___"), replyMarkup: Buttons.GenerateProfileKeyboard(), cancellationToken: cancellationToken);
                }
                else if (text == DefaultContents.BackToMainMenu)
                {
                    await _user.AddActivityLog(user.Id, ActivityType.MainMenu, cancellationToken);
                    await botClient.SendTextMessageAsync(chatId, DefaultContents.BackToMainMenu, replyMarkup: Buttons.GenerateMainKeyboard(), cancellationToken: cancellationToken);
                }
                else if (text == DefaultContents.EditFirstName)
                {
                    await _user.AddActivityLog(user.Id, ActivityType.EditFirstName, cancellationToken);
                    await botClient.SendTextMessageAsync(chatId, DefaultContents.EditFirstName);
                }
                else if (text == DefaultContents.EditFirstName)
                {
                    await _user.AddActivityLog(user.Id, ActivityType.EditFirstName, cancellationToken);
                    await botClient.SendTextMessageAsync(chatId, DefaultContents.EditFirstName);
                }
                else
                {
                    if (lastActivity.ActivityType is ActivityType.EditFirstName)
                    {
                        await _user.AddActivityLog(user.Id, ActivityType.GetEditFirstNameConfirmation, cancellationToken);
                        await botClient.SendTextMessageAsync(chatId, DefaultContents.EditLastNameAlert, replyMarkup: Buttons.GenerateConfirmationKeyboard(text));
                    }
                }
            }
        }
        private async Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
