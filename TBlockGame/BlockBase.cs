using System;
using System.Drawing;
using System.Windows.Forms;

public abstract class BlockBase : Panel, IDraggable
{
    public abstract Point[] Shape { get; }

    protected int cellSize = 30;
    public Point GridPosition;

    private bool isDragging = false;
    private Point dragOffset;

   
    public static Point GridOffset = new Point(200, 100);

    public BlockBase()
    {
  
        var bounds = GetShapeBounds();
        this.Width = bounds.Width * cellSize;
        this.Height = bounds.Height * cellSize;
        this.BackColor = Color.Transparent;

        this.MouseDown += Block_MouseDown;
        this.MouseMove += Block_MouseMove;
        this.MouseUp += Block_MouseUp;

        this.DoubleBuffered = true;
    }

    private void Block_MouseDown(object sender, MouseEventArgs e)
    {
        isDragging = true;
        dragOffset = e.Location;
        OnDragStart();
    }

    private void Block_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            int newX = this.Left + (e.X - dragOffset.X);
            int newY = this.Top + (e.Y - dragOffset.Y);
            this.Location = new Point(newX, newY);
            OnDragging(newX, newY);
        }
    }

    private void Block_MouseUp(object sender, MouseEventArgs e)
    {
        isDragging = false;

        SnapToGrid();

        OnDragEnd();
        OnBlockPlaced?.Invoke();
    }

    private void SnapToGrid()
    {
        var bounds = GetShapeBounds();

        
        int gridX = (this.Left - GridOffset.X) / cellSize + bounds.X;
        int gridY = (this.Top - GridOffset.Y) / cellSize + bounds.Y;

       
        gridX = Math.Max(bounds.X, Math.Min(gridX, 9 - (bounds.Width - bounds.X)));
        gridY = Math.Max(bounds.Y, Math.Min(gridY, 9 - (bounds.Height - bounds.Y)));

        GridPosition = new Point(gridX - bounds.X, gridY - bounds.Y);


        this.Location = new Point(
            GridOffset.X + (gridX - bounds.X) * cellSize,
            GridOffset.Y + (gridY - bounds.Y) * cellSize
        );
    }

    private Rectangle GetShapeBounds()
    {
        if (Shape.Length == 0) return new Rectangle(0, 0, 3, 3);

        int minX = Shape[0].X, maxX = Shape[0].X;
        int minY = Shape[0].Y, maxY = Shape[0].Y;

        foreach (var point in Shape)
        {
            if (point.X < minX) minX = point.X;
            if (point.X > maxX) maxX = point.X;
            if (point.Y < minY) minY = point.Y;
            if (point.Y > maxY) maxY = point.Y;
        }

        return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
    }

    private int GetMaxShapeWidth()
    {
        int max = 0;
        foreach (var p in Shape)
            if (p.X > max) max = p.X;
        return max + 1;
    }

    private int GetMaxShapeHeight()
    {
        int max = 0;
        foreach (var p in Shape)
            if (p.Y > max) max = p.Y;
        return max + 1;
    }

    public virtual void OnDragStart() => this.BringToFront();

    public virtual void OnDragging(int x, int y)
    {
        var bounds = GetShapeBounds();
     
        int gridX = (x - GridOffset.X) / cellSize + bounds.X;
        int gridY = (y - GridOffset.Y) / cellSize + bounds.Y;
        GridPosition = new Point(gridX - bounds.X, gridY - bounds.Y);
    }

    public virtual void OnDragEnd() { }
    public System.Action OnBlockPlaced;

    public abstract void Place(GridManager grid);
    public abstract bool CanPlace(GridManager grid);

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

    
        var bounds = GetShapeBounds();

        foreach (var p in Shape)
        {
   
            int drawX = (p.X - bounds.X) * cellSize;
            int drawY = (p.Y - bounds.Y) * cellSize;

            Rectangle rect = new Rectangle(
                drawX,
                drawY,
                cellSize - 2,
                cellSize - 2
            );


            Brush brush = Brushes.SteelBlue;
            if (isDragging && Parent != null)
            {
               
                var gameController = FindGameController();
                if (gameController != null && !CanPlace(gameController.GetGrid()))
                {
                    brush = Brushes.Red; 
                }
                else
                {
                    brush = Brushes.LightGreen; 
                }
            }

            g.FillRectangle(brush, rect);
            g.DrawRectangle(Pens.Black, rect);
        }
    }

    private GameController FindGameController()
    {
       
        if (Parent is FormMain form)
        {
            var field = typeof(FormMain).GetField("game",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            return field?.GetValue(form) as GameController;
        }
        return null;
    }
}