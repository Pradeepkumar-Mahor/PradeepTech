using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PradeepTech.Domain.Context
{
    public class ExecContextOption
    {
        public ExecContextOption(bool disableCache)
        {
            DisableCache = disableCache;
        }

        public bool DisableCache { get; set; }
    }
}