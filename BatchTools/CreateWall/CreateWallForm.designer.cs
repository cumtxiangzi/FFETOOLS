namespace FFETOOLS
{
    partial class CreateWallForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateWallForm));
            this.btnCrossSel = new System.Windows.Forms.Button();
            this.btnLineSel = new System.Windows.Forms.Button();
            this.btnPointSel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbStructure = new System.Windows.Forms.RadioButton();
            this.rbAcrhitecture = new System.Windows.Forms.RadioButton();
            this.treeViewWallType = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBottomLevel = new System.Windows.Forms.ComboBox();
            this.cmbTopLevel = new System.Windows.Forms.ComboBox();
            this.chkbSegmentation = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbOffset = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCrossSel
            // 
            this.btnCrossSel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnCrossSel.BackgroundImage")));
            this.btnCrossSel.Location = new System.Drawing.Point(349, 263);
            this.btnCrossSel.Name = "btnCrossSel";
            this.btnCrossSel.Size = new System.Drawing.Size(25, 25);
            this.btnCrossSel.TabIndex = 23;
            this.btnCrossSel.UseVisualStyleBackColor = true;
            this.btnCrossSel.Click += new System.EventHandler(this.btnCrossSel_Click);
            // 
            // btnLineSel
            // 
            this.btnLineSel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLineSel.BackgroundImage")));
            this.btnLineSel.Location = new System.Drawing.Point(308, 263);
            this.btnLineSel.Name = "btnLineSel";
            this.btnLineSel.Size = new System.Drawing.Size(25, 25);
            this.btnLineSel.TabIndex = 22;
            this.btnLineSel.UseVisualStyleBackColor = true;
            this.btnLineSel.Click += new System.EventHandler(this.btnLineSel_Click);
            // 
            // btnPointSel
            // 
            this.btnPointSel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnPointSel.BackgroundImage")));
            this.btnPointSel.Location = new System.Drawing.Point(267, 263);
            this.btnPointSel.Name = "btnPointSel";
            this.btnPointSel.Size = new System.Drawing.Size(25, 25);
            this.btnPointSel.TabIndex = 21;
            this.btnPointSel.UseVisualStyleBackColor = true;
            this.btnPointSel.Click += new System.EventHandler(this.btnPointSel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbStructure);
            this.groupBox2.Controls.Add(this.rbAcrhitecture);
            this.groupBox2.Location = new System.Drawing.Point(185, 212);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(188, 44);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            // 
            // rbStructure
            // 
            this.rbStructure.AutoSize = true;
            this.rbStructure.Location = new System.Drawing.Point(105, 18);
            this.rbStructure.Name = "rbStructure";
            this.rbStructure.Size = new System.Drawing.Size(47, 16);
            this.rbStructure.TabIndex = 1;
            this.rbStructure.Text = "结构";
            this.rbStructure.UseVisualStyleBackColor = true;
            // 
            // rbAcrhitecture
            // 
            this.rbAcrhitecture.AutoSize = true;
            this.rbAcrhitecture.Checked = true;
            this.rbAcrhitecture.Location = new System.Drawing.Point(29, 18);
            this.rbAcrhitecture.Name = "rbAcrhitecture";
            this.rbAcrhitecture.Size = new System.Drawing.Size(47, 16);
            this.rbAcrhitecture.TabIndex = 0;
            this.rbAcrhitecture.TabStop = true;
            this.rbAcrhitecture.Text = "建筑";
            this.rbAcrhitecture.UseVisualStyleBackColor = true;
            // 
            // treeViewWallType
            // 
            this.treeViewWallType.Location = new System.Drawing.Point(13, 29);
            this.treeViewWallType.Name = "treeViewWallType";
            this.treeViewWallType.Size = new System.Drawing.Size(157, 227);
            this.treeViewWallType.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(200, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "顶高";
            // 
            // cmbBottomLevel
            // 
            this.cmbBottomLevel.FormattingEnabled = true;
            this.cmbBottomLevel.Location = new System.Drawing.Point(253, 41);
            this.cmbBottomLevel.Name = "cmbBottomLevel";
            this.cmbBottomLevel.Size = new System.Drawing.Size(121, 20);
            this.cmbBottomLevel.TabIndex = 17;
            // 
            // cmbTopLevel
            // 
            this.cmbTopLevel.FormattingEnabled = true;
            this.cmbTopLevel.Location = new System.Drawing.Point(253, 11);
            this.cmbTopLevel.Name = "cmbTopLevel";
            this.cmbTopLevel.Size = new System.Drawing.Size(121, 20);
            this.cmbTopLevel.TabIndex = 16;
            // 
            // chkbSegmentation
            // 
            this.chkbSegmentation.AutoSize = true;
            this.chkbSegmentation.Location = new System.Drawing.Point(203, 70);
            this.chkbSegmentation.Name = "chkbSegmentation";
            this.chkbSegmentation.Size = new System.Drawing.Size(174, 16);
            this.chkbSegmentation.TabIndex = 19;
            this.chkbSegmentation.Text = "若墙跨多楼层,按照层切分墙";
            this.chkbSegmentation.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(200, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "底高";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "墙类型";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(178, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(209, 12);
            this.label4.TabIndex = 24;
            this.label4.Text = "用户面朝墙起点到终点方向，墙中心线";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(178, 133);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(209, 12);
            this.label5.TabIndex = 25;
            this.label5.Text = "与轴线间距（偏左为正，偏右为负）mm";
            // 
            // cmbOffset
            // 
            this.cmbOffset.FormattingEnabled = true;
            this.cmbOffset.Location = new System.Drawing.Point(185, 162);
            this.cmbOffset.Name = "cmbOffset";
            this.cmbOffset.Size = new System.Drawing.Size(188, 20);
            this.cmbOffset.TabIndex = 26;
            // 
            // CreateWallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 299);
            this.Controls.Add(this.cmbOffset);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCrossSel);
            this.Controls.Add(this.btnLineSel);
            this.Controls.Add(this.btnPointSel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.treeViewWallType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbBottomLevel);
            this.Controls.Add(this.cmbTopLevel);
            this.Controls.Add(this.chkbSegmentation);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateWallForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CreateWallForm";
            this.TopMost = true;
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCrossSel;
        private System.Windows.Forms.Button btnLineSel;
        private System.Windows.Forms.Button btnPointSel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbStructure;
        private System.Windows.Forms.RadioButton rbAcrhitecture;
        private System.Windows.Forms.TreeView treeViewWallType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbBottomLevel;
        private System.Windows.Forms.ComboBox cmbTopLevel;
        private System.Windows.Forms.CheckBox chkbSegmentation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbOffset;
    }
}