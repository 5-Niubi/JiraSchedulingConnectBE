using System;
using ModelLibrary.DTOs.Skills;

namespace ModelLibrary.DTOs.PertSchedule
{
	public class TasksPertCreateTask
	{
        public class Request {

            public int ProjectId { get; set; }
            public string Name { get; set; }
            public double Duration { get; set; }
            public int MilestoneId { get; set; }

            public List<int>? RequiredSkillsId { get; set; }
            public List<int>? RequiredSkillsLevel { get; set; }
            public List<int>? PrecedencesId { get; set; }
        }
        
    }
}

