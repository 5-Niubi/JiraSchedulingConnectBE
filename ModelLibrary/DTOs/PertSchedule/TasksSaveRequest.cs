using System;
namespace ModelLibrary.DTOs.PertSchedule
{
	public class TasksSaveRequest
	{
        public int ProjectId { get; set; }
        public List<TaskPrecedencesTaskRequestDTO> TaskPrecedenceTasks { get; set; }
        public List<TaskSkillsRequiredRequestDTO> TaskSkillsRequireds { get; set; }
    }
}

