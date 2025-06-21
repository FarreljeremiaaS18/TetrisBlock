using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class FormMain : Form
{
    private GameController game;
    private Button btnSpawn;
    private Button btnRestart;
    private Label lblScore;
    private Panel gridPanel;

    public FormMain()
    {
        this.Text = "TBlockGame";
        this.Width = 600;
        this.Height = 500;
        this.StartPosition = FormStartPosition.CenterScreen;

        lblScore = new Label();
        lblScore.Text = "Score: 0";
        lblScore.Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);
        lblScore.AutoSize = true;
        lblScore.Location = new System.Drawing.Point(20, 20);
        this.Controls.Add(lblScore);

        btnSpawn = new Button();
        btnSpawn.Text = "Spawn Block";
        btnSpawn.Width = 120;
        btnSpawn.Height = 40;
        btnSpawn.Location = new System.Drawing.Point(20, 60);
        btnSpawn.Click += BtnSpawn_Click;
        this.Controls.Add(btnSpawn);

        btnRestart = new Button();
        btnRestart.Text = "Restart";
        btnRestart.Width = 120;
        btnRestart.Height = 40;
        btnRestart.Location = new System.Drawing.Point(20, 110);
        btnRestart.Click += BtnRestart_Click;
        this.Controls.Add(btnRestart);

        gridPanel = new GridPanel();
        gridPanel.Location = BlockBase.GridOffset;
        gridPanel.Size = new Size(9 * 30, 9 * 30);
        gridPanel.BackColor = Color.LightGray;
        this.Controls.Add(gridPanel);

        game = new GameController(this);
    }

    private void BtnSpawn_Click(object sender, EventArgs e)
    {
        game.SpawnBlock();
    }

    private void BtnRestart_Click(object sender, EventArgs e)
    {
        RestartGame();
    }

    public void RestartGame()
    {
        var controlsToRemove = new List<Control>();
        foreach (Control ctrl in this.Controls)
        {
            if (ctrl is BlockBase)
                controlsToRemove.Add(ctrl);
        }

        foreach (var ctrl in controlsToRemove)
        {
            this.Controls.Remove(ctrl);
        }

        game = new GameController(this);
        UpdateScoreLabel(0);

        gridPanel.Invalidate();
    }

    public void UpdateScoreLabel(int score)
    {
        lblScore.Text = $"Score: {score}";
    }

    public GameController GetGameController()
    {
        return game;
    }
}

public class GridPanel : Panel
{
    private int cellSize = 30;
    private int gridWidth = 9;
    private int gridHeight = 9;

    public GridPanel()
    {
        this.DoubleBuffered = true;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        Graphics g = e.Graphics;

        using (Pen pen = new Pen(Color.Gray, 1))
        {
            for (int x = 0; x <= gridWidth; x++)
            {
                g.DrawLine(pen, x * cellSize, 0, x * cellSize, gridHeight * cellSize);
            }

            for (int y = 0; y <= gridHeight; y++)
            {
                g.DrawLine(pen, 0, y * cellSize, gridWidth * cellSize, y * cellSize);
            }
        }

        if (Parent is FormMain form)
        {
            var gameController = form.GetGameController();
            if (gameController != null)
            {
                var grid = gameController.GetGrid();
                using (Brush brush = new SolidBrush(Color.DarkBlue))
                {
                    for (int x = 0; x < gridWidth; x++)
                    {
                        for (int y = 0; y < gridHeight; y++)
                        {
                            if (!grid.IsCellEmpty(x, y))
                            {
                                g.FillRectangle(brush,
                                    x * cellSize + 1,
                                    y * cellSize + 1,
                                    cellSize - 2,
                                    cellSize - 2);
                            }
                        }
                    }
                }
            }
        }
    }
}