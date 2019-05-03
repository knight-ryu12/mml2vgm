﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE
{
    [Serializable]
    public class ColorScheme
    {

        public int Azuki_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int Azuki_BackColor = Color.FromArgb(40, 30, 60).ToArgb();
        public int Azuki_IconBarBack = Color.FromArgb(70, 60, 90).ToArgb();
        public int Azuki_LineNumberBack_Normal = Color.FromArgb(40, 30, 60).ToArgb();
        public int Azuki_LineNumberFore_Normal = Color.FromArgb(80, 170, 200).ToArgb();
        public int Azuki_LineNumberBack_Trace = Color.FromArgb(150, 180, 60).ToArgb();
        public int Azuki_LineNumberFore_Trace = Color.FromArgb(20, 40, 10).ToArgb();
        public int Azuki_Keyword = Color.FromArgb(255, 190, 60).ToArgb();
        public int Azuki_Comment = Color.FromArgb(250, 190, 240).ToArgb();
        public int Azuki_DocComment = Color.FromArgb(230, 130, 230).ToArgb();
        public int Azuki_Number = Color.FromArgb(235, 235, 255).ToArgb();

        public int ErrorList_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int ErrorList_BackColor = Color.FromArgb(40, 30, 60).ToArgb();

        public int Log_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int Log_BackColor = Color.FromArgb(40, 30, 60).ToArgb();

        public int PartCounter_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int PartCounter_BackColor = Color.FromArgb(40, 30, 60).ToArgb();

        public int FolderTree_ForeColor = Color.FromArgb(235, 235, 255).ToArgb();
        public int FolderTree_BackColor = Color.FromArgb(40, 30, 60).ToArgb();

        public ColorScheme Copy()
        {
            ColorScheme ret = new ColorScheme();

            ret.Azuki_ForeColor = this.Azuki_ForeColor;
            ret.Azuki_BackColor = this.Azuki_BackColor;
            ret.Azuki_IconBarBack = this.Azuki_IconBarBack;
            ret.Azuki_LineNumberBack_Normal = this.Azuki_LineNumberBack_Normal;
            ret.Azuki_LineNumberFore_Normal = this.Azuki_LineNumberFore_Normal;
            ret.Azuki_LineNumberBack_Trace = this.Azuki_LineNumberBack_Trace;
            ret.Azuki_LineNumberFore_Trace = this.Azuki_LineNumberFore_Trace;
            ret.Azuki_Keyword = this.Azuki_Keyword;
            ret.Azuki_Comment = this.Azuki_Comment;
            ret.Azuki_DocComment = this.Azuki_DocComment;
            ret.Azuki_Number = this.Azuki_Number;

            ret.ErrorList_ForeColor = this.ErrorList_ForeColor;
            ret.ErrorList_BackColor = this.ErrorList_BackColor;

            ret.Log_ForeColor = this.Log_ForeColor;
            ret.Log_BackColor = this.Log_BackColor;

            ret.PartCounter_ForeColor = this.PartCounter_ForeColor;
            ret.PartCounter_BackColor = this.PartCounter_BackColor;

            ret.FolderTree_ForeColor = this.FolderTree_ForeColor;
            ret.FolderTree_BackColor = this.FolderTree_BackColor;

            return ret;
        }
    }
}
