using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Droid_trading.View.Trading
{
    // test with top option web site, gonna be deported in separate module like web screen manager
    public partial class DemoFirefox : Form
    {
        public DemoFirefox()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.OpenFirefox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.CloseFirefox();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.SetTrade5();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.SetTrade10();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.SetTrade25();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.SetTrade50();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.SetTrade100();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.SetTrade250();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.SetTrade500();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.SetTrade1000();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.TradeUp();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            TopOptionFirefox.TradeDown();
        }
    }
}
