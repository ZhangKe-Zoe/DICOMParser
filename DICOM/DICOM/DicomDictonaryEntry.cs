using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DICOM
{
    public class DicomDictonaryEntry
    {
        public string Tag { get; set; }
        public string Name{ get; set; }
        public string Keyword { get; set; }
        public string Vr { get; set; }
        public string Vm { get; set; }
        public int Index { get; set; }
        public byte[] buff { get; set; }
        public string Value { get; set; }
        public uint Length { get; set; }
        public string LengthS { get; set; }

    }
}

