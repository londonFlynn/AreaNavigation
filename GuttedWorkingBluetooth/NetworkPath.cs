using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;

namespace Capstone
{
    public class NetworkPath : IDisplayable, IComparable
    {
        public double Distance { get; private set; }
        public List<NetworkNode> Route = new List<NetworkNode>();
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
        public NetworkNode StartNode { get { return Route[0]; } }
        public NetworkNode EndNode { get { return Route[Route.Count - 1]; } }
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






        //Display Stuffs
        protected System.Windows.Controls.Canvas panel;
        protected double scale;
        protected double verticalOffset;
        protected double horizontalOffset;
        private System.Windows.Shapes.Line[] Lines;
        public virtual void StartDisplay()
        {
            Lines = new System.Windows.Shapes.Line[Route.Count - 1];
            for (int i = 1; i < Route.Count; i++)
            {
                StartDisplayingLine(Route[i - 1], Route[i], i - 1);
            }
        }
        private void StartDisplayingLine(NetworkNode start, NetworkNode end, int lineIndex)
        {
            var line = new System.Windows.Shapes.Line();
            Lines[lineIndex] = line;
            line.Stroke = new SolidColorBrush(Colors.White);
            UpdateDisplayingLine(start, end, lineIndex);
            panel.Children.Add(line);
        }
        public virtual void UpdateDisplay()
        {
            for (int i = 1; i < Route.Count; i++)
            {
                UpdateDisplayingLine(Route[i - 1], Route[i], i - 1);
            }
        }
        private void UpdateDisplayingLine(NetworkNode start, NetworkNode end, int lineIndex)
        {
            var x1 = (start.Position.x - horizontalOffset) * scale;
            var y1 = (start.Position.y - verticalOffset) * scale;
            var x2 = (end.Position.x - horizontalOffset) * scale;
            var y2 = (end.Position.y - verticalOffset) * scale;
            var line = Lines[lineIndex];
            line.X1 = 0;
            line.Y1 = 0;
            line.X2 = x2 - x1;
            //Y1 =?
            line.Y2 = y2 - y1;
            Canvas.SetLeft(line, x1);
            Canvas.SetTop(line, y1);
        }
        public virtual void SetPanel(System.Windows.Controls.Canvas panel)
        {
            this.panel = panel;
        }
        public virtual void SetScale(double scale, double horizontalOffset, double verticalOffset)
        {
            this.scale = scale;
            this.horizontalOffset = horizontalOffset;
            this.verticalOffset = verticalOffset;
        }
        public virtual double TopMostPosition()
        {
            double top = double.MaxValue;
            foreach (var node in Route)
            {
                if (node.Position.y < top)
                    top = node.Position.y;
            }
            return top;
        }
        public virtual double RightMostPosition()
        {
            double right = double.MinValue;
            foreach (var node in Route)
            {
                if (node.Position.x > right)
                    right = node.Position.x;
            }
            return right;
        }
        public virtual double LeftMostPosition()
        {
            double left = double.MaxValue;
            foreach (var node in Route)
            {
                if (node.Position.x < left)
                    left = node.Position.x;
            }
            return left;
        }
        public virtual double BottomMostPosition()
        {
            double bottom = double.MinValue;
            foreach (var node in Route)
            {
                if (node.Position.y > bottom)
                    bottom = node.Position.y;
            }
            return bottom;
        }
        public double MaxHeight()
        {
            return Math.Max(BottomMostPosition() - TopMostPosition(), 0);
        }
        public double MaxWidth()
        {
            return Math.Max(0, RightMostPosition() - LeftMostPosition());
        }
        public void NotifyDisplayChanged()
        {
            foreach (var listener in listeners)
            {
                listener.HearDisplayChanged();
            }
        }
        private List<ListenToDispalyChanged> listeners = new List<ListenToDispalyChanged>();
        public void SubsricbeDisplayChanged(ListenToDispalyChanged listener)
        {
            listeners.Add(listener);
        }
        public void UnsubsricbeDisplayChanged(ListenToDispalyChanged listener)
        {
            listeners.Remove(listener);
        }
        public virtual void StopDisplaying()
        {
            foreach (var line in Lines)
            {
                panel.Children.Remove(line);
            }
        }


    }
}
