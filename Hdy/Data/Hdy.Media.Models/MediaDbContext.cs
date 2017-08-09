using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hdy.Media.Models
{
   public class MediaDbContext: DbContext
    {
        public MediaDbContext()
            : base("MediaConnection")
        {
            //  Configuration.LazyLoadingEnabled = false;
          //  Configuration.ValidateOnSaveEnabled = false;
        }

        static MediaDbContext()
        {
           // Database.SetInitializer(new IdentityDbInit());
        }

        public static MediaDbContext Create()
        {
            return new MediaDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<CorpModel> MerchantModelModels { get; set; }
        public DbSet<AdvertisementModel> AdvertisementModels { get; set; }
        public DbSet<AdvertisementDetailModel> AdvertisementDetailModels { get; set; }
        public DbSet<SchedulingModel> SchedulingModels { get; set; }
        public DbSet<SchedulingDetailModel> SchedulingDetailModels { get; set; }
        public DbSet<DeviceModel> DeviceModels { get; set; }
        public DbSet<DeviceCategoryModel> DeviceCategoryModels { get; set; }
        public DbSet<AdPackageModel> AdPackageModels { get; set; }
        public DbSet<AdDowloadRecordModel> AdDowloadRecordModels { get; set; }
        public DbSet<OpenAreaModel> OpenAreaModels { get; set; }
    }
}
