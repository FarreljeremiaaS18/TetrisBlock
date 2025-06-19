
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
        block.Location = new System.Drawing.Point(300, 50);

        block.OnBlockPlaced = () =>
        {
            if (block.CanPlace(grid))
            {
                block.Place(grid);
                PlaySound("PLACE.wav");
                int cleared = grid.ClearFullLines();
                if (cleared > 0)
                {
                    AddScore(cleared * 100);
                    PlaySound("CLEAR.wav");
                    MessageBox.Show($"{cleared} line(s) cleared!");
                }
                form.Controls.Remove(block);

                if (IsGameOver())
                {
                    MessageBox.Show("Game Over!");
                }
            }
            else
            {
                MessageBox.Show("Tidak bisa ditempatkan!");
            }
        };


        form.Controls.Add(block);
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
        for (int y = 0; y <= 9 - 3; y++)
        {
            for (int x = 0; x <= 9 - 3; x++)
            {
                var testBlocks = new BlockBase[] { new BlockL(), new BlockT(), new BlockSquare() };
                foreach (var block in testBlocks)
                {
                    block.GridPosition = new System.Drawing.Point(x, y);
                    if (block.CanPlace(grid))
                        return false;
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
