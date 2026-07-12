using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace WeatherAppWinForms
{
    public partial class Form1 : Form
    {
        private const string API_KEY = "0ef3d7388ff8a37cc683193dee15532b";
        private const string BASE_URL = "https://api.openweathermap.org/data/2.5/weather";
        private readonly HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();

            // ПРИВЯЗКА СОБЫТИЙ (если не привязались автоматически)
            button1.Click += button1_Click;
            button2.Click += button2_Click;

            // Настройка текстов
            label1.Text = "Город:";
            button1.Text = "Показать погоду";
            button2.Text = "Прогноз на 5 дней";
            // Очищаем поля
            label2.Text = "";
            label3.Text = "";
            label4.Text = "";
            label5.Text = "";
            label6.Text = "";
            label7.Text = "";
            label8.Text = "";
            listBox1.Items.Clear();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string city = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(city))
            {
                MessageBox.Show("Введите город!");
                return;
            }
            await ShowWeather(city);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            string city = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(city))
            {
                MessageBox.Show("Введите город!");
                return;
            }
            await ShowForecast(city);
        }

        private async Task ShowWeather(string city)
        {
            string url = $"{BASE_URL}?q={city}&appid={API_KEY}&units=metric&lang=ru";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JObject data = JObject.Parse(json);

                    label2.Text = $"Название: {data["name"]}";
                    label3.Text = $"Температура: {data["main"]["temp"]}°C";
                    label4.Text = $"Ощущается как: {data["main"]["feels_like"]}°C";
                    label5.Text = $"Влажность: {data["main"]["humidity"]}%";
                    label6.Text = $"Описание: {data["weather"][0]["description"]}";
                    label7.Text = $"Давление: {data["main"]["pressure"]} гПа";
                    label8.Text = $"Ветер: {data["wind"]["speed"]} м/с";
                }
                else
                {
                    MessageBox.Show($"Город '{city}' не найден.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async Task ShowForecast(string city)
        {
            string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={API_KEY}&units=metric&lang=ru&cnt=5";
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    JObject data = JObject.Parse(json);
                    var list = data["list"];

                    listBox1.Items.Clear();
                    listBox1.Items.Add($"ПРОГНОЗ НА 5 ДНЕЙ для {city}:");
                    foreach (var item in list)
                    {
                        string time = item["dt_txt"].ToString();
                        double temp = (double)item["main"]["temp"];
                        string desc = item["weather"][0]["description"].ToString();
                        listBox1.Items.Add($"{time}: {temp}°C, {desc}");
                    }
                }
                else
                {
                    MessageBox.Show($"Город '{city}' не найден.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}