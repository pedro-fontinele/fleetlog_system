namespace LOGHouseSystem.Adapters.API.MercadoLivre.Response
{
    public class GetAccessTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
        public string scope { get; set; }
        public string refresh_token { get; set; }
        public long user_id { get; set; }
    }
}
