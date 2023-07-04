using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RcpspAlgorithmLibrary.Models
{
    internal class TaskModel
    {
        public int Id { get; set; }
        public int Duration { get; set; }
        public List<int> SkillId { get; set; }
        public List<TaskModel> TaskBefore { get; set; }
    }
}
