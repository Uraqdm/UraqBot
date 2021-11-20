using System;
using System.Linq;
using VkNet.Model;

namespace UraqBot.VkBot
{
    class BotCommand
    {
        #region props
        public static BotCommand[] Commands { get; }

        /// <summary>
        /// Sequence of key words or key sentences
        /// </summary>
        public string[] KeyWords { get; private set; }

        /// <summary>
        /// Command action
        /// </summary>
        public Func<object, string> CommandAction { get; private set; }

        /// <summary>
        /// Command description
        /// </summary>
        public string Description { get; private set; }

        #endregion

        #region ctors
        static BotCommand()
        {
            Commands = new BotCommand[]
            {
                new BotCommand(new string[] { "привет", "бот" }, Hello, "Приветствие"),
                new BotCommand(new string[] { "бот", "ты", "тут" }, Check, "Проверяет тут ли бот или нет. Если бот тут, то он ответит утвердительно"),
                new BotCommand(new string[] { "бот", "сколько" } , HowMuch, "Отвечает сколько в диапазоне от 0 до 1000"),
                new BotCommand(new string[] { "кто", "бот"}, WhoIs, "Выбирает случайного участника группы и говорит его имя и фамилию"),
                new BotCommand(new string[] { "чья", "я", "девочка" }, WhoIsMyMaster),
                new BotCommand(new string[] { "бот", "когда" }, WhenIs, "Говорит случайную дату в промежутке между сегодняшним днем и 50 следующими"),
                new BotCommand(new string[] { "команды" }, ShowCommands, "Отвечает на ваши мольбы о помощи")
            };
        }

        

        public BotCommand(string[] keyWords, Func<object, string> command, string description = "Для этой команды нет описания")
        {
            KeyWords = keyWords;
            CommandAction = command;
            Description = description;
        }

        #endregion

        #region command methods

        private static string ShowCommands(object arg)
        {
            string res = "Ключевые слова должны содержаться в вашем сообщении в любом порядке. Бот не чувствителен к регистру\n";
            foreach (var item in Commands)
            {
                res += "Ключевые слова: \"";
                foreach (var word in item.KeyWords)
                {
                    res += word + " ";
                }
                res += "\"";
                res += $" / Описание: {item.Description}\n------------\n";
            }

            return res;
        }
        private static string WhenIs(object arg) => $"Я думаю, что {DateTime.Now.AddDays(new Random().Next(0, 50)).ToString("dd MM yyyy")}";
        private static string Hello(object param) => "Привет";
        private static string Check(object param) => "Да";
        private static string WhoIs(object user) => $"{GetUserName(user as User)}";
        private static string WhoIsMyMaster(object user) => $"Ты девочка {GetUserName(user as User)}";
        private static string HowMuch(object param) => $"Где-то примерно {new Random().Next(0, 1000)}";

        #endregion

        #region methods

        private static string GetUserName(User user)
        {
            return $"{user.FirstName} {user.LastName}";
        }

        public static BotCommand GetBotCommand(string text)
        {
            var keyWords = text.ToLower()
                                .Trim(',', '?', '.')
                                .Split(" ");

            var result = Commands.Where(x => keyWords.Intersect(x.KeyWords).Count() == x.KeyWords.Length).FirstOrDefault();

            return result;
        }

        #endregion
    }
}
