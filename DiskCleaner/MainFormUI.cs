using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace DiskCleaner {
    public partial class MainForm {
        // Template parsing
        private string templateText = null;
        private List<TemplateItem> templateItems = null;
        private List<CheckBox> templateCheckBoxes = null;

        // Stats
        private long totalSize;
        private int totalFiles;
        private int totalDirectories;

        // An extension of the C-Tor
        // Please don't use it elsewhere.
        protected void UI_InitializeComponent() {
            // Hide the huge "Loading..." label.
            // It should only be enabled if it's needed later
            loadingTextLabel.Visible = false;

            // Give the debugger where to print, and set its level
            Debugger.debugLevelThreshold = 0; // Beware, this var can also be set throug the deletion template
            Debugger.rtDebug = this.rtbDebug;
            Debugger.logPath = @"w10dc.log"; // Default log name

            // Set the read only attributes
            rtbDebug.ReadOnly = true;
            deletionSizeText.ReadOnly = true;

            // Make the table click take the focus off any of its textboxes
            targetTable.MouseClick += (sender, args) => {
                foreach (Control control in targetTable.Controls) {
                    if (control is TextBox) {
                        TextBox textBox = (TextBox)control;
                        textBox.SelectionLength = 0;
                    }
                }
            };
        }

        protected void UI_DeleteAllSelectedItems() {
            int pos = -1;
            int countEffectivelyInvokedDeleteAlls = 0;

            foreach (TemplateItem item in templateItems) {
                pos++;
                CheckBox cb = templateCheckBoxes[pos];
                if (cb.Checked) {
                    countEffectivelyInvokedDeleteAlls++;
                    item.DeleteAll();
                }
            }

            if (countEffectivelyInvokedDeleteAlls > 0) {
                long sizeBefore = this.totalSize;
                long totalFilesBefore = this.totalFiles;
                long totalDirectoriesBefore = this.totalDirectories;
                UI_AssessFilesFromTheTemplate();
                // It is safer right now to do a SelectNone() here, as the
                // table will switch to all-selected after Assess(). There
                // is some danger that the user would dare the app another
                // click+yes on deleting all, which would yield far less
                // hazardous results if nothing is selected for deletion.
                UI_SelectNone();
                Debugger.Print(
                    "Deleted {0}, {1} files, {2} directories",
                    Debugger.FileSizeToString(sizeBefore - totalSize),
                    totalFilesBefore - totalFiles,
                    totalDirectoriesBefore - totalDirectories
                );
            }
            else {
                Debugger.Print("No items selected for deletion.");
            }
        }

        protected void UI_AssessFilesFromTheTemplate() {
            this.templateItems = null;
            this.Core_ParseTheTemplate(); // Parse the template

            // Nothing to do? Return.
            if( this.templateItems == null
                || templateItems.Count == 0
            ) {
                Debugger.Print("Nothing to assess.");
                return;
            }

            // Set up the table layout panel
            int templateItemsCount = templateItems.Count;

            targetTable.Visible = false;

            // Clear
            targetTable.Controls.Clear();
            targetTable.RowStyles.Clear();
            targetTable.ColumnStyles.Clear();

            // Set size
            targetTable.ColumnCount = 3;
            targetTable.RowCount = templateItemsCount + 1; // The +1 is for the header

            if (templateCheckBoxes == null) {
                templateCheckBoxes = new List<CheckBox>();
            }
            else {
                templateCheckBoxes.Clear();
            }

            // Autoscroll, please
            targetTable.AutoScroll = true;

            // Set styles
            targetTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            targetTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            targetTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            targetTable.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
            targetTable.Padding = new Padding(0, 0, 0, 0);
            targetTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

            // Add the header as the first row
            targetTable.Controls.Add(new Label() { Text = "Delete?" }, 0, 0);
            targetTable.Controls.Add(new Label() { Text = "Size" }, 1, 0);
            targetTable.Controls.Add(new Label() { Text = "Template path" }, 2, 0);

            totalSize = 0;
            totalFiles = 0;
            totalDirectories = 0;
            int pos = 0;
            foreach (TemplateItem item in templateItems) {
                pos++;

                // Get and aggregate the size
                long assessedSize = 0;
                if (!item.IsCulled()) {
                    assessedSize = item.AssessSize();
                }

                CheckBox cb = new CheckBox();
                // The row's checkbox should be checked unless tagged with
                // checked: no. Also, if it is disabled, it cannot be checked.
                if (item.IsChecked() && !item.IsDisabled()) {
                    cb.Checked = true;
                    totalSize += assessedSize;
                    totalFiles += item.AssessedFileCount();
                    totalDirectories += item.AssessedDirectoryCount();
                }
                else {
                    cb.Checked = false;
                }

                // The row's checkbox should get its trigger for status change
                // if it is not disabled. Else, interaction with it should be
                // disabled.
                if (!item.IsDisabled()) {
                    cb.CheckedChanged += CheckBox_CheckedChanged;
                }
                else {
                    cb.Enabled = false;
                }

                // Tracking all template checkboxes
                templateCheckBoxes.Add(cb);

                // Prepare the string for location info
                StringBuilder locationInfo = new StringBuilder();
                locationInfo.Append(Debugger.FileSizeToString(assessedSize));
                if (item.AssessedDirectoryCount() > 0) {
                    locationInfo.AppendFormat("\n{0} dirs", item.AssessedDirectoryCount());
                }
                if (item.AssessedFileCount() > 0) {
                    locationInfo.AppendFormat("\n{0} files", item.AssessedFileCount());
                }

                // Fill the $pos row
                targetTable.Controls.Add(cb, 0, pos);
                targetTable.Controls.Add(
                    new Label() {
                        Text = locationInfo.ToString(),
                        AutoSize = true
                    },
                    1,
                    pos
                );

                targetTable.Controls.Add(
                    new TextBox() {
                        Text = item.Name(),
                        Dock = System.Windows.Forms.DockStyle.Fill,
                        BorderStyle = 0,
                        Enabled = true,
                        ReadOnly = true,
                        TabStop = false,
                        Cursor = Cursors.Arrow
                    },
                    2,
                    pos
                );
            }
            targetTable.Visible = true;
            deletionSizeText.Text = Debugger.FileSizeToString(totalSize);
        }

        protected void UI_SumUpDeletionSize() {
            if (this.templateItems == null) { return; }
            int pos = -1;
            long totalSize = 0;
            foreach (TemplateItem item in templateItems) {
                pos++;
                CheckBox cb = templateCheckBoxes[pos];
                if (cb.Checked) {
                    totalSize += item.AssessedSizeCached();
                }
            }
            deletionSizeText.Text = Debugger.FileSizeToString(totalSize);
        }

        protected void UI_DisableButtons() {
            this.assessButton.Enabled = false;
            this.cleanUpButton.Enabled = false;
            this.loadButton.Enabled = false;
            this.selectAllButton.Enabled = false;
            this.selectNoneButton.Enabled = false;
        }

        protected void UI_EnableButtons() {
            this.assessButton.Enabled = true;
            this.cleanUpButton.Enabled = true;
            this.loadButton.Enabled = true;
            this.selectAllButton.Enabled = true;
            this.selectNoneButton.Enabled = true;
        }

        protected void UI_LoadTheTemplate(string path) {
            this.templateText = this.Core_ObtainTemplateText(path);
            Debugger.Print("The template loaded ({0} chars)", this.templateText.Length);
            this.UI_AssessFilesFromTheTemplate();
            Debugger.Print(
                "The template parsed and displayed ({0} items, {1}, {2} files, {3} dirs).",
                this.templateItems.Count,
                Debugger.FileSizeToString(totalSize),
                this.totalFiles,
                this.totalDirectories
            );
        }

        protected void UI_SelectAll() {
            if (this.templateCheckBoxes == null) { return; }
            foreach (CheckBox cb in this.templateCheckBoxes) {
                if (cb.Enabled) {
                    cb.Checked = true;
                }
            }
        }

        protected void UI_SelectNone() {
            if (this.templateCheckBoxes == null) { return; }
            foreach (CheckBox cb in this.templateCheckBoxes) {
                if (cb.Enabled) {
                    cb.Checked = false;
                }
            }
        }

        protected void AssessButton_Click() {
            this.UI_DisableButtons();
            this.Update();

            this.UI_AssessFilesFromTheTemplate();

            this.UI_EnableButtons();
            this.Update();
        }

        private bool seekerModeOn = false;
        protected void UI_SeekerMode() {
            if(seekerModeOn) {
                this.seekerModeButton.Enabled = false;
                this.AssessButton_Click();
                this.seekerModeButton.Enabled = true;
                this.assessButton.Enabled = true;
            }
            else {
                this.assessButton.Enabled = false;
                this.seekerModeButton.Enabled = false;
                UI_LoadTheSeekerMode();
                this.seekerModeButton.Enabled = true;
            }
        }

        protected void UI_LoadTheSeekerMode() {
            this.UI_DisableButtons();
            this.Update();

            this.LocateTheLargestDirectories();

            this.UI_EnableButtons();
            this.Update();
        }

        protected void LocateTheLargestDirectories() {
            Debugger.Print("Seeking for the largest directories...");


            targetTable.Visible = false;

            // Clear
            targetTable.Controls.Clear();
            targetTable.RowStyles.Clear();
            targetTable.ColumnStyles.Clear();

            Dictionary<string, long> dirToSize = Core_LookUpLargeDirs();

            // Set size
            targetTable.ColumnCount = 3;
            targetTable.RowCount = dirToSize.Count + 1; // The +1 is for the header

            if (templateCheckBoxes == null) {
                templateCheckBoxes = new List<CheckBox>();
            }
            else {
                templateCheckBoxes.Clear();
            }

            // Autoscroll, please
            targetTable.AutoScroll = true;

            // Set styles
            targetTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            targetTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            targetTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            targetTable.GrowStyle = TableLayoutPanelGrowStyle.AddColumns;
            targetTable.Padding = new Padding(0, 0, 0, 0);
            targetTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset;

            // Add the header as the first row
            targetTable.Controls.Add(new Label() { Text = "Delete?" }, 0, 0);
            targetTable.Controls.Add(new Label() { Text = "Size" }, 1, 0);
            targetTable.Controls.Add(new Label() { Text = "Template path" }, 2, 0);

            totalSize = 0;
            totalFiles = 0;
            totalDirectories = 0;
            int pos = 0;
            foreach (string directory in dirToSize.Keys) {
                pos++;

                CheckBox cb = new CheckBox();
                cb.Checked = false;
                cb.Enabled = false;

                // Fill the $pos row
                targetTable.Controls.Add(cb, 0, pos);
                targetTable.Controls.Add(
                    new Label() {
                        Text = Debugger.FileSizeToString(dirToSize[directory]),
                        AutoSize = true
                    },
                    1,
                    pos
                );

                targetTable.Controls.Add(
                    new TextBox() {
                        Text = directory,
                        Dock = System.Windows.Forms.DockStyle.Fill,
                        BorderStyle = 0,
                        Enabled = true,
                        ReadOnly = true,
                        TabStop = false,
                        Cursor = Cursors.Arrow
                    },
                    2,
                    pos
                );
            }

            targetTable.Visible = true;
        }
    }
}
