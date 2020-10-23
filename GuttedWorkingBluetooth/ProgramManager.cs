namespace Capstone
{
    public class ProgramManager
    {
        protected ContructedMap ContructedMap;
        public Robot Robot;
        protected MainWindow Page;
        public ProgramManager(MainWindow page)
        {
            this.Page = page;
            this.Robot = new EV3Robot();
            this.ContructedMap = new ContructedMap(Robot);
            Page.AddDisplayItem(Robot);
            Page.AddDisplayItem(ContructedMap);
        }
    }
}
