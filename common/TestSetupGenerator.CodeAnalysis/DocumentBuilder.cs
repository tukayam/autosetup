using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;

namespace TestSetupGenerator.CodeAnalysis
{
    public interface IDocumentBuilder
    {
        DocumentBuilder WithSetupMethod(MethodDeclarationSyntax newSetupMethod);
        DocumentBuilder WithFields(IEnumerable<SyntaxNode> newFields);
        DocumentBuilder WithUsings(IEnumerable<UsingDirectiveSyntax> newUsingDirectives);
        Task<Document> BuildAsync(CancellationToken cancellationToken);
    }

    public class DocumentBuilder : IDocumentBuilder
    {
        private readonly IMemberFinder _memberFinder;

        private readonly Document _document;
        private readonly ClassDeclarationSyntax _testClass;

        private MethodDeclarationSyntax _newSetupMethod;
        private IEnumerable<SyntaxNode> _newFields;
        private IEnumerable<UsingDirectiveSyntax> _newUsingDirectives;

        public DocumentBuilder(IMemberFinder memberFinder, Document document, ClassDeclarationSyntax testClass)
        {
            _memberFinder = memberFinder;
            _document = document;
            _testClass = testClass;
        }

        public DocumentBuilder WithSetupMethod(MethodDeclarationSyntax newSetupMethod)
        {
            _newSetupMethod = newSetupMethod;
            return this;
        }

        public DocumentBuilder WithFields(IEnumerable<SyntaxNode> newFields)
        {
            _newFields = newFields;
            return this;
        }

        public DocumentBuilder WithUsings(IEnumerable<UsingDirectiveSyntax> newUsingDirectives)
        {
            _newUsingDirectives = newUsingDirectives;
            return this;
        }

        public async Task<Document> BuildAsync(CancellationToken cancellationToken)
        {
            var root = await _document.GetSyntaxRootAsync(cancellationToken);
            var members = _testClass.Members;

            if (_newSetupMethod != null)
            {
                members = AddToMembers(members, _newSetupMethod);
            }

            if (_newFields != null)
            {
                foreach (var syntaxNode in _newFields)
                {
                    var newField = (FieldDeclarationSyntax)syntaxNode;
                    members = AddToMembers(members, newField);
                }
            }

            var newClass = _testClass.WithMembers(members);
            var newRoot = root.ReplaceNode(_testClass, newClass);

            if (_newUsingDirectives != null)
            {
                newRoot = AddUsingDirectives(newRoot);
            }

            var newDocument = _document.WithSyntaxRoot(newRoot);
            return newDocument;
        }

        private SyntaxList<MemberDeclarationSyntax> AddToMembers(SyntaxList<MemberDeclarationSyntax> members, MemberDeclarationSyntax newMember)
        {
            if (_memberFinder.FindSimilarNode(newMember, _testClass) is MemberDeclarationSyntax existingMember)
            {
                members = members.Replace(existingMember, newMember);
            }
            else
            {
                members = members.Insert(0, newMember);
            }

            return members;
        }

        private SyntaxNode AddUsingDirectives(SyntaxNode newRoot)
        {
            var existingUsingDirectives = newRoot.DescendantNodes().OfType<UsingDirectiveSyntax>().ToList();

            var firstUsingDirectiveInFile = existingUsingDirectives.FirstOrDefault();

            var usingsToAdd = _newUsingDirectives.Where(_ => existingUsingDirectives.All(u => u.Name != _.Name));

            if (firstUsingDirectiveInFile != null)
            {
                newRoot = newRoot.InsertNodesAfter(firstUsingDirectiveInFile, usingsToAdd);
            }
            return newRoot;
        }
    }
}
