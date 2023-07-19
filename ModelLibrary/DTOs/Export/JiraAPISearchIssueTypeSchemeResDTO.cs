using Newtonsoft.Json;

namespace ModelLibrary.DTOs.Export
{
    public class JiraAPISearchIssueTypeSchemeResDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class AvatarUrls
        {
            [JsonProperty("48x48")]
            public string _48x48 { get; set; }

            [JsonProperty("24x24")]
            public string _24x24 { get; set; }

            [JsonProperty("16x16")]
            public string _16x16 { get; set; }

            [JsonProperty("32x32")]
            public string _32x32 { get; set; }
        }

        public class ProjectCategory
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
        }

        public class Projects
        {
            public int? MaxResults { get; set; }
            public int? StartAt { get; set; }
            public int? Total { get; set; }
            public bool? IsLast { get; set; }
            public List<Value> Values { get; set; }
        }

        public class Root
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public Projects Projects { get; set; }
        }

        public class Value
        {
            public string Self { get; set; }
            public string Id { get; set; }
            public string Key { get; set; }
            public string Name { get; set; }
            public string ProjectTypeKey { get; set; }
            public bool? Simplified { get; set; }
            public AvatarUrls AvatarUrls { get; set; }
            public ProjectCategory ProjectCategory { get; set; }
        }


    }
}
