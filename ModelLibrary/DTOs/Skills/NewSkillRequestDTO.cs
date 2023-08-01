using System;
namespace ModelLibrary.DTOs.Skills
{
	public class NewSkillRequestDTO
	{
        public string Name { get; set; }
        public int Level { get; set; }
    }


    public class NewSkillResponeDTO
    {
        public int Id { get; set; }
        public int Level { get; set; }
    }
}

