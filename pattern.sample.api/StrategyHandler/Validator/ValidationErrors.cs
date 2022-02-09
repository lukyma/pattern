using FluentValidation.Results;
using System.Collections.Generic;

namespace pattern.sample.api.StrategyHandler.Validator
{
    public interface IValidationErrors : IList<ValidationFailure>
    {
    }

    public class ValidationErrors : List<ValidationFailure>, IValidationErrors
    {

    }
}
