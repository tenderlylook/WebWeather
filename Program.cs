using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WeatherApp
{
    class Program
    {
        private const string API_KEY = "0ef3d7388ff8a37cc683193dee15532b";
        private const string BASE_URL = "https://api.openweathermap.org/data/2.5/weather";

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("ПРИЛОЖЕНИЕ ПОГОДЫ");
            Console.WriteLine("=====================\n");

            while (true)
            {
                Console.Write("Введите город (или 'exit' для выхода): ");
                string city = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(city) || city.ToLower() == "exit")
                    break;

                await ShowWeather(city);

                Console.Write("\nПоказать прогноз на 5 дней? (y/n): ");
                string choice = Console.ReadLine()?.Trim().ToLower();

                if (choice == "y")
                    await ShowForecast(city);

                Console.WriteLine("\n" + new string('-', 40) + "\n");
            }

            Console.WriteLine("До свидания!");
        }

        static async Task ShowWeather(string city)
        {
            string url = $"{BASE_URL}?q={city}&appid={API_KEY}&units=metric&lang=ru";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        JObject data = JObject.Parse(json);

                        Console.WriteLine($"\nГород: {data["name"]}");
                        Console.WriteLine($"Температура: {data["main"]["temp"]}C");
                        Console.WriteLine($"Ощущается как: {data["main"]["feels_like"]}C");
                        Console.WriteLine($"Влажность: {data["main"]["humidity"]}%");
                        Console.WriteLine($"Описание: {data["weather"][0]["description"]}");
                        Console.WriteLine($"Давление: {data["main"]["pressure"]} гПа");
                        Console.WriteLine($"Ветер: {data["wind"]["speed"]} м/с");
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка! Город '{city}' не найден.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        static async Task ShowForecast(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={API_KEY}&units=metric&lang=ru&cnt=5";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        JObject data = JObject.Parse(json);
                        var list = data["list"];

                        Console.WriteLine($"\nПРОГНОЗ НА 5 ДНЕЙ для {city}:\n");
                        foreach (var item in list)
                        {
                            string time = item["dt_txt"].ToString();
                            double temp = (double)item["main"]["temp"];
                            string desc = item["weather"][0]["description"].ToString();
                            Console.WriteLine($"   {time}: {temp}C, {desc}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка! Город '{city}' не найден.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}