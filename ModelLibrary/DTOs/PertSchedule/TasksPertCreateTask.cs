using System;
using ModelLibrary.DTOs.Skills;

namespace ModelLibrary.DTOs.PertSchedule
{
	public class TasksPertCreateTask
	{
        public string Name { get; set; }
        public double Duration { get; set; }
        public int MilestoneId { get; set; }

        public List<int> Skills { get; set; }
        public List<int> Precedences { get; set; }
    }
}

