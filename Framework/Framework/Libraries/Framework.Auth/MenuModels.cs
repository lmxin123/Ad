using System;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Web;

using Framework.Common.Xml;

namespace Framework.Auth
{
    [XmlRoot("Menus")]
    public class Menu
    {
        [XmlElement(typeof(ParentMenuItem), ElementName = "Menu")]
        public List<ParentMenuItem> MenuList { get; set; }
    }
    /// <summary>
    /// 菜单数据
    /// </summary>
    public class SubMenuItem
    {
        [XmlAttribute]
        [DisplayName("子模块编码")]
        public string SubCode { get; set; }
        [XmlAttribute]
        [DisplayName("子模块名")]
        public string Name { get; set; }
        [XmlAttribute]
        [DisplayName("地址")]
        public string Url { get; set; }
        [XmlAttribute]
        [DisplayName("查询")]
        public bool Select { get; set; }
        [XmlAttribute]
        [DisplayName("添加")]
        public bool Create { get; set; }
        [XmlAttribute]
        [DisplayName("修改")]
        public bool Update { get; set; }
        [XmlAttribute]
        [DisplayName("删除")]
        public bool Delete { get; set; }
        [XmlAttribute]
        [DisplayName("审核")]
        public bool Auditing { get; set; }

        public bool All
        {
            get
            {
                return Select || Create || Update || Delete || Auditing;
            }
        }
    }

    public class ParentMenuItem
    {
        [XmlAttribute]
        [DisplayName("模块编码")]
        public string Code { get; set; }
        [XmlAttribute]
        [DisplayName("模块名称")]
        public string Name { get; set; }
        [XmlAttribute]
        [DisplayName("地址")]
        public string Url { get; set; }
        [XmlAttribute]
        [DisplayName("图标")]
        public string Icon { get; set; }
        [XmlElement(typeof(SubMenuItem), ElementName = "SubMenu")]
        public List<SubMenuItem> SubMenuList { get; set; }

        public bool All { get; set; }
    }

    public class MenuManager
    {
        public static List<ParentMenuItem> GetAllMenus()
        {
            string file = HttpContext.Current.Server.MapPath("~/menus.xml");
            Menu menu = Serializer.XML2ObjectFromFile<Menu>(file);
            return menu.MenuList;
        }
    }
}
