using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Export
{
    public class JiraAPIGetCustomFieldContextProjectMappingResDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Root
        {
            public string contextId { get; set; }
            public string projectId { get; set; }
        }


    }
}
