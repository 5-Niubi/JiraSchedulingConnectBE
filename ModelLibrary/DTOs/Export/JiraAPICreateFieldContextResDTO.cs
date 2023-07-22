namespace ModelLibrary.DTOs.Export
{
    public class JiraAPICreateFieldContextResDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Root
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public List<object> ProjectIds { get; set; }
            public List<string> IssueTypeIds { get; set; }
        }
    }
}
