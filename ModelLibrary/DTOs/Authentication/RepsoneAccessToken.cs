namespace ModelLibrary.DTOs.Authentication
{
    public class RepsoneAccessToken
    {
        public string? access_token
        {
            get; set;
        }
        public string? refresh_token
        {
            get; set;
        }

        // Time expired of access_token
        public int expires_in
        {
            get; set;
        }
        public string scope
        {
            get; set;
        }
    }
}
