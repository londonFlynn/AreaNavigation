namespace Capstone
{
    public class SimulatedMap : Map
    {
        public SimulatedMap(int minWalls = 1, int maxWalls = 10)
        {
            var walls = new Wall[new System.Random().Next(minWalls, maxWalls + 1)];
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i] = new SimulatedWall();
            }
            this.Walls = new System.Collections.Generic.List<Wall>(walls);
        }
    }
}
