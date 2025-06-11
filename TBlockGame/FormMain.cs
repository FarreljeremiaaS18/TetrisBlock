
using System;
using System.Windows.Forms;

public class FormMain : Form
{
    private GameController game;
    private Button btnSpawn;
    private Button btnRestart;
    private Label lblScore;

    public FormMain()
    {
        this.Text = "TBlockGame";
        this.Width = 600;
        this.Height = 400;
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
        foreach (Control ctrl in this.Controls)
        {
            if (ctrl is BlockBase)
                this.Controls.Remove(ctrl);
        }

        game = new GameController(this);
        UpdateScoreLabel(0);
    }

    public void UpdateScoreLabel(int score)
    {
        lblScore.Text = $"Score: {score}";
    }
}
