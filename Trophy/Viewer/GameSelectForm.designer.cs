namespace HyoutaTools.Trophy.Viewer
{
    partial class GameSelectForm
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
            this.labelSortBy = new System.Windows.Forms.Label();
            this.radioButtonSortByID = new System.Windows.Forms.RadioButton();
            this.radioButtonSortByDate = new System.Windows.Forms.RadioButton();
            this.checkBoxDescending = new System.Windows.Forms.CheckBox();
            this.listBox1 = new CustomDrawListBox();
            this.SuspendLayout();
            // 
            // labelSortBy
            // 
            this.labelSortBy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelSortBy.AutoSize = true;
            this.labelSortBy.Location = new System.Drawing.Point(12, 434);
            this.labelSortBy.Name = "labelSortBy";
            this.labelSortBy.Size = new System.Drawing.Size(43, 13);
            this.labelSortBy.TabIndex = 2;
            this.labelSortBy.Text = "Sort by:";
            // 
            // radioButtonSortByID
            // 
            this.radioButtonSortByID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButtonSortByID.AutoSize = true;
            this.radioButtonSortByID.Checked = true;
            this.radioButtonSortByID.Location = new System.Drawing.Point(61, 433);
            this.radioButtonSortByID.Name = "radioButtonSortByID";
            this.radioButtonSortByID.Size = new System.Drawing.Size(72, 17);
            this.radioButtonSortByID.TabIndex = 3;
            this.radioButtonSortByID.TabStop = true;
            this.radioButtonSortByID.Text = "Trophy ID";
            this.radioButtonSortByID.UseVisualStyleBackColor = true;
            // 
            // radioButtonSortByDate
            // 
            this.radioButtonSortByDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButtonSortByDate.AutoSize = true;
            this.radioButtonSortByDate.Location = new System.Drawing.Point(139, 433);
            this.radioButtonSortByDate.Name = "radioButtonSortByDate";
            this.radioButtonSortByDate.Size = new System.Drawing.Size(93, 17);
            this.radioButtonSortByDate.TabIndex = 4;
            this.radioButtonSortByDate.Text = "Time Acquired";
            this.radioButtonSortByDate.UseVisualStyleBackColor = true;
            // 
            // checkBoxDescending
            // 
            this.checkBoxDescending.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxDescending.AutoSize = true;
            this.checkBoxDescending.Location = new System.Drawing.Point(61, 456);
            this.checkBoxDescending.Name = "checkBoxDescending";
            this.checkBoxDescending.Size = new System.Drawing.Size(83, 17);
            this.checkBoxDescending.TabIndex = 5;
            this.checkBoxDescending.Text = "Descending";
            this.checkBoxDescending.UseVisualStyleBackColor = true;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 60;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(548, 413);
            this.listBox1.TabIndex = 1;
            this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
            // 
            // GameSelectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 478);
            this.Controls.Add(this.checkBoxDescending);
            this.Controls.Add(this.radioButtonSortByDate);
            this.Controls.Add(this.radioButtonSortByID);
            this.Controls.Add(this.labelSortBy);
            this.Controls.Add(this.listBox1);
            this.Name = "GameSelectForm";
            this.Text = "GameSelectForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomDrawListBox listBox1;
        private System.Windows.Forms.Label labelSortBy;
        private System.Windows.Forms.RadioButton radioButtonSortByID;
        private System.Windows.Forms.RadioButton radioButtonSortByDate;
        private System.Windows.Forms.CheckBox checkBoxDescending;
    }
}