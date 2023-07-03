using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.AlgorithmController
{
    public class TaskSimilarityGenerateInputToORDTO
    {
        public int TaskCount { get; set; }
        public int SkillCount { get; set; }
        public int FunctionCount { get; set; }
        public int[][] TaskSkillWithLevel { get; set; }
        public int[][] TaskFunctionWithTime { get; set; }
    }
}
