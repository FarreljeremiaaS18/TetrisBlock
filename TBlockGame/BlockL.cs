using System.Drawing;

public class BlockL : BlockBase
{
    public override Color BlockColor { get { return Color.Orange; } }

    public override Point[][] AllShapes => new Point[][]
    {
        new Point[] //R1
        {
            new Point(0,0),
            new Point(0,1),
            new Point(0,2),
            new Point(1,2)
        },

        new Point[]//R2
        {
            new Point(0,0),
            new Point(1,0),
            new Point(2,0),
            new Point(0,1)
        },

        new Point[]//R3
        {
            new Point(0,0),
            new Point(1,0),
            new Point(1,1),
            new Point(1,2)
        },

        new Point[]//R4
        {
            new Point(2,0),
            new Point(0,1),
            new Point(1,1),
            new Point(2,1)
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

           //Cek apakah keluar batas grid
            if (x < 0 || x >= 9 || y < 0 || y >= 9)
                return false;

            //Cek sel sudah terisi atau belum
            if (!grid.IsCellEmpty(x, y))
                return false;
        }
        //jika valid, return true
        return true;
    }
}