using PradeepTech.DataAccess.DBTran.Interface;
using PradeepTech.DataAccess.DBTran.Interface.Core;
using PradeepTech.DataAccess.DBTran.Repositories;
using PradeepTech.DataAccess.DBTran.Repositories.Core;

namespace PradeepTech.WebApp.Services
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddDataAccessService(this IServiceCollection services)
        {
            #region Auth

            services.AddTransient<IAuthService, AuthService>();

            #endregion Auth

            #region Core repository

            services.AddTransient<IProductRepository, ProductRepository>();

            #endregion Core repository

            return services;
        }
    }
}