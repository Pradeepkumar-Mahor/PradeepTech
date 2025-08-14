using CachingFramework.Redis.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PradeepTech.DataAccess.Helpers;
using PradeepTech.DataAccess.Models;
using PradeepTech.Domain.Context;
using System.Data;

namespace PradeepTech.DataAccess.Base
{
    public class ExecRepository<Tin, Tout> : IExecRepository<Tin, Tout> where Tin : class, new() where Tout : class, new()
    {
        public string CommandText { get; set; }

        public string[] CacheKeySuffix { get; set; }

        public bool IsCachedByUser { get; set; } = true;

        public int AbsoluteExpirationRelativeToNowMinute { get; set; } = 60;

        private readonly ExecContext _context;

        private readonly IContext _redisContext;

        private readonly ILogger<ExecContext> _logger;

        public ExecRepository(ExecContext context, IContext redisContext, ILogger<ExecContext> logger)
        {
            _context = context;
            _redisContext = redisContext;
            _logger = logger;
        }

        public virtual async Task<Tout> ExecAsync(BaseSp baseSp, Tin tInput)
        {
            Tout? tOutput = new();

            System.Data.Common.DbConnection conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using (System.Data.Common.DbCommand command = conn.CreateCommand())
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

                if (tInput != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(tInput, false).ToArray());
                }

                if (tOutput != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameterOutput<Tout>().ToArray());
                }

                _ = await command.ExecuteNonQueryAsync();

                tOutput = Helper.ConvertDbParameterToObject<Tout>(command.Parameters);

                command.Dispose();
            }
            conn.Close();

            return tOutput;
        }

        public virtual async Task<Tout> ExecCacheAsync(BaseSp baseSp, Tin tInput, string[] tags)
        {
            Tout? tOutput = new();
            string sql = "";
            bool cacheInError = false;

            System.Data.Common.DbConnection conn = _context.Database.GetDbConnection();

            using (System.Data.Common.DbCommand command = conn.CreateCommand())
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

                if (tInput != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(tInput, false).ToArray());
                }

                if (!_context.DisableCache)
                {
                    sql = Helper.CommandAsSql(command);
                }

                if (tOutput != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameterOutput<Tout>().ToArray());
                }

                if (!_context.DisableCache)
                {
                    if (!IsCachedByUser)
                    {
                        sql = sql.Replace("@UIUserId = " + baseSp.UIUserId.ToString() + ", ", "");
                    }

                    sql += CacheKeySuffix == null ? "" : " " + string.Join("|", CacheKeySuffix);

                    try
                    {
                        Tout cached = await _redisContext.Cache.GetObjectAsync<Tout>(sql);

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
                _ = await command.ExecuteNonQueryAsync();

                tOutput = Helper.ConvertDbParameterToObject<Tout>(command.Parameters);

                command.Dispose();

                if (!_context.DisableCache && !cacheInError)
                {
                    await _redisContext.Cache.SetObjectAsync<Tout>(sql, tOutput, tags, TimeSpan.FromMinutes(AbsoluteExpirationRelativeToNowMinute));
                }
                cacheInError = false;
            }
            conn.Close();

            return tOutput;
        }

        public virtual async Task<Tout> StorageValuedFunctionAsync(BaseSp baseSp, Tin tInput)
        {
            Tout? tOutput = new();

            System.Data.Common.DbConnection conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using (System.Data.Common.DbCommand command = conn.CreateCommand())
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

                if (tInput != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(tInput, false).ToArray());
                }

                if (tOutput != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameterReturnValue<Tout>().ToArray());
                }

                _ = await command.ExecuteNonQueryAsync();

                tOutput = Helper.ConvertDbParameterToObject<Tout>(command.Parameters);
                command.Dispose();
            }
            conn.Close();

            return tOutput;
        }

        public virtual async Task<Tout> StorageValuedFunctionCacheAsync(BaseSp baseSp, Tin tInput, string[] tags)
        {
            Tout? tOutput = new();
            string sql = "";
            bool cacheInError = false;

            System.Data.Common.DbConnection conn = _context.Database.GetDbConnection();

            using (System.Data.Common.DbCommand command = conn.CreateCommand())
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

                if (tInput != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameter(tInput, false).ToArray());
                }

                if (!_context.DisableCache)
                {
                    sql = Helper.CommandAsSql(command);
                }

                if (tOutput != null)
                {
                    command.Parameters.AddRange(Helper.FillSqlParameterReturnValue<Tout>().ToArray());
                }

                if (!_context.DisableCache)
                {
                    if (!IsCachedByUser)
                    {
                        sql = sql.Replace("@UIUserId = " + baseSp.UIUserId.ToString() + ", ", "");
                    }

                    sql += CacheKeySuffix == null ? "" : " " + string.Join("|", CacheKeySuffix);

                    try
                    {
                        Tout cached = await _redisContext.Cache.GetObjectAsync<Tout>(sql);

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
                _ = await command.ExecuteNonQueryAsync();

                tOutput = Helper.ConvertDbParameterToObject<Tout>(command.Parameters);

                command.Dispose();

                if (!_context.DisableCache && !cacheInError)
                {
                    await _redisContext.Cache.SetObjectAsync<Tout>(sql, tOutput, tags, TimeSpan.FromMinutes(AbsoluteExpirationRelativeToNowMinute));
                }
                cacheInError = false;
            }
            conn.Close();

            return tOutput;
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}