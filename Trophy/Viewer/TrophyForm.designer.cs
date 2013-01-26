namespace HyoutaTools.Trophy.Viewer
{
    partial class TrophyForm
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
            this.labelName = new System.Windows.Forms.Label();
            this.labelTType = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.labelHidden = new System.Windows.Forms.Label();
            this.labelUserArea = new System.Windows.Forms.Label();
            this.labelUnlocked = new System.Windows.Forms.Label();
            this.labelTimestamp = new System.Windows.Forms.Label();
            this.labelTimestamp2 = new System.Windows.Forms.Label();
            this.labelTrophyID = new System.Windows.Forms.Label();
            this.listBox1 = new CustomDrawListBox();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(303, 13);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(39, 13);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name";
            // 
            // labelTType
            // 
            this.labelTType.AutoSize = true;
            this.labelTType.Location = new System.Drawing.Point(303, 33);
            this.labelTType.Name = "labelTType";
            this.labelTType.Size = new System.Drawing.Size(67, 13);
            this.labelTType.TabIndex = 2;
            this.labelTType.Text = "Trophy Type";
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(303, 53);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(60, 13);
            this.labelDescription.TabIndex = 3;
            this.labelDescription.Text = "Description";
            // 
            // labelHidden
            // 
            this.labelHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelHidden.AutoSize = true;
            this.labelHidden.Location = new System.Drawing.Point(565, 33);
            this.labelHidden.Name = "labelHidden";
            this.labelHidden.Size = new System.Drawing.Size(47, 13);
            this.labelHidden.TabIndex = 4;
            this.labelHidden.Text = "Hidden?";
            this.labelHidden.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelUserArea
            // 
            this.labelUserArea.AutoSize = true;
            this.labelUserArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelUserArea.Location = new System.Drawing.Point(306, 162);
            this.labelUserArea.Name = "labelUserArea";
            this.labelUserArea.Size = new System.Drawing.Size(100, 13);
            this.labelUserArea.TabIndex = 5;
            this.labelUserArea.Text = "User Information";
            // 
            // labelUnlocked
            // 
            this.labelUnlocked.AutoSize = true;
            this.labelUnlocked.Location = new System.Drawing.Point(306, 182);
            this.labelUnlocked.Name = "labelUnlocked";
            this.labelUnlocked.Size = new System.Drawing.Size(59, 13);
            this.labelUnlocked.TabIndex = 6;
            this.labelUnlocked.Text = "Unlocked?";
            // 
            // labelTimestamp
            // 
            this.labelTimestamp.AutoSize = true;
            this.labelTimestamp.Location = new System.Drawing.Point(306, 202);
            this.labelTimestamp.Name = "labelTimestamp";
            this.labelTimestamp.Size = new System.Drawing.Size(58, 13);
            this.labelTimestamp.TabIndex = 7;
            this.labelTimestamp.Text = "Timestamp";
            // 
            // labelTimestamp2
            // 
            this.labelTimestamp2.AutoSize = true;
            this.labelTimestamp2.Location = new System.Drawing.Point(306, 222);
            this.labelTimestamp2.Name = "labelTimestamp2";
            this.labelTimestamp2.Size = new System.Drawing.Size(58, 13);
            this.labelTimestamp2.TabIndex = 8;
            this.labelTimestamp2.Text = "Timestamp";
            // 
            // labelTrophyID
            // 
            this.labelTrophyID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTrophyID.AutoSize = true;
            this.labelTrophyID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTrophyID.Location = new System.Drawing.Point(592, 13);
            this.labelTrophyID.Name = "labelTrophyID";
            this.labelTrophyID.Size = new System.Drawing.Size(20, 13);
            this.labelTrophyID.TabIndex = 9;
            this.labelTrophyID.Text = "ID";
            this.labelTrophyID.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 60;
            this.listBox1.Location = new System.Drawing.Point(13, 13);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(279, 578);
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // TrophyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 602);
            this.Controls.Add(this.labelTrophyID);
            this.Controls.Add(this.labelTimestamp2);
            this.Controls.Add(this.labelTimestamp);
            this.Controls.Add(this.labelUnlocked);
            this.Controls.Add(this.labelUserArea);
            this.Controls.Add(this.labelHidden);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.labelTType);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.listBox1);
            this.Name = "TrophyForm";
            this.Text = "TrophyForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomDrawListBox listBox1;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelTType;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.Label labelHidden;
        private System.Windows.Forms.Label labelUserArea;
        private System.Windows.Forms.Label labelUnlocked;
        private System.Windows.Forms.Label labelTimestamp;
        private System.Windows.Forms.Label labelTimestamp2;
        private System.Windows.Forms.Label labelTrophyID;
    }
}