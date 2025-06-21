using System;
using System.Media;
using System.Windows.Forms;

public class GameController
{
    private GridManager grid;
    private FormMain form;
    private Random rnd = new Random();
    private int score = 0;

    public GameController(FormMain form)
    {
        this.form = form;
        grid = new GridManager(9, 9);
    }

    public void SpawnBlock()
    {
        BlockBase block = GenerateRandomBlock();

        // Position the block di area spawn (kiri atas form)
        block.Location = new System.Drawing.Point(20, 200);

        // Set initial grid position yang tidak valid (di luar grid)
        block.GridPosition = new System.Drawing.Point(-10, -10);

        block.OnBlockPlaced = () =>
        {
            // Cek apakah block bisa ditempatkan
            if (block.CanPlace(grid))
            {
                block.Place(grid);
                PlaySound("PLACE.wav");

                RefreshGridDisplay();

                int cleared = grid.ClearFullLines();
                if (cleared > 0)
                {
                    AddScore(cleared * 100);
                    PlaySound("CLEAR.wav");
                    MessageBox.Show($"{cleared} line(s) cleared!");

                    RefreshGridDisplay();
                }

                form.Controls.Remove(block);

                if (IsGameOver())
                {
                    MessageBox.Show("Game Over!");
                }
            }
            else
            {
                MessageBox.Show("Cannot place block here!");
                // Kembalikan ke posisi spawn
                block.Location = new System.Drawing.Point(20, 200);
                block.GridPosition = new System.Drawing.Point(-10, -10);
            }
        };

        form.Controls.Add(block);
    }

    private bool IsBlockInGridArea(BlockBase block)
    {
        // Cek apakah block berada di dalam area grid 9x9
        var gridBounds = new System.Drawing.Rectangle(
            BlockBase.GridOffset.X,
            BlockBase.GridOffset.Y,
            9 * 30, // 9 cells * 30 pixels
            9 * 30  // 9 cells * 30 pixels
        );

        var blockBounds = new System.Drawing.Rectangle(
            block.Location.X,
            block.Location.Y,
            block.Width,
            block.Height
        );

        // Return true jika ada overlap antara block dan grid
        return gridBounds.IntersectsWith(blockBounds);
    }

    private void RefreshGridDisplay()
    {
        foreach (Control ctrl in form.Controls)
        {
            if (ctrl is GridPanel)
            {
                ctrl.Invalidate();
                break;
            }
        }
    }

    private BlockBase GenerateRandomBlock()
    {
        int i = rnd.Next(3);
        return i switch
        {
            0 => new BlockL(),
            1 => new BlockT(),
            _ => new BlockSquare(),
        };
    }

    public GridManager GetGrid() => grid;

    public void AddScore(int amount)
    {
        score += amount;
        form.UpdateScoreLabel(score);
    }

    public bool IsGameOver()
    {
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                var testBlocks = new BlockBase[] { new BlockL(), new BlockT(), new BlockSquare() };
                foreach (var testBlock in testBlocks)
                {
                    for (int rotation = 0; rotation < testBlock.AllShapes.Length; rotation++)
                    {
                        testBlock.CurrentRotation = rotation;
                        testBlock.GridPosition = new System.Drawing.Point(x, y);
                        if (testBlock.CanPlace(grid))
                            return false;
                    }
                }
            }
        }
        return true;
    }

    private void PlaySound(string fileName)
    {
        try
        {
            SoundPlayer player = new SoundPlayer($"Resources\\{fileName}");
            player.Play();
        }
        catch { }
    }
}