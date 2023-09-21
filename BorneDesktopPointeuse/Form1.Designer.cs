namespace BorneDesktopPointeuse
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pictureBox = new PictureBox();
            txtQRCode = new TextBox();
            timer1 = new System.Windows.Forms.Timer(components);
            lblMessage = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.BorderStyle = BorderStyle.Fixed3D;
            pictureBox.Location = new Point(233, 303);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(544, 486);
            pictureBox.TabIndex = 2;
            pictureBox.TabStop = false;
            // 
            // txtQRCode
            // 
            txtQRCode.Location = new Point(1170, 445);
            txtQRCode.Multiline = true;
            txtQRCode.Name = "txtQRCode";
            txtQRCode.Size = new Size(474, 297);
            txtQRCode.TabIndex = 4;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Leelawadee UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblMessage.ForeColor = SystemColors.ControlText;
            lblMessage.Location = new Point(233, 152);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(1632, 54);
            lblMessage.TabIndex = 5;
            lblMessage.Text = "Bienvenue à l'école 3iL ! Veuillez scanner votre QR Code en l'approchant de la caméra";
            lblMessage.TextAlign = ContentAlignment.TopCenter;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(17F, 41F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(2095, 1155);
            Controls.Add(lblMessage);
            Controls.Add(txtQRCode);
            Controls.Add(pictureBox);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Borne Pointeuse";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private PictureBox pictureBox;
        private TextBox txtQRCode;
        private System.Windows.Forms.Timer timer1;
        private Label lblMessage;
    }
}