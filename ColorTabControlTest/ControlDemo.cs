using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ColorTabControlTest
{
    public partial class ControlDemo : Form
    {
        public ControlDemo()
        {
            InitializeComponent();
        }
        public ControlDemo(string Style)
        {
            InitializeComponent();
            if (Style == "Windows")
                CTabControl.SelectedTab = tabOne;
            else
                CTabControl.SelectedTab = tabThree;
        }

    }
}