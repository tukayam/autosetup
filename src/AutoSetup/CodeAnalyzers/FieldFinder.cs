using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoSetup.CodeAnalyzers
{
    public interface IFieldFinder
    {
        FieldDeclarationSyntax FindSimilarNode(SyntaxList<MemberDeclarationSyntax> members, FieldDeclarationSyntax node);
    }

    public class FieldFinder : IFieldFinder
    {
        public FieldDeclarationSyntax FindSimilarNode(SyntaxList<MemberDeclarationSyntax> members, FieldDeclarationSyntax node)
        {
            var nodesWithSameType = members.OfType<FieldDeclarationSyntax>()
                .Where(_ => NodesAreSameType(node, _)).ToList();

            return nodesWithSameType.FirstOrDefault();
        }

        private bool NodesAreSameType(FieldDeclarationSyntax node, FieldDeclarationSyntax nodeToCompare)
        {
            return GetFieldType(node) == GetFieldType(nodeToCompare);
        }

        private string GetFieldType(FieldDeclarationSyntax node)
        {
            return node.Declaration.Type.ToString();
        }
    }
}
