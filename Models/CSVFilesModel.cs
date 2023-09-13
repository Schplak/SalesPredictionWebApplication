using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace SalesPredictionWebApplication.Models
{
    public class CSVFilesModel
    {
        [BindProperty, Display(Name = "Choose or Drag Files Here")]
        public List<IFormFile> Files { get; set; } = new();

        public CSVFilesModel()
        {
        }

        public void OnGet()
        {
        }
    }
}
