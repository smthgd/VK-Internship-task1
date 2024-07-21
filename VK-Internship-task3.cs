using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace SteamTopSellers
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var topSellers = await GetTopSellers();

                Console.WriteLine("Топ-10 игр в Steam (РФ):");
                Console.WriteLine("-----------------------------");

                for (int i = 0; i < topSellers.Count; i++)
                {
                    if (topSellers[i].Price != 0)
                    {
                        Console.WriteLine($"Место: {i + 1}. {topSellers[i].Name} - {topSellers[i].Price} руб.");
                    }
                    else
                    {
                        Console.WriteLine($"Место: {i + 1}. {topSellers[i].Name} - Бесплатно");
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка сети: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла непредвиденная ошибка: {ex.Message}");
            }
        }

        private static async Task<List<Game>> GetTopSellers()
        {
            string topsellersUrl = "https://store.steampowered.com/search/?filter=topsellers\\RU";
            List<Game> topSellers = new List<Game>();

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(topsellersUrl);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Ошибка при получении данных: {response.StatusCode}");
                }

                string htmlContent = await response.Content.ReadAsStringAsync();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                HtmlNode topSellingPanel = doc.DocumentNode.SelectSingleNode("//div[@data-panel]");

                for (int i = 0; i < 10; i++)
                {
                    var gameName = topSellingPanel.SelectNodes("//span[@class='title']")[i].InnerText;
                    var gamePrice = topSellingPanel.SelectNodes("//div[@class='col search_discount_and_price responsive_secondrow']")[i].InnerText;

                    if (!gamePrice.Contains("Free"))
                    {
                        string clearGamePrice = gamePrice.Replace(" ", "").Replace("\n", "").Replace("\r", "");
                        string[] finalPrice = clearGamePrice.Split("руб");

                        if (finalPrice.Length > 2)
                        {
                            topSellers.Add(new Game { Name = gameName, Price = Convert.ToInt32(finalPrice[1]) });
                        }
                        else
                        {
                            topSellers.Add(new Game { Name = gameName, Price = Convert.ToInt32(finalPrice[0]) });
                        }
                    }
                    else
                    {
                        topSellers.Add(new Game { Name = gameName, Price = 0 });
                    }
                }

                return topSellers;
            }
        }
    }

    public class Game
    {
        public string Name { get; set; }

        public int Price { get; set; }
    }
}