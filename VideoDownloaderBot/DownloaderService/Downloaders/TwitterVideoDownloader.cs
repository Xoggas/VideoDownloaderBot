using PuppeteerSharp;

namespace Xoggas.VideoDownloaderBot;

public sealed class TwitterVideoDownloader : IVideoDownloader
{
    public string ServiceName => "Twitter";

    private readonly HttpClient _httpClient = new();

    public TwitterVideoDownloader()
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

        //Sets headers
        await page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
        {
            {
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 Edg/91.0.864.59"
            }
        });

        //Opens SaveTwitter page
        await page.GoToAsync("https://twitsave.com/");

        //Puts text into the URI input field
        await page.TypeAsync("#url-input", pageUri);

        //Clicks the download button
        await page.ClickAsync("button#download");

        //Waits for the download page to load
        await page.WaitForSelectorAsync(".dl-success", new WaitForSelectorOptions
        {
            Timeout = 30_000
        });

        //Gets the download button
        var downloadButtons = await page.QuerySelectorAllAsync(".dl-success");

        //Gets download button with the highest quality
        IElementHandle? highestQualityDownloadButton = downloadButtons.First();

        //Gets the URI property of the button
        IJSHandle? downloadButtonHref = await highestQualityDownloadButton.GetPropertyAsync("href");

        //Returns the video uri
        return await downloadButtonHref.JsonValueAsync<string>();
    }
}