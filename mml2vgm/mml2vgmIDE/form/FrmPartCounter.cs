﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using mml2vgmIDE.MMLParameter;
using WeifenLuo.WinFormsUI.Docking;

namespace mml2vgmIDE
{
    public partial class FrmPartCounter : DockContent, IForm
    {
        public Action parentUpdate = null;
        private MMLParameter.Manager mmlParams = null;
        private Setting setting = null;
        private Brush[] meterBrush = new Brush[256];


        public FrmPartCounter(Setting setting)
        {
            InitializeComponent();
            this.setting = setting;

            dgvPartCounter.BackgroundColor = Color.FromArgb(setting.ColorScheme.PartCounter_BackColor);
            dgvPartCounter.DefaultCellStyle.BackColor = Color.FromArgb(setting.ColorScheme.PartCounter_BackColor);
            dgvPartCounter.ForeColor = Color.FromArgb(setting.ColorScheme.PartCounter_ForeColor);
            EnableDoubleBuffering(dgvPartCounter);
            SetDisplayIndex(setting.location.PartCounterClmInfo);

            double r = 0;
            double g = 0;
            double b = 0;
            double sr = 0;
            double sg = 0;
            double sb = 0;
            double tr = 0;
            double tg = 0;
            double tb = 0;
            double div = 0;
            int cnt = 0;

            for (int i = 0; i < 256; i++)
            {
                if (i == 0)
                {
                    r = 140.0;
                    g = 120.0;
                    b = 215.0;

                    sr = r;
                    sg = g;
                    sb = b;
                    tr = 80.0;
                    tg = 80.0;
                    tb = 160.0;

                    div = 20.0;
                    cnt = 20;
                }
                if (i == 80)
                {
                    sr = r;
                    sg = g;
                    sb = b;
                    tr = 70.0;
                    tg = 70.0;
                    tb = 120.0;

                    div = 60.0;
                    cnt = 60;
                }
                if (i == 210)
                {
                    sr = r;
                    sg = g;
                    sb = b;
                    tr = 40.0;
                    tg = 40.0;
                    tb = 80.0;

                    div = 40.0;
                    cnt = 40;
                }

                if (cnt > 0)
                {
                    r += (tr - sr) / div;
                    g += (tg - sg) / div;
                    b += (tb - sb) / div;
                    cnt--;
                }

                Color c = Color.FromArgb(255
                    , Math.Max(Math.Min((int)r,255),0)
                    , Math.Max(Math.Min((int)g, 255), 0)
                    , Math.Max(Math.Min((int)b, 255), 0)
                    );
                meterBrush[255 - i] = new SolidBrush(c);
            }
        }

        public void ClearCounter()
        {
            dgvPartCounter.Rows.Clear();
        }

        public void AddPartCounter(object[] cells)
        {
            DataGridViewRow r = new DataGridViewRow();
            r.CreateCells(dgvPartCounter);

            r.Cells[dgvPartCounter.Columns["ClmPartNumber"].Index].Value = cells[0];
            r.Cells[dgvPartCounter.Columns["ClmChipIndex"].Index].Value = cells[1];
            r.Cells[dgvPartCounter.Columns["ClmChipNumber"].Index].Value = cells[2];
            r.Cells[dgvPartCounter.Columns["ClmPart"].Index].Value = cells[3];
            r.Cells[dgvPartCounter.Columns["ClmChip"].Index].Value = cells[4];
            r.Cells[dgvPartCounter.Columns["ClmCOunter"].Index].Value = cells[5];

            dgvPartCounter.Rows.Add(r);
        }

        public void Start(MMLParameter.Manager mmlParams)
        {
            timer.Enabled = true;
            this.mmlParams = mmlParams;
        }

        public void Stop()
        {
            timer.Enabled = false;
            mmlParams = null;
        }


        /// <summary>
        /// ダブルバッファリングを有効にする(from DOBON)
        /// </summary>
        public static void EnableDoubleBuffering(Control control)
        {
            control.GetType().InvokeMember(
               "DoubleBuffered",
               BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
               null,
               control,
               new object[] { true });
        }

        private void FrmPartCounter_FormClosing(object sender, FormClosingEventArgs e)
        {
            setting.location.PartCounterClmInfo = getDisplayIndex();

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                parentUpdate?.Invoke();
                return;
            }
        }


        private void FrmPartCounter_FormClosed(object sender, FormClosedEventArgs e)
        {
            for(int i = 0; i < 256; i++)
            {
                meterBrush[i].Dispose();
            }

        }

        protected override string GetPersistString()
        {
            return this.Name;
        }

        private void FrmPartCounter_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (mmlParams == null) return;

            //パラメータ取得

            //パラメータ描画
            dgvPartCounter.SuspendLayout();

            for (int p = 0; p < dgvPartCounter.Rows.Count; p++)
            {
                string chip = (string)dgvPartCounter.Rows[p].Cells["ClmChip"].Value;
                int r = (int)dgvPartCounter.Rows[p].Cells["ClmPartNumber"].Value - 1;
                int chipIndex = (int)dgvPartCounter.Rows[p].Cells["ClmChipIndex"].Value;
                int isSecondary = (int)dgvPartCounter.Rows[p].Cells["ClmChipNumber"].Value;

                if (mmlParams.Insts.ContainsKey(chip))
                {
                    if (mmlParams.Insts[chip].ContainsKey(chipIndex) && mmlParams.Insts[chip][chipIndex].ContainsKey(isSecondary))
                    {
                        MMLParameter.Instrument mmli = mmlParams.Insts[chip][chipIndex][isSecondary];

                        dgvPartCounter.Rows[p].Cells["ClmInstrument"].Value = mmli.inst[r] == null ? "-" : mmli.inst[r].ToString();
                        dgvPartCounter.Rows[p].Cells["ClmEnvelope"].Value = mmli.envelope[r] == null ? "-" : mmli.envelope[r].ToString();
                        dgvPartCounter.Rows[p].Cells["ClmVolume"].Value = mmli.vol[r] == null ? "-" : mmli.vol[r].ToString();
                        dgvPartCounter.Rows[p].Cells["ClmExpression"].Value = mmli.expression[r] == null ? "-" : mmli.expression[r].ToString();
                        dgvPartCounter.Rows[p].Cells["ClmVelocity"].Value = mmli.velocity[r] == null ? "-" : mmli.velocity[r].ToString();
                        dgvPartCounter.Rows[p].Cells["ClmPan"].Value = mmli.pan[r] == null ? "-" : mmli.pan[r];
                        dgvPartCounter.Rows[p].Cells["ClmNote"].Value = mmli.notecmd[r] == null ? "-" : mmli.notecmd[r];
                        dgvPartCounter.Rows[p].Cells["ClmLength"].Value = mmli.length[r] == null ? "-" : mmli.length[r];
                        dgvPartCounter.Rows[p].Cells["ClmEnvSw"].Value = mmli.envSw[r] == null ? "-" : mmli.envSw[r];
                        dgvPartCounter.Rows[p].Cells["ClmLfoSw"].Value = mmli.lfoSw[r] == null ? "-" : mmli.lfoSw[r];
                        dgvPartCounter.Rows[p].Cells["ClmDetune"].Value = mmli.detune[r] == null ? "-" : mmli.detune[r].ToString();
                        dgvPartCounter.Rows[p].Cells["ClmKeyShift"].Value = mmli.keyShift[r] == null ? "-" : mmli.keyShift[r].ToString();
                        DrawMeter(dgvPartCounter.Rows[p].Cells["ClmMeter"], mmli, r);
                    }
                }
            }

            dgvPartCounter.ResumeLayout();
        }

        private void DrawMeter(DataGridViewCell dataGridViewCell, Instrument mmli,int pn)
        {
            DataGridViewImageCell cell = (DataGridViewImageCell)dataGridViewCell;
            int cw = cell.Size.Width;
            int ch = cell.Size.Height;
            int x = 2;
            int y = (int)((ch - 4) / 6.0) + 2;
            int p = mmli.keyOnMeter[pn] == null ? 0 : (int)mmli.keyOnMeter[pn];
            int w = (int)((cw - 6) / 256.0 * p);
            int h = (int)((ch - 4) / 6.0 * 4.0);
            p = Common.Range(p, 0, meterBrush.Length - 1);

            Bitmap canvas = new Bitmap(cw, ch);
            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.Transparent);
                g.FillRectangle(meterBrush[p], x, y, w, h);
            }

            //PictureBox1に表示する
            cell.Value = canvas;

            if (mmli.keyOnMeter[pn] != null)
            {
                mmli.keyOnMeter[pn] -= 4;
                mmli.keyOnMeter[pn] = Math.Max((int)mmli.keyOnMeter[pn], 0);
            }
        }

        private dgvColumnInfo[] getDisplayIndex()
        {
            List<dgvColumnInfo> ret = new List<dgvColumnInfo>();

            for (int i = 0; i < dgvPartCounter.Columns.Count; i++)
            {
                dgvColumnInfo info = (dgvColumnInfo)dgvPartCounter.Columns[i].Tag;
                if (info == null)
                {
                    info = new dgvColumnInfo();
                }

                info.columnName = dgvPartCounter.Columns[i].Name;
                info.displayIndex = dgvPartCounter.Columns[i].DisplayIndex;
                info.size = dgvPartCounter.Columns[i].Width;
                info.visible = dgvPartCounter.Columns[i].Visible;

                ret.Add(info);
            }

            return ret.ToArray();
        }

        private void SetDisplayIndex(dgvColumnInfo[] aryIndex)
        {
            if (aryIndex == null || aryIndex.Length < 1) return;

            for (int i = 0; i < aryIndex.Length; i++)
            {
                if (aryIndex[i] == null) continue;
                if (!dgvPartCounter.Columns.Contains(aryIndex[i].columnName)) continue;

                dgvPartCounter.Columns[aryIndex[i].columnName].DisplayIndex = aryIndex[i].displayIndex;
                dgvPartCounter.Columns[aryIndex[i].columnName].Width = Math.Max(aryIndex[i].size, 10);
                dgvPartCounter.Columns[aryIndex[i].columnName].Visible = aryIndex[i].visible;
                dgvPartCounter.Columns[aryIndex[i].columnName].Tag = aryIndex[i];
            }

            //spacerは常に最後にする
            dgvPartCounter.Columns["ClmSpacer"].DisplayIndex = dgvPartCounter.Columns.Count  - 1;
        }

        private void DgvPartCounter_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void DgvPartCounter_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void DgvPartCounter_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            if (e.RowIndex != -1) return;
            if (setting == null || setting.location == null || setting.location.PartCounterClmInfo == null) return;

            //メニューのアイテムを生成する
            //  hide / show all / セパレータの追加
            cmsMenu.Items.Clear();
            string txt = dgvPartCounter.Columns[e.ColumnIndex].HeaderText;
            if (!string.IsNullOrEmpty(txt))
            {
                cmsMenu.Items.Add(string.Format("Hide {0}", txt));
                cmsMenu.Items[0].Tag = dgvPartCounter.Columns[e.ColumnIndex].Tag;
                cmsMenu.Items[0].Click += MenuItem_Click;
            }
            cmsMenu.Items.Add("Show all");
            cmsMenu.Items[cmsMenu.Items.Count - 1].Click += MenuItem_Click;
            cmsMenu.Items.Add("-");

            //  その他の列を全て追加する
            foreach (DataGridViewColumn c in dgvPartCounter.Columns)
            {
                if (txt == c.HeaderText) continue;
                if (string.IsNullOrEmpty(c.HeaderText)) continue;
                if (c.Name == "ClmChipIndex") continue;
                if (c.Name == "ClmChipNumber") continue;
                if (c.Name == "ClmPartNumber") continue;
                if (c.Name == "ClmIsSecondary") continue;

                cmsMenu.Items.Add(c.HeaderText);
                cmsMenu.Items[cmsMenu.Items.Count - 1].Tag = c.Tag;
                cmsMenu.Items[cmsMenu.Items.Count - 1].Click += MenuItem_Click;
                ((ToolStripMenuItem)cmsMenu.Items[cmsMenu.Items.Count - 1]).Checked = c.Visible;
            }

            cmsMenu.Show(Cursor.Position);

        }

        private void CmsMenu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyValue != 13) return;
            foreach (ToolStripItem i in cmsMenu.Items)
            {
                if (!i.Selected) continue;
                //MenuItem_Click(i, null);
                break;
            }
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem i = (ToolStripMenuItem)sender;
            foreach(DataGridViewColumn c in dgvPartCounter.Columns)
            {
                if (c.Tag != i.Tag) continue;
                c.Visible = !c.Visible;
                return;
            }

            //show all
            foreach (DataGridViewColumn c in dgvPartCounter.Columns)
            {
                if (c.Name == "ClmChipIndex") continue;
                if (c.Name == "ClmChipNumber") continue;
                if (c.Name == "ClmPartNumber") continue;
                if (c.Name == "ClmIsSecondary") continue;
                c.Visible = true;
                c.Width = Math.Max(c.Width, 10);
            }
        }

    }

    public class dgvColumnInfo
    {
        public string columnName = "";
        public int displayIndex = 0;
        public int size = 10;
        public bool visible = true;
    }
}