using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class Workforce
    {
        public Workforce()
        {
            ParameterResources = new HashSet<ParameterResource>();
            WorkforceSkills = new HashSet<WorkforceSkill>();
        }

        public int Id { get; set; }
        public string? AccountId { get; set; }
        public string? Email { get; set; }
        public string? AccountType { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? DisplayName { get; set; }
        public int? Active { get; set; }
        public string? CloudId { get; set; }
        public double? UnitSalary { get; set; }
        public int? WorkingType { get; set; }
        public string WorkingEffort { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual ICollection<ParameterResource> ParameterResources { get; set; }
        public virtual ICollection<WorkforceSkill> WorkforceSkills { get; set; }
    }
}
