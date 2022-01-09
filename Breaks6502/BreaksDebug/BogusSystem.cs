// A dummy device that uses the 6502 as a processor.
// Contains 64 Kbytes of memory and nothing else.

using System;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using Be.Windows.Forms;

namespace BreaksDebug
{
    class BogusSystem
    {
        public int Cycle = 0;

        byte CLK = 0;
        IByteProvider mem;

        public class CpuPads
        {
            [Category("Inputs")]
            public byte n_NMI { get; set; }
            [Category("Inputs")]
            public byte n_IRQ { get; set; }
            [Category("Inputs")]
            public byte n_RES { get; set; }
            [Category("Inputs")]
            public byte PHI0 { get; set; }
            [Category("Inputs")]
            public byte RDY { get; set; }
            [Category("Inputs")]
            public byte SO { get; set; }
            [Category("Outputs")]
            public byte PHI1 { get; set; }
            [Category("Outputs")]
            public byte PHI2 { get; set; }
            [Category("Outputs")]
            public byte RnW { get; set; }
            [Category("Outputs")]
            public byte SYNC { get; set; }
            [Category("Address Bus")]
            public byte[] A { get; set; } = new byte[16];
            [Category("Address Bus")]
            public string Addr { get; set; }
            [Category("Data Bus")]
            public byte [] D { get; set; } = new byte[8];
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        unsafe struct CpuPadsRaw
        {
            public byte n_NMI;
            public byte n_IRQ;
            public byte n_RES;
            public byte PHI0;
            public byte RDY;
            public byte SO;
            public byte PHI1;
            public byte PHI2;
            public byte RnW;
            public byte SYNC;
            public fixed byte A[16];
            public fixed byte D[8];
        }

        public class CpuDebugInfo_RegsBuses
        {
            [Category("Internal buses")]
            public string SB { get; set; }
            [Category("Internal buses")]
            public string DB { get; set; }
            [Category("Internal buses")]
            public string ADL { get; set; }
            [Category("Internal buses")]
            public string ADH { get; set; }
        }

        public class CpuDebugInfo_Decoder
        {
            // Decoder

            [TypeConverter(typeof(ListConverter))]
            [Category("Decoder")]
            public List<string> decoder_out { get; set; } = new List<string>();
        }

        public class CpuDebugInfo_Commands
        {
            // Control commands

            [TypeConverter(typeof(ListConverter))]
            [Category("Control commands")]
            public List<string> commands { get; set; } = new List<string>();
            [Category("Control commands")]
            public byte n_ACIN { get; set; }
            [Category("Control commands")]
            public byte n_DAA { get; set; }
            [Category("Control commands")]
            public byte n_DSA { get; set; }
            [Category("Control commands")]
            public byte n_1PC { get; set; }
        }

        enum ControlCommand
        {
            Y_SB = 0,
            SB_Y,
            X_SB,
            SB_X,
            S_ADL,
            S_SB,
            SB_S,
            S_S,
            NDB_ADD,
            DB_ADD,
            Z_ADD,
            SB_ADD,
            ADL_ADD,
            ANDS,
            EORS,
            ORS,
            SRS,
            SUMS,
            ADD_SB7,
            ADD_SB06,
            ADD_ADL,
            SB_AC,
            AC_SB,
            AC_DB,
            ADH_PCH,
            PCH_PCH,
            PCH_ADH,
            PCH_DB,
            ADL_PCL,
            PCL_PCL,
            PCL_ADL,
            PCL_DB,
            ADH_ABH,
            ADL_ABL,
            Z_ADL0,
            Z_ADL1,
            Z_ADL2,
            Z_ADH0,
            Z_ADH17,
            SB_DB,
            SB_ADH,
            DL_ADL,
            DL_ADH,
            DL_DB,
            Max,
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        unsafe struct CpuDebugInfoRaw
        {
            // Regs & Buses

            public byte SB;
            public byte DB;
            public byte ADL;
            public byte ADH;

            // Dispatcher

            // Decoder

            public fixed byte decoder_out[130];

            // Control commands

            public fixed byte cmd[(int)ControlCommand.Max];
            public byte n_ACIN;
            public byte n_DAA;
            public byte n_DSA;
            public byte n_1PC;          // From Dispatcher
        }

        public CpuPads cpu_pads = new CpuPads();
        CpuDebugInfoRaw info = new CpuDebugInfoRaw();

        public void Step()
        {
            Console.WriteLine("Step");

            cpu_pads.PHI0 = CLK;

            // Execute M6502Core::sim

            CpuPadsRaw pads = SerializePads (cpu_pads);

            Sim(ref pads, ref info);

            cpu_pads = DeserializePads(pads);

            // TODO: CpuDebugInfo

            // Handling memory operations

            UInt16 address = 0;

            for (int i = 0; i < 16; i++)
            {
                if (cpu_pads.A[i] != 0)
                {
                    address |= (UInt16)(1 << i);
                }
            }

            if (cpu_pads.RnW == 1)
            {
                // CPU Read

                byte data = mem.ReadByte(address);

                for (int i =0; i<8; i++)
                {
                    if ((data & (1 << i)) != 0)
                    {
                        cpu_pads.D[i] = 1;
                    }
                    else
                    {
                        cpu_pads.D[i] = 0;
                    }
                }
            }
            else
            {
                // CPU Write

                byte data = 0;

                for (int i = 0; i < 8; i++)
                {
                    if (cpu_pads.D[i] != 0)
                    {
                        data |= (byte)(1 << i);
                    }
                }

                mem.WriteByte(address, data);
            }

            // Clockgen

            if (CLK != 0)
            {
                CLK = 0;
                Cycle++;
            }
            else
            {
                CLK = 1;
            }
        }
  
        public void AttatchMemory (IByteProvider prov)
        {
            mem = prov;
        }

        CpuPadsRaw SerializePads (CpuPads pads)
        {
            CpuPadsRaw pads_raw = new CpuPadsRaw();

            pads_raw.n_NMI = pads.n_NMI;
            pads_raw.n_IRQ = pads.n_IRQ;
            pads_raw.n_RES = pads.n_RES;
            pads_raw.PHI0 = pads.PHI0;
            pads_raw.RDY = pads.RDY;
            pads_raw.SO = pads.SO;
            pads_raw.PHI1 = pads.PHI1;
            pads_raw.PHI2 = pads.PHI2;
            pads_raw.RnW = pads.RnW;
            pads_raw.SYNC = pads.SYNC;

            unsafe
            {
                for (int i = 0; i < 16; i++)
                {
                    pads_raw.A[i] = pads.A[i];
                }

                for (int i = 0; i < 8; i++)
                {
                    pads_raw.D[i] = pads.D[i];
                }
            }

            return pads_raw;
        }

        CpuPads DeserializePads(CpuPadsRaw pads_raw)
        {
            CpuPads pads = new CpuPads();

            pads.n_NMI = pads_raw.n_NMI;
            pads.n_IRQ = pads_raw.n_IRQ;
            pads.n_RES = pads_raw.n_RES;
            pads.PHI0 = pads_raw.PHI0;
            pads.RDY = pads_raw.RDY;
            pads.SO = pads_raw.SO;
            pads.PHI1 = pads_raw.PHI1;
            pads.PHI2 = pads_raw.PHI2;
            pads.RnW = pads_raw.RnW;
            pads.SYNC = pads_raw.SYNC;

            unsafe
            {
                UInt16 Addr = 0;

                for (int i = 0; i < 16; i++)
                {
                    pads.A[i] = pads_raw.A[i];

                    if (pads.A[i] != 0)
                    {
                        Addr |= (UInt16)(1 << i);
                    }
                }

                pads.Addr = "0x" + Addr.ToString("X4");

                for (int i = 0; i < 8; i++)
                {
                    pads.D[i] = pads_raw.D[i];
                }
            }

            return pads;
        }

        public CpuDebugInfo_RegsBuses GetRegsBuses()
        {
            CpuDebugInfo_RegsBuses res = new CpuDebugInfo_RegsBuses();

            res.SB = "0x" + info.SB.ToString("X2");
            res.DB = "0x" + info.DB.ToString("X2");
            res.ADL = "0x" + info.ADL.ToString("X2");
            res.ADH = "0x" + info.ADH.ToString("X2");

            return res;
        }

        public CpuDebugInfo_Decoder GetDecoder()
        {
            CpuDebugInfo_Decoder res = new CpuDebugInfo_Decoder();

            for (int n = 0; n < 130; n++)
            {
                unsafe
                {
                    if (info.decoder_out[n] != 0)
                    {
                        string text = n.ToString() + ": " + DecoderDecoder.GetDecoderOutName(n);
                        res.decoder_out.Add(text);
                    }
                }
            }

            return res;
        }

        public CpuDebugInfo_Commands GetCommands()
        {
            CpuDebugInfo_Commands res = new CpuDebugInfo_Commands();

            for (int n=0; n<(int)ControlCommand.Max; n++)
            {
                unsafe
                {
                    if (info.cmd[n] != 0)
                    {
                        res.commands.Add(((ControlCommand)n).ToString());
                    }
                }
            }

            res.n_ACIN = info.n_ACIN;
            res.n_DAA = info.n_DAA;
            res.n_DSA = info.n_DSA;
            res.n_1PC = info.n_1PC;

            return res;
        }

        [DllImport("M6502CoreInterop.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void Sim(ref CpuPadsRaw pads, ref CpuDebugInfoRaw debugInfo);


        // https://stackoverflow.com/questions/32582504/propertygrid-expandable-collection

        public class ListConverter : CollectionConverter
        {
            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                IList list = value as IList;
                if (list == null || list.Count == 0)
                    return base.GetProperties(context, value, attributes);

                var items = new PropertyDescriptorCollection(null);
                for (int i = 0; i < list.Count; i++)
                {
                    object item = list[i];
                    items.Add(new ExpandableCollectionPropertyDescriptor(list, i));
                }
                return items;
            }

            public class ExpandableCollectionPropertyDescriptor : PropertyDescriptor
            {
                private IList collection;
                private readonly int _index;

                public ExpandableCollectionPropertyDescriptor(IList coll, int idx)
                    : base(GetDisplayName(coll, idx), null)
                {
                    collection = coll;
                    _index = idx;
                }

                private static string GetDisplayName(IList list, int index)
                {
                    return "[" + index + "]  " + CSharpName(list[index].GetType());
                }

                private static string CSharpName(Type type)
                {
                    var sb = new StringBuilder();
                    var name = type.Name;
                    if (!type.IsGenericType)
                        return name;
                    sb.Append(name.Substring(0, name.IndexOf('`')));
                    sb.Append("<");
                    sb.Append(string.Join(", ", type.GetGenericArguments().Select(CSharpName)));
                    sb.Append(">");
                    return sb.ToString();
                }

                public override bool CanResetValue(object component)
                {
                    return true;
                }

                public override Type ComponentType
                {
                    get { return this.collection.GetType(); }
                }

                public override object GetValue(object component)
                {
                    return collection[_index];
                }

                public override bool IsReadOnly
                {
                    get { return false; }
                }

                public override string Name
                {
                    get { return _index.ToString(CultureInfo.InvariantCulture); }
                }

                public override Type PropertyType
                {
                    get { return collection[_index].GetType(); }
                }

                public override void ResetValue(object component)
                {
                }

                public override bool ShouldSerializeValue(object component)
                {
                    return true;
                }

                public override void SetValue(object component, object value)
                {
                    collection[_index] = value;
                }
            }
        }

    }

}
