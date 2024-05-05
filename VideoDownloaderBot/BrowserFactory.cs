using PuppeteerSharp;

namespace Xoggas.VideoDownloaderBot;

public static class BrowserRunner
{
    private static readonly string? s_browserPath = Environment.GetEnvironmentVariable("PUPPETEER_PATH");

    private static readonly LaunchOptions s_launchOptions = new()
    {
        Headless = true,
        Args = new[] { "--no-sandbox", "--disable-gpu" },
        ExecutablePath = s_browserPath
    };

    public static async Task<IBrowser> Run()
    {
        return await Puppeteer.LaunchAsync(s_launchOptions);
    }
}