namespace LOGHouseSystem.Adapters.Extensions.Discord.Models
{
    public enum DiscordStatusColor
    {
        ERROR = 16711680,
        WARNING = 16776960,
        SUCCESS = 8388352
    }

    public class DiscordContent
    {
        public List<Embed> embeds { get; set; }
    }

    public class Embed
    {
        public string title { get; set; }
        public DiscordStatusColor color { get; set; }
        public string description { get; set; }
    }
}
