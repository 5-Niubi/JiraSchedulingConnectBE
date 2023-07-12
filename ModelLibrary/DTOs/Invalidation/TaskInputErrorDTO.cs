using System;
using ModelLibrary.DTOs.PertSchedule;

namespace ModelLibrary.DTOs.Invalidation
{


	public class SkillRequestErrorDTO {
       
        public int SkillId { get; set; }
        public int? Level { get; set; }
        public String? Messages { get; set; }

    }


    public class TaskPrecedenceErrorDTO
    {

        public int TaskId { get; set; } // task id of current task
        public int PrecedenceId { get; set; } //  task id it depenedence 
        public String? Messages { get; set; }

    }

    public class TaskInputErrorDTO
	{

		public int TaskId { get; set; }
        public int? MilestoneId { get; set; }
        public List<SkillRequestErrorDTO>? SkillRequireds { get; set; }
        public List<TaskPrecedenceErrorDTO>? TaskPrecedences { get; set; }
        public String? Messages { get; set; }
	
	}
}

