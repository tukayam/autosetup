# AutoSetup

Automatically (re)generates setup method in test class with mocks for dependencies of the class under test.

## Getting Started

Please install prerequisites, clone the source code and follow the installment steps to get a development environment running.

### Prerequisites

This project requires 
* .NET Compiler Platform SDK. 

To install **.NET Compiler Platform SDK** please run Visual Studio Installer. There choose Modify and then make sure to include "Visual Studio extension development" and within this category check the box for ".NET Compiler Platform SDK".

Note: .Net Compiler Platform SDK Extension from Microsoft Marketplace is not compatible anymore with Visual Studio 2017. Please install using the Visual Studio Installer as suggested above.

### Installing

* Restore nuget packages
* Build solution
* Run TestSetupGenerator.XUnitMoq.Vsix to test the Visual Studio extension
* Or, install the generated nuget package from TestSetupGenerator.XUnitMoq\bin\debug into a project where you would like to see the features in action.

You should be able to generate the setup method in a test class by following below steps:

1. Open "LibraryUnderTest.sln" under path "Example"
2. Restore nuget packages (please notice nuget package is not published to nuget.org yet. Please make sure the generated nupkg from TestSetupGenerator.XUnitMoq project is available in your available package sources).
3. Build solution
4. Go to LibraryTests project and open Class1Tests.cs
5. Locate the cursor on class name "Class1Tests"
6. See lightbulb "Re-generate setup" and click
7. See generated constructor for class under test (LibraryUnderTest\Class1.cs)
<!--
## Running the tests

Explain how to run the automated tests for this system

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

Explain what these tests test and why

```
Give an example
```
-->

## Deployment

Build and release pipelines are being setup. See [Issue #1](https://github.com/tukaya/autosetup/issues/1) and [Issue #5](https://github.com/tukaya/autosetup/issues/5).

<!--
## Built With

* [Dropwizard](http://www.dropwizard.io/1.0.2/docs/) - The web framework used
* [Maven](https://maven.apache.org/) - Dependency Management
* [ROME](https://rometools.github.io/rome/) - Used to generate RSS Feeds

## Contributing

Please read [CONTRIBUTING.md](https://gist.github.com/PurpleBooth/b24679402957c63ec426) for details on our code of conduct, and the process for submitting pull requests to us.
-->
## Versioning

We use [SemVer](http://semver.org/) for versioning. 

## Authors

* **Tuba Kaya** - *Initial work* - [autosetup](https://github.com/tukaya/autosetup)

See also the list of [contributors](https://github.com/tukaya/autosetup/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

<!--
## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
-->