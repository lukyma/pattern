using FluentValidation;

namespace pattern.strategy.test.Fakes
{
    public class RequestFake
    {
        public class Request
        {
            public string Name { get; set; }
        }
        public class RequestValidator : AbstractValidator<Request>
        {
            public RequestValidator()
            {
                RuleFor(o => o.Name)
                    .NotEmpty()
                    .WithErrorCode("001")
                    .WithMessage("test error message");
            }
        }
    }
}
