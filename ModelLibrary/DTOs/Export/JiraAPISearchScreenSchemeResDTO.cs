using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Export
{
    public class JiraAPISearchScreenSchemeResDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class IssueTypeScreenSchemes
        {
            public int? MaxResults { get; set; }
            public int? StartAt { get; set; }
            public int? Total { get; set; }
            public bool? IsLast { get; set; }
            public List<Value> Values { get; set; }
        }

        public class Root
        {
            public int? Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public Screens Screens { get; set; }
            public IssueTypeScreenSchemes IssueTypeScreenSchemes { get; set; }
        }

        public class Screens
        {
            public int? Default { get; set; }
            public int? Edit { get; set; }
            public int? Create { get; set; }
            public int? View { get; set; }
        }

        public class Value
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }


    }
}
