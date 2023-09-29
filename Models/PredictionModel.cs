using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SalesPredictionWebApplication.CustomValidation;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesPredictionWebApplication.Models
{
    //public getters and setters and empty constructor as per Entity Framework requirements
    public class PredictionModel
    {
        [CustomPredictionDateValidation(ErrorMessage = "Please enter a valid date in the future")]
        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }
        public int DaysOfHistory { get; set; } = 0;
        public int NumberOfHistoricalSales { get; set; } = 0;
        public float PredictedSales { get; set; } = 0f;

        public PredictionModel()
        {
        }
    }
}
