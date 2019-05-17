using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoSetup.CodeAnalyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoSetup
{
    public class DocumentBuilder
    {
        private readonly IMemberFinder _memberFinder;
        private readonly IFieldFinder _fieldFinder;

        private readonly Document _document;
        private readonly ClassDeclarationSyntax _testClass;

        private MemberDeclarationSyntax _newSetupMethod;
        private IEnumerable<FieldDeclarationSyntax> _newFields;
        private IEnumerable<UsingDirectiveSyntax> _newUsingDirectives;

        public DocumentBuilder(IMemberFinder memberFinder, IFieldFinder fieldFinder, Document document, ClassDeclarationSyntax testClass)
        {
            _memberFinder = memberFinder;
            _fieldFinder = fieldFinder;
            _document = document;
            _testClass = testClass;
        }

        public DocumentBuilder WithSetupMethod(MemberDeclarationSyntax newSetupMethod)
        {
            _newSetupMethod = newSetupMethod;
            return this;
        }

        public DocumentBuilder WithFields(IEnumerable<FieldDeclarationSyntax> newFields)
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
                if (_memberFinder.FindSimilarNode(members, _newSetupMethod) is MemberDeclarationSyntax existingSetupMethod)
                {
                    members = members.Remove(existingSetupMethod);
                }

                members = members.Insert(0, _newSetupMethod);
            }

            if (_newFields != null)
            {
                foreach (var newField in _newFields)
                {
                    var existingFieldWithSameType = _fieldFinder.FindSimilarNode(members, newField);
                    if (existingFieldWithSameType != null)
                    {
                        members = members.Remove(existingFieldWithSameType);
                    }
                }

                members = members.InsertRange(0, _newFields);
            }

            var newClass = _testClass.WithMembers(members);
            var newRoot = root.ReplaceNode(_testClass, newClass);

            if (_newUsingDirectives != null)
            {
                //newRoot = AddUsingDirectives(newRoot);
            }

            var newDocument = _document.WithSyntaxRoot(newRoot);
            return newDocument;
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
