namespace osu_live
{
    partial class Form2
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
            this.components = new System.ComponentModel.Container();
            this.button1 = new System.Windows.Forms.Button();
            this.canvas = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.canvas2 = new System.Windows.Forms.PictureBox();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvas2)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = ">";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.Color.White;
            this.canvas.Location = new System.Drawing.Point(12, 41);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(256, 144);
            this.canvas.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.canvas.TabIndex = 1;
            this.canvas.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // canvas2
            // 
            this.canvas2.BackColor = System.Drawing.Color.White;
            this.canvas2.Location = new System.Drawing.Point(274, 41);
            this.canvas2.Name = "canvas2";
            this.canvas2.Size = new System.Drawing.Size(256, 144);
            this.canvas2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.canvas2.TabIndex = 2;
            this.canvas2.TabStop = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(41, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(107, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "particle test";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(542, 400);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.canvas2);
            this.Controls.Add(this.canvas);
            this.Controls.Add(this.button1);
            this.DoubleBuffered = true;
            this.Name = "Form2";
            this.Text = "Form2";
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.canvas2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.PictureBox canvas2;
        private System.Windows.Forms.Button button2;
    }
}