using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Framework.Common;
using Framework.Common.Extensions;

namespace Framework.Data
{
    /// <summary>
    /// 父级实体，建议新建实体都继承至此类
    /// </summary>
    public partial class BaseModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [StringLength(50)]
        public virtual string ID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual DateTime CreateDate { get; set; } = DateTime.Now;
        /// <summary>
        /// 数据状态，默认为发布状态
        /// </summary>
        [Display(Name = "状态")]
        public virtual RecordStates RecordState { get; set; } = RecordStates.AuditPass;
        /// <summary>
        /// 操作员
        /// </summary>
        [StringLength(50)]
        [Display(Name = "操作员")]
        public virtual string Operator { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(500)]
        [Display(Name = "备注")]
        public virtual string Remark { get; set; }
    }

    public partial class BaseModel
    {
        protected const string DateTimeFormetStr = "yyyy-MM-dd HH:mm:ss";
        protected const string DateFormetStr = "yyyy-MM-dd";
        /// <summary>
        /// 状态字符
        /// </summary>
        [NotMapped]
        public virtual string RecordStateDisplay
        {
            get
            {
                return RecordState.GetDisplayName();
            }
        }
        /// <summary>
        /// 某些序列化格式会乱
        /// </summary>
        [NotMapped]
        public virtual string CreateDateDisplay
        {
            get { return CreateDate == null ? string.Empty : CreateDate.ToString(DateTimeFormetStr); }
        }
    }
}
