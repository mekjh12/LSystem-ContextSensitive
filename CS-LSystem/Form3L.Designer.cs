namespace LSystem
{
    partial class Form3L
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
            this.glControl1 = new OpenGL.GlControl();
            this.SuspendLayout();
            // 
            // glControl1
            // 
            this.glControl1.Animation = true;
            this.glControl1.AutoSize = true;
            this.glControl1.BackColor = System.Drawing.Color.Gray;
            this.glControl1.ColorBits = ((uint)(24u));
            this.glControl1.DepthBits = ((uint)(24u));
            this.glControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.glControl1.Location = new System.Drawing.Point(0, 0);
            this.glControl1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.glControl1.MultisampleBits = ((uint)(0u));
            this.glControl1.Name = "glControl1";
            this.glControl1.Size = new System.Drawing.Size(825, 485);
            this.glControl1.StencilBits = ((uint)(8u));
            this.glControl1.SwapInterval = -1;
            this.glControl1.TabIndex = 1;
            this.glControl1.Render += new System.EventHandler<OpenGL.GlControlEventArgs>(this.glControl1_Render);
            this.glControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.glControl1_KeyDown);
            this.glControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseMove);
            this.glControl1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.glControl1_MouseWheel);
            // 
            // Form3L
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 485);
            this.Controls.Add(this.glControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form3L";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form3L";
            this.Load += new System.EventHandler(this.Form3L_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenGL.GlControl glControl1;
    }
}