using System;
using System.Windows.Forms;
using Modbus.IntegrationTests;

namespace Test_Compact_Framework
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void serialButton_Click(object sender, EventArgs e)
		{
			ExecuteTest(TestCases.Serial);			
		}

		private void udpButton_Click(object sender, EventArgs e)
		{
			ExecuteTest(TestCases.Udp);
		}

		private void tcpButton_Click(object sender, EventArgs e)
		{
			ExecuteTest(TestCases.Tcp);
		}

		private void ExecuteTest(Action test)
		{
			try
			{
				textBox1.Text = "Executing test...";
				test.Invoke();
				textBox1.Text = "Success.";
			}
			catch (Exception e)
			{
				textBox1.Text = e.ToString();
			}			
		}		
	}
}