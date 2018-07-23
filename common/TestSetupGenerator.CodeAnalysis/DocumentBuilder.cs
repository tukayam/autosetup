using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeGenerators;

namespace TestSetupGenerator.CodeAnalysis
{
    public class DocumentBuilder
    {
        private readonly IMemberReplacer _memberReplacer;

        private Document _document;
        private ClassDeclarationSyntax _testClass;

        private Document _newDocument;
        private ClassDeclarationSyntax _newtestClass;

        public DocumentBuilder(IMemberReplacer memberReplacer)
        {
            _memberReplacer = memberReplacer;
        }

        public DocumentBuilder SetDocument(Document document)
        {
            _document = document;
            return this;
        }

        public DocumentBuilder SetTestClass(ClassDeclarationSyntax testClass)
        {
            _testClass = testClass;
            _newtestClass = testClass;
            return this;
        }

        public DocumentBuilder WithSetupMethodAsync(MethodDeclarationSyntax newSetupMethod)
        {
            _newtestClass = _memberReplacer.ReplaceOrInsert(_newtestClass, newSetupMethod);
            return this;
        }

        public DocumentBuilder WithFieldsAsync(IEnumerable<SyntaxNode> newFields)
        {
            _newtestClass = _memberReplacer.InsertIfNotExists(_newtestClass, newFields) as ClassDeclarationSyntax;
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
