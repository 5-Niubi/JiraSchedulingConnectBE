using Newtonsoft.Json;

namespace ModelLibrary.DTOs.Export
{
    public class JiraAPIGetUsersResDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
        // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
        public class AvatarUrls
        {
            [JsonProperty("48x48")]
            public string _48x48;

            [JsonProperty("24x24")]
            public string _24x24;

            [JsonProperty("16x16")]
            public string _16x16;

            [JsonProperty("32x32")]
            public string _32x32;
        }

        public class Root
        {
            public string? self;
            public string? accountId;
            public string? accountType;
            public string? emailAddress;
            public AvatarUrls? avatarUrls;
            public string? displayName;
            public bool active;
            public string? timeZone;
            public string? locale;
        }
    }
}
