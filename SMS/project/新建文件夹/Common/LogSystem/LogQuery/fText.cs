using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LogQuery
{
    public partial class fText : Form
    {
        public fText(string text)
        {
            InitializeComponent();
            this.txt.Text = text;
        }

        private void fText_Load(object sender, EventArgs e)
        {

        }
    }
}
