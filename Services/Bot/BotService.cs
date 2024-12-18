using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TGBot_TW_Stock_Webhook.Interface.Services;
using TGBot_TW_Stock_Webhook.Model.DTOs;

namespace TGBot_TW_Stock_Webhook.Services.Bot
{
    public class BotService(ILogger<NotificationService> _logger,ITelegramBotClient _botClient) : IBotService
    {
        public async Task<Message> SendTextMessageAsync(SendTextDto dto)
        {
            try
            {
                dto.CancellationToken.ThrowIfCancellationRequested();
                return await _botClient.SendMessage(
                    chatId: dto.Message.Chat.Id,
                    text: dto.Text,
                    replyMarkup: dto.ReplyMarkup,
                    parseMode: dto.ParseMode ?? ParseMode.Html,
                    cancellationToken: dto.CancellationToken);
            }
            catch (ApiRequestException ex)
            {
                _logger.LogError(ex.Message, "SendTextMessageAsync");
            }

            return new Message();
        }

        public async Task<Message> SendPhotoAsync(SendPhotoDto dto)
        {
            try
            {
                dto.CancellationToken.ThrowIfCancellationRequested();
                return await _botClient.SendPhoto(
                    caption: dto.Caption,
                    chatId: dto.Message.Chat.Id,
                    photo: dto.Photo,
                    parseMode: ParseMode.Html,
                    cancellationToken: dto.CancellationToken);
            }
            catch (ApiRequestException ex)
            {
                _logger.LogError(ex.Message, "SendPhotoAsync");
            }

            return new Message();
        }

        public async Task<Message> SendHelloMessageAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = $"Hello {message.Chat.FirstName} {message.Chat.LastName}",
                    ParseMode = ParseMode.Html,
                    CancellationToken = cancellationToken
                });
            }
            catch (ApiRequestException ex)
            {
                _logger.LogError(ex.Message, "SendPhotoAsync");
            }

            return new Message();
        }

        public async Task SendErrorMessageAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = "指令錯誤請重新輸入",
                    ParseMode = ParseMode.Html,
                    CancellationToken = cancellationToken
                });
            }
            catch (ApiRequestException ex)
            {
                _logger.LogError(ex.Message, "SendPhotoAsync");
            }

            return;
        }

        public async Task<Message> SendWaitMessageAsync(Message message, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await SendTextMessageAsync(new SendTextDto
                {
                    Message = message,
                    Text = $"<b>-讀取中，請稍後⏰-</b>",
                    ReplyMarkup = new ReplyKeyboardRemove(),
                    ParseMode = ParseMode.Html,
                    CancellationToken = cancellationToken
                });
            }
            catch (ApiRequestException ex)
            {
                _logger.LogError(ex.Message, "SendPhotoAsync");
            }

            return new Message();
        }

        public async Task DeleteMessageAsync(DeleteMessageDto dto)
        {
            try
            {
                dto.CancellationToken.ThrowIfCancellationRequested();
                await _botClient.DeleteMessage(chatId: dto.Message.Chat.Id,
                                               messageId: dto.Reply.MessageId,
                                               cancellationToken: dto.CancellationToken);
            }
            catch (ApiRequestException ex)
            {
                _logger.LogError(ex.Message, "SendPhotoAsync");
            }
            return ;
        }


        public async Task ErrorNotify(Message message, string errorMessage, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await _botClient.SendMessage(
                    text: $"UserId：{message.Chat.Id}\n" +
                    $"Username：{message.Chat.Username}\n" +
                    $"錯誤：\n {errorMessage}",
                    chatId: 806077724,
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken);
            }
            catch (ApiRequestException ex)
            {
                _logger.LogError(ex.Message, "SendPhotoAsync");
            }
            return;
        }
    }
}