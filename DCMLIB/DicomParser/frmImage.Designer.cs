namespace DicomParser
{
    partial class frmImage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImage));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.窗宽 = new System.Windows.Forms.ToolStripLabel();
            this.tsWindow = new System.Windows.Forms.ToolStripTextBox();
            this.窗位 = new System.Windows.Forms.ToolStripLabel();
            this.tsLevel = new System.Windows.Forms.ToolStripTextBox();
            this.lable = new System.Windows.Forms.ToolStripLabel();
            this.tsRefresh = new System.Windows.Forms.ToolStripButton();
            this.tsList = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.窗宽,
            this.tsWindow,
            this.窗位,
            this.tsLevel,
            this.lable,
            this.tsRefresh,
            this.tsList});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(563, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "2000";
            // 
            // 窗宽
            // 
            this.窗宽.Name = "窗宽";
            this.窗宽.Size = new System.Drawing.Size(44, 22);
            this.窗宽.Text = "窗宽：";
            // 
            // tsWindow
            // 
            this.tsWindow.Name = "tsWindow";
            this.tsWindow.Size = new System.Drawing.Size(50, 25);
            this.tsWindow.Text = "2000";
            // 
            // 窗位
            // 
            this.窗位.Name = "窗位";
            this.窗位.Size = new System.Drawing.Size(44, 22);
            this.窗位.Text = "窗位：";
            // 
            // tsLevel
            // 
            this.tsLevel.Name = "tsLevel";
            this.tsLevel.Size = new System.Drawing.Size(50, 25);
            this.tsLevel.Text = "0";
            // 
            // lable
            // 
            this.lable.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lable.Image = ((System.Drawing.Image)(resources.GetObject("lable.Image")));
            this.lable.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.lable.Name = "lable";
            this.lable.Size = new System.Drawing.Size(44, 22);
            this.lable.Text = "窗类：";
            // 
            // tsRefresh
            // 
            this.tsRefresh.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsRefresh.Image")));
            this.tsRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRefresh.Name = "tsRefresh";
            this.tsRefresh.Size = new System.Drawing.Size(76, 22);
            this.tsRefresh.Text = "刷新图像";
            this.tsRefresh.Click += new System.EventHandler(this.tsRefresh_Click);
            // 
            // tsList
            // 
            this.tsList.Items.AddRange(new object[] {
            "肺窗",
            "骨窗",
            "脑窗"});
            this.tsList.Name = "tsList";
            this.tsList.Size = new System.Drawing.Size(121, 25);
            this.tsList.Text = "默认";
            this.tsList.SelectedIndexChanged += new System.EventHandler(this.tsList_SelectedIndexChanged);
            // 
            // frmImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(563, 334);
            this.Controls.Add(this.toolStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmImage";
            this.Text = "frmImage";
            this.Load += new System.EventHandler(this.frmImage_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmImage_Paint);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel 窗宽;
        private System.Windows.Forms.ToolStripLabel 窗位;
        private System.Windows.Forms.ToolStripTextBox tsWindow;
        private System.Windows.Forms.ToolStripTextBox tsLevel;
        private System.Windows.Forms.ToolStripLabel lable;
        private System.Windows.Forms.ToolStripButton tsRefresh;
        private System.Windows.Forms.ToolStripComboBox tsList;
    }
}