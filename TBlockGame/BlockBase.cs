using System;
using System.Drawing;
using System.Windows.Forms;

public abstract class BlockBase : Panel, IDraggable
{
    public abstract Point[][] AllShapes { get; }
    public virtual Color BlockColor { get { return Color.SteelBlue; } }

    protected int currentRotation = 0;
    public int CurrentRotation
    {
        get => currentRotation;
        set
        {
            currentRotation = value;
            var bounds = GetShapeBounds();
            this.Width = bounds.Width * cellSize;
            this.Height = bounds.Height * cellSize;
            this.Invalidate();
        }
    }
    public Point[] Shape => AllShapes[currentRotation];

    protected int cellSize = 30;
    public Point GridPosition;

    private bool isDragging = false;
    private Point dragOffset;
    private Point initialLocation; //Menyimpan posisi awal before dragging

    public static Point GridOffset = new Point(200, 100);

    public BlockBase()
    {
        currentRotation = 0;

        var bounds = GetShapeBounds();
        this.Width = bounds.Width * cellSize;
        this.Height = bounds.Height * cellSize;
        this.BackColor = Color.Transparent;

        this.MouseDown += Block_MouseDown;
        this.MouseMove += Block_MouseMove;
        this.MouseUp += Block_MouseUp;

        this.DoubleBuffered = true;
    }

    public void SetRotation(int rotation)
    {
        if (rotation >= 0 && rotation < AllShapes.Length)
        {
            CurrentRotation = rotation;
        }
    }

    private void Block_MouseDown(object sender, MouseEventArgs e)
    {
        //rotasi block jika klik kanan
        if (e.Button == MouseButtons.Right)
        {
            CurrentRotation = (currentRotation + 1) % AllShapes.Length;
            return;
        }
        //mulai drag jika klik kiri
        if (e.Button == MouseButtons.Left)
        {
            isDragging = true;
            dragOffset = e.Location;
            initialLocation = this.Location; //Simpan posisi awal panel
            OnDragStart();
        }
    }

    private void Block_MouseMove(object sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            //Hitung posisi baru berdasarkan offset drag
            int newX = this.Left + (e.X - dragOffset.X);
            int newY = this.Top + (e.Y - dragOffset.Y);
            this.Location = new Point(newX, newY);
            OnDragging(newX, newY);
        }
    }

    private void Block_MouseUp(object sender, MouseEventArgs e)
    {
        if (isDragging)
        {
            isDragging = false;

            //Cek apakah block di dalam area grid
            if (IsInGridArea())
            {
                SnapToGrid();

                
                if (IsValidGridPosition() && CanPlace(FindGameController()?.GetGrid()))
                {
                    OnBlockPlaced?.Invoke();
                }
                else
                {
                
                    this.Location = initialLocation;
                    GridPosition = new Point(-10, -10);
                }
            }
            else
            {
               //Jika dilepaskan diluar grid, kembalikan ke posisi awal
                this.Location = initialLocation;
                GridPosition = new Point(-10, -10);
            }

            OnDragEnd();
        }
    }

    private bool IsInGridArea()
    {
       
        var gridBounds = new Rectangle(
            GridOffset.X,
            GridOffset.Y,
            9 * cellSize, 
            9 * cellSize 
        );

        var blockCenter = new Point(
            this.Location.X + this.Width / 2,
            this.Location.Y + this.Height / 2
        );

        return gridBounds.Contains(blockCenter);
    }

    private void SnapToGrid()
    {
        var bounds = GetShapeBounds();


        int blockLeftInGrid = this.Left - GridOffset.X;
        int blockTopInGrid = this.Top - GridOffset.Y;

        
        int gridX = (int)Math.Round((double)blockLeftInGrid / cellSize);
        int gridY = (int)Math.Round((double)blockTopInGrid / cellSize);

 
        int offsetX = bounds.X;
        int offsetY = bounds.Y;

       
        GridPosition = new Point(gridX - offsetX, gridY - offsetY);

        
        GridPosition = new Point(
            Math.Max(0, Math.Min(GridPosition.X, 9 - bounds.Width)),
            Math.Max(0, Math.Min(GridPosition.Y, 9 - bounds.Height))
        );

       
        this.Location = new Point(
            GridOffset.X + (GridPosition.X + bounds.X) * cellSize,
            GridOffset.Y + (GridPosition.Y + bounds.Y) * cellSize
        );
    }

    private Rectangle GetShapeBounds()
    {
        if (Shape == null || Shape.Length == 0)
            return new Rectangle(0, 0, 3, 3);

        int minX = Shape[0].X, maxX = Shape[0].X;
        int minY = Shape[0].Y, maxY = Shape[0].Y;

        foreach (var point in Shape)
        {
            if (point.X < minX) minX = point.X;
            if (point.X > maxX) maxX = point.X;
            if (point.Y < minY) minY = point.Y;
            if (point.Y > maxY) maxY = point.Y;
        }

        int width = Math.Max(1, maxX - minX + 1);
        int height = Math.Max(1, maxY - minY + 1);

        return new Rectangle(minX, minY, width, height);
    }

    public virtual void OnDragStart() => this.BringToFront();

    public virtual void OnDragging(int x, int y)
    {
       
        var bounds = GetShapeBounds();

        
        int blockCenterX = x + (this.Width / 2);
        int blockCenterY = y + (this.Height / 2);

        int gridX = (blockCenterX - GridOffset.X) / cellSize;
        int gridY = (blockCenterY - GridOffset.Y) / cellSize;

        
        GridPosition = new Point(gridX - bounds.X - (bounds.Width / 2),
                                gridY - bounds.Y - (bounds.Height / 2));

        
        this.Invalidate();
    }

    public virtual void OnDragEnd() { }
    public System.Action OnBlockPlaced;

    public abstract void Place(GridManager grid);
    public abstract bool CanPlace(GridManager grid);

    
    public bool IsValidGridPosition()
    {
        foreach (var p in Shape)
        {
            int checkX = GridPosition.X + p.X;
            int checkY = GridPosition.Y + p.Y;
            if (checkX < 0 || checkX >= 9 || checkY < 0 || checkY >= 9)
                return false;
        }
        return true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        var bounds = GetShapeBounds();

        foreach (var p in Shape)
        {
            int drawX = (p.X - bounds.X) * cellSize;
            int drawY = (p.Y - bounds.Y) * cellSize;

            Rectangle rect = new Rectangle(drawX, drawY, cellSize - 2, cellSize - 2);

            if (rect.Width <= 0 || rect.Height <= 0)
                continue;

            if (isDragging && Parent != null)
            {
                var gameController = FindGameController();
                if (gameController != null)
                {
                   
                    bool validPosition = IsValidGridPosition();
                    bool canPlace = validPosition && CanPlace(gameController.GetGrid());

                  
                    using (Brush brush = new SolidBrush(canPlace ? Color.FromArgb(150, Color.LightGreen) : Color.FromArgb(150, Color.Red)))
                    {
                        g.FillRectangle(brush, rect);
                    }
                }
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(BlockColor))
                {
                    g.FillRectangle(brush, rect);
                }
            }

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