using Newtonsoft.Json;

namespace ModelLibrary.DTOs.Export
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CreateJiraUserDTO
    {

        public class Request
        {
            public string? displayName
            {
                get; set;
            }
            public string? emailAddress
            {
                get; set;
            }
            public string[]? products
            {
                get; set;
            }
        }
        public class Response
        {
            public string? self
            {
                get; set;
            }
            public string? key
            {
                get; set;
            }
            public string? accountId
            {
                get; set;
            }
            public string? accountType
            {
                get; set;
            }
            public string? name
            {
                get; set;
            }
            public string? emailAddress
            {
                get; set;
            }
            public string? displayName
            {
                get; set;
            }

        }
    }


}
