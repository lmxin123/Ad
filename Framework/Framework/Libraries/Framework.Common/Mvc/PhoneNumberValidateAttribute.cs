using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Framework.Common.Mvc
{
    /// <summary>
    /// 验证手机号码验证规则：11位数字，以1开头。
    /// 验证规则：区号+号码，区号以0开头，3位或4位号码由7位或8位数字组成区号与号码之间可以无连接符，也可以“-”连接
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    public class PhoneNumberValidateAttribute : ValidationAttribute
    {
        public PhoneNumberValidateAttribute(string phonePattern = "", string telPattern = "")
            : base("{0} 格式不正确")
        {
            PhonePattern = phonePattern;
            TelPattern = telPattern;
        }
        public string PhonePattern { get; set; } = @"/^1\d{10}$/";
        public string TelPattern { get; set; } = @"/^0\d{2,3}-?\d{7,8}$/";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string valueAsString = value.ToString().Trim();
                if (!Regex.IsMatch(valueAsString, PhonePattern) && !Regex.IsMatch(valueAsString, TelPattern))
                {
                    var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(errorMessage);
                }
            }
            return ValidationResult.Success;
        }

    }
}
