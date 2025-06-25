
using System;
using System.Windows.Forms;

public class FormMenu : Form
{
    
    private Button btnStart;
    private Button btnExit;
    private Label lblTitle;

    public FormMenu()
    {
        
        this.Text = "TBlockGame - Main Menu";
        this.Width = 320;
        this.Height = 250;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormClosed += FormMenu_FormClosed;

        lblTitle = new Label();
        lblTitle.Text = "TBlockGame";
        lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
        lblTitle.AutoSize = true;
        lblTitle.Location = new System.Drawing.Point(90, 30);
        this.Controls.Add(lblTitle);

        btnStart = new Button();
        btnStart.Text = "Start Game";
        btnStart.Width = 200;
        btnStart.Height = 40;
        btnStart.Location = new System.Drawing.Point(50, 80);
        btnStart.Click += BtnStart_Click;
        this.Controls.Add(btnStart);

        btnExit = new Button();
        btnExit.Text = "Exit";
        btnExit.Width = 200;
        btnExit.Height = 40;
        btnExit.Location = new System.Drawing.Point(50, 130);
        btnExit.Click += (s, e) => Application.Exit();
        this.Controls.Add(btnExit);
    }

    private void BtnStart_Click(object sender, EventArgs e)
    {
        FormMain gameForm = new FormMain();
        gameForm.Show();
        this.Hide();
    }

    private void FormMenu_FormClosed(object sender, FormClosedEventArgs e)
    {
        Application.Exit();
    }
}
