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
    /// Form chọn kích thước bàn cờ (3x3 hoặc 19x19)
    /// </summary>
    public partial class BoardSizeForm : Form
    {
        private readonly GameMode _gameMode;

        public BoardSizeForm(GameMode gameMode)
        {
            _gameMode = gameMode;
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }

        private void InitializeComponent()
        {
            this.Text = "Choose Board Size";
            this.Size = new Size(450, 350);
            this.BackColor = Color.FromArgb(240, 240, 240);

            // Title
            var lblTitle = new Label
            {
                Text = "Select Board Size",
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94),
                AutoSize = true,
                Location = new Point(0, 30)
            };
            lblTitle.Left = (this.ClientSize.Width - lblTitle.Width) / 2;
            this.Controls.Add(lblTitle);

            // Info label
            var lblInfo = new Label
            {
                Text = "3x3: Connect 3 | 19x19: Connect 5",
                Font = new Font("Arial", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(127, 140, 141),
                AutoSize = true,
                Location = new Point(0, 70)
            };
            lblInfo.Left = (this.ClientSize.Width - lblInfo.Width) / 2;
            this.Controls.Add(lblInfo);

            var buttonWidth = 160;
            var buttonHeight = 120;
            var spacing = 20;
            var totalWidth = buttonWidth * 2 + spacing;
            var startX = (this.ClientSize.Width - totalWidth) / 2;
            var startY = 110;

            // Button 3x3
            var btn3x3 = CreateSizeButton("3 × 3", "Quick Game", startX, startY, buttonWidth, buttonHeight);
            btn3x3.Click += (s, e) => OpenGameForm(3);
            this.Controls.Add(btn3x3);

            // Button 19x19
            var btn19x19 = CreateSizeButton("19 × 19", "Classic", startX + buttonWidth + spacing, startY, buttonWidth, buttonHeight);
            btn19x19.Click += (s, e) => OpenGameForm(19);
            this.Controls.Add(btn19x19);

            // Back button
            var btnBack = new Button
            {
                Text = "← Back to Menu",
                Size = new Size(150, 35),
                Location = new Point((this.ClientSize.Width - 150) / 2, 260),
                Font = new Font("Arial", 10, FontStyle.Regular),
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => this.Close();
            this.Controls.Add(btnBack);
        }

        private Button CreateSizeButton(string mainText, string subText, int x, int y, int width, int height)
        {
            var btn = new Button
            {
                Size = new Size(width, height),
                Location = new Point(x, y),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Arial", 24, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;

            // Custom paint để vẽ 2 dòng text
            btn.Paint += (s, e) =>
            {
                e.Graphics.Clear(btn.BackColor);

                // Main text
                var mainFont = new Font("Arial", 24, FontStyle.Bold);
                var mainSize = e.Graphics.MeasureString(mainText, mainFont);
                e.Graphics.DrawString(mainText, mainFont, Brushes.White,
                    (width - mainSize.Width) / 2, 30);

                // Sub text
                var subFont = new Font("Arial", 11, FontStyle.Regular);
                var subSize = e.Graphics.MeasureString(subText, subFont);
                e.Graphics.DrawString(subText, subFont, Brushes.White,
                    (width - subSize.Width) / 2, 75);
            };

            // Hover effect
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(39, 174, 96);
                btn.Invalidate();
            };
            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Color.FromArgb(46, 204, 113);
                btn.Invalidate();
            };

            return btn;
        }

        private void OpenGameForm(int boardSize)
        {
            this.Hide();

            Form gameForm;
            if (_gameMode == GameMode.Online)
            {
                gameForm = new OnlineGameForm(boardSize);
            }
            else
            {
                gameForm = new GameForm(boardSize, _gameMode);
            }

            gameForm.FormClosed += (s, e) =>
            {
                this.Close();
            };

            gameForm.Show();
        }
    }
}