namespace ModelLibrary.DTOs.PertSchedule
{
    public class TasksSaveRequest
    {
        public int ProjectId { get; set; }
        public List<TaskPrecedencesTaskRequestDTO> TaskPrecedenceTasks { get; set; }
        public List<TaskSkillsRequiredRequestDTO> TaskSkillsRequireds { get; set; }
    }


    public class TaskRequest
    {

        public int TaskId{ get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public int MileStoneId { get; set; }
        
        public List<int> TaskPrecedences { get; set; }
        public List<SkillRequiredRequestDTO> TaskSkillsRequireds { get; set; }


    }

    public class TasksSaveRequestV2
    {
        public int ProjectId { get; set; }
        public List<TaskRequest> TaskSaveRequests { get; set; }

    }

}

