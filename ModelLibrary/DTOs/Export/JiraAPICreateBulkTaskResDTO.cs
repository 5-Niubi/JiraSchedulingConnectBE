using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Export
{
    public class JiraAPICreateBulkTaskResDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class ErrorCollection
        {
            public List<object> ErrorMessages { get; set; }
            public Errors Errors { get; set; }
        }

        public class Errors
        {
        }

        public class Issue
        {
            public string Id { get; set; }
            public string Key { get; set; }
            public string Self { get; set; }
            public Transition Transition { get; set; }
        }

        public class Root
        {
            public List<Issue> Issues { get; set; }
            public List<object> Errors { get; set; }
        }

        public class Transition
        {
            public int? Status { get; set; }
            public ErrorCollection ErrorCollection { get; set; }
        }



    }
}
