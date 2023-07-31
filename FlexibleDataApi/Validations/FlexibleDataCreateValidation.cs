using FlexibleDataApi.Models.DTO;
using FluentValidation;

namespace FlexibleDataApi.Validations
{
	public class FlexibleDataCreateValidation: AbstractValidator<FlexibleDataCreateDto>
	{
		public FlexibleDataCreateValidation()
		{
            RuleFor(dto => dto.Data)
                .NotEmpty().WithMessage("The 'Data' dictionary must not be empty.")
                .NotNull().WithMessage("The 'Data' dictionary must be provided.");

        }
    }
}

