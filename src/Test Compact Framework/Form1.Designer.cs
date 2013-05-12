namespace Test_Compact_Framework
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.MainMenu mainMenu1;

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
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.serialButton = new System.Windows.Forms.Button();
			this.udpButton = new System.Windows.Forms.Button();
			this.tcpButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Location = new System.Drawing.Point(0, 0);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(638, 23);
			this.textBox1.TabIndex = 0;
			// 
			// serialButton
			// 
			this.serialButton.Location = new System.Drawing.Point(3, 72);
			this.serialButton.Name = "serialButton";
			this.serialButton.Size = new System.Drawing.Size(72, 20);
			this.serialButton.TabIndex = 1;
			this.serialButton.Text = "Serial";
			this.serialButton.Click += new System.EventHandler(this.serialButton_Click);
			// 
			// udpButton
			// 
			this.udpButton.Location = new System.Drawing.Point(81, 72);
			this.udpButton.Name = "udpButton";
			this.udpButton.Size = new System.Drawing.Size(72, 20);
			this.udpButton.TabIndex = 2;
			this.udpButton.Text = "UDP";
			this.udpButton.Click += new System.EventHandler(this.udpButton_Click);
			// 
			// tcpButton
			// 
			this.tcpButton.Location = new System.Drawing.Point(159, 72);
			this.tcpButton.Name = "tcpButton";
			this.tcpButton.Size = new System.Drawing.Size(72, 20);
			this.tcpButton.TabIndex = 3;
			this.tcpButton.Text = "TCP";
			this.tcpButton.Click += new System.EventHandler(this.tcpButton_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(638, 455);
			this.Controls.Add(this.tcpButton);
			this.Controls.Add(this.udpButton);
			this.Controls.Add(this.serialButton);
			this.Controls.Add(this.textBox1);
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button serialButton;
		private System.Windows.Forms.Button udpButton;
		private System.Windows.Forms.Button tcpButton;
	}
}

