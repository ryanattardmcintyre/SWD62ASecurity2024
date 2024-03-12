using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Validators
{
    public class YearValidationAttribute: ValidationAttribute
    {
        public YearValidationAttribute() {
            ErrorMessage = "Year has to be in the past";
        }
        public override bool IsValid(object? value)
        {
            int inputYear = (int)value;
            if (inputYear <= DateTime.Today.Year) { return true; }
            else return false;
        }
    }
}
