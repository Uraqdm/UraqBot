using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UraqBot.BotConfig;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Model;
using VkNet.Model.GroupUpdate;
using VkNet.Model.Keyboard;
using VkNet.Model.RequestParams;

namespace UraqBot.VkBot
{
    internal class Bot
    {
        #region fields

        /// <summary>
        /// VK API
        /// </summary>
        private VkApi _vkClient;

        /// <summary>
        /// Object, which helps to connect to the server and helps to recieve messages
        /// </summary>
        private LongPollServerResponse _longPollServerResponse;

        /// <summary>
        /// Bot config
        /// </summary>
        private VKConfig _vkConfig;

        /// <summary>
        /// Index of last recieved message
        /// </summary>
        private string _currentTs;

        #endregion

        #region ctor

        public Bot(VKConfig config, bool isShowLog = false)
        {
            _vkConfig = config;

            _vkClient = new VkApi();
            _vkClient.Authorize(new ApiAuthParams
            {
                AccessToken = config.AccessToken,
                Settings = Settings.All | Settings.Messages
            });

            _longPollServerResponse = _vkClient.Groups.GetLongPollServer(config.GroupId);

            _currentTs = _longPollServerResponse.Ts;

            if (isShowLog) ViewLog();
        }

        

        #endregion

        #region methods
        private void ViewLog()
        {
            Console.WriteLine($"longPollServerResponse.Key = {_longPollServerResponse.Key}");
            Console.WriteLine($"longPollServerResponse.Pts = {_longPollServerResponse.Pts}");

            Console.WriteLine($"longPollServerResponse.Ts = {_longPollServerResponse.Ts}");
            Console.WriteLine($"longPollServerResponse.Server = {_longPollServerResponse.Server}");
        }

        public void Start(Action<GroupUpdate> onMessage = null)
        {
            if (onMessage != null) OnMessage += onMessage;
            else OnMessage += DefaultOnMessage;

            new Thread(OnUpdate).Start();
        }

        private async void OnUpdate()
        {
            while (true)
            {
                var res = await _vkClient.Groups.GetBotsLongPollHistoryAsync
                (
                    new BotsLongPollHistoryParams
                    {
                        Key = _longPollServerResponse.Key,
                        Ts = _currentTs,
                        Server = _longPollServerResponse.Server
                    }   
                );

                if(OnMessage != null)
                {
                    foreach (var item in res.Updates)
                    {
                        _currentTs = res.Ts;
                        if (item?.Message?.RandomId != 0) continue;

                        OnMessage.Invoke(item);

                        // only 20 msg per second
                        Thread.Sleep(100);
                    }
                }
                Thread.Sleep(2000);
            }
            
        }

        private async void DefaultOnMessage(GroupUpdate e)
        {
            Console.WriteLine();

            Console.WriteLine(string.Format(
                "Type: {0}\n" + 
                "PeerId {1}\n" +
                "Text: {2}\n" +
                "FromId: {3}\n" +
                "FirstName {4}\n",
                e.Type,
                e.Message.PeerId,
                e?.Message?.Text,
                e?.Message?.FromId,
                (await GetUserInfo(e.Message.FromId.Value)).FirstName));

            var answer = await GetAnswer(e?.Message?.Text, e.Message.PeerId.Value);

            if (answer != null)
                SendMessage( peerId: e.Message.PeerId.Value, text: answer, replyToMessageId: e.Message.Id);
        }

        private async void SendMessage(long peerId, string text, long? replyToMessageId)
        {
            var msg = new MessagesSendParams
            {
                RandomId = Guid.NewGuid().GetHashCode(),
                Message = text,
                PeerId = peerId
            };

            if (replyToMessageId != -1) msg.ReplyTo = replyToMessageId;

            await _vkClient.Messages.SendAsync(msg);
        }

        /// <summary>
        /// Gets command depends of sended message
        /// </summary>
        /// <param name="text">message text</param>
        /// <returns>answer</returns>
        private async Task<string> GetAnswer(string text, long peerId)
        {
            string result = null;

            #region commands
            try
            {
                var command = BotCommand.GetBotCommand(text);

                if(command != null)
                {
                    var user = await GetRandomUser(peerId);
                    result = command.CommandAction.Invoke(user);
                }
            }
            catch (Exception ex)
            {
                result = "Ты меня сломал :(" + $"\nErrorMessage - {ex.Message}";
            }
            #endregion

            return result;
        }

        /// <summary>
        /// Gets user info by sender id
        /// </summary>
        /// <param name="fromId">sender id</param>
        /// <returns>sender as User</returns>
        private async Task<User> GetUserInfo(long fromId)
        {
            var result = await _vkClient.Users.GetAsync(new long[] { fromId });
            return result.FirstOrDefault();
        }

        private async Task<User> GetRandomUser(long peerId)
        {
            try
            {
                var users = await GetUsersAsync(peerId);
                var randUserId = new Random().Next(0, users.Count() - 1);

                var badUser = await GetUserInfo(users[randUserId]);

                return badUser;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        private async Task<long[]> GetUsersAsync(long peerId)
        {
            var test = await _vkClient.Messages.GetConversationMembersAsync(peerId);
            return test.Items.Select(x => x.MemberId).ToArray();

            //var chat = await _vkClient.Messages.GetChatAsync(chatId);
            //return chat.Users.ToArray();
        }

        #endregion

        #region events

        /// <summary>
        /// Event, executing when new message recieved
        /// </summary>
        public event Action<GroupUpdate> OnMessage;

        #endregion
    }
}