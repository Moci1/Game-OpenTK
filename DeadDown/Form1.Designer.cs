namespace DeadDown
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
            this.gameLayer1 = new DeadDown.GameLayer();
            this.button1 = new System.Windows.Forms.Button();
            this.gameLayer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gameLayer1
            // 
            this.gameLayer1.Controls.Add(this.button1);
            this.gameLayer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gameLayer1.IsDebugMode = false;
            this.gameLayer1.IsRendering = false;
            this.gameLayer1.Location = new System.Drawing.Point(0, 0);
            this.gameLayer1.Name = "gameLayer1";
            this.gameLayer1.Size = new System.Drawing.Size(800, 600);
            this.gameLayer1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(287, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
           // this.Controls.Add(this.gameLayer1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.gameLayer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GameLayer gameLayer1;
        //        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button button1;



    }
}

