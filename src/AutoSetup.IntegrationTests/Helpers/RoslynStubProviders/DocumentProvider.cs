using System.Collections.Generic;
using System.Linq;
using AutoSetup.IntegrationTests.Helpers.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace AutoSetup.IntegrationTests.Helpers.RoslynStubProviders
{
    static class DocumentProvider
    {
        private static readonly MetadataReference CorlibReference = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
        private static readonly MetadataReference SystemCoreReference = MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location);
        private static readonly MetadataReference CSharpSymbolsReference = MetadataReference.CreateFromFile(typeof(CSharpCompilation).Assembly.Location);
        private static readonly MetadataReference CodeAnalysisReference = MetadataReference.CreateFromFile(typeof(Compilation).Assembly.Location);

        internal static string DefaultFilePathPrefix = "Test";
        internal static string CSharpDefaultFileExt = "cs";
        internal static string VisualBasicDefaultExt = "vb";
        internal static string TestProjectName = "TestProject";


        /// <summary>
        /// Create a Document from a string through creating a project that contains it.
        /// </summary>
        /// <param name="source">Classes in the form of a string</param>
        /// <param name="language">The language the source code is in</param>
        /// <returns>A Document created from the source string</returns>
        public static Document CreateDocumentFromFile(string filePath, string language = LanguageNames.CSharp)
        {
            return CreateCompilationAndReturnDocuments(new[] { filePath }, language).First().Value;
        }

        /// <summary>
        /// Create a project using the inputted strings as sources.
        /// </summary>
        /// <param name="sources">Classes in the form of strings</param>
        /// <param name="language">The language the source code is in</param>
        /// <returns>A Project created out of the Documents created from the source strings</returns>
        public static Dictionary<string, Document> CreateCompilationAndReturnDocuments(IEnumerable<string> filePaths, string language = LanguageNames.CSharp)
        {
            string fileNamePrefix = DefaultFilePathPrefix;
            string fileExt = language == LanguageNames.CSharp ? CSharpDefaultFileExt : VisualBasicDefaultExt;

            var projectId = ProjectId.CreateNewId(debugName: TestProjectName);

            var solution = new AdhocWorkspace()
                .CurrentSolution
                .AddProject(projectId, TestProjectName, TestProjectName, language)
                .AddMetadataReference(projectId, CorlibReference)
                .AddMetadataReference(projectId, SystemCoreReference)
                .AddMetadataReference(projectId, CSharpSymbolsReference)
                .AddMetadataReference(projectId, CodeAnalysisReference);

            var documents = new Dictionary<string, Document>();
            int count = 0;

            foreach (var filePath in filePaths)
            {

                var newFileName = fileNamePrefix + count + "." + fileExt;
                var documentId = DocumentId.CreateNewId(projectId, debugName: newFileName);
                var source = TextFileReader.ReadFile(filePath);
                solution = solution.AddDocument(documentId, newFileName, SourceText.From(source));
                count++;
                documents.Add(filePath, solution.GetDocument(documentId));
            }
            return documents;
        }
    }
}
