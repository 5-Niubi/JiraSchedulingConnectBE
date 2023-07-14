using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Export
{
    public class JiraAPICreateWorkerResDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Config
        {
            public Scope Scope { get; set; }
            public List<object> Attributes { get; set; }
        }

        public class Global
        {
        }

        public class Leader
        {
            public string Name { get; set; }
            public string Email { get; set; }
        }

        public class Projects2
        {
            public int? Id { get; set; }
            public List<string> Attributes { get; set; }
        }

        public class Properties
        {
            public Leader Leader { get; set; }
            public int? Members { get; set; }
            public string Description { get; set; }
            public string Founded { get; set; }
        }

        public class Root
        {
            public int? Id { get; set; }
            public string Value { get; set; }
            public Properties Properties { get; set; }
            public Config Config { get; set; }
        }

        public class Scope
        {
            public List<object> Projects { get; set; }
            public List<Projects2> Projects2 { get; set; }
            public Global Global { get; set; }
        }


    }
}
