using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace VNoteReader
{
    /// <summary>
    /// フォームクラス
    /// </summary>
    public partial class Form1 : Form
    {
        #region フィールド

        /// <summary>
        /// 読込結果
        /// </summary>
        private List<Dictionary<string, string>> analyzeResult;

        #endregion

        #region 初期処理

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        #endregion

        #region メニューバーイベント

        /// <summary>
        /// ファイル - 読み込み
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void 読み込みLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Read(openFileDialog1.FileName);
            }
        }

        /// <summary>
        /// ファイル - エクスポート
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void エクスポートEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (analyzeResult == null)
            {
                MessageBox.Show(this, "先にVNoteファイルを読み込んでください。", "エラー");
                return;
            }

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Export(saveFileDialog1.FileName);
            }
        }

        /// <summary>
        /// ファイル - 設定
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void 設定SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting();
        }

        /// <summary>
        /// ファイル - 終了
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void 終了XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Quit();
        }

        #endregion

        #region イベント

        /// <summary>
        /// メモリストドラッグエンター時処理
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// メモリストドラッグドロップ時処理
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            Read(s[0]);
        }

        /// <summary>
        /// メモリスト選択変更時処理
        /// </summary>
        /// <param name="sender">イベント発生オブジェクト</param>
        /// <param name="e">イベント引数</param>
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                ClearText();
            }
            else
            {
                ShowText(listBox1.SelectedIndex);
            }
        }

        #endregion

        #region 処理

        /// <summary>
        /// vntファイル読み込み＆表示
        /// </summary>
        /// <param name="vntFilePath">vntファイルパス</param>
        private void Read(string vntFilePath)
        {
            try
            {
                analyzeResult = VNoteUtil.AnalyzeFile(vntFilePath);
                ShowList();
                ShowText(0);
            }
            catch (Exception ex)
            {
                ErrorTrap(ex);
            }
        }

        /// <summary>
        /// メモリスト表示
        /// </summary>
        private void ShowList()
        {
            var i = 1;
            var formatText = analyzeResult.Count < 1000 ? "D3" : "";
            foreach (var x in analyzeResult)
            {
                var text = x["TEXT"];
                if (text == null) text = "";
                listBox1.Items.Add((i++).ToString(formatText) + " " + text);
            }
        }

        /// <summary>
        /// メモクリア
        /// </summary>
        private void ClearText()
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        /// <summary>
        /// メモ表示
        /// </summary>
        /// <param name="i"></param>
        private void ShowText(int i)
        {
            textBox1.Text = analyzeResult[i]["TEXT"].Replace("\n", "\r\n");
            textBox2.Text = analyzeResult[i]["BLOCK"].Replace("\n", "\r\n");
        }

        /// <summary>
        /// エクスポート
        /// </summary>
        private void Export(string filename)
        {
            try
            {
                Exporter.Execute(analyzeResult, filename);
                MessageBox.Show(this, "エクスポートしました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception ex)
            {
                ErrorTrap(ex);
            }
        }

        /// <summary>
        /// 設定
        /// </summary>
        private void Setting()
        {
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                listBox1.Font = fontDialog1.Font;
                textBox1.Font = fontDialog1.Font;
            }
        }

        /// <summary>
        /// 終了
        /// </summary>
        private void Quit()
        {
            Dispose();
        }

        /// <summary>
        /// エラー処理
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        private void ErrorTrap(Exception ex)
        {
            MessageBox.Show(this, "エラーが発生しました。\r\n詳細:" + ex.ToString(), "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion
    }
}
