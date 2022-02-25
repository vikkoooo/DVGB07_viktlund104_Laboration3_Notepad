using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

/*
 * Simple notepad application.
 * Created for course: DVGB07, Laboration 3
 * By: Viktor Lundberg
 * 2022-02-25
 */
namespace DVGB07_viktlund104_Laboration3_Notepad
{
	public partial class Notepad : Form
	{
		// Tools
		private OpenFileDialog load;
		private SaveFileDialog save;
		private bool isSaved; // tracking save status
		private bool startup; // possible to start application and instantly open file without YesNoCancel popup
		private string filePathExists; // to allow quicksave

		// Constructor
		public Notepad()
		{
			InitializeComponent();
			richTextBox.Font = new Font("Consolas", 11, FontStyle.Regular); // Set font on launch
			isSaved = false; // initialize
			startup = true; // first start
			filePathExists = ""; // initialize to avoid null error when comparing
		}

		// "Arkiv -> Ny" button (hotkey CTRL + N)
		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Check if file is saved and and if we are on a fresh start
			if (isSaved == false && startup == false)
			{
				// Ask user to save first
				var result = MessageBox.Show("Vill du spara först?", "Notepad", MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Stop);

				// Handle user response
				if (result == DialogResult.Yes)
				{
					AttemptSave();
					Clear();
				}
				else if (result == DialogResult.No)
				{
					Clear();
				}
			}
			else
			{
				Clear(); // File was already saved
			}
		}

		// "Arkiv -> Öppna" button (hotkey CTRL + O)
		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (isSaved == false && startup == false)
			{
				var result = MessageBox.Show("Vill du spara först?", "Notepad", MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Stop);

				if (result == DialogResult.Yes)
				{
					AttemptSave();
					Open();
				}
				else if (result == DialogResult.No)
				{
					Open(); // Send to open method
				}
			}
			else
			{
				Open();
			}
		}

		// "Arkiv -> Spara" button (hotkey CTRL + S)
		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			AttemptSave();
		}

		// "Arkiv -> Spara som" button (hotkey CTRL + SHIFT + S)
		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(); // Will skip quicksave function, sends user straight to save as method
		}

		// "Arkiv -> Avsluta" button (hotkey CTRL + SHIFT + S)
		// This function will trigger FormClosing event. This way, we get same functionality with using the "Avsluta"
		// button as well as X on the right top corner
		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit(); // Force trigger FormClosing event
		}

		// "Hjälp -> Om Notepad" button. Information about the creator. 
		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Notepad v1.0\nFree to use\nBy Viktor Lundberg\nCreated 2022-02-25", "About",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}

		// This method will check whether we are running a new file or an already existing file.
		// If it is an already existing file, it will quick save into the old file.
		// If it is a new file, it will ask you to choose a path and filename to save. 
		private void AttemptSave()
		{
			// First check for the '*' sign. Create substring without the '*'
			string title = this.Text;

			if (title[0].Equals('*'))
			{
				title = title.Substring(1);
			}

			// If this statement is true, we are running an already existing file
			if (title == Path.GetFileName(filePathExists))
			{
				QuickSave();
			}
			// Otherwise, we have to open the "save as" dialog
			else
			{
				SaveAs();
			}
		}

		// "Save as" function. Uses SaveFileDialog and File.WriteAllText as writer. 
		private void SaveAs()
		{
			save = new SaveFileDialog(); // Open new SaveFileDialog
			save.Filter = "Text Files (*.txt)|*.txt|All Files (*)|*";
			var result = save.ShowDialog(); // Store response

			// Handle user response
			if (result == DialogResult.OK)
			{
				try
				{
					File.WriteAllText(save.FileName, richTextBox.Text);
					this.Text = Path.GetFileName(save.FileName); // Set title to the filename (without path)
					isSaved = true; // Update to saved state
					filePathExists = save.FileName; // Store the name in filePathExists for future references
				}
				catch (Exception ex)
				{
					// Dummy handling of exception, just in case something goes wrong with the writer.
					MessageBox.Show($"Error: {ex.Message}\nIf the issue persists, contact system administrator.",
						"Error", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}
		}

		// Save into already existing file function. No need to get a new file name, we already know it.
		private void QuickSave()
		{
			try
			{
				File.WriteAllText(filePathExists, richTextBox.Text);
				this.Text = Path.GetFileName(filePathExists);
				isSaved = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error: {ex.Message}\nIf the issue persists, contact system administrator.", "Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

		// Open a new file method. Will use OpenFileDialog and File.ReadAllText to load the data. 
		private void Open()
		{
			load = new OpenFileDialog();
			load.Filter = "Text Files (*.txt)|*.txt|All Files (*)|*";
			var result = load.ShowDialog();

			if (result == DialogResult.OK)
			{
				try
				{
					richTextBox.Text = File.ReadAllText(load.FileName);
					this.Text = Path.GetFileName(load.FileName);
					isSaved = true;
					filePathExists = load.FileName;
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Error: {ex.Message}\nIf the issue persists, contact system administrator.",
						"Error", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}
		}

		// Clear text and set default title
		private void Clear()
		{
			richTextBox.Text = "";
			this.Text = "Namnlös";
			isSaved = false; // No saved mode
			startup = true; // Simulate new startup
		}

		// Checking for changes in the textbox. Has the user wrote anything yet?
		// Sets a '*' in front of file name to show that file has been edited.
		private void richTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (startup == true)
			{
				this.Text = "*" + this.Text;
				startup = false;
			}

			if (isSaved == true)
			{
				this.Text = "*" + this.Text;
				isSaved = false;
			}
		}

		// Using this event we will catch user clicks off the X button as well as menu exits
		private void Notepad_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (isSaved == false && startup == false)
			{
				var result = MessageBox.Show("Vill du spara först?", "Notepad", MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Stop);
				
				if (result == DialogResult.Yes)
				{
					AttemptSave();
				}
			}
		}

	}
}