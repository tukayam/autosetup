
[![Build status](https://ci.appveyor.com/api/projects/status/51wn45ti1i8k4obv/branch/master?svg=true)](https://ci.appveyor.com/project/TubaKaya/autosetup/branch/master)
[![Nuget Downloads](https://img.shields.io/nuget/dt/autosetup.svg)](https://www.nuget.org/packages/AutoSetup/)

# AutoSetup

**AutoSetup** generates mocks and setup for a class under test. 

## Usage

Install nuget package on your test project. 

Package manager command:
`Install-Package AutoSetup`

![Steps](https://user-images.githubusercontent.com/6681935/49291645-535d2b80-f4ab-11e8-9676-0d8a2c1e466e.gif)

1. *Go to test class*
2. Locate the cursor on test class name
3. See lightbulb "(Re-)Generate SetUp"

Please note:
* Class under test is detected based on naming convention. For a test class with name "CalculatorTests", class under test is assumed to have the name "Calculator". Class under test with name "Calculator" will be detected from within the solution. For each parameter in class under test's constructor, mocks will be generated.
* Locate the cursor on test class name to see option to "(Re-)Generate Setup"

## License

This project is licensed under the [Apache 2.0 License](LICENSE.txt).
