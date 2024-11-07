using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Xunit.Sdk;

namespace HabitTracker.Server.CustomValidation
{
    public class OptionalEmailAddress : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(value.ToString(), emailPattern);
        }
    }
}
