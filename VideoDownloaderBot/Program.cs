namespace Xoggas.VideoDownloaderBot;

public static class Program
{
    public static async Task Main()
    {
        var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");

        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentException("Token wasn't set!");
        }

        var bot = new Bot(token);

        bot.Start();

        await Task.Delay(-1);
    }
}