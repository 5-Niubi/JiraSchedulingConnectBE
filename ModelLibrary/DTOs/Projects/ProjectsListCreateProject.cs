namespace ModelLibrary.DTOs.Projects
{
    public class ProjectsListCreateProject
    {
        public string? ImageAvatar { get; set; }

        public string? Name { get; set; }

        public string? AccountId { get; set; }

        public DateTime? StartDate { get; set; }

        public double? Budget { get; set; }

        public string? BudgetUnit { get; set; } = "$";

        public DateTime? Deadline { get; set; }

        public double? ObjectiveTime { get; set; }

        public double? ObjectiveCost { get; set; }

        public double? ObjectiveQuality { get; set; }

    }
}
