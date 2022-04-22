using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FFETOOLS
{
    public partial class CreateWallForm : Form
    {
        WallCreater m_Creater;
        int m_ClickCount = 0;
        Autodesk.Revit.DB.Document document = null;

        public CreateWallForm(WallCreater creater, Autodesk.Revit.DB.Document doc)
        {
            m_Creater = creater;
            document = doc;
            InitializeComponent();
            InitialezeProjectInfos();
        }

        protected void InitialezeProjectInfos()
        {
            List<string> wallTypeInfos = m_Creater.WallTypeInfos;
            foreach (string infos in wallTypeInfos)
            {
                TreeNode familyNode = new TreeNode(infos);
                treeViewWallType.Nodes.Add(familyNode);
                familyNode.ExpandAll();
            }

            List<string> levelInfos = m_Creater.LevelInfos;
            foreach (string levelName in levelInfos)
            {
                cmbTopLevel.Items.Add(levelName);
                cmbBottomLevel.Items.Add(levelName);
            }
            cmbBottomLevel.SelectedIndex = 0;
            cmbTopLevel.SelectedIndex = 1;
            cmbOffset.Text = "0";
        }

        private void btnPointSel_Click(object sender, EventArgs e)
        {
            TreeNode selNode = null;
            if (!SelectedWallType(ref selNode))
                return;
            int wallTypeIndex = selNode.Index;
            int topLevelIndex = cmbTopLevel.SelectedIndex;
            int bottomLevelIndex = cmbBottomLevel.SelectedIndex;
            double offset = System.Convert.ToDouble(cmbOffset.Text);

            this.Hide();

            string transeformName = "select a segement on grid" + m_ClickCount++.ToString();
            m_Creater.AddWallByGridSegement(transeformName, wallTypeIndex, topLevelIndex, bottomLevelIndex,
                offset, chkbSegmentation.Checked, rbStructure.Checked);

            this.Show();
        }

        private void btnLineSel_Click(object sender, EventArgs e)
        {
            TreeNode selNode = null;
            if (!SelectedWallType(ref selNode))
                return;
            int wallTypeIndex = selNode.Index;
            int topLevelIndex = cmbTopLevel.SelectedIndex;
            int bottomLevelIndex = cmbBottomLevel.SelectedIndex;
            double offset = System.Convert.ToDouble(cmbOffset.Text);

            this.Hide();

            string transeformName = "select a grid" + m_ClickCount++.ToString();
            m_Creater.AddWallBySingleGrid(transeformName, wallTypeIndex, topLevelIndex, bottomLevelIndex,
                offset, chkbSegmentation.Checked, rbStructure.Checked);

            this.Show();
        }

        private void btnCrossSel_Click(object sender, EventArgs e)
        {
            TreeNode selNode = null;
            if (!SelectedWallType(ref selNode))
                return;
            int wallTypeIndex = selNode.Index;
            int topLevelIndex = cmbTopLevel.SelectedIndex;
            int bottomLevelIndex = cmbBottomLevel.SelectedIndex;
            double offset = System.Convert.ToDouble(cmbOffset.Text);

            this.Hide();

            string transeformName = "cross grids" + m_ClickCount++.ToString();
            m_Creater.AddWallByCrossGrids(document, transeformName, wallTypeIndex, topLevelIndex, bottomLevelIndex,
                offset, chkbSegmentation.Checked, rbStructure.Checked);

            this.Show();
        }

        private bool SelectedWallType(ref TreeNode selNode)
        {
            selNode = treeViewWallType.SelectedNode;
            if (null == selNode)
            {
                MessageBox.Show("not select type");
                return false;
            }
            return true;
        }
    }
}
