using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VNoteReader
{
    public partial class Form1 : Form
    {
        private List<Dictionary<string, string>> analyzeResult;

        public Form1()
        {
            InitializeComponent();
        }

        private void 読み込みLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Execute(openFileDialog1.FileName);
            }
        }

        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            Execute(s[0]);
        }

        private void Execute(string filePath)
        {
            analyzeResult = VNoteUtil.AnalyzeFile(filePath);
            foreach(var x in analyzeResult)
            {
                listView1.Items.Add(x["TEXT"].Substring(0, 10));
            }
            ShowText(0);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedIndices.Count == 0)
            {
                ClearText();
            }
            else
            {
                ShowText(listView1.SelectedIndices[0]);
            }
            
        }

        private void ShowText(int i)
        {
            textBox1.Text = analyzeResult[i]["TEXT"].Replace("\n", "\r\n");
        }

        private void ClearText()
        {
            textBox1.Text = "";
        }


    }
}
