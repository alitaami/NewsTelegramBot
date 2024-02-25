using FarzamNews.Utilities;
using Microsoft.AspNetCore.Mvc;
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
                    await _user.CheckUserBychatId(chatId, update, CancellationToken.None);
                    await _bot.SendTextMessageAsync(chatId, DefaultContents.WelcomeToBot, replyMarkup: Buttons.GenerateMainKeyboard());
                }
                else if(text == DefaultContents.Location)
                {
                    await _bot.SendVenueAsync(chatId, 45.87654, 56.76543, "دفتر مرکزی", "خیابان ایکس پلاک 2");
                }
                else if (text == DefaultContents.ContactUs)
                {
                    await _bot.SendTextMessageAsync(chatId, DefaultContents.ContactUsMessage);
                }
            }
            return Ok();
        }
    }
}
