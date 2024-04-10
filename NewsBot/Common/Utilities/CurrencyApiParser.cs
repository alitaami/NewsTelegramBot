using FarzamNews.Utilities;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;

namespace NewsBot.Common.Utilities
{
    public static class CurrencyApiParser
    {
        public static async Task<string> GetLatestPriceAsync(string type)
        {
            try
            {
                var date = DateConverter.ToShamsi(DateTime.Now);
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync($"http://api.navasan.tech/dailyCurrency/?api_key=freeIj62pyf4CG5A9oLDrwKvvkJ0ypS8&item={type}&date={date}");
                string message = string.Empty;

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JArray.Parse(jsonResponse);
                if (data.Any())
                {
                    var sellPrice = data.LastOrDefault()["value"].ToString();
                    if (sellPrice.EndsWith(".000"))
                    {
                        sellPrice = sellPrice.Substring(0, sellPrice.Length - 4);
                    }
                    switch (type)
                    {
                        case "usd_buy":
                            message = DefaultContents.MoneyMessage + " " + DefaultContents.Dollar + " : " + sellPrice;
                            break;

                        case "dirham_dubai":
                            message = DefaultContents.MoneyMessage + " " + DefaultContents.Derham + " : " + sellPrice;
                            break;

                        case "bahar":
                            message = DefaultContents.MoneyMessage + " " + DefaultContents.Bahar + " : " + sellPrice;
                            break;

                        case "nim":
                            message = DefaultContents.MoneyMessage + " " + DefaultContents.Nim + " : " + sellPrice;
                            break;

                        case "rob":
                            message = DefaultContents.MoneyMessage + " " + DefaultContents.Rob + " : " + sellPrice;
                            break;

                        case "18ayar":
                            message = DefaultContents.MoneyMessage + " " + DefaultContents._18Ayar + " : " + sellPrice;
                            break;
                    }
                }
                else
                {
                    message = DefaultContents.TryLater;
                }

                return message;
            }
            catch (Exception ex)
            {
                return DefaultContents.ErrorOccured;
            }
        }
    }
}
