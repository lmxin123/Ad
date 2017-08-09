using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;


namespace Framework.Auth
{
    public class CustomRoleValidator : RoleValidator<ApplicationRole>
    {
        public bool RequireName { get; set; }
        public int NameLength { get; set; } = 10;

        public CustomRoleValidator(RoleManager<ApplicationRole, string> manager) : base(manager)
        {
        }
        public override async Task<IdentityResult> ValidateAsync(ApplicationRole item)
        {
            IdentityResult result = new IdentityResult();
            if (string.IsNullOrEmpty(item.Name) && RequireName)
            {
                var errors = result.Errors.ToList();
                errors.Add("角色名称不能为空");
                result = new IdentityResult(errors);
                return result;
            }
            else if (item.Name.Length > NameLength)
            {
                var errors = result.Errors.ToList();
                errors.Add($"角色名称长度不能超过{NameLength}个字符");
                result = new IdentityResult(errors);
                return result;
            }
            result = await base.ValidateAsync(item);
            return result;
        }
    }
}
