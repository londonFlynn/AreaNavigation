namespace Capstone
{
    public class ProgramManager
    {
        protected ContructedMap ContructedMap;
        protected Robot Robot;
        protected MainPage Page;
        public ProgramManager(MainPage page)
        {
            this.Page = page;
            this.ContructedMap = new ContructedMap();
            this.Robot = new EV3Robot();
            //Page.AddDisplayItem(ContructedMap);
        }
    }
}
