using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PradeepTech.DataAccess.Models
{
    public class CacheTable
    {
        public string Name { get; set; } = string.Empty;

        public List<CacheTableRow> CacheTableRows { get; set; }

        public CacheTable(string name)
        {
            Name = name;
            CacheTableRows = new List<CacheTableRow>();
        }
    }

    public class CacheTableRow
    {
        public string Root { get; set; } = string.Empty;

        public short UIAppId { get; set; }

        public short UIEntityId { get; set; }

        public string Key { get; set; } = string.Empty;
    }
}