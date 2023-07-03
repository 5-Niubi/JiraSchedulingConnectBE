using System;
namespace ModelLibrary.DTOs
{
    public class EquipmentDTO
    {
        public class Request
        {
            public int Id { get; set; }
            public string Name { get; set; } = null!;
            public int? Quantity { get; set; }
            public string? Unit { get; set; }
            public double? UnitPrice { get; set; }
        }
        public class Response
        {
            public int Id { get; set; }
            public string? CloudId { get; set; }
            public string Name { get; set; } = null!;
            public int? Quantity { get; set; }
            public string? Unit { get; set; }
            public double? UnitPrice { get; set; }
            public DateTime? CreateDatetime { get; set; }
            public bool? IsDelete { get; set; }
            public DateTime? DeleteDatetime { get; set; }
        }
    }
}

