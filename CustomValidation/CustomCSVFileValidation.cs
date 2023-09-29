using System.ComponentModel.DataAnnotations;

namespace SalesPredictionWebApplication.CustomValidation
{
    //Custom file validation to confirm CSV file type
    public class CustomCSVFileValidation : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is List<IFormFile> files)
            {
                foreach (var file in files)
                {
                    if (!file.ContentType.Equals("text/csv"))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
