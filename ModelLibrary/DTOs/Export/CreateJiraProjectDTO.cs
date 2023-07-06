using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Export
{
    [JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
    public class CreateJiraProjectDTO
    {
        public string? assigneeType { get; set; }
        public int? avatarId { get; set; }
        public int? categoryId { get; set; }
        public string? description { get; set; }
        public int? issueSecurityScheme { get; set; }
        public string? key { get; set;}
        public string? leadAccountId { get; set; }
        public string? name { get; set; }
        public int? notificationScheme { get; set; }
        public int? permissionScheme { get; set; }
        public string? projectTemplateKey { get; set; }
        public string? projectTypeKey { get; set; }
        public string? prl { get; set; }
    }
}
