# UraqBot

UraqBot is simple bot for fun.

## About

UraqBot is simple bot, working with [VK](vk.com) API and providing several commands, including "help". You can use it to have fun with your friends in a group chat.

## Commands

Every command contains key words, that should be in your message. Order and case doesn't matter, but every key word should be in your message.

Every default command has russian description and russian key words, but if you want, you can change it via `BotCommand.cs` file. 

Type command "команды" if you want to see all available commadns.

## How to start a bot

To start a bot you need to get VK token and paste it into `VkConfig.cs` file located at "UraqBot.BotConfig".
Also you need to paste your group id. Then add bot into your group chat and give it admin rights(bot can't kick or ban members, so there is nothing to worry about).

After all preparations start bot as console app and enjoy it.

Also bot log every text messages at console.

You don't need to start a server or something. Just run it and enjoy

## How to add new commands

To add new command you should follow `BotCommand` class template.
New command should contain `string` array of key words and `Func<object, string>` action, that returns `string` message.
Also you can add a desription for your command if you want.
