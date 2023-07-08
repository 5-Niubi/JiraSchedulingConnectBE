using System;
namespace ModelLibrary.DTOs.Tasks
{
	public class TasksListCreateTask
	{
		public class Request
		{
            public string? Name { get; set; }
            public double? Duration { get; set; }
            public string? CloudId { get; set; }
            public int? ProjectId { get; set; }
            public int? MilestoneId { get; set; }
        }
	}
}

