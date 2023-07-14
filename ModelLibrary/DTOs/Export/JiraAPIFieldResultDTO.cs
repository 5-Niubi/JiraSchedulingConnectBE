using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Export
{
    public class JiraAPIFieldResultDTO
    {

        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool? Custom { get; set; }
        public bool? Orderable { get; set; }
        public bool? Navigable { get; set; }
        public bool? Searchable { get; set; }
        public List<string>? ClauseNames { get; set; }
        public SchemaDTO? Schema { get; set; }

        public class SchemaDTO
        {
            public string? Type { get; set; }
            public string? System { get; set; }
        }
    }
}
