namespace ModelLibrary.DTOs.Tasks
{
    public class TaskPertChartDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? Duration { get; set; }
        public string? CloudId { get; set; }
        public int? ProjectId { get; set; }
        public int? MilestoneId { get; set; }
        public bool? IsDelete { get; set; }

        
          
    }
}

