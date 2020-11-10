using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace SerialPort
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            cmdClose.Enabled = false;
            foreach (String s in System.IO.Ports.SerialPort.GetPortNames()) 
            {
                txtPort.Items.Add(s);
            }
            cmbparity.Text = "None";
            cmbdatabits.Text = "8";
            cmbstopbits.Text = "One";
        }


        public System.IO.Ports.SerialPort sport;
        bool connected = false;


        public void txtReceiveAppendText(String input, byte withSent, byte withReceived)
        {
            DateTime dt = DateTime.Now;
            String dtn = dt.ToShortTimeString();

            if (withSent == 1 && withReceived == 0)

            {txtReceive.Text += ("[" + dtn + "] " + "Sent: " + input); txtReceive.Text += "\r\n";}

            else if(withSent == 0 && withReceived == 1)
            {txtReceive.Text += ("[" + dtn + "] " + "Received: " + input); txtReceive.Text += "\r\n"; }

            else
            {txtReceive.Text += ("[" + dtn + "] " + input); txtReceive.Text += "\r\n"; }

            txtReceive.SelectionStart = txtReceive.Text.Length;
            txtReceive.ScrollToCaret();
        }

        private void txtDatatoSend_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (connected)
                {
                    String data = txtDatatoSend.Text;
                    sport.Write(data);
                    sport.Write("\r");
                    txtReceiveAppendText(data, 1, 0);
                }
                else
                {
                    txtReceiveAppendText("Error Not-Connected", 0, 0);
                }
            }
        }

        public void serialport_connect(String port, int baudrate , Parity parity, int databits, StopBits stopbits) 
        {
            sport = new System.IO.Ports.SerialPort(
            port, baudrate, parity, databits, stopbits);
            try
            {
                sport.Open();
                cmdClose.Enabled = true;
                cmdConnect.Enabled = false;
                txtReceiveAppendText("Connected", 0, 0);
                sport.DataReceived += new SerialDataReceivedEventHandler(sport_DataReceived);
                connected = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "Error"); connected = false; }
        }

        char LF = (char)10;
        StringBuilder sb = new StringBuilder();

        private void sport_DataReceived(object sender, SerialDataReceivedEventArgs e) 
        {
            string data = "";
            System.Threading.Thread.Sleep(250);

            while (sport.BytesToRead > 0)
            {
                data += sport.ReadExisting();
               
            }
            //string data = sport.ReadExisting();
            txtReceiveAppendText(data, 0, 1);


        }

        private void cmdConnect_Click(object sender, EventArgs e)
        {
            try
            {
                String port = txtPort.Text;
                int baudrate = Convert.ToInt32(cmbbaudrate.Text);
                Parity parity = (Parity)Enum.Parse(typeof(Parity), cmbparity.Text);
                int databits = Convert.ToInt32(cmbdatabits.Text);
                StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), cmbstopbits.Text);
                serialport_connect(port, baudrate, parity, databits, stopbits);
            }
            catch (Exception)
            {MessageBox.Show("Zu wenige angaben"); }
            
            
        }

        private void cmdSend_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                String data = txtDatatoSend.Text;
                sport.Write(data);
                sport.Write("\r");
                txtReceiveAppendText(data, 1, 0);
            }
            else
            {
                txtReceiveAppendText("Error Not-Connected", 0, 0);
            }
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            String dtn = dt.ToShortTimeString();

            if (sport.IsOpen) 
            {
                sport.Close();
                cmdClose.Enabled = false;
                cmdConnect.Enabled = true;
                txtReceiveAppendText("Disconnected", 0, 0);
            }
            connected = false;
        }
    }
}
