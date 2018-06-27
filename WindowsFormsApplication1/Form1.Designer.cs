namespace WindowsFormsApplication1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlDealer = new System.Windows.Forms.Panel();
            this.pnlPlayer = new System.Windows.Forms.Panel();
            this.numBet = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.chkDoubleDown = new System.Windows.Forms.CheckBox();
            this.txtWinnings = new System.Windows.Forms.TextBox();
            this.lblWinnings = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numBet)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlDealer
            // 
            this.pnlDealer.Location = new System.Drawing.Point(12, 12);
            this.pnlDealer.Name = "pnlDealer";
            this.pnlDealer.Size = new System.Drawing.Size(588, 96);
            this.pnlDealer.TabIndex = 0;
            // 
            // pnlPlayer
            // 
            this.pnlPlayer.Location = new System.Drawing.Point(12, 227);
            this.pnlPlayer.Name = "pnlPlayer";
            this.pnlPlayer.Size = new System.Drawing.Size(588, 96);
            this.pnlPlayer.TabIndex = 1;
            // 
            // numBet
            // 
            this.numBet.Location = new System.Drawing.Point(72, 166);
            this.numBet.Name = "numBet";
            this.numBet.Size = new System.Drawing.Size(69, 20);
            this.numBet.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 166);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "&Bet:";
            // 
            // chkDoubleDown
            // 
            this.chkDoubleDown.AutoSize = true;
            this.chkDoubleDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDoubleDown.Location = new System.Drawing.Point(233, 165);
            this.chkDoubleDown.Name = "chkDoubleDown";
            this.chkDoubleDown.Size = new System.Drawing.Size(122, 21);
            this.chkDoubleDown.TabIndex = 4;
            this.chkDoubleDown.Text = "&Double Down";
            this.chkDoubleDown.UseVisualStyleBackColor = true;
            // 
            // txtWinnings
            // 
            this.txtWinnings.Font = new System.Drawing.Font("Garamond", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWinnings.Location = new System.Drawing.Point(522, 165);
            this.txtWinnings.Name = "txtWinnings";
            this.txtWinnings.ReadOnly = true;
            this.txtWinnings.Size = new System.Drawing.Size(78, 22);
            this.txtWinnings.TabIndex = 5;
            this.txtWinnings.TabStop = false;
            this.txtWinnings.Text = "12";
            // 
            // lblWinnings
            // 
            this.lblWinnings.AutoSize = true;
            this.lblWinnings.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWinnings.Location = new System.Drawing.Point(437, 166);
            this.lblWinnings.Name = "lblWinnings";
            this.lblWinnings.Size = new System.Drawing.Size(79, 17);
            this.lblWinnings.TabIndex = 6;
            this.lblWinnings.Text = "Winnings:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Green;
            this.ClientSize = new System.Drawing.Size(612, 335);
            this.Controls.Add(this.lblWinnings);
            this.Controls.Add(this.txtWinnings);
            this.Controls.Add(this.chkDoubleDown);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numBet);
            this.Controls.Add(this.pnlPlayer);
            this.Controls.Add(this.pnlDealer);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "N=New Game  H=Hit  S=Stay";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.numBet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlDealer;
        private System.Windows.Forms.Panel pnlPlayer;
        private System.Windows.Forms.NumericUpDown numBet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkDoubleDown;
        private System.Windows.Forms.TextBox txtWinnings;
        private System.Windows.Forms.Label lblWinnings;


    }
}

