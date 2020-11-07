using System.Collections.Generic;

namespace Capstone
{
    public class NetworkNode
    {
        public Vector2d<double> Position;
        public List<NetworkNode> Connections = new List<NetworkNode>();
        public NetworkNode(Vector2d<double> position)
        {
            this.Position = position;
        }
        public double DistanceToNode(NetworkNode that)
        {
            return DistanceBetweenNodes(this, that);
        }
        public static double DistanceBetweenNodes(NetworkNode node1, NetworkNode node2)
        {
            return (node1.Position - node2.Position).Magnitude();
        }
        public override bool Equals(object obj)
        {
            if (obj is NetworkNode)
            {
                var that = obj as NetworkNode;
                return this.Position.Equals(that.Position);
            }
            return false;
        }
    }
}
