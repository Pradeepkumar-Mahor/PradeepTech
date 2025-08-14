using CachingFramework.Redis.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PradeepTech.DataAccess.Helpers;
using PradeepTech.DataAccess.Models;
using PradeepTech.Domain.Context;
using System.Data;
using System.Data.Common;

namespace PradeepTech.DataAccess.Base
{
    public class ViewRepository<T> : IViewRepository<T> where T : class, new()
    {
        public string CommandText { get; set; }

        public string[] CacheKeySuffix { get; set; }

        public bool IsCachedByUser { get; set; } = true;

        public int AbsoluteExpirationRelativeToNowMinute { get; set; } = 60;

        private readonly ViewContext _context;

        private readonly IContext _redisContext;

        private readonly ILogger<ViewContext> _logger;

        public ViewRepository(ViewContext context, IContext redisContext, ILogger<ViewContext> logger)
        {
            _context = context;
            _redisContext = redisContext;
            _logger = logger;
        }

        public virtual T Get(BaseSp baseSp, ParameterSp parameterSp)
        {
            T item = new();

            DbConnection conn = _context.Database.GetDbConnection();
            conn.Open();

            using (DbCommand command = conn.CreateCommand())
            {
                command.CommandText = CommandText;
                command.CommandType = CommandType.StoredProcedure;
                if (_context.Database.GetCommandTimeout() != null)
                {
                    command.CommandTimeout = _context.Database.GetCommandTimeout().Value;
                }

                if (baseSp != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(baseSp).ToArray());
                }

                if (parameterSp != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(parameterSp, false).ToArray());
                }

                DbDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        item = Helper.ConvertDbDataReaderToObject<T>(reader);
                    }
                }
                reader.Dispose();
                command.Dispose();
            }
            conn.Close();

            return item;
        }

        public virtual async Task<T> GetAsync(BaseSp baseSp, ParameterSp parameterSp)
        {
            T item = new();

            DbConnection conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using (DbCommand command = conn.CreateCommand())
            {
                command.CommandText = CommandText;
                command.CommandType = CommandType.StoredProcedure;
                if (_context.Database.GetCommandTimeout() != null)
                {
                    command.CommandTimeout = _context.Database.GetCommandTimeout().Value;
                }

                if (baseSp != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(baseSp).ToArray());
                }

                if (parameterSp != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(parameterSp, false).ToArray());
                }

                DbDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        item = Helper.ConvertDbDataReaderToObject<T>(reader);
                    }
                }
                reader.Dispose();
                command.Dispose();
            }
            conn.Close();

            return item;
        }

        public virtual async Task<T> GetCacheAsync(BaseSp baseSp, ParameterSp parameterSp, string[] tags)
        {
            T item = new();
            string sql = "";
            bool cacheInError = false;

            DbConnection conn = _context.Database.GetDbConnection();

            using (DbCommand command = conn.CreateCommand())
            {
                command.CommandText = CommandText;
                command.CommandType = CommandType.StoredProcedure;
                if (_context.Database.GetCommandTimeout() != null)
                {
                    command.CommandTimeout = _context.Database.GetCommandTimeout().Value;
                }

                if (baseSp != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(baseSp).ToArray());
                }

                if (parameterSp != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(parameterSp, false).ToArray());
                }

                if (!_context.DisableCache)
                {
                    sql = Helper.CommandAsSql(command);

                    if (!IsCachedByUser)
                    {
                        sql = sql.Replace("@UIUserId = " + baseSp.UIUserId.ToString() + ", ", "");
                    }

                    try
                    {
                        T cached = await _redisContext.Cache.GetObjectAsync<T>(sql);

                        if (cached != null)
                        {
                            command.Dispose();
                            return cached;
                        }
                    }
                    catch (Exception ex)
                    {
                        cacheInError = true;
                        _logger.LogWarning(ex, sql);
                    }
                }

                await conn.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        item = Helper.ConvertDbDataReaderToObject<T>(reader);
                    }
                }

                reader.Dispose();
                command.Dispose();

                if (!_context.DisableCache && !cacheInError)
                {
                    await _redisContext.Cache.SetObjectAsync<T>(sql, item, tags, TimeSpan.FromMinutes(AbsoluteExpirationRelativeToNowMinute));
                }
                cacheInError = false;
            }
            conn.Close();

            return item;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(BaseSp baseSp, ParameterSp parameterSp)
        {
            List<T> items = [];

            DbConnection conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using (DbCommand command = conn.CreateCommand())
            {
                command.CommandText = CommandText;
                command.CommandType = CommandType.StoredProcedure;
                if (_context.Database.GetCommandTimeout() != null)
                {
                    command.CommandTimeout = _context.Database.GetCommandTimeout().Value;
                }

                if (baseSp != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(baseSp).ToArray());
                }

                if (parameterSp != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(parameterSp, false).ToArray());
                }

                DbDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        T item = Helper.ConvertDbDataReaderToObject<T>(reader);
                        items.Add(item);
                    }
                }
                reader.Dispose();

                command.Dispose();
            }
            conn.Close();

            return items;
        }

        public virtual async Task<IEnumerable<T>> GetAllCacheAsync(BaseSp baseSp, ParameterSp parameterSp, string[] tags)
        {
            List<T> items = [];
            string sql = "";
            bool cacheInError = false;

            DbConnection conn = _context.Database.GetDbConnection();

            using (DbCommand command = conn.CreateCommand())
            {
                command.CommandText = CommandText;
                command.CommandType = CommandType.StoredProcedure;
                if (_context.Database.GetCommandTimeout() != null)
                {
                    command.CommandTimeout = _context.Database.GetCommandTimeout().Value;
                }

                if (baseSp != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(baseSp).ToArray());
                }

                if (parameterSp != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(parameterSp, false).ToArray());
                }

                if (!_context.DisableCache)
                {
                    sql = Helper.CommandAsSql(command);

                    if (!IsCachedByUser)
                    {
                        sql = sql.Replace("@UIUserId = " + baseSp.UIUserId.ToString() + ", ", "");
                    }

                    sql += CacheKeySuffix == null ? "" : " " + string.Join("|", CacheKeySuffix);

                    try
                    {
                        List<T> cached = await _redisContext.Cache.GetObjectAsync<List<T>>(sql);

                        if (cached != null)
                        {
                            command.Dispose();
                            return cached;
                        }
                    }
                    catch (Exception ex)
                    {
                        cacheInError = true;
                        _logger.LogWarning(ex, sql);
                    }
                }

                await conn.OpenAsync();
                DbDataReader reader = await command.ExecuteReaderAsync();

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        T item = Helper.ConvertDbDataReaderToObject<T>(reader);
                        items.Add(item);
                    }
                }
                reader.Dispose();
                command.Dispose();

                if (!_context.DisableCache && !cacheInError)
                {
                    await _redisContext.Cache.SetObjectAsync(sql, items, tags, TimeSpan.FromMinutes(AbsoluteExpirationRelativeToNowMinute));
                }
                cacheInError = false;
            }
            conn.Close();

            return items;
        }

        public virtual async Task<DataTable> GetDataTableAsync(string sqlQuery)
        {
            DataTable dt = new();

            DbConnection conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using (DbCommand command = conn.CreateCommand())
            {
                command.CommandText = sqlQuery;
                command.CommandType = CommandType.Text;
                if (_context.Database.GetCommandTimeout() != null)
                {
                    command.CommandTimeout = _context.Database.GetCommandTimeout().Value;
                }

                DbDataReader reader = await command.ExecuteReaderAsync();

                if (reader.FieldCount > 0)
                {
                    dt.Load(reader);
                }
                reader.Dispose();

                command.Dispose();
            }
            conn.Close();

            return dt;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}