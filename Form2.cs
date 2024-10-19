using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineCleaner
{
    public partial class SettingsMenu : Form
    {
        public int mineLimit;
        public int fieldSize;

        public SettingsMenu()
        {
            InitializeComponent();

            MineCleaner mineCleaner = new MineCleaner();
            domainUpDown1.Text = mineCleaner.fieldSize.ToString();
            domainUpDown2.Text = mineCleaner.mineLimit.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                fieldSize = Int32.Parse(domainUpDown1.Text);
                mineLimit = Int32.Parse(domainUpDown2.Text);
            }
            catch
            {
                return;
            }

            if (fieldSize < 8 || fieldSize > 64)
            {
                MessageBox.Show("Invalid field size!");
                return;
            }
            else if (mineLimit >= fieldSize * fieldSize)
            {
                MessageBox.Show("Invalid mine number!");
                return;
            }
            else
            {
                this.Close();
            }
        }

        private void SettingsMenu_Load(object sender, EventArgs e)
        {
            for(int i = 64; i >= 8; i--)
            {
                domainUpDown1.Items.Add(i);
                domainUpDown1.SelectedIndex = domainUpDown1.Items.Count - 1;
            }
            for (int i = 64 * 64; i >= 1; i--)
            {
                domainUpDown2.Items.Add(i);
                domainUpDown2.SelectedIndex = domainUpDown2.Items.Count - 1;
            }
        }
    }
}
