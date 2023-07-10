using System;
namespace ModelLibrary.DTOs.PertSchedule
{
	public class PrecedenceDTO
    {
        
        public int TaskId { get; set; } // task id of current task
        public int PrecedenceId { get; set; } //  task id it depenedence 

    }
}

