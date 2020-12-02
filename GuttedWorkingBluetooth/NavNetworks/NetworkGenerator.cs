using RoboticNavigation.Surface;
using RoboticNavigation.VectorMath;
using System.Collections.Generic;

namespace RoboticNavigation.NavNetworks
{
    public class NetworkGenerator
    {
        public ObstacleSurface ObstacleSurface;
        public double ClearRadius;
        public static double ObstacleCertintyThreshold { get { return ApplicationConfig.NetworkObstacleCertintyThreshold; } }
        public NetworkGenerator(ObstacleSurface surface, double requiredClearRadius)
        {
            this.ObstacleSurface = surface;
            this.ClearRadius = requiredClearRadius;
        }
        private int GenerateIncriment()
        {
            return (int)System.Math.Floor(ClearRadius / (ObstacleSurface.CMPerPixel));
        }
        public Network GeneratePathNetwork()
        {
            int incriment = GenerateIncriment();
            var network = new Network();
            for (int x = incriment; x < ObstacleSurface.Width; x += incriment)
            {
                for (int y = incriment; y < ObstacleSurface.Height; y += incriment)
                {
                    var coord = new SurfaceCoordinate(x, y);
                    var position = ObstacleSurface.CenterOfCell(coord);
                    if (PointIsClear(position))
                    {
                        network.Nodes.Add(position, new NetworkNode(position));
                    }
                }
            }
            foreach (var node in network.Nodes.Values)
            {
                ConnectNeighbors(node, network);
                //PopulateNodeConnections(node, network);
            }
            return network;
        }
        private bool PointIsClear(Vector2d<double> point)
        {
            var area = ObstacleSurface.CellsWithinRadiusOfPoint(point, ClearRadius, false);
            return CellsAreClear(area);
        }

        private bool CellsAreClear(List<SurfaceCoordinate> area)
        {
            var highestObstacleCertinty = -1d;
            for (int i = 0; i < area.Count && highestObstacleCertinty < ObstacleCertintyThreshold; i++)
            {
                var cellValue = ObstacleSurface.GetPixelValue(area[i]);
                highestObstacleCertinty = cellValue > highestObstacleCertinty ? cellValue : highestObstacleCertinty;
            }
            return highestObstacleCertinty < ObstacleCertintyThreshold;
        }
        private void ConnectNeighbors(NetworkNode node, Network network)
        {
            var ns = network.Neighbors(node, ObstacleSurface.CMPerPixel * GenerateIncriment());
            foreach (var n in ns)
            {
                node.Connections.Add(n);
            }
        }
    }
}
