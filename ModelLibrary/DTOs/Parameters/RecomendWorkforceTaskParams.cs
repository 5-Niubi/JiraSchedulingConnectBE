using ModelLibrary.DTOs.Skills;
using ModelLibrary.DTOs.Tasks;

namespace ModelLibrary.DTOs.Parameters
{
    public class RecomendWorkforceTaskParams
    {
        public WorkforceViewDTOResponse Workforce { get; set; }
        public WorkforceSkillViewDTOResponse? NewWorkforce { get; set; }
        public List<TaskViewDTO> Tasks { get; set; }
    
    }
}

