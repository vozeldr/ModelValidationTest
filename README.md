# Model Validation Test

This project demonstrates an issue with model validation using DataAnnotations in unit tests.

When run in an MVC project, models decorated with these DataAnnotations do appear to behave correctly.

The problem __appears__ to be that for some reason only the RequiredAttribute is used by the validator.

Comments are added to the [ExampleModelTest.cs](test/TestProject1/ExampleModelTest.cs) class to describe the issues further.