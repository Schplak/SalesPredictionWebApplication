using System.ComponentModel.DataAnnotations;

namespace SalesPredictionWebApplication.CustomValidation
{
    //Custom sales data date validation to confirm the date is valid and in the past
    public class CustomSalesDataDateValidation : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date && date <= DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
