using PradeepTech.DataAccess.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PradeepTech.DataAccess.DBTran.Interface.Core
{
    public interface IProductRepository : IDataRepository<Domain.Models.Data.Product>
    {
    }
}