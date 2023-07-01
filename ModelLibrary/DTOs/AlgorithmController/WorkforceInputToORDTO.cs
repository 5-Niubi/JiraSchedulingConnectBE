using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.AlgorithmController
{
    public class WorkforceInputToORDTO
    {
        public int Id { get; set; }
        public List<SkillInputToORDTO> Skills { get; set; }
        public int UnitSalary { get; set; }
    }
}
