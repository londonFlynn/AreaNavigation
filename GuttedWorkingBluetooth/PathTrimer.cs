namespace Capstone
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
            var generator = new NetworkGenerator(this.Surface, this.ClearRadius);
            var network = new Network();
            for (int i = 0; i < Path.Route.Count; i++)
            {
                var clearedNode = new NetworkNode(Path.Route[i].Position);
                network.Nodes.Add(clearedNode.Position, clearedNode);
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
            return PathIsClear(node1.Position, node2.Position);
        }
        private bool PathIsClear(Vector2d<double> point1, Vector2d<double> point2)
        {
            return Surface.CellsWithinDistanceOfLineSegmantAreClear(point1, point2, ClearRadius, NetworkGenerator.ObstacleCertintyThreshold);
        }

    }
}
