namespace ModelLibrary.DTOs.Parameters
{
    public class EquipmentDTORequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? Quantity { get; set; }
        public string? Unit { get; set; }
        public double? UnitPrice { get; set; }
    }
}

