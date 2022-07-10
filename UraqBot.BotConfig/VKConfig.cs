namespace UraqBot.BotConfig
{
    public class VKConfig
    {
        #region fields

        private static readonly VKConfig _config;

        #endregion

        #region props

        public string AccessToken { get; }

        public ulong GroupId { get; }

        public static VKConfig Instance => _config;

        #endregion

        #region ctor

        static VKConfig()
        {
            _config = new VKConfig();
        }

        public VKConfig()
        {
            AccessToken = "Paste your token here";
            GroupId = 123456789; // Paste your group id here
        }

        #endregion
    }
}
