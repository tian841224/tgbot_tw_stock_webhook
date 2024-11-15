using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TGBot_TW_Stock_Webhook.Dto;
using TGBot_TW_Stock_Webhook.Interface;
using TGBot_TW_Stock_Webhook.Services.Web;

namespace TGBot_TW_Stock_Webhook.Services;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> _logger, IBotService botService, Lazy<TradingView> tradingView, Lazy<Cnyes> cnyes)
{
    private static readonly InputPollOption[] PollOptions = ["Hello", "World!"];
    private readonly IBotService _botService = botService;
    private readonly Lazy<TradingView> _tradingView = tradingView;
    private readonly Lazy<Cnyes> _cnyes = cnyes;

    public async Task HandleErrorAsync(Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HandleError: {Exception}", exception);
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message, cancellationToken),
            { EditedMessage: { } message } => OnMessage(message, cancellationToken),
            { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
            { InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
            { ChosenInlineResult: { } chosenInlineResult } => OnChosenInlineResult(chosenInlineResult),
            { Poll: { } poll } => OnPoll(poll),
            { PollAnswer: { } pollAnswer } => OnPollAnswer(pollAnswer),
            // ChannelPost:
            // EditedChannelPost:
            // ShippingQuery:
            // PreCheckoutQuery:
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    private async Task OnMessage(Message msg, CancellationToken cancellationToken)
    {
        _logger.LogInformation("收到消息類型: {MessageType}", msg.Type);
        try
        {

            if (msg.Text is not { } messageText)
                return;

            var parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return;

            var command = parts[0].ToLowerInvariant();

            switch (command)
            {
                case "/start":
                case "hello":
                    await _botService.SendHelloMessageAsync(msg, cancellationToken);
                    break;

                case "/chart":
                case "/range":
                case "/k":
                case "/v":
                case "/p":
                case "/n":
                    if (parts.Length < 2 || !int.TryParse(parts[1], out _))
                    {
                        await _botService.SendErrorMessageAsync(msg, cancellationToken);
                        return;
                    }

                    await ProcessStockCommand(command, parts, msg, cancellationToken);
                    break;

                default:
                    await _botService.SendErrorMessageAsync(msg, cancellationToken);
                    break;
            }
        }

        catch (Exception ex)
        {
            _logger.LogError($"Error:{ex.Message}");
            _logger.LogError($"UserId：{msg.Chat.Id}\n" + $"Username：{msg.Chat.Username}\n");
            await _botService.ErrorNotify(msg, ex.Message, cancellationToken);
        }
    }

    async Task<Message> Usage(Message msg)
    {
        const string usage = """
                <b><u>Bot menu</u></b>:
                /photo          - send a photo
                /inline_buttons - send inline buttons
                /keyboard       - send keyboard buttons
                /remove         - remove keyboard buttons
                /request        - request location or contact
                /inline_mode    - send inline-mode results list
                /poll           - send a poll
                /poll_anonymous - send an anonymous poll
                /throw          - what happens if handler fails
            """;
        return await bot.SendMessage(msg.Chat, usage, parseMode: ParseMode.Html, replyMarkup: new ReplyKeyboardRemove());
    }

    async Task<Message> SendPhoto(Message msg)
    {
        await bot.SendChatAction(msg.Chat, ChatAction.UploadPhoto);
        await Task.Delay(2000); // simulate a long task
        await using var fileStream = new FileStream("Files/bot.gif", FileMode.Open, FileAccess.Read);
        return await bot.SendPhoto(msg.Chat, fileStream, caption: "Read https://telegrambots.github.io/book/");
    }

    // Send inline keyboard. You can process responses in OnCallbackQuery handler
    async Task<Message> SendInlineKeyboard(Message msg)
    {
        var inlineMarkup = new InlineKeyboardMarkup()
            .AddNewRow("1.1", "1.2", "1.3")
            .AddNewRow()
                .AddButton("WithCallbackData", "CallbackData")
                .AddButton(InlineKeyboardButton.WithUrl("WithUrl", "https://github.com/TelegramBots/Telegram.Bot"));
        return await bot.SendMessage(msg.Chat, "Inline buttons:", replyMarkup: inlineMarkup);
    }

    async Task<Message> SendReplyKeyboard(Message msg)
    {
        var replyMarkup = new ReplyKeyboardMarkup(true)
            .AddNewRow("1.1", "1.2", "1.3")
            .AddNewRow().AddButton("2.1").AddButton("2.2");
        return await bot.SendMessage(msg.Chat, "Keyboard buttons:", replyMarkup: replyMarkup);
    }

    async Task<Message> RemoveKeyboard(Message msg)
    {
        return await bot.SendMessage(msg.Chat, "Removing keyboard", replyMarkup: new ReplyKeyboardRemove());
    }

    async Task<Message> RequestContactAndLocation(Message msg)
    {
        var replyMarkup = new ReplyKeyboardMarkup(true)
            .AddButton(KeyboardButton.WithRequestLocation("Location"))
            .AddButton(KeyboardButton.WithRequestContact("Contact"));
        return await bot.SendMessage(msg.Chat, "Who or Where are you?", replyMarkup: replyMarkup);
    }

    async Task<Message> StartInlineQuery(Message msg)
    {
        var button = InlineKeyboardButton.WithSwitchInlineQueryCurrentChat("Inline Mode");
        return await bot.SendMessage(msg.Chat, "Press the button to start Inline Query\n\n" +
            "(Make sure you enabled Inline Mode in @BotFather)", replyMarkup: new InlineKeyboardMarkup(button));
    }

    async Task<Message> SendPoll(Message msg)
    {
        return await bot.SendPoll(msg.Chat, "Question", PollOptions, isAnonymous: false);
    }

    async Task<Message> SendAnonymousPoll(Message msg)
    {
        return await bot.SendPoll(chatId: msg.Chat, "Question", PollOptions);
    }

    static Task<Message> FailingHandler(Message msg)
    {
        throw new NotImplementedException("FailingHandler");
    }

    // Process Inline Keyboard callback data
    private async Task OnCallbackQuery(CallbackQuery callbackQuery)
    {
        _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);
        await bot.AnswerCallbackQuery(callbackQuery.Id, $"Received {callbackQuery.Data}");
        await bot.SendMessage(callbackQuery.Message!.Chat, $"Received {callbackQuery.Data}");
    }

    #region Inline Mode

    private async Task OnInlineQuery(InlineQuery inlineQuery)
    {
        _logger.LogInformation("Received inline query from: {InlineQueryFromId}", inlineQuery.From.Id);

        InlineQueryResult[] results = [ // displayed result
            new InlineQueryResultArticle("1", "Telegram.Bot", new InputTextMessageContent("hello")),
            new InlineQueryResultArticle("2", "is the best", new InputTextMessageContent("world"))
        ];
        await bot.AnswerInlineQuery(inlineQuery.Id, results, cacheTime: 0, isPersonal: true);
    }

    private async Task OnChosenInlineResult(ChosenInlineResult chosenInlineResult)
    {
        _logger.LogInformation("Received inline result: {ChosenInlineResultId}", chosenInlineResult.ResultId);
        await bot.SendMessage(chosenInlineResult.From.Id, $"You chose result with Id: {chosenInlineResult.ResultId}");
    }

    #endregion

    private Task OnPoll(Poll poll)
    {
        _logger.LogInformation("Received Poll info: {Question}", poll.Question);
        return Task.CompletedTask;
    }

    private async Task OnPollAnswer(PollAnswer pollAnswer)
    {
        var answer = pollAnswer.OptionIds.FirstOrDefault();
        var selectedOption = PollOptions[answer];
        if (pollAnswer.User != null)
            await bot.SendMessage(pollAnswer.User.Id, $"You've chosen: {selectedOption.Text} in poll");
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }

    private async Task ProcessStockCommand(string command, string[] parts, Message message, CancellationToken cancellationToken)
    {
        var stockNumber = parts[1];
        Message reply = new Message();

        try
        {
            reply = await _botService.SendWaitMessageAsync(message, cancellationToken);

            switch (command)
            {
                case "/chart":
                    await _tradingView.Value.GetChartAsync(stockNumber, message, cancellationToken);
                    break;
                case "/range":
                    var range = parts.Length > 2 ? parts[2] : null;
                    await _tradingView.Value.GetRangeAsync(stockNumber, message, range, cancellationToken);
                    break;
                case "/k":
                    var kRange = parts.Length > 2 ? GetKRange(parts[2]) : "日K";
                    await _cnyes.Value.GetKlineAsync(stockNumber, message,cancellationToken, kRange);
                    break;
                case "/v":
                    await _cnyes.Value.GetDetialPriceAsync(stockNumber, message, cancellationToken);
                    break;
                case "/p":
                    await _cnyes.Value.GetPerformanceAsync(stockNumber, message, cancellationToken);
                    break;
                case "/n":
                    await _cnyes.Value.GetNewsAsync(stockNumber, message, cancellationToken);
                    break;
            }
        }
        catch
        {
            throw;
        }
        finally
        {
            if (reply != null)
            {
                await _botService.DeleteMessageAsync(new DeleteMessageDto
                {
                    Message = message,
                    Reply = reply,
                    CancellationToken = cancellationToken
                });
            }
        }
    }

    private string GetKRange(string input)
    {
        return input.ToLowerInvariant() switch
        {
            "h" => "分時",
            "d" => "日K",
            "w" => "週K",
            "m" => "月K",
            "5m" => "5分",
            "10m" => "10分",
            "15m" => "15分",
            "30m" => "30分",
            "60m" => "60分",
            _ => "日K"
        };
    }
}
