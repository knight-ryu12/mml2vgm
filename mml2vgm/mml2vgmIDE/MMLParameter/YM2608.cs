﻿using Core;
using System;
using System.Linq;
using musicDriverInterface;

namespace mml2vgmIDE.MMLParameter
{
    public class YM2608 : Instrument
    {
        public YM2608(SoundManager.Chip chip) : base(19,chip)
        {
            for (int i = 0; i < 19; i++)
            {
                //vol[i] = i < 9 ? 127 : (i < 12 ? 15 : (i < 18 ? 31 : 255));
                vol[i] = 0;
                beforeTie[i] = false;
            }
        }

        public override string Name => "YM2608";

        public bool isMub = false;
        private string[] noteStrTbl=new string[] { "c", "c+", "d", "d+", "e", "f", "f+", "g", "g+", "a", "a+", "b" };

        private int GetChNumFromMucChNum(int ch)
        {
            if (ch < 3) return ch;//FM1-3ch
            if (ch >= 3 && ch <= 5) return ch + 6;//SSG1-3ch
            if (ch == 6) return 12;//りずむ
            if (ch >= 7 && ch <= 9) return ch - 4;//FM4-6ch
            if (ch == 10) return 18;//adpcm

            return ch;
        }

        public override void SetParameter(outDatum od, int cc)
        {
            int n;
            string s;
            try
            {
                int ch = isMub ? GetChNumFromMucChNum(od.linePos.ch) : od.linePos.ch;

                if (isTrace) TraceInfo[ch].Enqueue(od);

                switch (od.type)
                {
                    case enmMMLType.ClockCounter:
                        if (od == null || od.args == null || od.args.Count < 1) break;
                        clockCounter[ch] = (int)od.args[0];
                        break;
                    case enmMMLType.Instrument:
                        if (od.linePos.part == "SSG")
                        {
                            envelope[ch] = (int)od.args[1];
                        }
                        else
                        {
                            inst[ch] = od.args[1].ToString();
                        }
                        break;
                    //case enmMMLType.Envelope:
                    //envelope[od.linePos.ch] = (int)od.args[1];
                    //break;
                    case enmMMLType.Volume:
                        if (od.linePos != null)
                            vol[ch] = (int)od.args[0];
                        break;
                    case enmMMLType.Pan:
                        n = (int)od.args[0];
                        if (od.linePos.part == "SSG")
                        {
                            pan[ch] = "-";
                        }
                        else
                        {
                            pan[ch] = n == 0 ? "-" : (n == 1 ? "Right" : (n == 2 ? "Left" : (n == 3 ? "Center" : n.ToString())));
                        }
                        break;
                    case enmMMLType.Octave:
                        octave[ch] = (int)od.args[0];
                        break;
                    case enmMMLType.OctaveDown:
                        octave[ch]--;
                        break;
                    case enmMMLType.OctaveUp:
                        octave[ch]++;
                        break;
                    case enmMMLType.Note:
                        if (!isMub)
                        {
                            if (od.args == null || od.args.Count <= 0) break;

                            Core.Note nt = (Core.Note)od.args[0];
                            int shift = nt.shift;
                            string f = Math.Sign(shift) >= 0 ? string.Concat(Enumerable.Repeat("+", shift)) : string.Concat(Enumerable.Repeat("-", -shift));
                            notecmd[ch] = string.Format("o{0}{1}{2}", octave[ch], nt.cmd, f);
                            length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / nt.length, nt.length);

                            if (!beforeTie[ch])
                            {
                                if (vol[ch] != null)
                                {
                                    keyOnMeter[ch] = (int)(256.0 / (
                                        od.linePos.part == "FMOPN" ? 128 : (
                                        od.linePos.part == "FMOPNex" ? 128 : (
                                        od.linePos.part == "SSG" ? 16 : (
                                        od.linePos.part == "RHYTHM" ? 32 : 256
                                        )))) * vol[ch]);
                                }
                            }
                            beforeTie[ch] = nt.tieSw;
                        }
                        else
                        {
                            if (od.args == null || od.args.Count <= 0) break;
                            octave[ch] = ((int)od.args[0] >> 4);
                            notecmd[ch] = string.Format("o{0}{1}", octave[ch], noteStrTbl[((int)od.args[0] & 0xf)]);
                            length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * clockCounter[ch] / (int)od.args[1], (int)od.args[1]);
                            if (vol[ch] != null)
                            {
                                keyOnMeter[ch] = (int)(256.0 / (
                                    od.linePos.part == "FM" ? 15 : (
                                    od.linePos.part == "SSG" ? 15 : (
                                    od.linePos.part == "RHYTHM" ? 63 : 255
                                    ))) * vol[ch]);
                            }
                        }
                        break;
                    case enmMMLType.Rest:
                        if (!isMub)
                        {
                            if (od.args != null)
                            {
                                Core.Rest rs = (Core.Rest)od.args[0];
                                notecmd[ch] = "r";
                                length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * cc / rs.length, rs.length);
                            }
                        }
                        else
                        {
                            if (od.args != null)
                            {
                                notecmd[ch] = "r";
                                length[ch] = string.Format("{0:0.##}(#{1:d})", 1.0 * clockCounter[ch] / (int)od.args[0], (int)od.args[0]);
                            }
                        }
                        break;
                    case enmMMLType.Envelope:
                        s = (string)od.args[0];
                        envSw[ch] = s == "EON" ? "ON " : "OFF";
                        break;
                    case enmMMLType.LfoSwitch:
                        s = (string)od.args[2];
                        lfoSw[ch] = s;
                        break;
                    case enmMMLType.Detune:
                        n = (int)od.args[0];
                        detune[ch] = n;
                        break;
                    case enmMMLType.KeyShift:
                        n = (int)od.args[0];
                        keyShift[ch] = n;
                        break;
                }

            }
            catch
            {
                ;//握りつぶす
            }
        }
    }
}
