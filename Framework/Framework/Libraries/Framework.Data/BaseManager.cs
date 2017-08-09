using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Linq;

using Framework.Common.Json;
using Framework.Common;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;

namespace Framework.Data
{
    /// <summary>
    /// 实体类的增删改查基本操作
    /// </summary>
    /// <typeparam name="TModel">实体类型</typeparam>
    public abstract class BaseManager<TDbContext, TModel> : IDisposable
        where TModel : BaseModel, new()
        where TDbContext : DbContext, new()
    {
        public BaseManager()
        {
            Db = new TDbContext();
        }

        public BaseManager(TDbContext context)
        {
            Db = context;
        }

        public readonly TDbContext Db;

        /// <summary>
        /// 指定对象的数据表实体
        /// </summary>
        public virtual DbSet<TModel> Model
        {
            get
            {
                return Db.Set<TModel>();
            }
        }
        /// <summary>
        /// 增加方法
        /// </summary>
        /// <param name="item">实体对象</param>
        /// <returns></returns>
        public virtual async Task<bool> SaveAsync(IEnumerable<TModel> items)
        {
            foreach (var item in items)
                await SaveAsync(item);
            return true;
        }
        /// <summary>
        /// 增加方法
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id">指定Id值，默认为Guid.NewGuid()</param>
        /// <returns></returns>
        public virtual async Task<bool> SaveAsync(TModel item, string id = null)
        {
            if (string.IsNullOrEmpty(item.ID))
            {
                item.ID = id ?? Guid.NewGuid().ToString();
                return await CUDAsync(item, EntityState.Added);
            }
            else
            {
                return await CUDAsync(item, EntityState.Modified);
            }
        }
        /// <summary>
        ///  删除，默认只把记录状态标记为 DeleteTag，
        ///  如需物理删除，请设realDelete为true
        /// </summary>
        /// <param name="id"></param>
        /// <param name="realDelete"></param>
        /// <example>EntityException</example>
        /// <returns></returns>
        public virtual async Task<bool> DeleteAsync(string id, bool realDelete = false)
        {
            var item = await GetByIdAsync(id);
            if (item == null)
                throw new EntityException("删除的实体不存！");
            bool result = await CUDAsync(item, EntityState.Deleted, realDelete);
            return result;
        }
        /// <summary>
        ///  删除，默认只把记录状态标记为 DeleteTag，
        ///  如需物理删除，请设realDelete为true
        /// </summary>
        /// <param name="id"></param>
        /// <param name="realDelete"></param>
        /// <example>EntityException</example>
        /// <returns></returns>
        public virtual bool Delete(string id, bool realDelete = false)
        {
            var item =  GetById(id);
            if (item == null)
                throw new EntityException("删除的实体不存！");
            bool result = CUD(item, EntityState.Deleted, realDelete);
            return result;
        }
        public virtual async Task<bool> DeleteAsync(TModel item, bool realDelete = false)
        {
            bool result = await CUDAsync(item, EntityState.Deleted, realDelete);
            return result;
        }
        /// <summary>
        /// 返回分页数据方法，如果需要按更多条件返回，请重写此方法
        /// </summary>
        /// <param name="id">实体主键</param>
        /// <param name="pageIndex">页码，必需参数，大于0</param>
        /// <param name="pageSize">页大小，必需参数</param>
        /// <param name="queryItem">对象查找条件</param>
        /// <returns>ResultModel 对象，需要参Data进行转换</returns>
        public virtual async Task<GeneralResponseModel<List<TModel>>> QueryAsync(int pageIndex, int pageSize, RecordStates state = RecordStates.AuditPass)
        {
            // 参数检查
            if (pageIndex < 1)
            {
                throw new ArithmeticException("pageIndex参数无效，必需大于0的整数");
            }
            if (pageSize < 1)
            {
                throw new ArithmeticException($"pageSize 参数无效，必需大于0的整数");
            }

            var items = Model
                   .Where(m => m.RecordState == state || m.RecordState == RecordStates.All);

            var list = items
                .OrderByDescending(f => f.CreateDate)
                .Skip(pageSize * (pageIndex - 1))
                .Take(pageSize);

            var result = new GeneralResponseModel<List<TModel>>
            {
                Data = list.ToList(),
                TotalCount = await items.CountAsync()
            };

            return result;
        }

        /// <summary>
        /// 返回分页数据方法，如果需要按更多条件返回，请重写此方法
        /// </summary>
        /// <returns>GeneralResponseModel<List<TModel>> 对象</returns>
        public virtual async Task<GeneralResponseModel<List<TModel>>> QueryAsync(Expression<Func<TModel, bool>> expression)
        {
            var querys = Model.Where(expression);
            var result = new GeneralResponseModel<List<TModel>>
            {
                Data = querys.ToList(),
                TotalCount = await querys.CountAsync()
            };
            return result;
        }

        /// <summary>
        /// 返回单个对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isNoTracking">没有ef跟踪的全新对象</param>
        /// <returns></returns>
        public virtual async Task<TModel> GetByIdAsync(string id, bool isNoTracking = false)
        {
            if (isNoTracking)
                return await Model.AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
            else
                return await Model.FirstOrDefaultAsync(m => m.ID == id);
        }
        /// <summary>
        /// 返回单个对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isNoTracking">没有ef跟踪的全新对象</param>
        /// <returns></returns>
        public virtual TModel GetById(string id, bool isNoTracking = false)
        {
            if (isNoTracking)
                return Model.AsNoTracking().FirstOrDefault(m => m.ID == id);
            else
                return Model.FirstOrDefault(m => m.ID == id);
        }
        /// <summary>
        /// 返回单个对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<TModel> GetByExpreAsync(Expression<Func<TModel, bool>> expression)
        {
            var item = await Model.FirstOrDefaultAsync(expression);
            return item;
        }
        /// <summary>
        /// 返回表中所有的数据，如果数据量太大，请慎用此方法
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<TModel>> GetAllAsync()
        {
            return await Model.ToListAsync();
        }

        /// <summary>
        /// 返回多个对象
        /// </summary>
        /// <param name="ids">id数据</param>
        /// <returns></returns>
        public virtual async Task<List<TModel>> GetByIdsAsync(string[] ids)
        {
            var items = Model
                .Where(m => ids.Contains(m.ID))
                .ToListAsync();

            return await items;
        }
        /// <summary>
        /// C：Create，D：Delete，U：Update增删改统一方法，
        /// 默认只把记录标识为删除状态
        /// </summary>
        /// <param name="item"></param>
        /// <param name="state"></param>
        /// <param name="realDelete">是否物理删除</param>
        /// <returns></returns>
        private async Task<bool> CUDAsync(TModel item, EntityState state, bool realDelete = false)
        {
            if (item == null)
            {
                throw new ArithmeticException($"{nameof(item)}参数无效，必需不能为null的实体对象。");
            }

            if (state == EntityState.Added)
            {
                if (string.IsNullOrEmpty(item.ID))
                    item.ID = Guid.NewGuid().ToString();
                Db.Entry(item).State = EntityState.Added;
            }
            else if (state == EntityState.Modified)
            {
                Db.Entry(item).State = EntityState.Modified;
            }
            else if (state == EntityState.Deleted)
            {
                if (realDelete)
                {
                    Db.Entry(item).State = EntityState.Deleted;
                }
                else
                {
                    item.RecordState = RecordStates.DeleteTag;
                    Db.Entry(item).State = EntityState.Modified;
                }
            }
            else
            {
                throw new Exception($"{state.ToString()}类型不受支持！");
            }
            return await Db.SaveChangesAsync() == 1;
        }
        /// <summary>
        /// C：Create，D：Delete，U：Update增删改统一方法，
        /// 默认只把记录标识为删除状态
        /// </summary>
        /// <param name="item"></param>
        /// <param name="state"></param>
        /// <param name="realDelete">是否物理删除</param>
        /// <returns></returns>
        private bool CUD(TModel item, EntityState state, bool realDelete = false)
        {
            if (item == null)
            {
                throw new ArithmeticException($"{nameof(item)}参数无效，必需不能为null的实体对象。");
            }

            if (state == EntityState.Added)
            {
                if (string.IsNullOrEmpty(item.ID))
                    item.ID = Guid.NewGuid().ToString();
                Db.Entry(item).State = EntityState.Added;
            }
            else if (state == EntityState.Modified)
            {
                Db.Entry(item).State = EntityState.Modified;
            }
            else if (state == EntityState.Deleted)
            {
                if (realDelete)
                {
                    Db.Entry(item).State = EntityState.Deleted;
                }
                else
                {
                    item.RecordState = RecordStates.DeleteTag;
                    Db.Entry(item).State = EntityState.Modified;
                }
            }
            else
            {
                throw new Exception($"{state.ToString()}类型不受支持！");
            }
            return  Db.SaveChanges() == 1;
        }
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Db != null)
                {
                    Db.Dispose();
                }
            }
        }
    }
}
