using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portfolio.Web.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public ApplicationUser() : base()
        {
            UserCreated = DateTime.Now.ToUniversalTime();
        }
        public DateTime UserCreated { get; set; }
        public DateTime UserLastLogin { get; set; }
        public float Version { get; set; }
    }
}
