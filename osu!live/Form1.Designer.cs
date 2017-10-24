namespace osu_live
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer_status_check = new System.Windows.Forms.Timer(this.components);
            this.canvas = new System.Windows.Forms.PictureBox();
            this.timer_status_change = new System.Windows.Forms.Timer(this.components);
            this.action_change_bg = new System.Windows.Forms.Timer(this.components);
            this.action_display = new System.Windows.Forms.Timer(this.components);
            this.action_change_info = new System.Windows.Forms.Timer(this.components);
            this.action_particle = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).BeginInit();
            this.SuspendLayout();
            // 
            // timer_status_check
            // 
            this.timer_status_check.Enabled = true;
            this.timer_status_check.Interval = 10;
            this.timer_status_check.Tick += new System.EventHandler(this.timer_status_check_Tick);
            // 
            // canvas
            // 
            this.canvas.BackColor = System.Drawing.Color.Black;
            this.canvas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canvas.Location = new System.Drawing.Point(0, 0);
            this.canvas.Name = "canvas";
            this.canvas.Size = new System.Drawing.Size(535, 286);
            this.canvas.TabIndex = 0;
            this.canvas.TabStop = false;
            // 
            // timer_status_change
            // 
            this.timer_status_change.Interval = 1;
            this.timer_status_change.Tick += new System.EventHandler(this.timer_status_change_Tick);
            // 
            // action_change_bg
            // 
            this.action_change_bg.Interval = 1;
            this.action_change_bg.Tick += new System.EventHandler(this.action_change_bg_Tick);
            // 
            // action_display
            // 
            this.action_display.Interval = 10;
            this.action_display.Tick += new System.EventHandler(this.action_display_Tick);
            // 
            // action_change_info
            // 
            this.action_change_info.Interval = 1;
            this.action_change_info.Tick += new System.EventHandler(this.action_change_info_Tick);
            // 
            // action_particle
            // 
            this.action_particle.Interval = 1;
            this.action_particle.Tick += new System.EventHandler(this.action_particle_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 286);
            this.Controls.Add(this.canvas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.canvas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer_status_check;
        private System.Windows.Forms.PictureBox canvas;
        private System.Windows.Forms.Timer timer_status_change;
        private System.Windows.Forms.Timer action_change_bg;
        private System.Windows.Forms.Timer action_display;
        private System.Windows.Forms.Timer action_change_info;
        private System.Windows.Forms.Timer action_particle;
    }
}

