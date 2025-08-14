using PradeepTech.DataAccess.Models;

namespace PradeepTech.DataAccess.Base
{
    public interface IExecRepository<Tin, Tout> : IDisposable
    {
        Task<Tout> ExecAsync(BaseSp baseSp, Tin tInput);

        Task<Tout> ExecCacheAsync(BaseSp baseSp, Tin tInput, string[] tags);

        Task<Tout> StorageValuedFunctionAsync(BaseSp baseSp, Tin tInput);

        Task<Tout> StorageValuedFunctionCacheAsync(BaseSp baseSp, Tin tInput, string[] tags);
    }
}