using ModelLibrary.DTOs.Skills;

namespace ModelLibrary.DTOs.Parameters
{
    public class WorkforceViewDTOResponse
    {
        public int Id
        {
            get; set;
        }
        public string? Name
        {
            get; set;
        }

    }


    public class WorkforceSkillViewDTOResponse
    {
        public int Id
        {
            get; set;
        }
        public string? Name
        {
            get; set;
        }
        public List<SkillDTOResponse> Skills
        {
            get; set;
        }

    }
}

