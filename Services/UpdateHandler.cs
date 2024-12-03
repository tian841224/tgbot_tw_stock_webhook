using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using TGBot_TW_Stock_Webhook.Interface.Services;
using TGBot_TW_Stock_Webhook.Model.DTOs;

namespace TGBot_TW_Stock_Webhook.Services;

public class UpdateHandler(ITelegramBotClient bot, ILogger<UpdateHandler> _logger, IBotService botService, ICommandFactory commandFactory)
{
    private static readonly InputPollOption[] PollOptions = ["Hello", "World!"];
    private readonly IBotService _botService = botService;
    private readonly ICommandFactory _commandFactory = commandFactory;
    public async Task HandleErrorAsync(Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HandleError: {Exception}", exception);
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await (update switch
        {
            { Message: { } message } => OnMessage(message, cancellationToken),
            { EditedMessage: { } message } => OnMessage(message, cancellationToken),
            //{ CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
            //{ InlineQuery: { } inlineQuery } => OnInlineQuery(inlineQuery),
            //{ ChosenInlineResult: { } chosenInlineResult } => OnChosenInlineResult(chosenInlineResult),
            //{ Poll: { } poll } => OnPoll(poll),
            //{ PollAnswer: { } pollAnswer } => OnPollAnswer(pollAnswer),
            // ChannelPost:
            // EditedChannelPost:
            // ShippingQuery:
            // PreCheckoutQuery:
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    private async Task OnMessage(Message msg, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _logger.LogInformation("收到消息類型: {MessageType}", msg.Type);

        Message? reply = null;
        try
        {
            if (msg.Text is not { } messageText)
                return;

            var parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = parts.FirstOrDefault()?.ToLowerInvariant();

            if (command == null)
            {
                await _botService.SendErrorMessageAsync(msg, cancellationToken);
                return;
            }

            switch (command)
            {
                case "hello":
                    await _botService.SendHelloMessageAsync(msg, cancellationToken);
                    break;
                case "/use":
                case "/start":
                    await Usage(msg);
                    break;
                default:
                    var cmd = _commandFactory.GetCommand(command);
                    if (cmd == null)
                    {
                        await _botService.SendErrorMessageAsync(msg, cancellationToken);
                        return;
                    }

                    var (arg1, arg2) = parts.Length switch
                    {
                        >= 3 => (parts[1], parts[2]),
                        2 => (parts[1], null),
                        _ => (null, null)
                    };

                    await cmd.ExecuteAsync(msg, cancellationToken, arg1, arg2);
                    break;
            }
        }

        catch (Exception ex)
        {
            _logger.LogError($"Error:{ex.Message}");
            _logger.LogError($"UserId：{msg.Chat.Id}\n" + $"Username：{msg.Chat.Username}\n");
            await _botService.ErrorNotify(msg, ex.Message, cancellationToken);
        }
        finally
        {
            if (reply != null)
            {
                await _botService.DeleteMessageAsync(new DeleteMessageDto
                {
                    Message = msg,
                    Reply = reply,
                    CancellationToken = cancellationToken
                });
            }
        }
    }

    async Task<Message> Usage(Message msg)
    {
        const string usage = """
            *台股機器人指令指南*

            *K線圖表指令*

            *📊 基本K線圖*
            格式：`/k [股票代碼] [時間範圍]`

            時間範圍選項（預設：d）：
            - `h` - 時K線
            - `d` - 日K線 
            - `w` - 週K線
            - `m` - 月K線
            - `5m` - 5分K線
            - `15m` - 15分K線
            - `30m` - 30分K線
            - `60m` - 60分K線

            *📈 TradingView K線圖*
            格式：`/c [股票代碼] [時間範圍]`

            時間範圍選項（預設：1d）：
            - `1d` - 一日
            - `5d` - 五日
            - `1m` - 一個月
            - `3m` - 三個月
            - `6m` - 六個月
            - `ytd` - 今年度
            - `1y` - 一年
            - `5y` - 五年
            - `all` - 全部時間

            *股票資訊指令*
            - `/d [股票代碼]` - 查詢股票詳細資訊
            - `/p [股票代碼]` - 查詢股票績效
            - `/n [股票代碼]` - 查詢股票新聞
            - `/yn [股票代碼]` - 查詢Yahoo股票新聞（預設：台股新聞）
            - `/i [股票代碼]` - 查詢當日收盤資訊

            *市場總覽指令*
            - `/m` - 查詢大盤資訊
            - `/t` - 查詢當日交易量前20名
            """;
        return await bot.SendMessage(msg.Chat, usage, parseMode: ParseMode.Markdown, replyMarkup: new ReplyKeyboardRemove());
    }

    // 範例程式
#if DEBUG
    async Task<Message> SendPhoto(Message msg)
    {
        await bot.SendChatAction(msg.Chat, ChatAction.UploadPhoto);
        await Task.Delay(2000); // simulate a long task
        await using var fileStream = new FileStream("Files/bot.gif", FileMode.Open, FileAccess.Read);
        return await bot.SendPhoto(msg.Chat,
                                   fileStream,
                                   caption: "Read https://telegrambots.github.io/book/");
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

#endif

}
