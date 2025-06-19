
using System.Drawing;
using System.Windows.Forms;

public abstract class BlockBase : Panel, IDraggable
{
    public abstract Point[] Shape { get; }

    protected int cellSize = 30;
    public Point GridPosition;

    private bool isDragging = false;
    private Point dragOffset;

    public BlockBase()
    {
        this.Width = 3 * cellSize;
        this.Height = 3 * cellSize;
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
        OnDragEnd();
        OnBlockPlaced?.Invoke();
    }

    public virtual void OnDragStart() => this.BringToFront();
    public virtual void OnDragging(int x, int y) => GridPosition = new Point(x / cellSize, y / cellSize);
    public virtual void OnDragEnd() { }
    public Action OnBlockPlaced;

    public abstract void Place(GridManager grid);
    public abstract bool CanPlace(GridManager grid);

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;
        foreach (var p in Shape)
        {
            g.FillRectangle(Brushes.SteelBlue, p.X * cellSize, p.Y * cellSize, cellSize - 2, cellSize - 2);
        }
    }
}
