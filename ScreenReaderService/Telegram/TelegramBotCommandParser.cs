namespace ScreenReaderService.Telegram
{
    public class TelegramBotCommandParser
    {
        private const string PAUSE_CMD = "/pause";
        private const string RESUME_CMD = "/resume";

        private const string GET_FILTERS_INFO_CMD = "/get_filters_info";
        private const string GET_CONSTRAINTS_INFO_CMD = "/get_constraints_info";

        private const string DELIVERED_CMD = "/delivered";

        public Command<string> ParseCommand(string text)
        {
            if (text == null)
                return null;

            if (text == PAUSE_CMD)
                return new Command<string>(CommandType.PAUSE, null);

            if(text == RESUME_CMD)
                return new Command<string>(CommandType.RESUME, null);

            if (text == GET_FILTERS_INFO_CMD)
                return new Command<string>(CommandType.GET_FILTERS_INFO, null);

            if (text == GET_CONSTRAINTS_INFO_CMD)
                return new Command<string>(CommandType.GET_CONSTRAINTS_INFO, null);

            if (text.StartsWith(DELIVERED_CMD))
            {
                string id = text.Replace(DELIVERED_CMD, "").Trim();
                return new Command<string>(CommandType.DELIVERED, id);
            }

            return null;
        }
    }
}
