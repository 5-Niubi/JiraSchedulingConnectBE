using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Export
{
    public class JiraAPISearchIssueTypeScreenSchemeDTO
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ProjectsDTO Projects { get; set; }

        public class AvatarUrlsDTO
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

        public class ProjectCategoryDTO
        {
            public string Id { get; set; }
            public string Description { get; set; }
            public string Name { get; set; }
        }

        public class ProjectsDTO
        {
            public int? MaxResults { get; set; }
            public int? StartAt { get; set; }
            public int? Total { get; set; }
            public bool? IsLast { get; set; }
            public List<ValueDTO> Values { get; set; }
        }

        public class ValueDTO
        {
            public string Self { get; set; }
            public string Id { get; set; }
            public string Key { get; set; }
            public string Name { get; set; }
            public string ProjectTypeKey { get; set; }
            public bool? Simplified { get; set; }
            public AvatarUrlsDTO AvatarUrls { get; set; }
            public ProjectCategoryDTO ProjectCategory { get; set; }
        }
    }
}
