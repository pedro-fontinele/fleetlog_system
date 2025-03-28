namespace LOGHouseSystem.Model.API.Responses
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public string UserName { get; set; }

        public string UserRole { get; set; }
    }
}
