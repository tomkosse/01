using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _15
{

    public class Node
    {
        public int RiskLevel { get; }
        public int DistanceToEnd
        {
            get;
            set;
        }
        public int X { get; }
        public int Y { get; }

        public int LowestFoundRisk = int.MaxValue;

        public IList<Node> Neighbours { get; set; }
        public bool IsEndNode { get; set; }
        public bool IsStartNode { get; set; }

        public Node(int x, int y, int riskLevel)
        {
            this.RiskLevel = riskLevel;
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return $"Node {X}:{Y} - {RiskLevel}";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);
            var sw = Stopwatch.StartNew();

            var numbers = lines
                .Select(
                        line => line
                            .ToCharArray()
                            .Select(c => int.Parse(c.ToString()))
                            .ToArray()
                        ).ToArray();

            var nodes = CreateNodeMatrix(numbers, 5, 5);
            //var nodes = CreateNodeMatrix(numbers, 1, 1);

            var startNode = nodes[0][0];
            int outcome = Visit(startNode);

            System.Console.WriteLine($"Answer: {outcome - startNode.RiskLevel} in {sw.ElapsedMilliseconds}ms");
        }

        private static Node[][] CreateNodeMatrix(int[][] numberMatrix, int xMultiplier, int yMultiplier)
        {
            Node[][] nodes = new Node[numberMatrix.Length * yMultiplier][];

            for (int y = 0; y < numberMatrix.Length; y++)
            {
                for (int yMulti = 0; yMulti < yMultiplier; yMulti++)
                {
                    int newY = y + (yMulti * numberMatrix.Length);
                    
                    var row = numberMatrix[y];
                    nodes[newY] = new Node[row.Length * xMultiplier];
                    for (int x = 0; x < row.Length; x++)
                    {
                        for (int xMulti = 0; xMulti < xMultiplier; xMulti++)
                        {
                            int newX = x + (xMulti * row.Length);
                            var riskLevel = numberMatrix[y][x] + xMulti + yMulti;
                            riskLevel = riskLevel > 9 ? riskLevel - 9 : riskLevel;
                            nodes[newY][newX] = new Node(newX, newY, riskLevel);
                        }
                    }
                }
            }

            nodes.First().First().IsStartNode = true;
            var endNode = nodes.Last().Last();
            endNode.IsEndNode = true;
            foreach (var node in nodes.SelectMany(o => o))
            {
                node.DistanceToEnd = endNode.X - node.X + endNode.Y - node.Y;
                node.Neighbours = GetAdjecentNodes(nodes, node).OrderBy(n => n.DistanceToEnd + n.RiskLevel).ToArray();
            }

            return nodes;
        }

        private static int Visit(Node currentNode, int currentSum = 0, int lowestRiskLevelFound = int.MaxValue)
        {
            int currentTotalRisk = currentSum + currentNode.RiskLevel;

            if (currentNode.IsEndNode)
            {
                return currentTotalRisk;
            }
            else
            {
                int localLowestRiskLevelFound = lowestRiskLevelFound;
                foreach (var n in currentNode.Neighbours)
                {
                    int minimumTotalRisk = currentTotalRisk + n.RiskLevel + n.DistanceToEnd;
                    if (minimumTotalRisk < n.LowestFoundRisk)
                    {
                        n.LowestFoundRisk = minimumTotalRisk;
                        if (minimumTotalRisk < lowestRiskLevelFound)
                        {
                            localLowestRiskLevelFound = Visit(n, currentTotalRisk, localLowestRiskLevelFound);
                        }
                   }
                }
                return localLowestRiskLevelFound;
            }
        }

        private static Node[] GetAdjecentNodes(Node[][] nodeMatrix, Node node)
        {
            List<Node> nodes = new List<Node>();
            var row = nodeMatrix[node.Y];
            if (node.Y > 0)
            {
                nodes.Add(nodeMatrix[node.Y - 1][node.X]);
            }
            if (node.Y < nodeMatrix.Length - 1)
            {
                nodes.Add(nodeMatrix[node.Y + 1][node.X]);
            }
            if (node.X > 0)
            {
                nodes.Add(nodeMatrix[node.Y][node.X - 1]);
            }
            if (node.X < row.Length - 1)
            {
                nodes.Add(nodeMatrix[node.Y][node.X + 1]);
            }
            return nodes.ToArray();
        }
    }
}
