using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PradeepTech.DataAccess.Models
{
    [Serializable]
    public class DataField
    {
        public string DataValueField { get; set; } = string.Empty;

        public string DataTextField { get; set; } = string.Empty;

        public string DataGroupField { get; set; } = string.Empty;
    }
}