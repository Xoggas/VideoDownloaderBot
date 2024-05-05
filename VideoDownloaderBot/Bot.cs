using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Xoggas.VideoDownloaderBot;

public sealed class Bot
{
    private readonly TelegramBotClient _bot;
    private readonly VideoDownloaderService _videoDownloaderService;

    public Bot(string token)
    {
        _bot = new TelegramBotClient(token);
        _videoDownloaderService = new VideoDownloaderService();
    }

    public void Start()
    {
        SetupServices();
        ListenToMessages();
    }

    private void SetupServices()
    {
        _videoDownloaderService.AddDownloader(@"https?:\/\/(www\.)?youtube.com\/shorts\/.+",
            new YoutubeVideoDownloader());

        _videoDownloaderService.AddDownloader(@"https?:\/\/(?:www\.tiktok\.com\/@.+\/video\/\d+.+|vm\.tiktok\.com\/.+)",
            new TikTokVideoDownloader());

        _videoDownloaderService.AddDownloader(@"https?:\/\/www\.instagram\.com\/reel\/.+",
            new InstagramVideoDownloader());

        _videoDownloaderService.AddDownloader(@"https?:\/\/(?:x|twitter)\.com\/.+",
            new TwitterVideoDownloader());
    }

    private void ListenToMessages()
    {
        using CancellationTokenSource cancellationTokenSource = new();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = new[] { UpdateType.Message }
        };

        _bot.StartReceiving(
            HandleRequestsAsync,
            HandleErrorsAsync,
            receiverOptions,
            cancellationTokenSource.Token
        );
    }

    private async Task HandleRequestsAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Message is not { } message)
        {
            return;
        }

        if (message.Text is not { } videoUri)
        {
            return;
        }

        var chatId = message.Chat.Id;

        IVideoDownloader? videoDownloader = _videoDownloaderService.GetDownloader(videoUri);

        if (videoDownloader is null)
        {
            await bot.SendTextMessageAsync(chatId, "The service isn't supported!", cancellationToken: ct);

            return;
        }

        await bot.SendTextMessageAsync(chatId, $"Downloading video from {videoDownloader.ServiceName}.",
            cancellationToken: ct);

        try
        {
            Stream videoStream = await videoDownloader.Download(videoUri);

            await bot.SendVideoAsync(chatId, InputFile.FromStream(videoStream), cancellationToken: ct);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);

            await bot.SendTextMessageAsync(chatId, "Error occured!", cancellationToken: ct);
        }
    }

    private static Task HandleErrorsAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);

        return Task.CompletedTask;
    }
}