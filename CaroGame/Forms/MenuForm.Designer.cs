namespace CaroGame.Winforms.Forms
{
    partial class MenuForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblSubtitle = new Label();
            btnPvC = new Button();
            btnPvP = new Button();
            btnOnline = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Arial", 36F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = Color.OrangeRed;
            lblTitle.Location = new Point(191, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(419, 84);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Caro Game";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Arial", 14F, FontStyle.Regular, GraphicsUnit.Point);
            lblSubtitle.Location = new Point(265, 114);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(270, 32);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Choose Game Mode";
            // 
            // btnPvC
            // 
            btnPvC.AutoSize = true;
            btnPvC.BackColor = Color.FromArgb(52, 152, 219);
            btnPvC.Cursor = Cursors.Hand;
            btnPvC.FlatAppearance.BorderSize = 0;
            btnPvC.FlatStyle = FlatStyle.Flat;
            btnPvC.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            btnPvC.ForeColor = Color.White;
            btnPvC.Location = new Point(249, 179);
            btnPvC.Name = "btnPvC";
            btnPvC.Size = new Size(303, 50);
            btnPvC.TabIndex = 2;
            btnPvC.Text = "🤖 Play vs Computer";
            btnPvC.UseVisualStyleBackColor = false;
            btnPvC.Click += btnPvC_Click;
            // 
            // btnPvP
            // 
            btnPvP.AutoSize = true;
            btnPvP.BackColor = Color.FromArgb(52, 152, 219);
            btnPvP.Cursor = Cursors.Hand;
            btnPvP.FlatAppearance.BorderSize = 0;
            btnPvP.FlatStyle = FlatStyle.Flat;
            btnPvP.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            btnPvP.ForeColor = Color.White;
            btnPvP.Location = new Point(249, 262);
            btnPvP.Name = "btnPvP";
            btnPvP.Size = new Size(303, 50);
            btnPvP.TabIndex = 2;
            btnPvP.Text = "👥 Play vs Friend";
            btnPvP.UseVisualStyleBackColor = false;
            btnPvP.Click += btnPvP_Click;
            // 
            // btnOnline
            // 
            btnOnline.AutoSize = true;
            btnOnline.BackColor = Color.FromArgb(52, 152, 219);
            btnOnline.Cursor = Cursors.Hand;
            btnOnline.FlatAppearance.BorderSize = 0;
            btnOnline.FlatStyle = FlatStyle.Flat;
            btnOnline.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            btnOnline.ForeColor = Color.White;
            btnOnline.Location = new Point(249, 340);
            btnOnline.Name = "btnOnline";
            btnOnline.Size = new Size(303, 50);
            btnOnline.TabIndex = 2;
            btnOnline.Text = "🌐 Play Online";
            btnOnline.UseVisualStyleBackColor = false;
            btnOnline.Click += btnOnline_Click;
            // 
            // MenuForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(800, 450);
            Controls.Add(btnOnline);
            Controls.Add(btnPvP);
            Controls.Add(btnPvC);
            Controls.Add(lblSubtitle);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "MenuForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Caro Game - Menu";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnPvC;
        private Button btnPvP;
        private Button btnOnline;
    }
}