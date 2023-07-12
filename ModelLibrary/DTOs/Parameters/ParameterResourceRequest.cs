using System;
namespace ModelLibrary.DTOs.Parameters
{
	public class ParameterResourceRequest
	{
        public int ResourceId { get; set; }
        public string Type { get; set; } = null!;
    }
}

