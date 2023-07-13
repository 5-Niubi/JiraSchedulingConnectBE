using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.Export
{
    public class JiraAPIPrepareResultDTO
    {
        public int ProjectId { get; set; }
        public string? IssueTypeId { get; set; }
        public Dictionary<string, string?>? FieldDict {  get; set; }
    }
}
