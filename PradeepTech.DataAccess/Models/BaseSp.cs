using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PradeepTech.DataAccess.Models
{
    public class BaseSp
    {
        public short? UIAppId { get; set; }

        public short? UIEntityId { get; set; }

        public short? UIProfileId { get; set; }

        public Guid? UIUserId { get; set; }

        public string UICultureInfoId { get; set; } = string.Empty;
    }
}