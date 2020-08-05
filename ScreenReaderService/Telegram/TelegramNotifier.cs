using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using RestSharp;

using Newtonsoft.Json;

namespace ScreenReaderService.Telegram
{
    public class TelegramNotifier
    {
        private const string BOT_ENDPOINT = "https://api.telegram.org/bot";

        private const string SEND_MESSAGE_ENDPOINT = "sendMessage";
        private const string GET_UPDATES_ENDPOINT = "getUpdates";

        private const string TOKEN = "1257258578:AAFcCEnEkga8jS3nJ6jjRizx_Qudfgx9hME";
        private const string CHAT_ID = "-1001423642114";

        private static TelegramBotCommandParser CommandParser = new TelegramBotCommandParser();

        public async Task<IEnumerable<Command<string>>> GetUpdates(string username)
        {
            TelegramUpdatesInfo updates = await GetUpdates(-1);
            List<Command<string>> commands = new List<Command<string>>();

            if (!updates.Status)
                return commands;

            foreach (UpdateInfo update in updates.Updates)
            {
                Command<string> command = CommandParser.ParseCommand(update.Message?.Text);

                if (command != null)
                {
                    if (!UsernameEquals(update.Message.Sender.Username, username))
                    {
                        await GetUpdates(update.Id); //forgetting old or useless updates
                        return commands;
                    }
                    else
                        commands.Add(command);
                }
            }

            if (updates.Updates.Count() > 0)
                await GetUpdates(updates.Updates.Last().Id + 1); //forgetting old or useless updates

            return commands;
        }

        private bool UsernameEquals(string username, string usernameOther)
        {
            return username.Trim('@', ' ').ToLower() == usernameOther.Trim('@', ' ').ToLower();
        }

        private async Task<TelegramUpdatesInfo> GetUpdates(int offset = -1)
        {
            RestClient client = new RestClient($"{BOT_ENDPOINT}{TOKEN}/{GET_UPDATES_ENDPOINT}") { Timeout = -1 };
            RestRequest request = new RestRequest(Method.GET);

            if (offset != -1)
                request.AddParameter("offset", offset);

            IRestResponse response = await client.ExecuteAsync(request);
            return JsonConvert.DeserializeObject<TelegramUpdatesInfo>(response.Content);
        }

        public async Task NotifyMessage(string message)
        {
            RestClient client = new RestClient($"{BOT_ENDPOINT}{TOKEN}/{SEND_MESSAGE_ENDPOINT}") { Timeout = -1 };
            RestRequest request = new RestRequest(Method.POST);

            TelegramMessageDto dto = new TelegramMessageDto(
                CHAT_ID, message
            );

            string json = JsonConvert.SerializeObject(dto);
            request.AddJsonBody(json);

            IRestResponse response = await client.ExecuteAsync(request);
        }

        private class TelegramUpdatesInfo
        {
            [JsonProperty("ok")]
            public bool Status { get; private set; }

            [JsonProperty("result")]
            public IEnumerable<UpdateInfo> Updates { get; private set; }
        }

        private class TelegramMessageDto
        {
            [JsonProperty("chat_id")]
            public string ChatId { get; private set; }

            [JsonProperty("text")]
            public string Text { get; private set; }

            public TelegramMessageDto(string chatId, string text)
            {
                ChatId = chatId;
                Text = text;
            }
        }
    }
}
