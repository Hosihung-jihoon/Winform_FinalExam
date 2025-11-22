using CaroGame.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroGame.Winforms.Forms
{
    public partial class BoardSizeForm : Form
    {
        private readonly GameMode _gameMode;
        public BoardSizeForm()
        {
            InitializeComponent();
        }
        public BoardSizeForm(GameMode gameMode) : this()
        {
            _gameMode = gameMode;

            SetupHoverEffects();
        }
        private void SetupHoverEffects()
        {
            // btn3x3 hover
            btn3x3.MouseEnter += (s, e) => btn3x3.BackColor = Color.FromArgb(39, 174, 96);
            btn3x3.MouseLeave += (s, e) => btn3x3.BackColor = Color.FromArgb(46, 204, 113);

            // btn19x19 hover
            btn19x19.MouseEnter += (s, e) => btn19x19.BackColor = Color.FromArgb(39, 174, 96);
            btn19x19.MouseLeave += (s, e) => btn19x19.BackColor = Color.FromArgb(46, 204, 113);

            // btnBack hover
            btnBack.MouseEnter += (s, e) => btnBack.BackColor = Color.FromArgb(127, 140, 141);
            btnBack.MouseLeave += (s, e) => btnBack.BackColor = Color.FromArgb(149, 165, 166);
        }
        private void btn3x3_Click(object sender, EventArgs e)
        {
            OpenGameForm(3);
        }
        private void btn19x19_Click(object sender, EventArgs e)
        {
            OpenGameForm(19);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
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
