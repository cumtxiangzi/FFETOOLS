using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Autodesk.Revit;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace FFETOOLS
{
    public partial class AxisNameForm : System.Windows.Forms.Form
    {
        private Autodesk.Revit.DB.Document m_Doc = null;
        public string NewName { get; set; }

        public AxisNameForm(Autodesk.Revit.DB.Document doc)
        {
            InitializeComponent();
            m_Doc = doc;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            NewName = this.textBoxName.Text;
        }

        private void AxisNameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Grid findGrid = null;
            if (Common.isDuplicationName(m_Doc, this.textBoxName.Text, ref findGrid))
            {
                MessageBox.Show("轴线重名，请重新填写。");
                e.Cancel = true;
            }            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.textBoxName.Text = "";
        }


    }
}
