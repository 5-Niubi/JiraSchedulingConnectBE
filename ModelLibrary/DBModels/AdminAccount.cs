using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class AdminAccount
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }
    }
}
