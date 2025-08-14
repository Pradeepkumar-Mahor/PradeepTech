using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PradeepTech.DataAccess.Models
{
    public class ParameterSp
    {
        public string Id { get; set; }

        public Guid? GuidId { get; set; }

        public string Search { get; set; }

        public DateTime? Date { get; set; }

        public DateTimeOffset? DateTime { get; set; }

        public DateTimeOffset? StartDateTime { get; set; }

        public DateTimeOffset? EndDateTime { get; set; }
    }
}