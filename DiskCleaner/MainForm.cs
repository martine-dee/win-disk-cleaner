using System;
using System.IO;
using System.Windows.Forms;

namespace DiskCleaner {
    public partial class MainForm : Form {

        // The C-Tor
        public MainForm() {
            // Generic
            InitializeComponent();
            // Custom
            UI_InitializeComponent();
        }

        override protected void OnShown(EventArgs eventArgs) {
            base.OnShown(eventArgs);

            // If the default file path is present, load it at start-up
            // This should save time users who are already settled
            string defaultFilePath = @"DeletionTemplate.txt";
            if (File.Exists(defaultFilePath)) {
                loadingTextLabel.Visible = true;
                UI_DisableButtons();
                this.Update();

                // Load the template
                UI_LoadTheTemplate(defaultFilePath);
                
                loadingTextLabel.Visible = false;
                UI_EnableButtons();
                this.Update();
            }
        }
        
        private void CheckBox_CheckedChanged(Object sender, EventArgs e) {
            this.UI_SumUpDeletionSize();
        }

        private void assessButton_Click(object sender, EventArgs e) {
            this.AssessButton_Click();
        }

        private void cleanUpButton_Click(object sender, EventArgs e) {
            // If there is nothing to do, tell that to the user and return.
            if(templateItems == null || templateItems.Count == 0) {
                Debugger.Print("Sorry, nothing to do. No file is selected for deletion.");
                return;
            }

            this.UI_DisableButtons();
            DialogResult dialogResult = MessageBox.Show(
                "Are you sure you want to delete the listed files?\nYou proceed at your own responsibility.\n\nClick No to ABORT. Click Yes to DELETE the files and directories.",
                "You are about to delete Files and Directories from your System.",
                MessageBoxButtons.YesNo
            );
            if (dialogResult == DialogResult.Yes) {
                this.UI_DeleteAllSelectedItems();
            }
            else {
                Debugger.Print("The Clean-Up aborted!\n");
            }
            this.UI_EnableButtons();
        }

        private void loadButton_Click(object sender, EventArgs e) {
            this.UI_DisableButtons();
            System.Windows.Forms.OpenFileDialog selectFileDialog;
            selectFileDialog = new System.Windows.Forms.OpenFileDialog();
            selectFileDialog.DefaultExt = "txt";
            DialogResult dialogResult = selectFileDialog.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK && selectFileDialog.FileNames.Length > 0) {
                string path = selectFileDialog.FileNames[0];
                loadingTextLabel.Visible = true;
                this.Update();
                UI_LoadTheTemplate(path);
                loadingTextLabel.Visible = false;
            }
            this.UI_EnableButtons();
        }

        private void selectAllButton_Click(object sender, EventArgs e) {
            UI_SelectAll();
            UI_SumUpDeletionSize();
        }

        private void selectNoneButton_Click(object sender, EventArgs e) {
            UI_SelectNone();
            UI_SumUpDeletionSize();
        }

        private void seekerMode_Click(object sender, EventArgs e) {
            UI_SeekerMode();
        }
    }
}
