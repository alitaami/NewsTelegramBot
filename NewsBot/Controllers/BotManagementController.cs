using FarzamNews.Utilities;
using Microsoft.AspNetCore.Mvc;
using NewsBot.Entities;
using NewsBot.Enums;
using NewsBot.Services.Interfaces;
using Newtonsoft.Json;
using Telegram.Bot;
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
        public BotManagementController(TelegramBotClient telegramBot, IUserService user)
        {
            _bot = telegramBot;
            _user = user;
        }

        [HttpGet]
        public async Task<ActionResult> SetWebhook()
        {
            var allowedUpdates = new UpdateType[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery
            };

            await _bot.SetWebhookAsync(url: " https://errefer7lhhxpmiwcht7bm.hooks.webhookrelay.com", allowedUpdates: allowedUpdates);
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> ReceiveUpdate(object model)
        {
            var update = JsonConvert.DeserializeObject<Update>(model.ToString());

            if (update is null)
                return Ok();

            if (update.Message != null)
            {
                var text = update.Message.Text;
                var chatId = update.Message.Chat.Id;

                if (text == DefaultContents.Start)
                {
                    await _user.CheckUserBychatId(chatId, update, ActivityType.StartBot, CancellationToken.None);
                    await _bot.SendTextMessageAsync(chatId, DefaultContents.WelcomeToBot, replyMarkup: Buttons.GenerateMainKeyboard());
                }
                else if (text == DefaultContents.Location)
                {
                    await _user.CheckUserBychatId(chatId, update, ActivityType.GetLocation, CancellationToken.None);
                    await _bot.SendVenueAsync(chatId, 45.87654, 56.76543, "دفتر مرکزی", "خیابان ایکس پلاک 2");
                }
                else if (text == DefaultContents.ContactUs)
                {
                    await _user.CheckUserBychatId(chatId, update, ActivityType.GetContact, CancellationToken.None);
                    await _bot.SendTextMessageAsync(chatId, DefaultContents.ContactUsMessage);
                }
                else if (text == DefaultContents.Money)
                {
                    await _user.CheckUserBychatId(chatId, update, ActivityType.GetMoneyNews, CancellationToken.None);
                    await _bot.SendTextMessageAsync(chatId, DefaultContents.MoneyMessage);
                }
                else if (text == DefaultContents.Profile)
                {
                    var userId = await _user.CheckUserBychatId(chatId, update, ActivityType.Profile, CancellationToken.None);
                    var user = _user.GetUserById(userId.Data, CancellationToken.None);
                    await _bot.SendTextMessageAsync(chatId, string.Format(DefaultContents.Profile, user.FirstName, user.LastName ?? "___", user.Username ?? "___"), replyMarkup: Buttons.GenerateProfileKeyboard());

                }
                else if(text == DefaultContents.BackToMainMenu)
                {
                    var userId = await _user.CheckUserBychatId(chatId,update,ActivityType.MainMenu, CancellationToken.None);
                    var user = _user.GetUserById(userId.Data, CancellationToken.None);
                    await _bot.SendTextMessageAsync(chatId, DefaultContents.BackToMainMenu, replyMarkup: Buttons.GenerateMainKeyboard());

                }
            }
            return Ok();
        }
    }
}
