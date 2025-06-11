
using System.Drawing;

public class BlockSquare : BlockBase
{
    public override Point[] Shape => new Point[]
    {
        new Point(0,0),
        new Point(1,0),
        new Point(0,1),
        new Point(1,1)
    };

    public override void Place(GridManager grid)
    {
        foreach (var p in Shape)
            grid.SetCell(GridPosition.X + p.X, GridPosition.Y + p.Y, true);
    }

    public override bool CanPlace(GridManager grid)
    {
        foreach (var p in Shape)
            if (!grid.IsCellEmpty(GridPosition.X + p.X, GridPosition.Y + p.Y))
                return false;
        return true;
    }
}
