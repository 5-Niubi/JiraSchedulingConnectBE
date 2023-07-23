namespace ModelLibrary.DTOs.Export
{
    public class JiraAPIPrepareResultDTO
    {
        public int ProjectId { get; set; }
        public string ProjectKey { get; set; }
        public string ProjectName { get; set; }
        public string? IssueTypeId { get; set; }
        public string WorkerFieldContext { get; set; }
        public Dictionary<string, string?>? FieldDict { get; set; }
    }
}
