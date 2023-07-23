using Newtonsoft.Json;

namespace ModelLibrary.DTOs.Export
{
    public class JiraAPIGetProjectResDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Assignee
        {
            public string self { get; set; }
            public string key { get; set; }
            public string accountId { get; set; }
            public string accountType { get; set; }
            public string name { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
        }

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

        public class Component
        {
            public string self { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public Lead lead { get; set; }
            public string assigneeType { get; set; }
            public Assignee assignee { get; set; }
            public string realAssigneeType { get; set; }
            public RealAssignee realAssignee { get; set; }
            public bool isAssigneeTypeValid { get; set; }
            public string project { get; set; }
            public int projectId { get; set; }
        }

        public class Insight
        {
            public int totalIssueCount { get; set; }
            public DateTime lastIssueUpdateTime { get; set; }
        }

        public class IssueType
        {
            public string self { get; set; }
            public string id { get; set; }
            public string description { get; set; }
            public string iconUrl { get; set; }
            public string name { get; set; }
            public bool subtask { get; set; }
            public int avatarId { get; set; }
            public int hierarchyLevel { get; set; }
            public string entityId { get; set; }
            public Scope scope { get; set; }
        }

        public class Lead
        {
            public string self { get; set; }
            public string key { get; set; }
            public string accountId { get; set; }
            public string accountType { get; set; }
            public string name { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
        }

        public class Project
        {
            public string id { get; set; }
        }

        public class ProjectCategory
        {
            public string self { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
        }

        public class Properties
        {
            public string propertyKey { get; set; }
        }

        public class RealAssignee
        {
            public string self { get; set; }
            public string key { get; set; }
            public string accountId { get; set; }
            public string accountType { get; set; }
            public string name { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public string displayName { get; set; }
            public bool active { get; set; }
        }

        public class Roles
        {
            public string Developers { get; set; }
        }

        public class Root
        {
            public string self { get; set; }
            public string id { get; set; }
            public string key { get; set; }
            public string description { get; set; }
            public Lead lead { get; set; }
            public List<Component> components { get; set; }
            public List<IssueType> issueTypes { get; set; }
            public string url { get; set; }
            public string email { get; set; }
            public string assigneeType { get; set; }
            public List<object> versions { get; set; }
            public string name { get; set; }
            public Roles roles { get; set; }
            public AvatarUrls avatarUrls { get; set; }
            public ProjectCategory projectCategory { get; set; }
            public bool simplified { get; set; }
            public string style { get; set; }
            public Properties properties { get; set; }
            public Insight insight { get; set; }
        }

        public class Scope
        {
            public string type { get; set; }
            public Project project { get; set; }
        }


    }
}
