namespace ModelLibrary.DTOs.Export
{
    public class JiraAPIGetIssueCustomFieldContextResDTO
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

        public string Id
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string Description
        {
            get; set;
        }
        public bool? IsGlobalContext
        {
            get; set;
        }
        public bool? IsAnyIssueType
        {
            get; set;
        }
    }
}
