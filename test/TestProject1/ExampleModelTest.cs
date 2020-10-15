using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace TestProject1
{
    /// <summary>
    /// An example model class with data annotations for <see cref="RequiredAttribute"/>,
    /// <see cref="RegularExpressionAttribute"/> and <see cref="StringLengthAttribute"/>.
    /// </summary>
    public class ExampleModel
    {
        #region Constants

        /// <summary>
        /// The regular expression used to validation that a Guid is not <see cref="Guid.Empty"/>.
        /// </summary>
        public const string EmptyGuidValidationRegex = "^(?<!00000000-0000-0000-0000-000000000000)$";

        /// <summary>
        /// The maximum allowed length of the <see cref="Name"/> property.
        /// </summary>
        public const int MaxNameLength = 50;

        #endregion Constants

        #region Static Methods

        /// <summary>
        /// Generates a string that is longer than the allowed length for the <see cref="Name"/>.
        /// </summary>
        /// <returns>A string that is longer than the allowed length for the <see cref="Name"/>.</returns>
        public static string GenerateInvalidName()
        {
            return new string('c', MaxNameLength + 10);
        }

        #endregion Static Methods

        #region Properties

        /// <summary>
        /// The unique identifier for the model.
        /// </summary>
        [Key, RegularExpression(EmptyGuidValidationRegex)]
        public Guid Id { get; set; }

        /// <summary>
        /// The name for the model.
        /// </summary>
        [Required, StringLength(MaxNameLength)]
        public string Name { get; set; }

        #endregion Properties
    }

    /// <summary>
    /// Tests that demonstrate the issues with validation.
    /// </summary>
    public class ExampleModelTest
    {
        /// <summary>
        /// This test gets rid of lint warnings in the IDE for me complaining about getters not being used.
        /// It's not relevant to this demonstration.
        /// </summary>
        [Fact]
        public void AccessorsShouldWork()
        {
            Guid id = Guid.NewGuid();
            const string name = "Bob";

            ExampleModel model = new ExampleModel
            {
                Id = id,
                Name = name
            };

            Assert.Equal(id, model.Id);
            Assert.Equal(name, model.Name);
        }

        /// <summary>
        /// This test affirms that the regular expression defined does in fact fail validation when compared with
        /// <see cref="Guid.Empty"/>.
        /// </summary>
        [Fact]
        public void EmptyGuidShouldFailRegex()
        {
            RegularExpressionAttribute
                attribute = new RegularExpressionAttribute(ExampleModel.EmptyGuidValidationRegex);
            Assert.False(attribute.IsValid(Guid.Empty));
        }

        /// <summary>
        /// This test affirms that the string length does in fact fail validation when compared with the result of
        /// <see cref="ExampleModel.GenerateInvalidName()"/>
        /// </summary>
        [Fact]
        public void LongStringShouldFailValidation()
        {
            StringLengthAttribute attribute = new StringLengthAttribute(ExampleModel.MaxNameLength);
            Assert.False(attribute.IsValid(ExampleModel.GenerateInvalidName()));
        }

        /// <summary>
        /// This test should (and does) pass because the id is valid but the name is not provided.
        /// </summary>
        [Fact]
        public void ShouldFailNameRequiredValidation()
        {
            ExampleModel model = new ExampleModel
            {
                Id = Guid.NewGuid(),
                Name = string.Empty
            };
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(model);
            Assert.False(Validator.TryValidateObject(model, context, validationResults));
            Assert.Single(validationResults);
            Assert.Equal("Name", validationResults[0].MemberNames.First());
            Assert.Equal("The Name field is required.", validationResults[0].ErrorMessage);
        }

        /// <summary>
        /// This test SHOULD pass because there should be multiple validation errors: the name is not provided
        /// but is required AND the id is <see cref="Guid.Empty"/>.
        /// </summary>
        [Fact]
        public void ShouldHaveMultipleValidationErrors()
        {
            ExampleModel model = new ExampleModel();
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(model);
            Assert.False(Validator.TryValidateObject(model, context, validationResults));
            Assert.NotEmpty(validationResults);
            Assert.NotEqual(1, validationResults.Count);
        }

        /// <summary>
        /// This test SHOULD pass because the name is valid but the id is <see cref="Guid.Empty"/>.
        /// </summary>
        [Fact]
        public void ShouldRecognizeEmptyGuidAsInvalid()
        {
            ExampleModel model = new ExampleModel
            {
                Name = "Bob"
            };
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(model);
            Assert.False(Validator.TryValidateObject(model, context, validationResults));
            Assert.Single(validationResults);
        }

        /// <summary>
        /// This test SHOULD pass because the id is valid but the name is too long.
        /// </summary>
        [Fact]
        public void ShouldFailNameLengthValidation()
        {
            ExampleModel model = new ExampleModel
            {
                Id = Guid.NewGuid(),
                Name = ExampleModel.GenerateInvalidName()
            };
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(model);
            Assert.False(Validator.TryValidateObject(model, context, validationResults));
            Assert.Single(validationResults);
        }
    }
}