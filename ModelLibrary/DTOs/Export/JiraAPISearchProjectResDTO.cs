using Newtonsoft.Json;

namespace ModelLibrary.DTOs.Export
{
    public class JiraAPISearchProjectResDTO
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

        public class Insight
        {
            public int? TotalIssueCount { get; set; }
            public DateTime? LastIssueUpdateTime { get; set; }
        }

        public class ProjectCategory
        {
            public string Self { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class Root
        {
            public string Self { get; set; }
            public string Id { get; set; }
            public string Key { get; set; }
            public string Name { get; set; }
            public AvatarUrls AvatarUrls { get; set; }
            public ProjectCategory ProjectCategory { get; set; }
            public bool? Simplified { get; set; }
            public string Style { get; set; }
            public Insight Insight { get; set; }
        }
    }
}
