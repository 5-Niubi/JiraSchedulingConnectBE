namespace ModelLibrary.DTOs.Parameters
{
    public class ParameterDTO
    {
        public int Id
        {
            get; set;
        }
        public int? ProjectId
        {
            get; set;
        }
        public long? Budget
        {
            get; set;
        }
        public int? ObjectiveTime
        {
            get; set;
        }
        public int? ObjectiveCost
        {
            get; set;
        }
    }
}

