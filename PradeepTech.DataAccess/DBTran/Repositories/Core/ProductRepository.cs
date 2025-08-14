using PradeepTech.DataAccess.Base;
using PradeepTech.DataAccess.DBTran.Interface.Core;
using PradeepTech.Domain.Context;
using PradeepTech.Domain.Models.Data;

namespace PradeepTech.DataAccess.DBTran.Repositories.Core
{
    public class ProductRepository : DataRepository<Product>, IProductRepository
    {
        public ProductRepository(DataContext context) : base(context)
        {
        }
    }
}