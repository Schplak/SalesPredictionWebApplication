using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SalesPredictionWebApplication.CustomValidation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesPredictionWebApplication.Models
{
    //public getters and setters and empty constructor as per Entity Framework requirements
    public class SalesDataModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "The date is required")]
        [CustomSalesDataDateValidation(ErrorMessage = "The date must be a valid date in the past")]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "The amount is required and must not be negative")]
        [Range(0, float.MaxValue)]
        public float Amount { get; set; }

        public SalesDataModel()
        {
        }
    }
}
