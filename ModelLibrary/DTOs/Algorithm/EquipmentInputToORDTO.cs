namespace ModelLibrary.DTOs.Algorithm
{
    public class EquipmentInputToORDTO
    {
        public int Id
        {
            get; set;
        }
        public List<FunctionInputToORDTO> Functions
        {
            get; set;
        }
        public long UnitPrice
        {
            get; set;
        }
    }
}
