namespace AlgorithmLibrary.Models
{
    internal class TaskModel
    {
        public int Id { get; set; }
        public int Duration { get; set; }
        public List<int> SkillId { get; set; }
        public List<TaskModel> TaskBefore { get; set; }
    }
}
