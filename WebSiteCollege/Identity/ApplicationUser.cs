using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebSiteCollege.Identity
{
    [Table("Users")]
    public class ApplicationUser
    {
        public string UserType { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

    }
}
