using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace _12
{
    public class Node
    {

        public string Identifier { get; }

        public IList<Node> Neighbours { get; }

        public bool IsBig => Identifier.ToUpperInvariant() == Identifier;

        public bool IsEndNode => Identifier == "end";
        public bool IsStartNode => Identifier == "start";

        public Node(string identifier)
        {
            Identifier = identifier;
            Neighbours = new List<Node>();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);
            List<Node> nodes = new List<Node>();
            foreach (var line in lines)
            {
                var terms = line.Split("-");

                if (!nodes.Any(n => n.Identifier == terms[0]))
                {
                    nodes.Add(new Node(terms[0]));
                }
                if (!nodes.Any(n => n.Identifier == terms[1]))
                {
                    nodes.Add(new Node(terms[1]));
                }

                var node1 = nodes.First(n => n.Identifier == terms[0]);
                var node2 = nodes.First(n => n.Identifier == terms[1]);

                node1.Neighbours.Add(node2);
                node2.Neighbours.Add(node1);
            }

            var currentNode = nodes.Single(n => n.Identifier == "start");

            var routeChains = new List<List<Node>>();

            Visit(currentNode, routeChains, new List<Node>());
            
            routeChains.ForEach(rc => System.Console.WriteLine(string.Join(",", rc.Select(r => r.Identifier))));
            System.Console.WriteLine($"Found {routeChains.Count} routes");
        }

        private static void Visit(Node currentNode, List<List<Node>> routeChain, List<Node> visitedNodes)
        {
            visitedNodes.Add(currentNode);
            if(currentNode.IsEndNode)
            {
                routeChain.Add(visitedNodes);
                return;
            }

            int maxSmallVisits = 2;
            if(visitedNodes.Where(vn => !vn.IsBig).GroupBy(vn => vn.Identifier).Any(gr => gr.Count() > 1))
            {
                maxSmallVisits = 1;
            }
            foreach (var n in currentNode.Neighbours)
            {
                if((visitedNodes.Count(vn => vn == n) < maxSmallVisits || n.IsBig) && !n.IsStartNode)
                {
                    Visit(n, routeChain, visitedNodes.ToList());
                }
            }
        }
    }
}
