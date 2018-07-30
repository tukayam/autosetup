using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TestSetupGenerator.CodeAnalysis.CodeAnalyzers;

namespace TestSetupGenerator.CodeAnalysis
{
    public interface IMemberReplacer
    {
        ClassDeclarationSyntax ReplaceOrInsert(ClassDeclarationSyntax root, MemberDeclarationSyntax newMember);
        SyntaxNode InsertIfNotExists<T>(SyntaxNode root, IEnumerable<T> newMembers) where T : SyntaxNode;
        IEnumerable<SyntaxNode> ReplaceFieldDeclarations(ClassDeclarationSyntax classUnderTestDeclarationSyntax, SyntaxList<MemberDeclarationSyntax> members, IEnumerable<SyntaxNode> fieldDeclarations);
    }

    public class MemberReplacer : IMemberReplacer
    {
        private readonly IMemberFinder _memberFinder;

        public MemberReplacer(IMemberFinder memberFinder)
        {
            _memberFinder = memberFinder;
        }

        public ClassDeclarationSyntax ReplaceOrInsert(ClassDeclarationSyntax root, MemberDeclarationSyntax newMember)
        {
            var members = root.Members;
            if (_memberFinder.FindSimilarNode(newMember, root) is MemberDeclarationSyntax existingMember)
            {
                members = members.Replace(existingMember, newMember);
            }
            else
            {
                members = members.Insert(0, newMember);
            }

            return root.WithMembers(members);
        }

        public SyntaxNode InsertIfNotExists<T>(SyntaxNode root, IEnumerable<T> newMembers) where T : SyntaxNode
        {
            var existingMembers = root.DescendantNodes().OfType<T>().ToList();
            var membersToInsert = new List<T>();
            foreach (var newMember in newMembers)
            {
                var memberAlreadyExists = _memberFinder.FindSimilarNode(newMember, root) != null;
                if (!memberAlreadyExists)
                {
                    membersToInsert.Add(newMember);
                }
            }

            var firstMember = existingMembers.FirstOrDefault();
            var nodeToInsertAfter = firstMember ?? root.DescendantNodes().First();
            if (firstMember != null)
            {
                return root.InsertNodesAfter(firstMember, membersToInsert);
            }

            return root.InsertNodesBefore(nodeToInsertAfter, membersToInsert);
        }

        public IEnumerable<SyntaxNode> ReplaceFieldDeclarations(ClassDeclarationSyntax classUnderTestDeclarationSyntax, SyntaxList<MemberDeclarationSyntax> members, IEnumerable<SyntaxNode> fieldDeclarations)
        {
            var existingFieldDeclarationVariables = members.OfType<FieldDeclarationSyntax>()
                .SelectMany(_ => _.Declaration.Variables).Select(_ => _.Identifier.Text).ToList();

            var index = 0;
            //foreach field replace or add
            foreach (FieldDeclarationSyntax fieldDeclaration in fieldDeclarations)
            {
                var fieldDeclarationText = fieldDeclaration.Declaration.Variables.Select(_ => _.Identifier.Text);
                if (!existingFieldDeclarationVariables.Any(_ => fieldDeclarationText.Contains(_)))
                {
                    members = members.Insert(index++, fieldDeclaration);
                }
            }

            return members;
        }
    }
}