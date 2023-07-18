namespace ModelLibrary.DBModels
{
    public partial class Function
    {
        public Function()
        {
            EquipmentsFunctions = new HashSet<EquipmentsFunction>();
            TaskFunctions = new HashSet<TaskFunction>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? CloudId { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual ICollection<EquipmentsFunction> EquipmentsFunctions { get; set; }
        public virtual ICollection<TaskFunction> TaskFunctions { get; set; }
    }
}
