using PS3FileSystem;
using System;
using System.IO;
using System.Windows.Forms;

namespace GSecPs3Decrypter
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();						
		}

        private void button1_Click(object sender, EventArgs e)
        {
			if (this.fbd.ShowDialog() == DialogResult.OK)
			{
				this.textBox1.Text = this.fbd.SelectedPath;
				main1();
			}
		}

		public Ps3SaveManager ps3SaveManager;

		private void button2_Click(object sender, EventArgs e)
		{
			if(this.button2.Text == "Encrypt")
			{
				this.listView1.Items.Clear();
				this.SetStatus(ps3SaveManager.EncryptAllFiles().ToString() + " Files Encrypted");
				File.Delete(this.dirpath + "\\\\~decrypted.txt");
				for (int i = 0; i <= ps3SaveManager.Param_PFD.Entries.Length - 1; i++)
				{
					this.listView1.Items.Add("[🔒] " + ps3SaveManager.Param_PFD.Entries[i].file_name);
				}
				this.button2.Text = "Decrypt";
				this.pictureBox1.Image = global::GSecPs3Decrypter.Properties.Resources._lock;
			}
			else if(this.button2.Text == "Decrypt")
			{
				this.listView1.Items.Clear();
				var f1 = ps3SaveManager.DecryptAllFiles();
				this.SetStatus(f1.ToString() + " Files Decrypted");
				if (f1 == 0) { Console.WriteLine("zero"); return; }
				string text = "";
				for (int i = 0; i <= ps3SaveManager.Param_PFD.Entries.Length - 1; i++)
				{
					text = text + " " + ps3SaveManager.Param_PFD.Entries[i].file_name;
					this.listView1.Items.Add("[✓] " + ps3SaveManager.Param_PFD.Entries[i].file_name);
				}
				File.WriteAllText(this.dirpath + "\\\\~decrypted.txt", text);
				this.button2.Text = "Encrypt";
				this.pictureBox1.Image = global::GSecPs3Decrypter.Properties.Resources.unlock;
			}
		}

		public void SetStatus(string text)
		{
			this.textBox3.Text = text;
		}

		public FolderBrowserDialog fbd = new FolderBrowserDialog();

		// Token: 0x0400003F RID: 63
		private string dirpath;

		// Token: 0x04000040 RID: 64
		public byte[] key = null;/* new byte[]
		{
			202,
			239,
			174,
			185,
			39,
			34,
			167,
			203,
			83,
			93,
			82,
			135,
			34,
			250,
			104,
			221
		};*/

		private void Form1_Load(object sender, EventArgs e)
		{
			this.textBox1.KeyDown += textBox1_KeyDown;
		}

		string tid0;

		public static SecureFileInfo[] GameConfigList = Functions.DownloadAldosGameConfig();

		private void main1()
		{
			this.listView1.Items.Clear();
			this.dirpath = this.textBox1.Text;
			this.fbd.SelectedPath = this.textBox1.Text;

			this.button2.Visible = true;
			this.button2.Enabled = true;
			string state;
			if (File.Exists(this.dirpath + "\\\\~decrypted.txt"))
			{
				this.button2.Text = "Encrypt";
				state = "✓";
				this.pictureBox1.Image = global::GSecPs3Decrypter.Properties.Resources.unlock;
			}
			else
			{
				this.button2.Text = "Decrypt";
				state = "🔒";
				this.pictureBox1.Image = global::GSecPs3Decrypter.Properties.Resources._lock;
			}
			try
			{
				ps3SaveManager = new Ps3SaveManager(this.textBox1.Text);
				for (int i = 0; i <= ps3SaveManager.Param_PFD.Entries.Length - 1; i++)
				{
					this.listView1.Items.Add("[" + state + "] " + ps3SaveManager.Param_PFD.Entries[i].file_name);
				}
				this.SetStatus("Loaded directory");

				var tid1 = ps3SaveManager.Param_SFO.Title;
				if (tid0 != tid1) key = ps3SaveManager.GetSecureFileIdFromConfigList(GameConfigList);
				else Console.WriteLine("same title & key");
				tid0 = tid1;
				//ps3SaveManager.Param_PFD.SecureFileID = key;
				ps3SaveManager.setKey(key);
			}
			catch (Exception z)
			{
				this.listView1.Items.Clear();
				this.button2.Visible = false;
				this.button2.Enabled = false;
				this.pictureBox1.Image = global::GSecPs3Decrypter.Properties.Resources.icon;
				this.SetStatus(z.Message);
			}
		}

		private void textBox1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				if (Directory.Exists(this.textBox1.Text))
				{
					main1();
				}
				else
				{
					this.pictureBox1.Image = global::GSecPs3Decrypter.Properties.Resources.icon;
					this.listView1.Items.Clear();
					this.SetStatus("Directory does not exist");
				}
					
			}
		}
	}
}
