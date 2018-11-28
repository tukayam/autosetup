[![Build status](https://ci.appveyor.com/api/projects/status/51wn45ti1i8k4obv/branch/master?svg=true)](https://ci.appveyor.com/project/TubaKaya/autosetup/branch/master)
[![Nuget Downloads](https://img.shields.io/nuget/dt/autosetup.svg)](https://www.nuget.org/packages/AutoSetup/)

# AutoSetup

**AutoSetup** automatically generates mocks and setup for a class under test. It finds the class under test based on naming convention. Detects its dependencies and generates mocks for each dependency, as well as a setup method initializing a target instance using the mocked fields.

## Usage

Please first install nuget package on your test project. 

Package manager command:
`Install-Package AutoSetup`

#### Steps

![Alt Text](https://im.ezgif.com/tmp/ezgif-1-bace72b027d2.gif)
1. *Go to test class*
2.  Locate the cursor on test class name
3. See lightbulb "(Re-)Generate SetUp" and click
4. See generated constructor for class under test

## Contributing

Please install prerequisites, clone the source code and follow installation steps to get a development environment running.

### Prerequisites

This project requires 
* .NET Compiler Platform SDK. 

To install **.NET Compiler Platform SDK** please run Visual Studio Installer. There choose Modify and then make sure to include "Visual Studio extension development" and within this category check the box for ".NET Compiler Platform SDK".

Note: .Net Compiler Platform SDK Extension from Microsoft Marketplace is not compatible anymore with Visual Studio 2017. Please install using the Visual Studio Installer as suggested above.

### Installing

* Add AppVeyor nuget feed source to local environment (please ask for the credentials)
* Restore nuget packages
* Build solution
* Run AutoSetup.Vsix to test the Visual Studio extension
* Or, install the generated nuget package autosetup.pkg into a project.

## License

This project is licensed under the [Apache 2.0 License](LICENSE.txt).

<!--
## Acknowledgments

* Hat tip to anyone whose code was used
* Inspiration
* etc
-->
