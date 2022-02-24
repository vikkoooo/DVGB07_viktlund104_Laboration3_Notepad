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
					SaveAs(false); // Send to save method
				}
				else if (result == DialogResult.No)
				{
					Clear(); // Clear
				}
				else
				{
					return; // Cancel, we don't wanna run rest of the code
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
					SaveAs(false); // Send to save method
				}
				else if (result == DialogResult.No)
				{
					Open(); // Send to open method
				}
				else
				{
					return; // Cancel, we don't wanna run rest of the code
				}
			}
			else
			{
				Open();
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			// if this statement is true, we have opened an existing file or have already saved an file
			// so we just wanna write to the same path again
			if (this.Text == Path.GetFileName(filePathExists))
			{
				QuickSave(false);
			}
			// In this case, we have to open the save as dialog
			else
			{
				SaveAs(false);
			}
		}
		


		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs(false);
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

		private void SaveAs(bool quitSave)
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
					statusBar.Text = isSaved.ToString();

					if (quitSave)
					{
						Application.Exit();
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show($"$Error: {ex.Message}", "Error", MessageBoxButtons.OK,
						MessageBoxIcon.Error);
				}
			}
		}
		
		private void QuickSave(bool quitSave)
		{
			try
			{
				File.WriteAllText(filePathExists, richTextBox.Text);
				isSaved = true;
				statusBar.Text = isSaved.ToString();

				if (quitSave)
				{
					Application.Exit();
				}
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


		private void richTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			isSaved = false;
			startup = false;
			statusBar.Text = isSaved.ToString();
		}

		private void Exit()
		{
			if (isSaved == false && startup == false)
			{
				var result = MessageBox.Show("Vill du spara först?", "Notepad", MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Stop);
				if (result == DialogResult.Yes)
				{
					// Send to save
					if (this.Text == Path.GetFileName(filePathExists))
					{
						QuickSave(true);
					}
					else
					{
						SaveAs(true);
					}
				}
				else if (result == DialogResult.No)
				{
					Application.Exit();
				}
				else
				{
					return; // Cancel, we don't wanna run rest of the code
				}
			}
			else
			{
				Application.Exit();
			}
		}

		// Using this event we will catch user clicks off the X button as well as menu exits
		private void Notepad_FormClosing(object sender, FormClosingEventArgs e)
		{
			Exit();
		}
	}
}