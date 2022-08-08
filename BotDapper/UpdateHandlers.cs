

using DataAccess.Concrete;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotDapper
{

    public static class UpdateHandlers
    {
        public static Task PollingErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                // UpdateType.Poll:
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
                UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery!),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult!),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;
            }
#pragma warning disable CA1031
            catch (Exception exception)
#pragma warning restore CA1031
            {
                await PollingErrorHandler(botClient, exception, cancellationToken);
            }
        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Console.WriteLine($"Receive message type: {message.Type}");
            if (message.Text is not { } messageText)
                return;
            int number = 0;

            if (!messageText.Contains("Coin"))
            {

                var action = messageText.Split(' ')[0] switch
                {
                    "/inline" => Choose(botClient, message),
                    "/keyboard" => SendReplyKeyboard(botClient, message),
                    "/remove" => RemoveKeyboard(botClient, message),
                    "/request" => RequestContactAndLocation(botClient, message),
                    "/photo" => SendFile(botClient, message),
                    "/getcoins"=> ShowAllCoins(botClient, message),
                    _ => Usage(botClient, message),

                    //"/merhaba" => SendMerhaba(botClient, message),
                };
            }




            var action2 = message.Text.Split(' ')[0] switch
            {
                "/Coin1" => SendData(botClient, message, number = 1),
                "/Coin2" => SendData(botClient, message, number = 2),
                "/Coin3" => SendData(botClient, message, number = 3),
                "/Coin4" => SendData(botClient, message, number = 4),
                "/Coin5" => SendData(botClient, message, number = 5),

            };
            Message sentMessage = await action2;

            Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");

            static async Task<Message> SendData(ITelegramBotClient botClient, Message message, int number)
            {
                var _coinRepository = new CoinRepository();
                var myCoinData = _coinRepository.GetCoinById(number);
                var data = myCoinData.Id;
                var data1 = myCoinData.Price;
                var data2 = myCoinData.Name;
                var data3 = myCoinData.Change;

                if (data3 == true)
                {
                    await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                    const string filePath = @"C:\Users\90507\source\repos\BotDapper\BotDapper\Files\artıs.png";
                    using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                    return await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                                                          photo: new InputOnlineFile(fileStream, fileName),
                                                          caption: $"Coin Id : {data}\n" +
                                                          $"Coin Name : {data2}\n" +
                                                          $"Coin Price : {data1}");
                }
                else
                {
                    await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                    const string filePath = @"C:\Users\90507\source\repos\BotDapper\BotDapper\Files\dusus.png";
                    using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                    return await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                                                          photo: new InputOnlineFile(fileStream, fileName),
                                                          caption: $"Coin Id : {data}\n" +
                                                          $"Coin Name : {data2}\n" +
                                                          $"Coin Price : {data1}");

                }
            }



            // Send inline keyboard
            // You can process responses in BotOnCallbackQueryReceived handler
            static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message, int number)
            {
                var _coinRepository = new CoinRepository();
                var myCoinData = _coinRepository.GetCoinById(number);
                var data = myCoinData.Id;
                var data1 = myCoinData.Price;
                var data2 = myCoinData.Name;
                var data3 = myCoinData.Change;


                // Simulate longer running task
                await Task.Delay(500);

                InlineKeyboardMarkup inlineKeyboard = new(
                    new[]
                    {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("data1", $"Price : {data1}"),
                        InlineKeyboardButton.WithCallbackData("data2", $"Name : {data2}"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("data3", $"Change : {data3}"),
                        InlineKeyboardButton.WithCallbackData("data", $"Id : {data}"),
                    },

                    });

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "Choose",
                                                            replyMarkup: inlineKeyboard);
            }

            static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message)
            {
                ReplyKeyboardMarkup replyKeyboardMarkup = new(
                    new[]
                    {
                        new KeyboardButton[] { "/Coin1", "/Coin2" },
                        new KeyboardButton[] { "/Coin3", "/Coin4" },
                    })
                {
                    ResizeKeyboard = true
                };

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "Choose",
                                                            replyMarkup: replyKeyboardMarkup);
            }

            static async Task<Message> RemoveKeyboard(ITelegramBotClient botClient, Message message)
            {
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "Removing keyboard",
                                                            replyMarkup: new ReplyKeyboardRemove());
            }

            static async Task<Message> SendFile(ITelegramBotClient botClient, Message message)
            {
                await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string filePath = @"C:\Users\90507\source\repos\BotDapper\BotDapper\Files\tux.png";
                using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

                return await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                                                      photo: new InputOnlineFile(fileStream, fileName),
                                                      caption: "Nice Picture");
            }
            


            static async Task<Message> RequestContactAndLocation(ITelegramBotClient botClient, Message message)
            {
                ReplyKeyboardMarkup RequestReplyKeyboard = new(
                    new[]
                    {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                    });

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "Who or Where are you?",
                                                            replyMarkup: RequestReplyKeyboard);
            }


            static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
            {
                const string usage = "Usage:\n" +
                                     "/inline   - send inline keyboard\n" +
                                     "/keyboard - send custom keyboard\n" +
                                     "/remove   - remove custom keyboard\n" +
                                     "/photo    - send a photo\n" +
                                     "/request  - request location or contact";

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: usage,
                                                            replyMarkup: new ReplyKeyboardRemove());
            }
            static async Task<Message> Choose(ITelegramBotClient botClient, Message message)
            {
                const string choose = "Choose:\n" +
                                     "/Coin1\n" +
                                     "/Coin2\n" +
                                     "/Coin3\n" +
                                     "/Coin4\n" +
                                     "/Coin5";

                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: choose,
                                                            replyMarkup: new ReplyKeyboardRemove());


            }
            static async Task<Message> ShowAllCoins(ITelegramBotClient botClient, Message message)
            {
                string Coins = "";
                var _coinRepository = new CoinRepository();
                var myCoinData = _coinRepository.GetAllCoins();
                foreach (var coin in myCoinData)
                {
                    Coins = Coins+"Coin" + coin.Id +"--"+ coin.Name + "--" + coin.Price + "\n";
                    
                    
                }
                return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                           text: Coins,
                                                           replyMarkup: new ReplyKeyboardRemove());




            }
        }

        // Process Inline Keyboard callback data
        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await botClient.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}");

            await botClient.SendTextMessageAsync(
                chatId: callbackQuery.Message!.Chat.Id,
                text: $"Received {callbackQuery.Data}");
        }

        private static async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
        {
            Console.WriteLine($"Received inline query from: {inlineQuery.From.Id}");

            InlineQueryResult[] results = {
            // displayed result
            new InlineQueryResultArticle(
                id: "1",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent(
                    "hello"
                )
            )
        };

            await botClient.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
                                                   results: results,
                                                   isPersonal: true,
                                                   cacheTime: 0);
        }


        private static Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
    }
}