using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.DTOs.AlgorithmController
{
    public class TaskOutput
    {
        public int TaskId { get; set; }
        public int WorkerId { get; set; }
        public List<int> EquipmentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class OutputFromORDTO
    {
        public List<TaskOutput> task = new List<TaskOutput>();

    }
}
