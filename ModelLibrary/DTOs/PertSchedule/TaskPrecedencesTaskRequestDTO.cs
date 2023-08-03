namespace ModelLibrary.DTOs.PertSchedule
{
    public class TaskPrecedencesTaskRequestDTO
    {
        public int TaskId
        {
            get; set;
        }
        public List<int> TaskPrecedences
        {
            get; set;
        }

    }
}

