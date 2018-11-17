using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;

namespace TestSetupGenerator.CodeAnalysis
{
    public class DocumentBuilder
    {
        private readonly IMemberFinder _memberFinder;
        private readonly IFieldFinder _fieldFinder;

        private readonly Document _document;
        private readonly ClassDeclarationSyntax _testClass;

        private MemberDeclarationSyntax _newSetupMethod;
        private IEnumerable<SyntaxNode> _newFields;
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
                members = AddToFields(members, _newFields.Select(_ => _ as FieldDeclarationSyntax));
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

        private SyntaxList<MemberDeclarationSyntax> AddToFields(SyntaxList<MemberDeclarationSyntax> members, IEnumerable<FieldDeclarationSyntax> newFields)
        {
            var membersToInsert = new List<MemberDeclarationSyntax>();
            foreach (var newMember in newFields)
            {
                if (_fieldFinder.FindSimilarNode(_testClass, newMember) == null)
                {
                    membersToInsert.Add(newMember);
                }
            }

            members = members.InsertRange(0, membersToInsert);
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
