namespace Capstone
{
    public class SimulationManager : ProgramManager
    {
        private SimulatedMap SimulatedMap;
        public SimulationManager(MainWindow page) : base(page)
        {
            this.Robot = new SimulatedRobot();
            this.SimulatedMap = new SimulatedMap();
            //Page.AddDisplayItem(ContructedMap);
            Page.AddDisplayItem(SimulatedMap);
            Page.AddDisplayItem(Robot);
        }
    }
}
