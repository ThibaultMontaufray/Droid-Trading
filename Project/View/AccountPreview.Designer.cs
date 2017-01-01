
namespace Droid_trading.View
{
    partial class AccountPreview
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelSolde = new System.Windows.Forms.Label();
            this.labelTrades = new System.Windows.Forms.Label();
            this.labelRate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelSolde
            // 
            this.labelSolde.Dock = System.Windows.Forms.DockStyle.Left;
            this.labelSolde.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSolde.Location = new System.Drawing.Point(0, 0);
            this.labelSolde.Name = "labelSolde";
            this.labelSolde.Size = new System.Drawing.Size(109, 24);
            this.labelSolde.TabIndex = 0;
            this.labelSolde.Text = "Solde : ";
            this.labelSolde.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelTrades
            // 
            this.labelTrades.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelTrades.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTrades.Location = new System.Drawing.Point(109, 0);
            this.labelTrades.Name = "labelTrades";
            this.labelTrades.Size = new System.Drawing.Size(135, 24);
            this.labelTrades.TabIndex = 1;
            this.labelTrades.Text = "Trade : ";
            this.labelTrades.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelRate
            // 
            this.labelRate.Dock = System.Windows.Forms.DockStyle.Right;
            this.labelRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRate.Location = new System.Drawing.Point(244, 0);
            this.labelRate.Name = "labelRate";
            this.labelRate.Size = new System.Drawing.Size(110, 24);
            this.labelRate.TabIndex = 2;
            this.labelRate.Text = "Rate : ";
            this.labelRate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AccountPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(37)))));
            this.Controls.Add(this.labelTrades);
            this.Controls.Add(this.labelRate);
            this.Controls.Add(this.labelSolde);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.Name = "AccountPreview";
            this.Size = new System.Drawing.Size(354, 24);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelSolde;
        private System.Windows.Forms.Label labelTrades;
        private System.Windows.Forms.Label labelRate;
    }
}

