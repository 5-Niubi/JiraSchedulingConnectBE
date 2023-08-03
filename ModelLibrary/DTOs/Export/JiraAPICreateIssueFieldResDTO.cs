namespace ModelLibrary.DTOs.Export
{
    public class JiraAPICreateIssueFieldResDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Root
        {
            public string Id
            {
                get; set;
            }
            public string Key
            {
                get; set;
            }
            public string Name
            {
                get; set;
            }
            public string UntranslatedName
            {
                get; set;
            }
            public bool? Custom
            {
                get; set;
            }
            public bool? Orderable
            {
                get; set;
            }
            public bool? Navigable
            {
                get; set;
            }
            public bool? Searchable
            {
                get; set;
            }
            public List<string> ClauseNames
            {
                get; set;
            }
            public Schema Schema
            {
                get; set;
            }
        }

        public class Schema
        {
            public string Type
            {
                get; set;
            }
            public string Custom
            {
                get; set;
            }
            public int? CustomId
            {
                get; set;
            }
        }

    }
}
