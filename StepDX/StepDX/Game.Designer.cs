namespace StepDX
{
    partial class Game
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
            this.scorebox = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // scorebox
            // 
            this.scorebox.AutoSize = true;
            this.scorebox.Location = new System.Drawing.Point(652, 9);
            this.scorebox.Name = "scorebox";
            this.scorebox.Size = new System.Drawing.Size(37, 13);
            this.scorebox.TabIndex = 0;
            this.scorebox.Text = "30000";
            // 
            // Game
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 462);
            this.Controls.Add(this.scorebox);
            this.Name = "Game";
            this.Text = "Step Game";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label scorebox;
    }
}

