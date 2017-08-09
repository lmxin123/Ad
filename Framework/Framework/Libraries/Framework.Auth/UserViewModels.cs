using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Framework.Auth
{
    public class LoginViewModel
    {
        [Display(Name = "用户名")]
        [Required(ErrorMessage = "请输入{0}")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        [Required(ErrorMessage = "请输入{0}")]
        public string Password { get; set; }

        [Display(Name = "记住我?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }

    [NotMapped]
    public class RegisterViewModel : ApplicationUser
    {

        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        public string Password { get; set; }
        [Display(Name = "角色")]
        [Required(ErrorMessage = "请选择{0}")]
        public string RoleId { get; set; }
        //[DataType(DataType.Password)]
        //[Display(Name = "确认密码")]
        //[Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        //public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        public string Name { get; set; }

        [Display(Name = "旧密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        public string OldPassword { get; set; }
        [Display(Name = "新密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        public string Password { get; set; }
        [Display(Name = "确认新密码")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "请输入{0}")]
        [Compare("Password", ErrorMessage = "新密码两次输入不一至。")]
        [StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 6)]
        public string ConfirmPassword { get; set; }
    }

    [NotMapped]
    public class UserQueryViewModel : ApplicationUser
    {
        public string QueryText { get; set; }
        public string RoleId { get; set; }
    }
}
