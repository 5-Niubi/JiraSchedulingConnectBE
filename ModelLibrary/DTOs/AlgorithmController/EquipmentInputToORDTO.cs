using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.AlgorithmController
{
    public class EquipmentInputToORDTO
    {
        public int Id { get; set; }
        public List<FunctionInputToORDTO> Functions { get; set; }
        public int UnitPrice { get; set; }
    }
}
