using RoboticNavigation.Display;
using RoboticNavigation.VectorMath;
using System;
using System.Collections.Generic;

namespace RoboticNavigation.NavNetworks
{
    public class NetworkPath : IComparable, IDisplayablePositionedItem
    {
        public double Distance { get; private set; }


        public List<NetworkNode> Route = new List<NetworkNode>();
        public NetworkNode StartNode { get { return Route[0]; } }
        public NetworkNode EndNode { get { return Route[Route.Count - 1]; } }
        private NetworkPathDisplayer Displayer;

        public NetworkNode NodeInRouteAtPosition(Vector2d<double> position)
        {
            NetworkNode result = null;
            foreach (var node in Route)
            {
                if (node.Position.Equals(position))
                {
                    result = node;
                    break;
                }
            }
            return result;
        }
        public NetworkPath(NetworkNode startNode, NetworkNode endNode)
        {
            this.Route.Add(startNode);
            if (!startNode.Equals(endNode))
            {
                this.Route.Add(EndNode);
                if (this.StartNode.Connections.Contains(EndNode))
                {
                    this.Distance = StartNode.DistanceToNode(EndNode);
                }
                else
                {
                    this.Distance = double.PositiveInfinity;
                }
            }
            else
            {
                this.Distance = 0;
            }
        }
        public NetworkPath(NetworkPath that, NetworkNode expansion)
        {
            foreach (var node in that.Route)
            {
                this.Route.Add(node);
            }
            this.Distance = that.Distance + this.EndNode.DistanceToNode(expansion);
            this.Route.Add(expansion);
        }
        public void RemoveNodeConnectNeighbors(NetworkNode node)
        {
            if (Route.Count > 2)
            {
                if (node.Equals(StartNode))
                {
                    Route.RemoveAt(0);
                    StartNode.Connections.Add(Route[1]);
                    Route[1].Connections.Add(StartNode);
                }
                else if (node.Equals(EndNode))
                {
                    Route.RemoveAt(Route.Count - 1);
                    EndNode.Connections.Add(Route[Route.Count - 2]);
                    Route[Route.Count - 2].Connections.Add(EndNode);
                }
                else
                {
                    for (int i = 1; i < Route.Count - 1; i++)
                    {
                        if (Route[i].Equals(node))
                        {
                            Route.RemoveAt(i);
                            Route[i - 1].Connections.Add(Route[i]);
                            Route[i].Connections.Add(Route[i - 1]);
                            break;
                        }
                    }
                }
            }
        }
        public int CompareTo(object obj)
        {
            if (obj is NetworkPath)
            {
                var that = obj as NetworkPath;
                return this.Distance.CompareTo(that);
            }
            throw new NotImplementedException();
        }
        public PositionedItemDisplayer GetDisplayer()
        {
            if (Displayer is null)
            {
                Displayer = new NetworkPathDisplayer(this);
            }
            return Displayer;
        }

        public void DisplayableValueChanged()
        {
            Displayer.PosistionedItemValueChanged();
        }
        public double LowestX()
        {
            var value = double.MaxValue;
            foreach (var node in this.Route)
            {
                if (node.Position.x < value)
                {
                    value = node.Position.x;
                }
            }
            return value;
        }

        public double HighestX()
        {
            var value = double.MinValue;
            foreach (var node in this.Route)
            {
                if (node.Position.x > value)
                {
                    value = node.Position.x;
                }
            }
            return value;
        }

        public double LowestY()
        {
            var value = double.MaxValue;
            foreach (var node in this.Route)
            {
                if (node.Position.y < value)
                {
                    value = node.Position.y;
                }
            }
            return value;
        }

        public double HighestY()
        {
            var value = double.MinValue;
            foreach (var node in this.Route)
            {
                if (node.Position.y > value)
                {
                    value = node.Position.y;
                }
            }
            return value;
        }
        public Vector2d<double>[] ToArray()
        {
            var array = new Vector2d<double>[this.Route.Count];
            for (int i = 0; i < Route.Count; i++)
            {
                array[i] = Route[i].Position;
            }
            return array;
        }
    }
}
