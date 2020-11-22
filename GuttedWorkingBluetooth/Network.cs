using System.Collections.Generic;

namespace Capstone
{
    public class Network
    {
        public Dictionary<Vector2d<double>, NetworkNode> Nodes = new Dictionary<Vector2d<double>, NetworkNode>();
        public NetworkPath GeneratePath(Vector2d<double> startPoint, Vector2d<double> endPoint)
        {
            NetworkNode startNode = AddNodeAtPointIfNotExist(startPoint);
            NetworkNode endNode = AddNodeAtPointIfNotExist(endPoint);
            return GeneratePath(startNode, endNode);
        }
        private NetworkNode AddNodeAtPointIfNotExist(Vector2d<double> point)
        {
            NetworkNode node;
            if (Nodes.ContainsKey(point))
            {
                node = Nodes[point];
            }
            else
            {
                node = new NetworkNode(point);
                var closest = ClosestNodeToPoint(point);
                node.Connections.Add(closest);
                closest.Connections.Add(node);
                Nodes.Add(point, node);
            }
            return node;
        }
        public NetworkNode ClosestNodeToPoint(Vector2d<double> point)
        {
            var shortestDistance = double.MaxValue;
            NetworkNode closestNode = null;
            foreach (var node in Nodes.Values)
            {
                var distance = (point - node.Position).Magnitude();
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestNode = node;
                }
            }
            return closestNode;
        }
        private NetworkPath GeneratePath(NetworkNode startNode, NetworkNode endNode)
        {
            var unvisited = new List<NetworkNode>(Nodes.Values);
            var nodePaths = InitialNodeDistances(startNode, unvisited);
            //unvisited.Remove(startNode);
            GeneratePathRecursion(NextNodeToCheck(unvisited, nodePaths, endNode), unvisited, nodePaths, endNode);
            if (double.IsPositiveInfinity(nodePaths[endNode].Distance))
            {
                NetworkPath closestPath = null;
                foreach (var path in nodePaths.Values)
                {
                    if (!double.IsPositiveInfinity(path.Distance) && (closestPath is null || path.EndNode.DistanceToNode(endNode) < closestPath.EndNode.DistanceToNode(endNode)))
                    {
                        closestPath = path;
                    }
                }
                System.Diagnostics.Debug.WriteLine("Could not find a path to the specified point.");
                return null;
            }
            else
            {
                return nodePaths[endNode];
            }
        }
        private void GeneratePathRecursion(NetworkNode currentNode, List<NetworkNode> unvisited, Dictionary<NetworkNode, NetworkPath> nodePaths, NetworkNode endNode)
        {
            if (!(currentNode is null) && !currentNode.Equals(endNode) && !double.IsInfinity(nodePaths[currentNode].Distance))
            {
                foreach (var node in currentNode.Connections)
                {
                    var distanceToNode = nodePaths[currentNode].Distance + currentNode.DistanceToNode(node);
                    if (distanceToNode < nodePaths[node].Distance)
                    {
                        nodePaths[node] = new NetworkPath(nodePaths[currentNode], node);
                        if (endNode.Equals(node))
                        {
                            System.Diagnostics.Debug.WriteLine($"Found a path to the end node");
                        }
                    }
                }
                unvisited.Remove(currentNode);
                GeneratePathRecursion(NextNodeToCheck(unvisited, nodePaths, endNode), unvisited, nodePaths, endNode);
            }
            else
            {
                if (currentNode is null)
                {
                    System.Diagnostics.Debug.WriteLine($"breaking becuase the current node is null");

                }
                else if (currentNode.Equals(endNode))
                {
                    System.Diagnostics.Debug.WriteLine($"breaking becuase the shortest path to the end node is found");
                }
                else if (double.IsInfinity(nodePaths[currentNode].Distance))
                {
                    System.Diagnostics.Debug.WriteLine($"breaking becuase their is no path to the current node");
                }
            }
        }
        private NetworkNode NextNodeToCheck(List<NetworkNode> unvisited, Dictionary<NetworkNode, NetworkPath> nodePaths, NetworkNode end)
        {
            if (unvisited.Count < 1)
            {
                return null;
            }
            NetworkNode closestNode = null;
            var closestDistance = double.MaxValue;
            //var list = new List<NetworkNode>(nodePaths.Values);
            foreach (var node in unvisited)
            {
                if (!double.IsInfinity(nodePaths[node].Distance) && (nodePaths[node].Distance < closestDistance || (nodePaths[node].Distance == closestDistance && node.DistanceToNode(end) < closestNode.DistanceToNode(end))))
                {
                    closestNode = node;
                    closestDistance = nodePaths[node].Distance;
                    //System.Diagnostics.Debug.WriteLine($"Found new closest at {closestDistance}");
                }
            }
            return closestNode;
        }
        private static Dictionary<NetworkNode, NetworkPath> InitialNodeDistances(NetworkNode startNode, List<NetworkNode> nodes)
        {
            var nodeDistances = new Dictionary<NetworkNode, NetworkPath>();
            foreach (var node in nodes)
            {
                nodeDistances.Add(node, new NetworkPath(startNode, node));
            }
            return nodeDistances;
        }
        public List<NetworkNode> Neighbors(NetworkNode node, double gap)
        {
            var neighbors = new List<NetworkNode>();
            for (double x = node.Position.x - gap; x <= node.Position.x + gap; x += gap)
            {
                for (double y = node.Position.y - gap; y <= node.Position.y + gap; y += gap)
                {
                    var point = new Vector2d<double>(new double[] { x, y });
                    if (!(x == node.Position.x && y == node.Position.y) && this.Nodes.ContainsKey(point))
                    {
                        neighbors.Add(this.Nodes[point]);
                    }
                }
            }
            gap = System.Math.Sqrt((gap * gap) + (gap * gap));
            for (double x = node.Position.x - gap; x <= node.Position.x + gap; x += gap)
            {
                for (double y = node.Position.y - gap; y <= node.Position.y + gap; y += gap)
                {
                    var point = new Vector2d<double>(new double[] { x, y });
                    if (!(x == node.Position.x && y == node.Position.y) && this.Nodes.ContainsKey(point))
                    {
                        neighbors.Add(this.Nodes[point]);
                    }
                }
            }
            //System.Diagnostics.Debug.WriteLine($"Found {neighbors.Count} neighbors");
            return neighbors;
        }
    }
}
