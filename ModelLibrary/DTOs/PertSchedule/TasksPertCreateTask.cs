using System;
using ModelLibrary.DTOs.Skills;

namespace ModelLibrary.DTOs.PertSchedule
{
	public class TasksPertCreateTask
	{

        public class SkillRequest {
            public int SkillId { get; set; }
            public int Level { get; set; }

        }


        public class PrecedenceRequest
        {
            public int PrecedenceId { get; set; }

        }


        public class TaskRequest {

            public int ProjectId { get; set; }
            public string Name { get; set; }
            public double Duration { get; set; }
            public int MilestoneId { get; set; }

            public List<SkillRequest>? SkillRequireds{ get; set; }
            public List<PrecedenceRequest>? Precedences { get; set; }
  
        }
        
    }
}

