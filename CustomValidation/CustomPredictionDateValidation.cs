using System.ComponentModel.DataAnnotations;

namespace SalesPredictionWebApplication.CustomValidation
{
    //Custom prediction date validation to confirm date is valid and in the future
    public class CustomPredictionDateValidation : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date && date > DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
