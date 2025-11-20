using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CaroGame.Core.Enums;

namespace CaroGame.WinForms.Forms
{
    /// <summary>
    /// Form menu chính - Chọn chế độ chơi
    /// </summary>
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
        }

        private void InitializeComponent()
        {
            this.Text = "Caro Game - Menu";
            this.Size = new Size(500, 400);
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Title Label
            var lblTitle = new Label
            {
                Text = "CARO GAME",
                Font = new Font("Arial", 32, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(0, 30)
            };
            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            this.Controls.Add(lblTitle);

            // Subtitle
            var lblSubtitle = new Label
            {
                Text = "Choose Game Mode",
                Font = new Font("Arial", 14, FontStyle.Regular),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(0, 90)
            };
            lblSubtitle.Left = (this.ClientSize.Width - lblSubtitle.Width) / 2;
            this.Controls.Add(lblSubtitle);

            // Button Style
            var buttonWidth = 300;
            var buttonHeight = 50;
            var buttonX = (this.ClientSize.Width - buttonWidth) / 2;
            var startY = 140;
            var spacing = 70;

            // Button 1: Player vs Computer
            var btnPvC = CreateMenuButton("🤖 Play vs Computer", buttonX, startY);
            btnPvC.Click += (s, e) => OpenBoardSizeForm(GameMode.PlayerVsComputer);
            this.Controls.Add(btnPvC);

            // Button 2: Player vs Player
            var btnPvP = CreateMenuButton("👥 Play vs Friend", buttonX, startY + spacing);
            btnPvP.Click += (s, e) => OpenBoardSizeForm(GameMode.PlayerVsPlayer);
            this.Controls.Add(btnPvP);

            // Button 3: Online
            var btnOnline = CreateMenuButton("🌐 Play Online", buttonX, startY + spacing * 2);
            btnOnline.Click += (s, e) => OpenBoardSizeForm(GameMode.Online);
            this.Controls.Add(btnOnline);
        }

        private Button CreateMenuButton(string text, int x, int y)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(300, 50),
                Location = new Point(x, y),
                Font = new Font("Arial", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            // Hover effect
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(41, 128, 185);
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(52, 152, 219);
            };

            return btn;
        }

        private void OpenBoardSizeForm(GameMode mode)
        {
            var boardSizeForm = new BoardSizeForm(mode);
            boardSizeForm.ShowDialog();
        }
    }
}
