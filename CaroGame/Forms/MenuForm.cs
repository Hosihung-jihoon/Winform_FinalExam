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
    public partial class MenuForm : Form
    {
        public MenuForm()
        {
            InitializeComponent();

            // Hover effect cho btnPvC
            btnPvC.MouseEnter += (s, e) => btnPvC.BackColor = Color.FromArgb(41, 128, 185);
            btnPvC.MouseLeave += (s, e) => btnPvC.BackColor = Color.FromArgb(52, 152, 219);

            // Hover effect cho btnPvP
            btnPvP.MouseEnter += (s, e) => btnPvP.BackColor = Color.FromArgb(41, 128, 185);
            btnPvP.MouseLeave += (s, e) => btnPvP.BackColor = Color.FromArgb(52, 152, 219);

            // Hover effect cho btnOnline
            btnOnline.MouseEnter += (s, e) => btnOnline.BackColor = Color.FromArgb(41, 128, 185);
            btnOnline.MouseLeave += (s, e) => btnOnline.BackColor = Color.FromArgb(52, 152, 219);
        }

        private void btnPvC_Click(object sender, EventArgs e)
        {
            var boardSizeForm = new BoardSizeForm(GameMode.PlayerVsComputer);
            boardSizeForm.ShowDialog();
        }

        private void btnPvP_Click(object sender, EventArgs e)
        {
            var boardSizeForm = new BoardSizeForm(GameMode.PlayerVsPlayer);
            boardSizeForm.ShowDialog();
        }

        private void btnOnline_Click(object sender, EventArgs e)
        {
            var boardSizeForm = new BoardSizeForm(GameMode.Online);
            boardSizeForm.ShowDialog();
        }
    }
}
