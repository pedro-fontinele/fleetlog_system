using LOGHouseSystem.Adapters.Extensions.Discord.Models;
using RestSharp;
using System.Net;

namespace LOGHouseSystem.Adapters.Extensions.Discord
{
    public class DiscordExtension
    {
        public async Task AddLogInDiscord(Embed content, string channelUrl = null)
        {
            var url = string.IsNullOrEmpty(channelUrl) ? Environment.DiscordWebHookUrl : channelUrl;

            switch (content.color)
            {
                case DiscordStatusColor.ERROR:
                    content.title = $":x: **[{content.title}]** :x:";
                    break;
                case DiscordStatusColor.SUCCESS:
                    content.title = $":white_check_mark: **[{content.title}]** :white_check_mark:";
                    break;
                case DiscordStatusColor.WARNING:
                    content.title = $":warning: **[{content.title}]** :warning:";
                    break;
            }


            List<Embed> embedArray = new List<Embed>();
            embedArray.Add(content);

            DiscordContent discordContent = new DiscordContent()
            {
                embeds = embedArray
            };

            try
            {
                RestClient client = new RestClient(url);
                RestRequest request = new RestRequest("", Method.Post);
                request.AddJsonBody(discordContent);
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                client.ExecuteAsync(request);
            }
            catch (Exception ex)
            {

            }
            
        }
    }
}
