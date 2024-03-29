﻿using FarzamNews.Utilities;
using Telegram.Bot.Types.ReplyMarkups;

namespace Utilities.KeyboardButtons
{
    public static class Buttons
    {
        public static ReplyKeyboardMarkup GenerateMainKeyboard()
        {
            var rows = new List<KeyboardButton[]>();

            rows.Add(new KeyboardButton[] {
                new KeyboardButton(DefaultContents.Search)
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
        public static ReplyKeyboardMarkup GenerateProfileKeyboard()
        {
            var rows = new List<KeyboardButton[]>();

            rows.Add(new KeyboardButton[] {
                new KeyboardButton(DefaultContents.Search)
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
    }
}
