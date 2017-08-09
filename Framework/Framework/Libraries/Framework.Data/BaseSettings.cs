using System.ComponentModel.DataAnnotations;

namespace Framework.Data
{
    public class CommonSettings
    {
        /// <summary>
        /// 系统名称
        /// </summary>
        [Display(Name = "系统名称")]
        [Required(ErrorMessage = "请输入{0}")]
        public virtual string SiteName { get; set; }
        /// <summary>
        /// 网站域名
        /// </summary>
        [Display(Name = "网站域名")]
        [Required(ErrorMessage = "请输入{0}")]
        public virtual string Domain { get; set; }
        /// <summary>
        /// 数据请求页大小
        /// </summary>
        [Display(Name = "读取数据分页大小")]
        [Required(ErrorMessage = "请输入{0}")]
        public virtual int PageSize { get; set; } = 20;
    }
}
