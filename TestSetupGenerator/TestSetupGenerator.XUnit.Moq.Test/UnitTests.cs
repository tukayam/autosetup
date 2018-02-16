using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NUnit.Framework;
using TestHelper;
using TestSetupGeneratorXUnit.Moq;

namespace TestSetupGenerator.Test
{
    [TestFixture]
    public class UnitTest : CodeFixVerifier
    {
        //No diagnostics expected to show up
        [TestMethod]
        public void NoDiagnosticsShown_When_EmptyCodeDocument()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestCaseSource(typeof(TestCases))]
        public void DiagnosticAndCodeFixBothCorrect(string test, string fixtest)
        {
            var expected = new DiagnosticResult
            {
                Id = "TestGenerator",
                Message = String.Format("(Re-)Generate SetUp for '{0}'", "TypeName"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 12, 15)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
            VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new TestSetupGeneratorCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new TestSetupGeneratorAnalyzer();
        }
    }
}