using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ButtonSynchronizationWithAwait
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OnClickWrapper("I Hate Button 1", 3000);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OnClickWrapper("I am from Button 2", 2000);
        }
        
        async void OnClickWrapper(string message, int delay)
        {
            OnClick(message, delay); 
        }

        async Task OnClick(string message , int delay)
        {
            await Task.Delay(delay);
            label1.Text = message;
        }

    }
}
