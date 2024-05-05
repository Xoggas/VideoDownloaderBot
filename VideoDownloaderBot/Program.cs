using Xoggas.VideoDownloaderBot;

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder.Build();

app.MapGet("/", () => "Hello World!");

var appThread = new Thread(() => app.Run($"http://0.0.0.0:{port}"));

appThread.Start();

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

if (string.IsNullOrEmpty(token))
{
    throw new ArgumentException("Token wasn't set!");
}

var bot = new Bot(token);

bot.Start();

await Task.Delay(-1);