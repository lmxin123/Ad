using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Newtonsoft.Json;

using Framework.Common.Extensions;
using System.Web.Mvc;

namespace Framework.Auth
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext()
            : base("AuthConnection", true)
        {
            //  Configuration.UseDatabaseNullSemantics = false;
            //Database.SetInitializer<IdentityDbContext>(null);// Remove default initializer
            Configuration.ProxyCreationEnabled = false;
            // Configuration.LazyLoadingEnabled = false;
        }

        static AuthDbContext()
        {
            // IdentityDbInit.InitialSetup(Create());
        }

        public static AuthDbContext Create()
        {
            return new AuthDbContext();
        }

        public DbSet<IdentityUserRole> UserRoles { get; set; }
        public DbSet<ApplicationRole> AppRoles { get; set; }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<ApplicationRole>()
        //        .HasKey(k => k.Id)
        //         .ToTable("Roles");

        //    modelBuilder.Entity<ApplicationUser>()
        //         .HasKey(k => k.Id)
        //      .ToTable("Users");

        //    modelBuilder.Entity<ApplicationUserRole>()
        //        .HasKey((ApplicationUserRole r) => new { UserId = r.UserId, RoleId = r.RoleId })
        //        .ToTable("UserRoles");

        //    modelBuilder.Entity<ApplicationUserLogin>()
        //        .HasKey((ApplicationUserLogin l) => new { l.LoginProvider, l.ProviderKey, l.UserId })
        //      .ToTable("UserLogins");
        //}
    }

    internal class IdentityDbInit
    {
        public static void InitialSetup(AuthDbContext context)
        {
            // 初始化配置将放在这儿
            ApplicationUserManager userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            ApplicationRoleManager roleMgr = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            string oper = "System";

            if (!roleMgr.RoleExists(AuthSetting.AdminRoleName))
            {
                roleMgr.Create(new ApplicationRole
                {
                    Name = AuthSetting.AdminRoleName,
                    Operator = oper,
                    Rights = JsonConvert.SerializeObject(RightManager.GetDefaultRights()),
                    Remark = "系统默认创建，不充许改动"
                });
            }

            ApplicationUser user = userMgr.FindByName(AuthSetting.Administrator);
            if (user == null)
            {
                userMgr.Create(new ApplicationUser
                {
                    UserName = AuthSetting.Administrator,
                    Email = "sa@example.com",
                    Operator = oper,
                }, AuthSetting.AdminPassowrd);
                user = userMgr.FindByName(AuthSetting.Administrator);
            }

            if (!userMgr.IsInRole(user.Id, AuthSetting.AdminRoleName))
            {
                userMgr.AddToRole(user.Id, AuthSetting.AdminRoleName);
            }

            context.SaveChanges();
        }
    }

    public enum UserStates
    {
        [Display(Name = "正常")]
        Normal,
        [Display(Name = "锁定")]
        Locked,
        [Display(Name = "删除")]
        Delete
    }

    public class ApplicationUser : IdentityUser
    {
        [DisplayName("用户名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        public override string UserName { get; set; }
        [DisplayName("电子邮件")]
        [EmailAddress(ErrorMessage = "{0}格式有误")]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        public override string Email { get; set; }
        [DisplayName("联系电话")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20, ErrorMessage = "{0}必需在{1}个字符以内")]
        [RegularExpression(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)", ErrorMessage = "{0}格式不正确")]
        public override string PhoneNumber { get; set; }
        [DisplayName("性别")]
        public byte Gender { get; set; }
        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        [Required]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        [DisplayName("操作员")]
        public string Operator { get; set; }
        [DisplayName("状态")]
        public UserStates UserState { get; set; }
        [DisplayName("备注")]
        [StringLength(500, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string Remark { get; set; }

        public string CreateTimeDisplay
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        public string UserStateDisplay
        {
            get
            {
                return UserState.GetDisplayName();
            }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // 请注意，authenticationType 必须与 CookieAuthenticationOptions.AuthenticationType 中定义的相应项匹配
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // 在此处添加自定义用户声明
            return userIdentity;
        }
    }
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() { }
        public ApplicationRole(string name) : base(name) { }
        [DisplayName("权限")]
        public string Rights { get; set; }
        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        [DisplayName("操作员")]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string Operator { get; set; }
        [DisplayName("备注")]
        [StringLength(500, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string Remark { get; set; }

        [NotMapped]
        public string CreateDateDisplay
        {
            get
            {
                return CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        [NotMapped]
        public List<ParentRightItem> RoleRights
        {
            get
            {
                if (string.IsNullOrEmpty(Rights))
                {
                    return RightManager.GetDefaultRights();
                }
                return JsonConvert.DeserializeObject<List<ParentRightItem>>(Rights);
            }
        }
    }
    public class ApplicationUserClaim : IdentityUserClaim
    {
        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
    public class ApplicationUserLogin : IdentityUserLogin
    {
        public override string LoginProvider { get; set; } = "default";
        public override string ProviderKey { get; set; } = Guid.NewGuid().ToString();
        [Required]
        [DisplayName("用户名称")]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string LoginName { get; set; }
        [DisplayName("登录时间")]
        public DateTime CreateTime { get; set; } = DateTime.Now;
        [Required]
        [StringLength(50)]
        [DisplayName("IP地址")]
        public string IpAdress { get; set; }
    }
}