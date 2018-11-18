using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestSetupGenerator.CodeAnalysis.CodeAnalyzers
{
    public interface IMemberFinder
    {
        SyntaxNode FindSimilarNode(SyntaxList<MemberDeclarationSyntax> members, SyntaxNode node);
    }

    public class MemberFinder : IMemberFinder
    {
        public SyntaxNode FindSimilarNode(SyntaxList<MemberDeclarationSyntax> members, SyntaxNode node)
        {
            var nodesWithSameName = members.Where(_ => CompareNodeNames(node, _)).ToList();

            if (nodesWithSameName.Any())
            {
                var similarNodes = nodesWithSameName.Where(_ => CompareNodeParameters(node, _)).ToList();

                if (similarNodes.Count == 1)
                {
                    return similarNodes.First();
                }
            }

            return null;
        }

        private bool CompareNodeNames(SyntaxNode node, SyntaxNode nodeToCompare)
        {
            var nameOfNode = GetNodeName(node);

            if (nameOfNode != null)
            {
                var nameOfComparedNode = GetNodeName(nodeToCompare);

                if (nameOfComparedNode != null)
                {
                    return nameOfNode == nameOfComparedNode;
                }
            }

            return false;
        }

        private string GetNodeName(SyntaxNode node)
        {
            var name = node.ChildTokens()?.FirstOrDefault(_ => _.IsKind(SyntaxKind.IdentifierToken));
            return name?.Text;
        }

        private bool CompareNodeParameters(SyntaxNode node, SyntaxNode nodeToCompare)
        {
            var parametersOfNode = node.DescendantNodes().OfType<ParameterSyntax>().ToList();
            var parameters = nodeToCompare.DescendantNodes().OfType<ParameterSyntax>().ToList();

            if (parametersOfNode.Any() && parameters.Any())
            {
                var sameAmount = parametersOfNode.Count() == parameters.Count();
                if (sameAmount)
                {
                    foreach (var parameter in parametersOfNode)
                    {
                        if (!parameters.Any(_ => _.Type.ToString() == parameter.Type.ToString()))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
