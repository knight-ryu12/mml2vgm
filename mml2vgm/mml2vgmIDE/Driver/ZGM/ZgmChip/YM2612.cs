﻿using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mml2vgmIDE.Driver.ZGM.ZgmChip
{
    public class YM2612 : ZgmChip
    {

        public YM2612(ChipRegister chipRegister, Setting setting, outDatum[] vgmBuf)
        {
            this.chipRegister = chipRegister;
            this.setting = setting;
            this.vgmBuf = vgmBuf;

            name = "YM2612";
        }

        public override void Setup(ref uint dataPos, ref Dictionary<int, Driver.ZGM.zgm.RefAction<outDatum, uint>> cmdTable)
        {
            base.Setup(ref dataPos, ref cmdTable);

            if (cmdTable.ContainsKey(defineInfo.commandNo)) cmdTable.Remove(defineInfo.commandNo);
            cmdTable.Add(defineInfo.commandNo, SendPort0);

            if (cmdTable.ContainsKey(defineInfo.commandNo + 1)) cmdTable.Remove(defineInfo.commandNo + 1);
            cmdTable.Add(defineInfo.commandNo + 1, SendPort1);

        }

        private void SendPort0(outDatum od,ref uint vgmAdr)
        {
            chipRegister.YM2612SetRegister(od, Audio.DriverSeqCounter, (int)defineInfo.commandNo, 0, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

        private void SendPort1(outDatum od,ref uint vgmAdr)
        {
            chipRegister.YM2612SetRegister(od, Audio.DriverSeqCounter, (int)defineInfo.commandNo, 1, vgmBuf[vgmAdr + 1].val, vgmBuf[vgmAdr + 2].val);
            vgmAdr += 3;
        }

    }
}