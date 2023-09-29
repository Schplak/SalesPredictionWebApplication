using Microsoft.AspNetCore.Mvc;
using SalesPredictionWebApplication.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace SalesPredictionWebApplication.Models
{
    //public getters and setters and empty constructor as per Entity Framework requirements
    public class CSVFilesModel
    {
        [CustomCSVFileValidation(ErrorMessage = "All files must be valid CSV files")]
        [BindProperty]
        public List<IFormFile> Files { get; set; } = new();

        public string CSVFileSuccessMessage { get; set; } = string.Empty;
        public List<string> CSVFileErrors { get; set; } = new();

        public CSVFilesModel()
        {
        }
    }
}
