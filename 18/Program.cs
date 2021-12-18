using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace _18
{
    public class Pair
    {
        public Pair Parent { get; private set; }
        public Pair Left { get; private set; }
        public Pair Right { get; private set; }
        public int? LeftValue { get; private set; }
        public int? RightValue { get; private set; }

        public int Magnitude
        {
            get
            {
                return (Left != null ? Left.Magnitude : LeftValue.Value) * 3 + (Right != null ? Right.Magnitude : RightValue.Value) * 2;
            }
        }

        public IEnumerable<Pair> AllChildren
        {
            get
            {
                List<Pair> children = new List<Pair>();
                if (Left != null)
                {
                    children.Add(Left);
                    children.AddRange(Left.AllChildren);
                }
                if (Right != null)
                {
                    children.Add(Right);
                    children.AddRange(Right.AllChildren);
                }
                return children;
            }
        }

        public bool ShouldExplode
        {
            get
            {
                int nodeDepth = 0;
                var curPar = Parent;
                while (curPar != null)
                {
                    nodeDepth++;
                    curPar = curPar.Parent;
                }
                return nodeDepth >= 4;
            }
        }

        public bool ShouldSplit
        {
            get
            {
                return LeftValue.GetValueOrDefault() >= 10 || (RightValue.GetValueOrDefault() >= 10 && (Left == null || !Left.ShouldSplit));
            }
        }

        public Pair(Pair parent = null) 
        {
            this.Parent = parent;
        }

        public Pair(Pair parent, Pair leftChild, Pair rightChild) : this(parent)
        {
            this.Left = leftChild;
            this.Right = rightChild;
        }

        public Pair(Pair parent, int leftValue, int rightValue) : this(parent)
        {
            this.LeftValue = leftValue;
            this.RightValue = rightValue;
        }

        public void Assign(int value)
        {
            if (!LeftValue.HasValue && Left == null)
            {
                LeftValue = value;
            }
            else if (!RightValue.HasValue && Right == null)
            {
                RightValue = value;
            }
        }
        public void Assign(Pair p)
        {
            if (!LeftValue.HasValue && Left == null)
            {
                Left = p;
            }
            else if (!RightValue.HasValue && Right == null)
            {
                Right = p;
            }
        }

        public Pair Add(Pair pair)
        {
            var newRoot = new Pair(null, this, pair);
            pair.Parent = newRoot;
            this.Parent = newRoot;

            while (newRoot.AllChildren.Any(c => c.ShouldExplode) || newRoot.AllChildren.Any(c => c.ShouldSplit))
            {
                if (newRoot.AllChildren.Any(c => c.ShouldExplode))
                {
                    var toExplode = newRoot.AllChildren.First(c => c.ShouldExplode);
                    toExplode.Parent.Explode(toExplode);
                }
                else if (newRoot.AllChildren.Any(c => c.ShouldSplit))
                {
                    var toSplit = newRoot.AllChildren.First(c => c.ShouldSplit);
                    toSplit.Split();
                }
            }
            return newRoot;
        }

        public void Split()
        {
            if (LeftValue.GetValueOrDefault() >= 10)
            {
                var firstHalf = LeftValue.Value / 2;
                Left = new Pair(this, firstHalf, LeftValue.Value - firstHalf);
                LeftValue = null;
            }
            else if (RightValue.GetValueOrDefault() >= 10)
            {
                var firstHalf = RightValue.Value / 2;
                Right = new Pair(this, firstHalf, RightValue.Value - firstHalf);
                RightValue = null;
            }
        }

        public void Explode(Pair pair)
        {
            bool searchRight = true;
            bool searchLeft = true;
            if (Left == pair)
            {
                Left = null;
                LeftValue = 0;

                if (RightValue.HasValue)
                {
                    RightValue += pair.RightValue;
                    searchRight = false;
                }
            }
            else if (Right == pair)
            {
                Right = null;
                RightValue = 0;

                if (LeftValue.HasValue)
                {
                    LeftValue += pair.LeftValue;
                    searchLeft = false;
                }
            }

            if (searchRight)
            {
                SearchRight(pair.RightValue.Value, this, pair);
            }
            if (searchLeft)
            {
                SearchLeft(pair.LeftValue.Value, this, pair);
            }
        }

        private void SearchRight(int value, Pair currentNode, Pair previousNode)
        {
            if (currentNode.RightValue.HasValue && currentNode != this)
            {
                currentNode.RightValue += value;
            }
            else if (currentNode.Right != null && currentNode.Right != previousNode)
            {
                var cur = currentNode.Right;
                while (!cur.LeftValue.HasValue)
                {
                    cur = cur.Left;
                }
                cur.LeftValue += value;
            }
            else if (currentNode.Parent != null)
            {
                SearchRight(value, currentNode.Parent, currentNode);
            }
        }

        private void SearchLeft(int value, Pair currentNode, Pair previousNode)
        {
            if (currentNode.LeftValue.HasValue && currentNode != this)
            {
                currentNode.LeftValue += value;
            }
            else if (currentNode.Left != null && currentNode.Left != previousNode)
            {
                var cur = currentNode.Left;
                while (!cur.RightValue.HasValue)
                {
                    cur = cur.Right;
                }
                cur.RightValue += value;
            }
            else if (currentNode.Parent != null)
            {
                SearchLeft(value, currentNode.Parent, currentNode);
            }
        }
    }


    public class Program
    {
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(args[0]);
            var sw = Stopwatch.StartNew();
            var stringLines = lines.Select(s => s.Replace(",", "")).ToArray();
            var sumQueue = new Queue<Pair>(stringLines.Select(l => ParseLine(l)));
            Pair runningTotal = sumQueue.Dequeue();
            
            while (sumQueue.Any())
            {
                runningTotal = runningTotal.Add(sumQueue.Dequeue());
            }

            int maxMagnitude = int.MinValue;
            for (int i = 0; i < stringLines.Count(); i++)
            {
                for (int j = 0; j < stringLines.Count(); j++)
                {
                    if (i != j)
                    {
                        var magnitude = ParseLine(stringLines[i]).Add(ParseLine(stringLines[j])).Magnitude;
                        if (magnitude > maxMagnitude)
                        {
                            maxMagnitude = magnitude;
                        }
                    }
                }
            }
            sw.Stop();
            System.Console.WriteLine($"Part 1: {runningTotal.Magnitude}");
            System.Console.WriteLine($"Part 2: {maxMagnitude}");
            System.Console.WriteLine($"Done in {sw.ElapsedMilliseconds}ms");
        }

        private static Pair ParseLine(string line)
        {
            Queue<char> charQueue = new Queue<char>(line.ToCharArray());
            var firstChar = charQueue.Dequeue(); // This is going to be a '[' so lets just make a rootnode
            var rootPair = new Pair();
            Stack<Pair> pairs = new Stack<Pair>();
            pairs.Push(rootPair);
            while(charQueue.Any())
            {
                var c = charQueue.Dequeue();
                if (c == '[')
                {
                    var parent = pairs.Peek();
                    var newPair = new Pair(parent, null, null);
                    parent.Assign(newPair);
                    pairs.Push(newPair);
                }
                else if (char.IsNumber(c))
                {
                    var currentPair = pairs.Peek();
                    currentPair.Assign((int)char.GetNumericValue(c));
                }
                else if (c == ']')
                {
                    pairs.Pop();
                }
            }
            return rootPair;
        }
    }
}