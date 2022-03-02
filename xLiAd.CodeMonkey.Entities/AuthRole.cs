using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace xLiAd.CodeMonkey.Entities
{
    [Table("DCC_AuthRole")]
    public class AuthRole
    {
        [Identity, Key]
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
    }
}
