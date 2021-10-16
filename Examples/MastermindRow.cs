using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Examples
{
    public partial class MastermindRow : UserControl
    {
        public MastermindRow()
        {
            InitializeComponent();
        }

        private void MastermindRow_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = colours.ToList();
            comboBox2.DataSource = colours.ToList();
            comboBox3.DataSource = colours.ToList();
            comboBox4.DataSource = colours.ToList();
        }

        public static readonly Color[] colours = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Purple, Color.Pink, Color.White, Color.Black };

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox1.BackColor = colours[comboBox1.SelectedIndex];
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox2.BackColor = colours[comboBox2.SelectedIndex];
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox3.BackColor = colours[comboBox3.SelectedIndex];
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            pictureBox4.BackColor = colours[comboBox4.SelectedIndex];
        }

        public event EventHandler MakeGuess;

        private void button1_Click(object sender, EventArgs e)
        {
            MakeGuess?.Invoke(this, e);
        }

        public void SendFeedback(int correct, int incorrect)
        {
            label1.Text = $"{correct} are in the correct position";
            label2.Text = $"{incorrect} are correct but in the wrong position";
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            comboBox3.Enabled = false;
            comboBox4.Enabled = false;
            button1.Visible = false;
        }

        public Color[] Guess => new Color[] {
            colours[comboBox1.SelectedIndex], 
            colours[comboBox2.SelectedIndex], 
            colours[comboBox3.SelectedIndex],
            colours[comboBox4.SelectedIndex],
        };
    }
}
