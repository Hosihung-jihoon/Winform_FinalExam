namespace CaroGame.Winforms.Forms
{
    partial class BoardSizeForm
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
            lblInfo = new Label();
            btn3x3 = new Button();
            btn19x19 = new Button();
            btnBack = new Button();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Arial", 20F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.ForeColor = Color.FromArgb(52, 73, 94);
            lblTitle.Location = new Point(37, 31);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(354, 46);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Select Board Size";
            // 
            // lblInfo
            // 
            lblInfo.AutoSize = true;
            lblInfo.Font = new Font("Arial", 10F, FontStyle.Italic, GraphicsUnit.Point);
            lblInfo.ForeColor = Color.FromArgb(172, 140, 141);
            lblInfo.Location = new Point(151, 91);
            lblInfo.Name = "lblInfo";
            lblInfo.Size = new Size(126, 24);
            lblInfo.TabIndex = 1;
            lblInfo.Text = "3x3 or 19x19";
            // 
            // btn3x3
            // 
            btn3x3.AutoSize = true;
            btn3x3.BackColor = Color.FromArgb(46, 204, 113);
            btn3x3.Cursor = Cursors.Hand;
            btn3x3.FlatAppearance.BorderSize = 0;
            btn3x3.FlatStyle = FlatStyle.Flat;
            btn3x3.Font = new Font("Arial", 24F, FontStyle.Bold, GraphicsUnit.Point);
            btn3x3.ForeColor = Color.White;
            btn3x3.Location = new Point(37, 118);
            btn3x3.Name = "btn3x3";
            btn3x3.Size = new Size(160, 120);
            btn3x3.TabIndex = 2;
            btn3x3.Text = "3 x 3";
            btn3x3.UseVisualStyleBackColor = false;
            btn3x3.Click += btn3x3_Click;
            // 
            // btn19x19
            // 
            btn19x19.AutoSize = true;
            btn19x19.BackColor = Color.FromArgb(46, 204, 113);
            btn19x19.Cursor = Cursors.Hand;
            btn19x19.FlatAppearance.BorderSize = 0;
            btn19x19.FlatStyle = FlatStyle.Flat;
            btn19x19.Font = new Font("Arial", 24F, FontStyle.Bold, GraphicsUnit.Point);
            btn19x19.ForeColor = Color.White;
            btn19x19.Location = new Point(203, 118);
            btn19x19.Name = "btn19x19";
            btn19x19.Size = new Size(195, 120);
            btn19x19.TabIndex = 2;
            btn19x19.Text = "19 x 19";
            btn19x19.UseVisualStyleBackColor = false;
            btn19x19.Click += btn19x19_Click;
            // 
            // btnBack
            // 
            btnBack.AutoSize = true;
            btnBack.BackColor = Color.FromArgb(149, 165, 166);
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Font = new Font("Arial", 10F, FontStyle.Regular, GraphicsUnit.Point);
            btnBack.ForeColor = Color.White;
            btnBack.Location = new Point(130, 260);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(169, 35);
            btnBack.TabIndex = 3;
            btnBack.Text = "← Back to Menu";
            btnBack.UseVisualStyleBackColor = false;
            btnBack.Click += btnBack_Click;
            // 
            // BoardSizeForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(428, 294);
            Controls.Add(btnBack);
            Controls.Add(btn19x19);
            Controls.Add(btn3x3);
            Controls.Add(lblInfo);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BoardSizeForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Choose Board Size";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private Label lblInfo;
        private Button btn3x3;
        private Button btn19x19;
        private Button btnBack;
    }
}