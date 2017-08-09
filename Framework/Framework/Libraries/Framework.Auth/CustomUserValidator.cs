using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;


namespace Framework.Auth
{
    public class CustomUserValidator : UserValidator<ApplicationUser>
    {
        public bool RequiredEmail { get; set; } = false;

        public CustomUserValidator(UserManager<ApplicationUser, string> manager) : base(manager)
        {
        }

        public override async Task<IdentityResult> ValidateAsync(ApplicationUser item)
        {
            IdentityResult result = await base.ValidateAsync(item);
            //if (RequiredEmail)
            //{
            //    if (!item.Email.ToLower().EndsWith("@example.com"))
            //    {
            //        var errors = result.Errors.ToList();
            //        errors.Add("Only example.com email addresses are allowed");
            //        result = new IdentityResult(errors);
            //    }
            //}
            return result;
        }
    }
}
