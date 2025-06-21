using System;
using System.Media;
using System.Windows.Forms;



public class GameController
{
    private GridManager grid;
    private FormMain form;
    private Random rnd = new Random();
    private int score = 0;
    private List<BlockBase> activeBlocks = new List<BlockBase>();

    public GameController(FormMain form)
    {
        this.form = form;
        grid = new GridManager(9, 9);
    }

    public void SpawnBlock()
    {
        // Hapus blok yang sudah ditempatkan (tidak lagi di form)
        activeBlocks.RemoveAll(b => !form.Controls.Contains(b));

        // Jika sudah ada 2 block aktif, jangan spawn lagi
        if (activeBlocks.Count >= 2)
        {
            MessageBox.Show("Maksimal 2 block aktif dalam satu waktu.");
            return;
        }

        // Cek apakah ada kemungkinan block apa pun bisa ditempatkan
        if (!CanAnyBlockFit())
        {
            MessageBox.Show("Game Over!\nTidak Tersedia Grid kosong yang mampu untuk menempatkan block lagi");
            return;
        }

        // Spawn block baru
        BlockBase block = GenerateRandomBlock();

        // Tentukan posisi spawn
        int spawnX = 20;
        int spawnY = 200 + (activeBlocks.Count * 120); // block ke-2 lebih bawah

        block.Location = new System.Drawing.Point(spawnX, spawnY);
        block.GridPosition = new System.Drawing.Point(-10, -10); // awal tidak valid

        block.OnBlockPlaced = () =>
        {
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
                activeBlocks.Remove(block);

                // ❗ Cek apakah game benar-benar sudah over setelah block ditempatkan
                if (activeBlocks.Count == 0 && !CanAnyBlockFit())
                {
                    PlaySound("GAMEOVER.mp3");
                    MessageBox.Show("Game Over!\nTidak ada lagi block yang bisa ditempatkan.");
                }
            }
            else
            {
                PlaySound("ALERT.wav");
                MessageBox.Show("Cannot place block here!");
                block.Location = new System.Drawing.Point(spawnX, spawnY);
                block.GridPosition = new System.Drawing.Point(-10, -10);
            }
        };
        form.Controls.Add(block);
        activeBlocks.Add(block);

        // Setelah spawn 2 block, cek apakah salah satunya bisa diletakkan
        if (activeBlocks.Count == 2 && !CanAnyActiveBlockBePlaced())
        {
            PlaySound("GAMEOVER.wav");
            MessageBox.Show("Game Over!\nKedua block tidak dapat diletakkan.");
        }
    }

    private bool CanAnyBlockFit()
    {
        var testBlocks = new BlockBase[] { new BlockL(), new BlockT(), new BlockSquare() };

        foreach (var block in testBlocks)
        {
            for (int rotation = 0; rotation < block.AllShapes.Length; rotation++)
            {
                block.SetRotation(rotation);
                for (int y = 0; y < 9; y++)
                {
                    for (int x = 0; x < 9; x++)
                    {
                        block.GridPosition = new System.Drawing.Point(x, y);
                        if (block.CanPlace(grid))
                            return true;
                    }
                }
            }
        }
        return false;
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

    private bool CanAnyActiveBlockBePlaced()
    {
        foreach (var block in activeBlocks)
        {
            for (int rotation = 0; rotation < block.AllShapes.Length; rotation++)
            {
                block.SetRotation(rotation);
                for (int y = 0; y < 9; y++)
                {
                    for (int x = 0; x < 9; x++)
                    {
                        block.GridPosition = new System.Drawing.Point(x, y);
                        if (block.CanPlace(grid))
                            return true;
                    }
                }
            }
        }
        return false;
    }


    private void PlaySound(string fileName)
    {
        try
        {
            string fullPath = System.IO.Path.Combine(Application.StartupPath, "Resources", fileName);
            if (System.IO.File.Exists(fullPath))
            {
                SoundPlayer player = new SoundPlayer(fullPath);
                player.Play();  // Tunggu sampai selesai (atau gunakan Thread/Task jika async)

            }
            else
            {
                MessageBox.Show($"Sound file not found: {fullPath}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error playing sound: {ex.Message}");
        }
    }

}