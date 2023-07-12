using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class Schedule
    {
        public int Id { get; set; }
        public int? ParameterId { get; set; }
        public int? Duration { get; set; }
        public int? Cost { get; set; }
        public int? Quality { get; set; }
        public string? Tasks { get; set; }
        public int? Selected { get; set; }
        public DateTime? Since { get; set; }
        public string? AccountId { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual ParameterRequestDTO? Parameter { get; set; }
    }
}
