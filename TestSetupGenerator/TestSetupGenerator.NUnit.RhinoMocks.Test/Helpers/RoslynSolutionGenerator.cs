using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace TestSetupGenerator.NUnit.Rhino.Test.Helpers
{
    public class RoslynSolutionGenerator
    {
        public Solution GetTestClassGeneratorSamplesSolutionAsync()
        {
            var msWorkspace = MSBuildWorkspace.Create();
            //string currentDir = Environment.CurrentDirectory;
            //string relativePath = "../../../../TestSamples/TestSamples.sln";
            //string solutionPath = currentDir + relativePath;

            var solutionPath = @"C:\Users\tuba\Projects\TestSetupGenerator\TestSamples\TestSamples.sln";

            //You must install the MSBuild Tools or this line will throw an exception:
            return msWorkspace.OpenSolutionAsync(solutionPath).Result;
        }

        public async Task<Solution> GetSolutionAsync(string relativePath)
        {
            var msWorkspace = MSBuildWorkspace.Create();
            string currentDir = Environment.CurrentDirectory;
            string solutionPath = currentDir + relativePath;

            //You must install the MSBuild Tools or this line will throw an exception:
            return await msWorkspace.OpenSolutionAsync(solutionPath);
        }
    }
}
