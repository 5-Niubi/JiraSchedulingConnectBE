namespace ModelLibrary.DTOs.PertSchedule
{
    public class TasksPrecedencesSaveRequest
    {
        public int ProjectId
        {
            get; set;
        }
        public List<TaskPrecedenceDTO> TaskPrecedences
        {
            get; set;
        }

    }
}

