using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Auth
{
    /// <summary>
    /// 自定定义密码验证器
    /// </summary>
  public  class CustomPasswordValidator: PasswordValidator
    {
        public override async Task<IdentityResult> ValidateAsync(string item)
        {
            IdentityResult result = await base.ValidateAsync(item);
            //if (item.Equals("123456"))
            //{
            //    var errors = result.Errors.ToList();
            //    errors.Add("密码太过于简单，不建议使用");
            //    result = new IdentityResult(errors);
            //}
            return result;
        }
    }
}
