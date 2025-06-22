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
    private Point initialLocation; // Menyimpan posisi awal untuk restore jika tidak valid

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
        if (e.Button == MouseButtons.Right)
        {
            CurrentRotation = (currentRotation + 1) % AllShapes.Length;
            return;
        }

        if (e.Button == MouseButtons.Left)
        {
            isDragging = true;
            dragOffset = e.Location;
            initialLocation = this.Location; // Simpan posisi awal
            OnDragStart();
        }
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
        if (isDragging)
        {
            isDragging = false;

            // Cek apakah block berada di area grid
            if (IsInGridArea())
            {
                SnapToGrid();

                // Hanya place jika posisi valid dan bisa ditempatkan
                if (IsValidGridPosition() && CanPlace(FindGameController()?.GetGrid()))
                {
                    OnBlockPlaced?.Invoke();
                }
                else
                {
                    // Kembalikan ke posisi awal jika tidak bisa ditempatkan
                    this.Location = initialLocation;
                    GridPosition = new Point(-10, -10);
                }
            }
            else
            {
                // Jika di-drop di luar grid area, kembalikan ke posisi awal
                this.Location = initialLocation;
                GridPosition = new Point(-10, -10);
            }

            OnDragEnd();
        }
    }

    private bool IsInGridArea()
    {
        // Cek apakah block berada di dalam area grid
        var gridBounds = new Rectangle(
            GridOffset.X,
            GridOffset.Y,
            9 * cellSize, // 9 cells * 30 pixels
            9 * cellSize  // 9 cells * 30 pixels
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

        // Hitung posisi kiri-atas block relatif ke grid panel
        int blockLeftInGrid = this.Left - GridOffset.X;
        int blockTopInGrid = this.Top - GridOffset.Y;

        // Hitung koordinat grid kasar (bukan center)
        int gridX = (int)Math.Round((double)blockLeftInGrid / cellSize);
        int gridY = (int)Math.Round((double)blockTopInGrid / cellSize);

        // Sesuaikan offset berdasarkan shape
        int offsetX = bounds.X;
        int offsetY = bounds.Y;

        // Koreksi agar posisi block sesuai dengan titik kiri-atas grid shape
        GridPosition = new Point(gridX - offsetX, gridY - offsetY);

        // Pastikan tidak keluar dari batas grid
        GridPosition = new Point(
            Math.Max(0, Math.Min(GridPosition.X, 9 - bounds.Width)),
            Math.Max(0, Math.Min(GridPosition.Y, 9 - bounds.Height))
        );

        // Update posisi visual agar sejajar dengan grid
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
        // Update grid position saat dragging untuk preview
        var bounds = GetShapeBounds();

        // Hitung posisi grid berdasarkan posisi block saat ini
        int blockCenterX = x + (this.Width / 2);
        int blockCenterY = y + (this.Height / 2);

        int gridX = (blockCenterX - GridOffset.X) / cellSize;
        int gridY = (blockCenterY - GridOffset.Y) / cellSize;

        // Sesuaikan dengan offset shape
        GridPosition = new Point(gridX - bounds.X - (bounds.Width / 2),
                                gridY - bounds.Y - (bounds.Height / 2));

        // Invalidate untuk update visual preview
        this.Invalidate();
    }

    public virtual void OnDragEnd() { }
    public System.Action OnBlockPlaced;

    public abstract void Place(GridManager grid);
    public abstract bool CanPlace(GridManager grid);

    // Helper method untuk debugging
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
                    // Cek apakah posisi valid dan bisa ditempatkan
                    bool validPosition = IsValidGridPosition();
                    bool canPlace = validPosition && CanPlace(gameController.GetGrid());

                    // Warna highlight
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