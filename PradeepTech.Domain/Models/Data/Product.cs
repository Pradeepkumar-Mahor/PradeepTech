using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PradeepTech.Domain.Models.Data
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string ProductCode { get; set; } = string.Empty;

        public string ProductName { get; set; } = string.Empty;

        public DateTime? ServiceDate { get; set; }

        public DateTime? WarrantyDate { get; set; }

        public string ProductDescription { get; set; } = string.Empty;

        public string? UploadPhoto { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}