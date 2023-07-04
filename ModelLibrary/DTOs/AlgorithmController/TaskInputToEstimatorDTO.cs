using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.AlgorithmController
{
    public class TaskInputToEstimatorDTO
    {
        public int Id { get; set; }
        public int? Duration { get; set;}
        public List<int> PrecedenceTaskId { get; set; }
        public List<SkillInputToORDTO> SkillRequired { get; set; }
    }
}
