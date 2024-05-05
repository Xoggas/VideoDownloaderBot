using Xoggas.VideoDownloaderBot;

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run($"http://0.0.0.0:{port}");

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

if (string.IsNullOrEmpty(token))
{
    throw new ArgumentException("Token wasn't set!");
}

var bot = new Bot(token);

bot.Start();

await Task.Delay(-1);