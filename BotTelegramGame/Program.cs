using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;


namespace BotTelegramGame
{
    class Program
    {

        static void Main(string[] args)
        {
            string token = File.ReadAllText(@"E:\TP\BotTelegram\file");
            TelegramBotClient bot = new TelegramBotClient(token);
            Console.WriteLine($"@{bot.GetMeAsync().Result.Username} start");

            int max = 5;
            Random rand = new Random();
            Dictionary<long, int> db = new Dictionary<long, int>();

            bot.OnMessage += (s, arg) =>
            {
                #region var

                string msgText = arg.Message.Text;
                string firstName = arg.Message.Chat.FirstName;
                string replyMsg = String.Empty;
                int msgId = arg.Message.MessageId;
                long chatId = arg.Message.Chat.Id;

                int user = 0;
                string path = $"id_{chatId.ToString().Substring(0, 5).Substring(0, 5)}";
                bool skip = false;

                Console.WriteLine($"{firstName}: {msgText}");

                #endregion

                if (!db.ContainsKey(chatId)
                || msgText == "/restart"
                || msgText.StartsWith("start")
                || msgText.ToLower().IndexOf("start") != -1
                )
                {
                    int startGame = rand.Next(20, 30);
                    db[chatId] = startGame;
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    skip = true;
                    replyMsg =  $"Загадано число: {db[chatId]}";

                }
                else
                {
                    if (db[chatId] <= 0) return;

                    int.TryParse(msgText, out user);
                    if (!(user >= 1 && user <= max))
                    {
                        skip = true;
                        replyMsg = $"Обнаружено читерство.\nЧисло: {db[chatId]}";
                    }
                    if (!skip)
                    {
                        db[chatId] -= user;

                        replyMsg = $"Ход {firstName}: {user}. Число: {db[chatId]}";
                        if (db[chatId] <= 0)
                        {
                            replyMsg = $"Ура. Победа, {firstName}!";
                            skip = true;
                        }
                    }
                }
                if (!skip)
                {
                    int temp = db[chatId] % (max + 1);
                    if (temp == 0) temp = rand.Next(max) + 1;

                    db[chatId] -= temp;
                    replyMsg = $"\nХод БОТА: {temp}\nЧисло: {db[chatId]}";
                    if (db[chatId] <= 0) replyMsg = $"Ура! Победа БОТА!";
                }
                Console.WriteLine($">>> {replyMsg}");

                /*Bitmap image = new Bitmap(200,200);
                Graphics graphics = Graphics.FromImage(image);
            
;
                graphics.DrawString(
                   s: replyMsg,
                   font: new Font("Consolas", 10),
                   brush: Brushes.Blue,
                   x: 10,
                   y: 100);

                path += $@"\file_{DateTime.Now.Ticks}.bmp";
                image.Save(path);
               
                bot.SendPhotoAsync(
                    chatId: chatId,

                    photo: new Telegram.Bot.Types.InputFiles.InputOnlineFile(new FileStream(path, FileMode.Open)),

                    replyToMessageId: msgId
                    );*/

                bot.SendTextMessageAsync(
                   chatId: chatId,
                    text: replyMsg,
                    replyToMessageId: msgId
                    );

            };

            bot.StartReceiving();
            Console.ReadKey();

        }
    }
}
