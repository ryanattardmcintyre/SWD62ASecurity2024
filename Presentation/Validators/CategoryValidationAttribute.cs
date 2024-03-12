using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Validators
{
    public class CategoryValidationAttribute : ValidationAttribute
    {
        public CategoryValidationAttribute()
        {
            ErrorMessage = "Category does not exist in the database";
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            CategoriesRepository catRepo = validationContext.GetService<CategoriesRepository>();
            if(catRepo == null)
            {
                return new ValidationResult("incorrect category");
            }

            var myExistingCategories = catRepo.GetCategories();

            int inputCategory = (int)value;

            if (myExistingCategories.Count(x=>x.Id == inputCategory) > 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("incorrect category");
        }
    }
}
