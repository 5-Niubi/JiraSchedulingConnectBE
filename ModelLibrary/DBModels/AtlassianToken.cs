using System;
using System.Collections.Generic;

namespace ModelLibrary.DBModels
{
    public partial class AtlassianToken
    {
        public AtlassianToken()
        {
            AccountRoles = new HashSet<AccountRole>();
        }

        public int Id { get; set; }
        public string? AccountInstalledId { get; set; }
        public string? CloudId { get; set; }
        public string? Site { get; set; }
        public string? AccessToken { get; set; }
        public string? RefressToken { get; set; }
        public DateTime? CreateDatetime { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? DeleteDatetime { get; set; }

        public virtual ICollection<AccountRole> AccountRoles { get; set; }
    }
}
