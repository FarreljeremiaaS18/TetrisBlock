public class BlockT : BlockBase
{
    public override Color BlockColor { get { return Color.Purple; } }

    public override Point[][] AllShapes => new Point[][]
    {
        new Point[]
        {
            new Point(1,0),
            new Point(0,1),
            new Point(1,1),
            new Point(2,1)
        },

        new Point[]
        {
            new Point(0,0),
            new Point(0,1),
            new Point(1,1),
            new Point(0,2)
        },

        new Point[]
        {
            new Point(0,0),
            new Point(1,0),
            new Point(2,0),
            new Point(1,1)
        },

        new Point[]
        {
            new Point(1,0),
            new Point(0,1),
            new Point(1,1),
            new Point(1,2)
        }
    };

    public override void Place(GridManager grid)
    {
        foreach (var p in Shape)
        {
            int x = GridPosition.X + p.X;
            int y = GridPosition.Y + p.Y;
            grid.SetCell(x, y, true);
        }
    }

    public override bool CanPlace(GridManager grid)
    {
        foreach (var p in Shape)
        {
            int x = GridPosition.X + p.X;
            int y = GridPosition.Y + p.Y;

          
            if (x < 0 || x >= 9 || y < 0 || y >= 9)
                return false;

            
            if (!grid.IsCellEmpty(x, y))
                return false;
        }
        return true;
    }
}