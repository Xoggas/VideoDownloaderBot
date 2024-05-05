using PuppeteerSharp;

namespace Xoggas.VideoDownloaderBot;

public sealed class InstagramVideoDownloader : IVideoDownloader
{
    public string ServiceName => "Instagram";

    private static readonly HttpClient s_httpClient = new();

    public async Task<Stream> Download(string pageUri)
    {
        var videoUri = await GetVideoUri(pageUri);

        if (videoUri is null)
        {
            throw new NullReferenceException();
        }

        return await s_httpClient.GetStreamAsync(videoUri);
    }

    private static async Task<string?> GetVideoUri(string pageUri)
    {
        //Setups browser instance
        await using IBrowser browser = await BrowserRunner.Run();

        //Opens the empty page
        await using IPage? page = await browser.NewPageAsync();

        //Opens the reel link
        await page.GoToAsync(pageUri);

        //Waits for pre-loader to disappear
        await page.WaitForSelectorAsync("video", new WaitForSelectorOptions
        {
            Timeout = 10_000
        });

        //Queries the video element
        IElementHandle? videoFrame = await page.QuerySelectorAsync("video");

        //Queries a src attribute of the video
        IJSHandle? srcHandle = await videoFrame.GetPropertyAsync("src");

        //Returns the video uri
        return await srcHandle.JsonValueAsync<string>();
    }
}