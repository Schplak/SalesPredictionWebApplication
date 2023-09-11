using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesPredictionWebApplication.Models
{
    public class SalesData
    {
        public int Id { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public float Amount { get; set; }

        public SalesData()
        {
        }
    }
}
