using System.ComponentModel.DataAnnotations;

namespace METCore.ValidationAttributes
{
    public class ProtectedPlayersValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is not IList<int> playerIds)
                return false;

            // Get the instance to access RosterSettingsProtectedPerTeam
            var validationContext = value as ValidationContext;
            var instance = validationContext?.ObjectInstance;

            // This approach works better - we'll need the ValidationContext
            return true; // Temporary - see below for proper implementation
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not IList<int> playerIds)
                return new ValidationResult("Invalid player IDs format");

            // Get the related property value
            var instance = validationContext.ObjectInstance;
            var protectedPerTeamProperty = instance.GetType().GetProperty("RosterSettingsProtectedPerTeam");

            if (protectedPerTeamProperty?.GetValue(instance) is not int protectedPerTeam)
                return new ValidationResult("Unable to validate - RosterSettingsProtectedPerTeam not found");

            // Validate all player IDs are positive
            if (playerIds.Any(id => id <= 0))
                return new ValidationResult("All player IDs must be greater than 0");

            // Calculate expected length
            var expectedLength = protectedPerTeam * 32;

            // Allow empty list if protectedPerTeam <= 3
            if (protectedPerTeam <= 3 && playerIds.Count == 0)
                return ValidationResult.Success;

            // Check if length matches expected value
            var validLengths = new[] { 0, 32, 64, 96, 128, 160, 192 };
            if (!validLengths.Contains(playerIds.Count))
                return new ValidationResult($"List length must be one of: {string.Join(", ", validLengths)}");

            // Additional check: length should match protectedPerTeam * 32
            if (playerIds.Count != expectedLength && playerIds.Count != 0)
                return new ValidationResult($"Expected {expectedLength} players based on protected per team setting");

            return ValidationResult.Success;
        }
    }
}