using FarzamNews.Utilities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Utilities.KeyboardButtons
{
    public static class Buttons
    {
        public static ReplyKeyboardMarkup GenerateMainKeyboard()
        {
            var rows = new List<KeyboardButton[]>();

            rows.Add(new KeyboardButton[] {
                new KeyboardButton(DefaultContents.Search),
                new KeyboardButton(DefaultContents.Start)
             });

            rows.Add(new KeyboardButton[]
            {
              new KeyboardButton(DefaultContents.HeadOfNews),
              new KeyboardButton(DefaultContents.Money)

            });
            rows.Add(new KeyboardButton[]
           {
              new KeyboardButton(DefaultContents.Location),
              new KeyboardButton(DefaultContents.ContactUs)

           });
            rows.Add(new KeyboardButton[]
           {
              new KeyboardButton(DefaultContents.Profile)

           });
            var keyboard = new ReplyKeyboardMarkup(rows);

            return keyboard;
        }

        public static InlineKeyboardMarkup GenerateCurrencyKeyboard()
        {
            var rows = new List<InlineKeyboardButton[]>();

            rows.Add(new InlineKeyboardButton[] {
                new InlineKeyboardButton(DefaultContents.Dollar){CallbackData = "Dollar"},
                new InlineKeyboardButton(DefaultContents.Derham){CallbackData="Derham"}
            });

            rows.Add(new InlineKeyboardButton[] {
                new InlineKeyboardButton(DefaultContents.Bahar){CallbackData="Bahar"},
                new InlineKeyboardButton(DefaultContents.Nim){CallbackData="Nim"}
             });

            rows.Add(new InlineKeyboardButton[] {
                new InlineKeyboardButton(DefaultContents.Rob){CallbackData="Rob"},
                new InlineKeyboardButton(DefaultContents._18Ayar){CallbackData="18Ayar"}
            });
             
            var keyboard = new InlineKeyboardMarkup(rows);

            return keyboard;
        }
        public static InlineKeyboardMarkup GenerateConfirmationKeyboard(string text)
        {
            var rows = new List<InlineKeyboardButton[]>();

            rows.Add(new InlineKeyboardButton[] {
                new InlineKeyboardButton(DefaultContents.Confirmed){CallbackData = $"Confirmed_{text}"}
             });

            rows.Add(new InlineKeyboardButton[] {
                new InlineKeyboardButton(DefaultContents.Canceled){CallbackData="Canceled"}
             });


            var keyboard = new InlineKeyboardMarkup(rows);

            return keyboard;
        }
        public static InlineKeyboardMarkup GenerateNewsKeyboard(int newsId, bool isSaved)
        {
            var rows = new List<InlineKeyboardButton[]>();

            if (isSaved)
            {
                rows.Add(new InlineKeyboardButton[] {
                new InlineKeyboardButton(DefaultContents.DeleteFromSavedNews){CallbackData = $"UnSave_{newsId}"}
             });
            }
            else
            {
                rows.Add(new InlineKeyboardButton[] {
                new InlineKeyboardButton(DefaultContents.SaveNews){CallbackData=$"Save_{newsId}"}
             });
            }

            var keyboard = new InlineKeyboardMarkup(rows);

            return keyboard;
        }
        public static ReplyKeyboardMarkup GenerateProfileKeyboard()
        {
            var rows = new List<KeyboardButton[]>();

            rows.Add(new KeyboardButton[]
            {
                new KeyboardButton(DefaultContents.EditFirstName)
            });

            rows.Add(new KeyboardButton[]
            {
                new KeyboardButton(DefaultContents.EditLastName)
            });

            rows.Add(new KeyboardButton[]
            {
                new KeyboardButton(DefaultContents.SavedNews)
            });

            rows.Add(new KeyboardButton[]
            {
                new KeyboardButton(DefaultContents.BackToMainMenu)
            });

            var keyboard = new ReplyKeyboardMarkup(rows);
            return keyboard;
        }
    }
}
