using FluentValidation;
using SeamlessDigital.ToDoSystem.Data;
using SeamlessDigital.ToDoSystem.Models;
using SeamlessDigital.ToDoSystem.ViewModels;
namespace SeamlessDigital.ToDoSystem.Validators
{
    public class TodoValidator : AbstractValidator<CreateTaskViewModel>
    {
        public TodoValidator()
        {
            RuleFor(todo => todo.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot be more than 100 characters.");

            RuleFor(todo => todo.Priority)
                .InclusiveBetween(1, 5)
                .WithMessage("Priority must be between 1 (High) and 5 (Low).");

            RuleFor(todo => todo.DueDate)
                .GreaterThan(DateTime.UtcNow)
                .When(todo => todo.DueDate.HasValue)
                .WithMessage("Due Date must be in the future.");

            RuleFor(todo => todo.UserId)
                .GreaterThan(0).WithMessage("UserId must be a valid positive number.");
            // Validate Latitude (optional and must be between -90 and 90)
            RuleFor(todo => todo.Latitude)
           .Must(ValidLatitude).WithMessage("Latitude must be between -90 and 90 degrees.")
           .When(todo => todo.Latitude.HasValue);

            // Validate Longitude (optional and must be between -180 and 180)
            RuleFor(todo => todo.Longitude)
                .Must(ValidLongitude).WithMessage("Longitude must be between -180 and 180 degrees.")
                .When(todo => todo.Longitude.HasValue);
        }
        private bool ValidLatitude(double? latitude)
        {
            return latitude >= -90 && latitude <= 90;
        }

        // Helper function to validate Longitude
        private bool ValidLongitude(double? longitude)
        {
            return longitude >= -180 && longitude <= 180;
        }
      }
    }

