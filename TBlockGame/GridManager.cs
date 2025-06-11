
public class GridManager
{
    private readonly int[,] grid;
    public int Width { get; }
    public int Height { get; }

    public GridManager(int width, int height)
    {
        Width = width;
        Height = height;
        grid = new int[width, height];
    }

    public bool IsCellEmpty(int x, int y)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height) return false;
        return grid[x, y] == 0;
    }

    public void SetCell(int x, int y, bool filled)
    {
        if (x < 0 || y < 0 || x >= Width || y >= Height) return;
        grid[x, y] = filled ? 1 : 0;
    }

    public int ClearFullLines()
    {
        int cleared = 0;

        for (int y = 0; y < Height; y++)
        {
            bool full = true;
            for (int x = 0; x < Width; x++)
            {
                if (grid[x, y] == 0) { full = false; break; }
            }
            if (full) { for (int x = 0; x < Width; x++) grid[x, y] = 0; cleared++; }
        }

        for (int x = 0; x < Width; x++)
        {
            bool full = true;
            for (int y = 0; y < Height; y++)
            {
                if (grid[x, y] == 0) { full = false; break; }
            }
            if (full) { for (int y = 0; y < Height; y++) grid[x, y] = 0; cleared++; }
        }

        return cleared;
    }
}
