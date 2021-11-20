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
            AccessToken = "702e57bce171c7900bf81a136c6918d491c0df6c6232061ec828af7cead0f522cd8870e74634a5bc3de5a";
            GroupId = 208935129;
        }

        #endregion
    }
}
