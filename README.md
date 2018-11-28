

# AutoSetup

Automated test setup generator.

Finds class under test from within the solution. Detects its dependencies. Generates mocks for each dependency, target instance and setup method.

## Usage

 *Install nuget package in your test project*
 
 `Install-Package AutoSetup`

*Locate to test class*<br/>
<img src="https://user-images.githubusercontent.com/6681935/49150529-7651d900-f30d-11e8-99e2-88012b076682.png" alt="Locate to Test class" height="200" />

*Click (Re-)Generate SetUp*<br/>
<img src="https://user-images.githubusercontent.com/6681935/49150214-80270c80-f30c-11e8-85b9-ff0dbc71d749.png" alt="Click Re-Generate SetUp" height="290" />

*See generated fields and constructor with setup*<br/>
<img src="https://user-images.githubusercontent.com/6681935/49150247-9208af80-f30c-11e8-9656-7e99c5a27b25.png" alt="See Generated setup as constructor for XUnit with mock objects using Moq" height="295" />

## Getting Started

Please install prerequisites, clone the source code and follow the installment steps to get a development environment running.

### Prerequisites

This project requires 
* .NET Compiler Platform SDK. 

To install **.NET Compiler Platform SDK** please run Visual Studio Installer. There choose Modify and then make sure to include "Visual Studio extension development" and within this category check the box for ".NET Compiler Platform SDK".

Note: .Net Compiler Platform SDK Extension from Microsoft Marketplace is not compatible anymore with Visual Studio 2017. Please install using the Visual Studio Installer as suggested above.

### Installing

* Add nuget feed source to local environment (please ask for the credentials)
* Restore nuget packages
* Build solution
* Run AutoSetup.Vsix to test the Visual Studio extension
* Or, install the generated nuget package autosetup.pkg into a project where you would like to see the features in action.

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

Uses AppVeyor for CI & CD pipelines. See [appveyor.yml](appveyor.yml).

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

<!--
## Authors

* **Tuba Kaya** - *Initial work* - [autosetup](https://github.com/tukaya/autosetup)

See also the list of [contributors](https://github.com/tukaya/autosetup/contributors) who participated in this project.

-->

## License

This project is licensed under the Apache 2.0 License - see the [LICENSE.txt](LICENSE.txt) file for details

<!--
## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
-->