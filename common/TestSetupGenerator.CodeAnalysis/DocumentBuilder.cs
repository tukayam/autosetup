using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;

namespace TestSetupGenerator.CodeAnalysis
{
    public interface IDocumentBuilder
    {
        DocumentBuilder WithSetupMethodAsync(MethodDeclarationSyntax newSetupMethod);
        DocumentBuilder WithFieldsAsync(IEnumerable<SyntaxNode> newFields);
        DocumentBuilder WithUsingsAsync(IEnumerable<UsingDirectiveSyntax> newUsingDirectives);
        Task<Document> Build();
    }

    public class DocumentBuilder : IDocumentBuilder
    {
        private readonly IMemberReplacer _memberReplacer;

        private readonly Document _document;
        private readonly ClassDeclarationSyntax _testClass;

        private Document _newDocument;
        private ClassDeclarationSyntax _newtestClass;

        public DocumentBuilder(IMemberReplacer memberReplacer, Document document, ClassDeclarationSyntax testClass)
        {
            _memberReplacer = memberReplacer;
            _document = document;
            _testClass = testClass;
            _newtestClass = testClass;
        }

        public DocumentBuilder WithSetupMethodAsync(MethodDeclarationSyntax newSetupMethod)
        {
            _newtestClass = _memberReplacer.ReplaceOrInsert(_newtestClass, newSetupMethod);
            return this;
        }

        public DocumentBuilder WithFieldsAsync(IEnumerable<SyntaxNode> newFields)
        {
            _memberReplacer.InsertIfNotExists(_newtestClass, newFields);
            return this;
        }

        public DocumentBuilder WithUsingsAsync(IEnumerable<UsingDirectiveSyntax> newUsingDirectives)
        {
            var docRoot = _document.GetSyntaxRootAsync().Result;
            var newDocRoot = _memberReplacer.InsertIfNotExists(docRoot, newUsingDirectives);
            _newDocument = _document.WithSyntaxRoot(newDocRoot);
            return this;
        }

        public async Task<Document> Build()
        {
            var newDocumentRoot = await _newDocument.GetSyntaxRootAsync();
            var newRoot = newDocumentRoot.ReplaceNode(_testClass, _newtestClass);
            return _newDocument.WithSyntaxRoot(newRoot);
        }
    }
}
