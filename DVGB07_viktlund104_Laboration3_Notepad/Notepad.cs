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

namespace DVGB07_viktlund104_Laboration3_Notepad
{
	public partial class Notepad : Form
	{
		// Tools
		private OpenFileDialog load;
		private SaveFileDialog save;
		private bool isSaved;
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

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// Check if file is saved and we are not on a fresh start
			if (isSaved == false && startup == false)
			{
				var result = MessageBox.Show("Vill du spara först?", "Notepad", MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Stop);
				if (result == DialogResult.Yes)
				{
					SaveAs(); // Send to save method
				}
				else if (result == DialogResult.No)
				{
					Clear(); // Clear
				}
			}
			else
			{
				// File was already saved, clear
				Clear();
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (isSaved == false && startup == false)
			{
				var result = MessageBox.Show("Vill du spara först?", "Notepad", MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Stop);
				if (result == DialogResult.Yes)
				{
					SaveAs(); // Send to save method
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

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// first check for the * sign. remove it
			string title = this.Text;
			
			if (title[0].Equals('*'))
			{
				title = title.Substring(1);
			}
			
			// if this statement is true, we have opened an existing file or have already saved an file
			// so we just wanna write to the same path again
			if (title == Path.GetFileName(filePathExists))
			{
				QuickSave();
			}
			// In this case, we have to open the save as dialog
			else
			{
				SaveAs();
			}
		}
		
		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit(); // To force trigger FormClosing event
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Notepad v1.0\nFree to use\nBy Viktor Lundberg\n2022-02-24", "About", MessageBoxButtons.OK,
				MessageBoxIcon.Information);
		}

		private void SaveAs()
		{
			save = new SaveFileDialog();
			save.Filter = "Text Files (*.txt)|*.txt|All Files (*)|*";
			var result = save.ShowDialog();

			if (result == DialogResult.OK)
			{
				try
				{
					File.WriteAllText(save.FileName, richTextBox.Text);
					this.Text = Path.GetFileName(save.FileName);
					filePathExists = save.FileName;
					isSaved = true;
				}
				catch (Exception ex)
				{
					MessageBox.Show($"$Error: {ex.Message}", "Error", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}
		}
		
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
				MessageBox.Show($"$Error: {ex.Message}", "Error", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
			}
		}

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
					filePathExists = load.FileName;
					isSaved = true;
				}
				catch (Exception ex)
				{
					MessageBox.Show($"$Error: {ex.Message}", "Error", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}
		}

		private void Clear()
		{
			// Clear text and set default title
			richTextBox.Text = "";
			this.Text = "Namnlös";
			isSaved = false;
			startup = true;
		}
		
		// Checking for changes in the textbox. Has the user wrote anything?
		private void richTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			//isSaved = false;

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
			Exit();
		}

		private void Exit()
		{
			if (isSaved == false && startup == false)
			{
				var result = MessageBox.Show("Vill du spara först?", "Notepad", MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Stop);
				if (result == DialogResult.Yes)
				{
					// first check for the * sign. remove it
					string title = this.Text;
			
					if (title[0].Equals('*'))
					{
						title = title.Substring(1);
					}
					
					// Send to save
					if (title == Path.GetFileName(filePathExists))
					{
						QuickSave();
					}
					else
					{
						SaveAs();
					}
				}
			}
		}

	}
}