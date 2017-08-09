using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Newtonsoft.Json;
using Framework.Data;


namespace Framework.Auth
{
    public class Right
    {
        public List<ParentRightItem> RightList { get; set; }
    }
    /// <summary>
    /// 菜单数据
    /// </summary>
    public class SubRightItem
    {
        /// <summary>
        /// 用于权限管理时设置选择状态
        /// </summary>
        public string SubCode { get; set; }
        public bool All { get; set; } = false;
        public bool? Select { get; set; }
        public bool? Create { get; set; }
        public bool? Update { get; set; }
        public bool? Delete { get; set; }
        public bool? Auditing { get; set; }
    }

    public class ParentRightItem
    {
        public string Code { get; set; }
        public bool All { get; set; } = false;
        public List<SubRightItem> SubRightList { get; set; }
    }

    public class RightManager
    {
        public static List<ParentRightItem> GetDefaultRights()
        {
            var defaultRights = new List<ParentRightItem>();
            MenuManager.GetAllMenus().ForEach(parent =>
            {
                var parentItem = new ParentRightItem
                {
                    Code = parent.Code,
                    SubRightList = new List<SubRightItem>()
                };
                parent.SubMenuList.ForEach(sub =>
                {
                    var subItem = new SubRightItem { SubCode = sub.SubCode };
                    if (sub.Auditing)
                        subItem.Auditing = false;
                    if (sub.Create)
                        subItem.Create = false;
                    if (sub.Delete)
                        subItem.Delete = false;
                    if (sub.Select)
                        subItem.Select = false;
                    if (sub.Update)
                        subItem.Update = false;
                    parentItem.SubRightList.Add(subItem);
                });
                defaultRights.Add(parentItem);
            });
            return defaultRights;
        }
    }
}