using FluentValidation;

namespace pattern.sample.api.StrategyHandler.Validator
{
    public class TestStrategyRequestValidator : AbstractValidator<TestStrategyRequest>
    {
        public TestStrategyRequestValidator()
        {
            RuleFor(o => o.Name)
                .NotEmpty()
                .WithErrorCode("001")
                .WithMessage("Message test validator");
        }
    }

    public class TestStrategyRequestValidator2 : AbstractValidator<TestStrategyRequest2>
    {
        public TestStrategyRequestValidator2()
        {
            RuleFor(o => o.Name)
                .NotEmpty()
                .WithErrorCode("001")
                .WithMessage("Message test validator");
        }
    }
}
