
namespace DiskCleaner
{
    partial class MainForm
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
            this.cleanUpButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.targetTable = new System.Windows.Forms.TableLayoutPanel();
            this.rtbDebug = new System.Windows.Forms.RichTextBox();
            this.deletionSizeText = new System.Windows.Forms.TextBox();
            this.deletionTextLabel = new System.Windows.Forms.Label();
            this.assessButton = new System.Windows.Forms.Button();
            this.selectAllButton = new System.Windows.Forms.Button();
            this.selectNoneButton = new System.Windows.Forms.Button();
            this.selectAllNoneLabel = new System.Windows.Forms.Label();
            this.loadingTextLabel = new System.Windows.Forms.Label();
            this.seekerModeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cleanUpButton
            // 
            this.cleanUpButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cleanUpButton.Location = new System.Drawing.Point(713, 334);
            this.cleanUpButton.Name = "cleanUpButton";
            this.cleanUpButton.Size = new System.Drawing.Size(112, 95);
            this.cleanUpButton.TabIndex = 0;
            this.cleanUpButton.Text = "Delete everything selected";
            this.cleanUpButton.UseVisualStyleBackColor = true;
            this.cleanUpButton.Click += new System.EventHandler(this.cleanUpButton_Click);
            // 
            // loadButton
            // 
            this.loadButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.loadButton.Location = new System.Drawing.Point(713, 12);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(112, 23);
            this.loadButton.TabIndex = 4;
            this.loadButton.Text = "Load a template...";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // targetTable
            // 
            this.targetTable.ColumnCount = 2;
            this.targetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.targetTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.targetTable.Location = new System.Drawing.Point(5, 12);
            this.targetTable.Name = "targetTable";
            this.targetTable.RowCount = 2;
            this.targetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.targetTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.targetTable.Size = new System.Drawing.Size(702, 500);
            this.targetTable.TabIndex = 5;
            // 
            // rtbDebug
            // 
            this.rtbDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rtbDebug.Location = new System.Drawing.Point(5, 519);
            this.rtbDebug.Name = "rtbDebug";
            this.rtbDebug.Size = new System.Drawing.Size(702, 129);
            this.rtbDebug.TabIndex = 6;
            this.rtbDebug.Text = "";
            // 
            // deletionSizeText
            // 
            this.deletionSizeText.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.deletionSizeText.Location = new System.Drawing.Point(713, 87);
            this.deletionSizeText.Name = "deletionSizeText";
            this.deletionSizeText.Size = new System.Drawing.Size(112, 20);
            this.deletionSizeText.TabIndex = 7;
            // 
            // deletionTextLabel
            // 
            this.deletionTextLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.deletionTextLabel.AutoSize = true;
            this.deletionTextLabel.Location = new System.Drawing.Point(713, 71);
            this.deletionTextLabel.Name = "deletionTextLabel";
            this.deletionTextLabel.Size = new System.Drawing.Size(72, 13);
            this.deletionTextLabel.TabIndex = 8;
            this.deletionTextLabel.Text = "Deletion Size:";
            // 
            // assessButton
            // 
            this.assessButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.assessButton.Location = new System.Drawing.Point(713, 41);
            this.assessButton.Name = "assessButton";
            this.assessButton.Size = new System.Drawing.Size(112, 23);
            this.assessButton.TabIndex = 3;
            this.assessButton.Text = "Re/Assess sizes";
            this.assessButton.UseVisualStyleBackColor = true;
            this.assessButton.Click += new System.EventHandler(this.assessButton_Click);
            // 
            // selectAllButton
            // 
            this.selectAllButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.selectAllButton.Location = new System.Drawing.Point(713, 128);
            this.selectAllButton.Name = "selectAllButton";
            this.selectAllButton.Size = new System.Drawing.Size(55, 24);
            this.selectAllButton.TabIndex = 9;
            this.selectAllButton.Text = "All";
            this.selectAllButton.UseVisualStyleBackColor = true;
            this.selectAllButton.Click += new System.EventHandler(this.selectAllButton_Click);
            // 
            // selectNoneButton
            // 
            this.selectNoneButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.selectNoneButton.Location = new System.Drawing.Point(774, 128);
            this.selectNoneButton.Name = "selectNoneButton";
            this.selectNoneButton.Size = new System.Drawing.Size(51, 24);
            this.selectNoneButton.TabIndex = 10;
            this.selectNoneButton.Text = "None";
            this.selectNoneButton.UseVisualStyleBackColor = true;
            this.selectNoneButton.Click += new System.EventHandler(this.selectNoneButton_Click);
            // 
            // selectAllNoneLabel
            // 
            this.selectAllNoneLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.selectAllNoneLabel.Location = new System.Drawing.Point(713, 110);
            this.selectAllNoneLabel.Name = "selectAllNoneLabel";
            this.selectAllNoneLabel.Size = new System.Drawing.Size(37, 19);
            this.selectAllNoneLabel.TabIndex = 11;
            this.selectAllNoneLabel.Text = "Select";
            // 
            // loadingTextLabel
            // 
            this.loadingTextLabel.Location = new System.Drawing.Point(5, 9);
            this.loadingTextLabel.Name = "loadingTextLabel";
            this.loadingTextLabel.Size = new System.Drawing.Size(702, 507);
            this.loadingTextLabel.TabIndex = 12;
            this.loadingTextLabel.Text = "Loading...";
            this.loadingTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // seekerModeButton
            // 
            this.seekerModeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.seekerModeButton.Location = new System.Drawing.Point(741, 594);
            this.seekerModeButton.Name = "seekerModeButton";
            this.seekerModeButton.Size = new System.Drawing.Size(84, 54);
            this.seekerModeButton.TabIndex = 13;
            this.seekerModeButton.Text = "Seeker mode (takes time)";
            this.seekerModeButton.UseVisualStyleBackColor = true;
            this.seekerModeButton.Click += new System.EventHandler(this.seekerMode_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 660);
            this.Controls.Add(this.seekerModeButton);
            this.Controls.Add(this.loadingTextLabel);
            this.Controls.Add(this.selectAllNoneLabel);
            this.Controls.Add(this.selectNoneButton);
            this.Controls.Add(this.selectAllButton);
            this.Controls.Add(this.deletionTextLabel);
            this.Controls.Add(this.deletionSizeText);
            this.Controls.Add(this.rtbDebug);
            this.Controls.Add(this.targetTable);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.assessButton);
            this.Controls.Add(this.cleanUpButton);
            this.Name = "MainForm";
            this.Text = "Disk Cleaner";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cleanUpButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.TableLayoutPanel targetTable;
        private System.Windows.Forms.RichTextBox rtbDebug;
        private System.Windows.Forms.TextBox deletionSizeText;
        private System.Windows.Forms.Label deletionTextLabel;
        private System.Windows.Forms.Button assessButton;
        private System.Windows.Forms.Button selectAllButton;
        private System.Windows.Forms.Button selectNoneButton;
        private System.Windows.Forms.Label selectAllNoneLabel;
        private System.Windows.Forms.Label loadingTextLabel;
        private System.Windows.Forms.Button seekerModeButton;
    }
}

