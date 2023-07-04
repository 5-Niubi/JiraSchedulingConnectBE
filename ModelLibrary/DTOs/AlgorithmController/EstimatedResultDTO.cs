using ModelLibrary.DTOs.AlgorithmController;

namespace AlgorithmServiceServer
{
    public class WorkforceOutputFromEsDTO
    {

        public int Id { get; set; }
        public List<SkillOutputFromEstimatorDTO> SkillOutputList { get; set; }
        public int Quantity { get; set; }

    }



    public class EstimatedResultDTO

    {
        public int Id { get; set; }
        public List<WorkforceOutputFromEsDTO> WorkforceOutputList { get; set; }


    }
}

