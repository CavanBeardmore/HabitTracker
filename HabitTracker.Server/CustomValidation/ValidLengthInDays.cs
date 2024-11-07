using System.ComponentModel.DataAnnotations;

namespace HabitTracker.Server.CustomValidation
{
    public class ValidLengthInDays : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                ErrorMessage = "Value cannot be null.";
                return false;
            }

            if (int.TryParse(value.ToString(), out int intValue) && intValue >= 1 && intValue <= 30)
            {
                return true;
            }

            ErrorMessage = "Value must be a positive integer between 1 and 30.";
            return false;
        }

    }
}
