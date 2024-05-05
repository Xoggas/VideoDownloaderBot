using PuppeteerSharp;

namespace Xoggas.VideoDownloaderBot;

public sealed class TikTokVideoDownloader : IVideoDownloader
{
    public string ServiceName => "TikTok";

    private readonly HttpClient _httpClient = new();

    public TikTokVideoDownloader()
    {
        _httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 Edg/91.0.864.59");
    }

    public async Task<Stream> Download(string pageUri)
    {
        var videoUri = await GetVideoUri(pageUri);

        if (videoUri is null)
        {
            throw new NullReferenceException();
        }

        return await _httpClient.GetStreamAsync(videoUri);
    }

    private static async Task<string?> GetVideoUri(string pageUri)
    {
        //Setups browser instance
        await using IBrowser browser = await BrowserRunner.Run();

        //Opens the empty page
        await using IPage? page = await browser.NewPageAsync();

        //Opens SnapTik page
        await page.GoToAsync("https://snaptik.app/");

        //Puts text into the URI input field
        await page.TypeAsync("input[name='url']", pageUri);

        //Clicks the download button
        await page.ClickAsync("button[type='submit']");

        //Waits for the download button to appear
        await page.WaitForSelectorAsync(".download-file", new WaitForSelectorOptions
        {
            Timeout = 60_000
        });

        //Gets the download button
        IElementHandle? downloadButton = await page.QuerySelectorAsync(".download-file");

        //Gets the URI property of the button
        IJSHandle? downloadButtonHref = await downloadButton.GetPropertyAsync("href");

        //Returns the video uri
        return await downloadButtonHref.JsonValueAsync<string>();
    }
}