using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestSetupGenerator.CodeAnalysis.CodeAnalyzers
{
    public interface IFieldFinder
    {
        FieldDeclarationSyntax FindSimilarNode(SyntaxNode root, FieldDeclarationSyntax node);
    }

    public class FieldFinder : IFieldFinder
    {
        public FieldDeclarationSyntax FindSimilarNode(SyntaxNode root, FieldDeclarationSyntax node)
        {
            var nodesWithSameType = root.DescendantNodesAndSelf().OfType<FieldDeclarationSyntax>()
                                        .Where(_ => NodesAreSameType(node, _)).ToList();

            return nodesWithSameType.FirstOrDefault();
        }

        private bool NodesAreSameType(FieldDeclarationSyntax node, FieldDeclarationSyntax nodeToCompare)
        {
            return GetFieldType(node) == GetFieldType(nodeToCompare);
        }

        private string GetFieldType(FieldDeclarationSyntax node)
        {
            var name = node.ChildNodes().OfType<VariableDeclarationSyntax>().First()
                .ChildTokens()?.FirstOrDefault(_ => _.IsKind(SyntaxKind.IdentifierToken));
            return name?.Text;
        }
    }
}
