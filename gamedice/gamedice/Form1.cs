using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace gamedice
{
    public partial class Form1 : Form
    {
        Engine eng = new Engine();
        public Form1()
        {
            InitializeComponent();
        }
        
        private void Form1_Shown(object sender, EventArgs e)
        {
            eng._Spawn(true);
        }
    }
}
