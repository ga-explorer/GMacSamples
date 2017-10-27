using System;
using System.IO;
using System.Windows.Forms;
using TextComposerLib.Files.UI;

namespace BladesLibraryComposer
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            InitializeInterface();
        }

        private void InitializeInterface()
        {
            for (var i = 0; i < LibraryComposerFactory.GMacDslCodeList.Count; i++)
                comboBoxGMacDslCode.Items.Add($"Code Sample {i + 1}");

            if (comboBoxGMacDslCode.Items.Count > 0)
                comboBoxGMacDslCode.SelectedIndex = 0;

            comboBoxTargetLanguage.Items.Add("C#");
            comboBoxTargetLanguage.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxOutputFolder.Text))
            {
                MessageBox.Show(@"Please select a valid output folder first", @"", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            toolStripStatusLabel1.Text = @"Composing Code...";

            var dslCode = LibraryComposerFactory.GMacDslCodeList[comboBoxGMacDslCode.SelectedIndex];

            var filesComposer = 
                LibraryComposerFactory.ComposeLibrary(
                    dslCode,
                    textBoxOutputFolder.Text,
                    checkBoxGenerateMacroCode.Checked,
                    comboBoxTargetLanguage.Text
                    );

            toolStripStatusLabel1.Text = @"Ready";

            if (filesComposer == null) return;

            //MessageBox.Show(@"Target code files saved successfully", @"Files Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //Show final generated files and read their contents from disk
            var formFiles = new FormFilesComposer(filesComposer);
            formFiles.ShowDialog(this);
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.Cancel)
                return;

            textBoxOutputFolder.Text = folderBrowserDialog1.SelectedPath;
        }

        private void comboBoxGMacDslCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGMacDslCode.SelectedIndex >= 0)
                textBoxGMacDslCode.Text = 
                    LibraryComposerFactory.GMacDslCodeList[comboBoxGMacDslCode.SelectedIndex];
        }
    }
}
