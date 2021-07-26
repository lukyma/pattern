using FluentValidation.Results;
using System.Collections.Generic;

namespace pattern.strategy
{
    public class ValidationErrors : List<ValidationFailure>, IValidationErrors
    {

    }

    public interface IValidationErrors : IList<ValidationFailure>
    {

    }
}
