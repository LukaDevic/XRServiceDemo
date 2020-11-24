using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using XRService.HistoricalXR.Domain;

namespace XRService.HistoricalXR.Validation
{
    public class AllParametersAreRequiredAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid
            (object value, ValidationContext validationContext)
        {
            var listParameters = (ListParameters) value;

            var errors = new List<string>();

            if (listParameters.Dates == null || listParameters.Dates.Count == 0)
            {
                errors.Add("your request doesn't have any dates, please add some with 'dates='some date here' query");
            }

            if (listParameters.BaseCurrency == null || listParameters.BaseCurrency.Length != 3)
            {
                errors.Add("your request doesn't contain base currency or base currency is wrongly entered, please add it with 'basecurrency='some three letter currency here' query");
            }

            if (listParameters.TargetCurrency == null || listParameters.TargetCurrency.Length != 3)
            {
                errors.Add("your request doesn't contain target currency or base currency is wrongly entered,, please add it with 'targetcurrency='some three letter currency here' query");
            }

            if (errors.Count == 0)
            {
                return ValidationResult.Success;
            }

            var errorMessage = string.Join(" ", errors);
            return new ValidationResult(errorMessage);
        }
    }
}
