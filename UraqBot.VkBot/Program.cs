using System;
using UraqBot.BotConfig;

namespace UraqBot.VkBot
{
    class Program
    {
        static void Main(string[] args)
        {
            new Bot(VKConfig.Instance).Start();

            Console.WriteLine("Press enter to stop bot.");
            Console.ReadLine();
        }
    }
}
