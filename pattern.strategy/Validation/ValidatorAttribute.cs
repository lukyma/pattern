using System;

namespace patterns.strategy
{
    public class ValidatorAttribute : Attribute
    {
        public ValidatorAttribute(Type validatiorType)
        {
            ValidationType = validatiorType;
        }
        public Type ValidationType { get; set; }
    }
}
