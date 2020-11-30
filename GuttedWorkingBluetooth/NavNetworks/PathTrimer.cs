using RoboticNavigation.Surface;
using RoboticNavigation.VectorMath;

namespace RoboticNavigation.NavNetworks
{
    public class PathTrimer
    {
        private NetworkPath Path;
        private ObstacleSurface Surface;
        private double ClearRadius;
        public PathTrimer(NetworkPath path, ObstacleSurface surface, double clearRadius)
        {
            this.Path = path;
            this.Surface = surface;
            this.ClearRadius = clearRadius / 2;
        }
        public NetworkPath Trim()
        {
            if (Path is null)
                return null;
            var generator = new NetworkGenerator(this.Surface, this.ClearRadius);
            var network = new Network();
            for (int i = 0; i < Path.Route.Count; i++)
            {
                var clearedNode = new NetworkNode(Path.Route[i].Position);
                network.Nodes.Add(clearedNode.Position, clearedNode);
                //foreach (var node in Path.Route[i].Connections)
                //{
                //    if (network.Nodes.ContainsKey(node.Position) && !network.Nodes[node.Position].Connections.Contains(network.Nodes[clearedNode.Position]))
                //    {
                //        network.Nodes[node.Position].Connections.Add(network.Nodes[clearedNode.Position]);
                //        network.Nodes[clearedNode.Position].Connections.Add(network.Nodes[node.Position]);
                //    }
                //}
            }
            foreach (var node in network.Nodes.Values)
            {
                ConnectAllViableConnections(node, network);
            }
            return network.GeneratePath(this.Path.StartNode.Position, this.Path.EndNode.Position);
        }
        private void ConnectAllViableConnections(NetworkNode node, Network network)
        {
            foreach (var otherNode in network.Nodes.Values)
            {
                if (!node.Equals(otherNode) && !node.Connections.Contains(otherNode) && ViableConnection(node, otherNode))
                {
                    node.Connections.Add(otherNode);
                    otherNode.Connections.Add(node);
                }
            }
        }
        private bool ViableConnection(NetworkNode node1, NetworkNode node2)
        {
            return PathIsClear(node1.Position, node2.Position) || Path.NodeInRouteAtPosition(node1.Position).Connections.Contains(node2);
        }
        private bool PathIsClear(Vector2d<double> point1, Vector2d<double> point2)
        {
            return Surface.CellsWithinDistanceOfLineSegmantAreClear(point1, point2, ClearRadius, NetworkGenerator.ObstacleCertintyThreshold);
        }

    }
}
