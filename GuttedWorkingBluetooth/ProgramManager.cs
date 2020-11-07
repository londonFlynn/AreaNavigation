using System.Threading.Tasks;

namespace Capstone
{
    public class ProgramManager
    {
        public ContructedMap ContructedMap;
        public Robot Robot;
        protected MainWindow Page;
        public ProgramManager(MainWindow page)
        {
            this.Page = page;
            this.Robot = new EV3Robot();
            this.ContructedMap = new ContructedMap(Robot);
            Page.AddDisplayItem(Robot);
            Page.AddDisplayItem(ContructedMap);
            //var arc = new ArcSegment(System.Math.PI / 2, new Vector2d<double>(new double[] { 0, 0 }), new Vector2d<double>(new double[] { 30, 0 }));
            //page.AddDisplayItem(arc);
        }
        public async Task<NetworkPath> PathFromRobotToPoint(Vector2d<double> point)
        {
            var generator = new NetworkGenerator(this.ContructedMap.ObstacleSurface, this.Robot.MaxiumDistanceFromCenter);
            var network = await Task.Factory.StartNew(() => generator.GeneratePathNetwork());
            var path = network.GeneratePath(this.Robot.Position, point);
            var trimer = new PathTrimer(path, this.ContructedMap.ObstacleSurface, this.Robot.MaxiumDistanceFromCenter);
            return trimer.Trim();
        }
    }
}
