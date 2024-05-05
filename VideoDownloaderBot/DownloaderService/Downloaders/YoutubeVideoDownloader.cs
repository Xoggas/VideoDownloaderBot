using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Xoggas.VideoDownloaderBot;

public sealed class YoutubeVideoDownloader : IVideoDownloader
{
    public string ServiceName => "YouTube Shorts";

    private readonly YoutubeClient _youtubeClient = new();

    public async Task<Stream> Download(string videoUri)
    {
        StreamManifest streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoUri);

        IVideoStreamInfo streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();

        return await _youtubeClient.Videos.Streams.GetAsync(streamInfo);
    }
}