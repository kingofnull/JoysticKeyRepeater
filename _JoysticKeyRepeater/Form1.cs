using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Threading;
using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;



namespace JoysticKeyRepeater
{
    public partial class Form1 : Form
    {
        ViGEmClient client = new ViGEmClient();
        IDualShock4Controller controller;

        public Form1()
        {
            InitializeComponent();
            controller=  client.CreateDualShock4Controller();
        }

        

        private void timer1_Tick(object sender, EventArgs e)
        {
          
        }

        Task mt;
        private void Form1_Load(object sender, EventArgs e)
        {
            controller.Connect();
            mt=Task.Run(() =>
            {
                while (true) {
                    //true or false
                    controller.SetButtonState(DualShock4Button.Circle, true);
                    if (IsHandleCreated) this.Invoke((MethodInvoker)delegate
                    {
                        BackColor = Color.Red;
                    });

                    Thread.Sleep(500);
                    controller.SetButtonState(DualShock4Button.Circle, false);
                    if(IsHandleCreated) this.Invoke((MethodInvoker)delegate
                    {
                        BackColor = Color.White;
                    });
                    Thread.Sleep(500);
                }
               
            });
           
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            controller.Disconnect();

        }
    }
}
