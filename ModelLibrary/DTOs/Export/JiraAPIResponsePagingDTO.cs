namespace ModelLibrary.DTOs.Export
{
    public class JiraAPIResponsePagingDTO<T>
    {
        public string? Self { get; set; }
        public string? NextPage { get; set; }
        public int MaxResults { get; set; }
        public int StartAt { get; set; }
        public int Total { get; set; }
        public bool IsLast { get; set; }
        public List<T>? Values { get; set; }
    }
}
