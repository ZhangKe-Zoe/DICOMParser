namespace DicomParser
{
    partial class DPUI
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbTransferSyntax = new System.Windows.Forms.ComboBox();
            this.RtxtInput = new System.Windows.Forms.RichTextBox();
            this.btnparse = new System.Windows.Forms.Button();
            this.lvOutput = new System.Windows.Forms.ListView();
            this.Tag = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.VR = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Length = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Value = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnfile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据集传输语法：";
            // 
            // cbTransferSyntax
            // 
            this.cbTransferSyntax.FormattingEnabled = true;
            this.cbTransferSyntax.Items.AddRange(new object[] {
            "ImplicitVRLittleEndian（默认）",
            "ExplicitVRLittleEndian",
            "ExplicitVRBigEndian"});
            this.cbTransferSyntax.Location = new System.Drawing.Point(134, 19);
            this.cbTransferSyntax.Name = "cbTransferSyntax";
            this.cbTransferSyntax.Size = new System.Drawing.Size(248, 20);
            this.cbTransferSyntax.TabIndex = 1;
            // 
            // RtxtInput
            // 
            this.RtxtInput.Location = new System.Drawing.Point(28, 59);
            this.RtxtInput.Name = "RtxtInput";
            this.RtxtInput.Size = new System.Drawing.Size(777, 126);
            this.RtxtInput.TabIndex = 2;
            this.RtxtInput.Text = "";
            // 
            // btnparse
            // 
            this.btnparse.Location = new System.Drawing.Point(388, 17);
            this.btnparse.Name = "btnparse";
            this.btnparse.Size = new System.Drawing.Size(75, 23);
            this.btnparse.TabIndex = 3;
            this.btnparse.Text = "Parse";
            this.btnparse.UseVisualStyleBackColor = true;
            this.btnparse.Click += new System.EventHandler(this.btnparse_Click);
            // 
            // lvOutput
            // 
            this.lvOutput.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Tag,
            this.VR,
            this.NM,
            this.Length,
            this.Value});
            this.lvOutput.GridLines = true;
            this.lvOutput.Location = new System.Drawing.Point(28, 201);
            this.lvOutput.Name = "lvOutput";
            this.lvOutput.Size = new System.Drawing.Size(777, 400);
            this.lvOutput.TabIndex = 4;
            this.lvOutput.UseCompatibleStateImageBehavior = false;
            this.lvOutput.View = System.Windows.Forms.View.Details;
            this.lvOutput.SelectedIndexChanged += new System.EventHandler(this.lvOutput_SelectedIndexChanged);
            // 
            // Tag
            // 
            this.Tag.Text = "Tag";
            // 
            // VR
            // 
            this.VR.Text = "VR";
            // 
            // NM
            // 
            this.NM.Name = "NM";
            this.NM.Text = "Name";
            // 
            // Length
            // 
            this.Length.Text = "Length";
            // 
            // Value
            // 
            this.Value.Text = "Value";
            // 
            // btnfile
            // 
            this.btnfile.Location = new System.Drawing.Point(484, 17);
            this.btnfile.Name = "btnfile";
            this.btnfile.Size = new System.Drawing.Size(75, 23);
            this.btnfile.TabIndex = 5;
            this.btnfile.Text = "File";
            this.btnfile.UseVisualStyleBackColor = true;
            this.btnfile.Click += new System.EventHandler(this.btnfile_Click);
            // 
            // DPUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 613);
            this.Controls.Add(this.btnfile);
            this.Controls.Add(this.lvOutput);
            this.Controls.Add(this.btnparse);
            this.Controls.Add(this.RtxtInput);
            this.Controls.Add(this.cbTransferSyntax);
            this.Controls.Add(this.label1);
            this.Name = "DPUI";
            this.Text = "DicomParser";
            this.Load += new System.EventHandler(this.DPUI_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbTransferSyntax;
        private System.Windows.Forms.RichTextBox RtxtInput;
        private System.Windows.Forms.Button btnparse;
        private System.Windows.Forms.ListView lvOutput;
        private System.Windows.Forms.ColumnHeader Tag;
       // private System.Windows.Forms.ColumnHeader Name;
        private System.Windows.Forms.ColumnHeader VR;
        private System.Windows.Forms.ColumnHeader Length;
        private System.Windows.Forms.ColumnHeader Value;
        //private System.Windows.Forms.ColumnHeader From1;
        private System.Windows.Forms.ColumnHeader NM;
        private System.Windows.Forms.Button btnfile;
    }
}

