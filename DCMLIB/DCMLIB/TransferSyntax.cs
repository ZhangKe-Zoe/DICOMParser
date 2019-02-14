using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DICOM;


namespace DCMLIB
{
    public abstract class TransferSyntax
    {
        public bool isBE;
        public bool isExplicit;
        public string uid;
        public string name;
        protected VR vrdecoder;
        protected VRFactory vrfactory;
        public DicomDictionary dicomDictionary;

        public TransferSyntax()
        {
            //初始化
            dicomDictionary = new DicomDictionary();
            dicomDictionary.Read(@"F:\2018-2019大三下\Homework\DICOM\dicom.txt");
            vrfactory = new VRFactory(this);
            vrdecoder = new UL(this); //从Decode方法移入，可以为任意vr子类

        }
        //TransferSyntaxes类，方便应用程序使用
        public class TransferSyntaxes
        {
            public TransferSyntax[] TSs = new TransferSyntax[3];
            public TransferSyntaxes()
            {
                TSs[0] = new ImplicitVRLittleEndian();
                TSs[1] = new ExplicitVRLittleEndian();
                TSs[2] = new ExplicitVRBigEndian();
            }
        }
        public virtual DCMAbstractType Decode(byte[] data, ref uint idx)
        {
            DCMDataElement element = new DCMDataElement();
            //读取TAG
            element.gtag = vrdecoder.GetGroupTag(data, ref idx);
            element.etag = vrdecoder.GetElementTag(data, ref idx);
            //处理SQ
            if (element.gtag == 0xfffe && element.etag == 0xe000) //处理SQ的三个特殊标记
            {
                DCMDataItem sqitem = new DCMDataItem(this);
                uint length = vrdecoder.GetUInt(data, ref idx); //不能用GetLength
                uint offset = idx;
                while (idx - offset < length)
                {

                    DCMAbstractType sqelem = Decode(data, ref idx);  //递归
                    if (length == 0xffffffff && sqelem.gtag == 0xfffe && sqelem.etag == 0xe00d) //条目结束标记
                        break;
                    sqitem.items.Add(sqelem);
                }
                return sqitem;
            }
            else if (element.gtag == 0xfffe && element.etag == 0xe0dd)  //序列结束标记
            {
                element.vr = "UL";
                element.length = vrdecoder.GetUInt(data, ref idx);  //不能用GetLength
                return element;
            }
            //查数据字典得到VR,Name,VM
            element.vr = vrdecoder.GetVR(data, ref idx);
            DicomDictonaryEntry entry = dicomDictionary.GetEntry(element.gtag, element.etag);
            if (entry != null)
            {
                if (element.vr == "") element.vr = entry.Vr;
                element.name = entry.Keyword;
                element.vm = entry.Vm;
            }
            else if (element.vr == "" && element.etag == 0)
                element.vr = "US";
            //根据得到的值表示法构造VR子类对象
            element.vrparser = vrfactory.GetVR(element.vr);
            //读取值长度
            element.length = element.vrparser.GetLength(data, ref idx);
            //读取值
            element.value = element.vrparser.GetValue(data, ref idx, element.length);
            element.idx = idx;
            return element;
        }

    }

    public abstract class VR
    {
        public TransferSyntax syntax;

        public VR(TransferSyntax syntax)
        {
            this.syntax = syntax;
        }

        public ushort GetGroupTag(byte[] data, ref uint idx)
        {
            ushort gtag;
            if (syntax.isBE)
                gtag = (ushort)(data[idx] * 256 + data[idx + 1]);
            else
                gtag = (ushort)(data[idx] + data[idx + 1] * 256);
            idx += 2;
            return gtag;
        }

        public ushort GetElementTag(byte[] data, ref uint idx)
        {
            return (this.GetGroupTag(data, ref idx));
        }

        public string GetVR(byte[] data, ref uint idx)
        {
            String vr;
            if (syntax.isExplicit)
            {
                vr = Encoding.Default.GetString(data, (int)idx, 2);
                idx += 2;
            }
            else vr = "";
            return vr;
        }

        public virtual uint GetLength(byte[] data, ref uint idx)
        {
            uint length = 0;
            if (syntax.isExplicit)
                length = (uint)GetGroupTag(data, ref idx);
            else
                length = (uint)GetUInt(data, ref idx);
            return length;
        }

        public uint GetUInt(byte[] data, ref uint idx)
        {
            uint length = 0;
            if (!syntax.isBE)
            {
                length = (uint)(data[idx + 0] + (data[idx + 1] << 8) +
                    (data[idx + 2] << 16) + (data[idx + 3] << 24));
            }
            else
            {
                length = (uint)(data[idx+3] + (data[idx +2]<<8) +
                    (data[idx + 1]<<16) + (data[idx + 0]<<24));
            }
            idx += 4;
            return length;
        }
        //读取值
        public virtual byte[] GetValue(byte[] data, ref uint idx, uint length)
        {
            byte[] value = new byte[length];
            //data.CopyTo(value, idx);
            Array.Copy(data, idx, value, 0, length);
            idx += length;
            return value;
        }
        //解码
        public abstract string GetString(byte[] data, string head);
        //ReadValue/WriteValue模板方法
        public abstract byte[] WriteValue<T>(T[] val);
        public abstract T[] ReadValue<T>(byte[] data);

    }


    public class ImplicitVRLittleEndian : TransferSyntax
    {
        public ImplicitVRLittleEndian()
        {
            isExplicit = false;
            isBE = false;
            uid = "1.2.840.10008.1.2";
            name = "implicitVRLittleEndian";
        }
    }

    public class ExplicitVRLittleEndian : TransferSyntax
    {
        public ExplicitVRLittleEndian()
        {
            isExplicit = true;
            isBE = false;
            uid = "1.2.840.10008.1.2.1";
            name = "explicitVRLittleEndian";
        }
    }

    public class ExplicitVRBigEndian : TransferSyntax
    {
        public ExplicitVRBigEndian()
        {
            isExplicit = true;
            isBE = true;
            uid = "1.2.840.10008.1.2.2";
            name = "explicitVRBigEndian";
        }
    }

    public class UL : VR
    {
        //保存基类参数
        public UL(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            uint idx = 0;
            Int32 value;
            //判断是否显式并解码
            if (!syntax.isBE)
            {
                value = (Int32)GetLength(data, ref idx);
            }
            else
            {
                Array.Reverse(data);
                value = (Int32)GetLength(data, ref idx);
            }
            idx += 4;
            return value.ToString();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(uint))    //判断是否请求的是对应的uint类型
            {
                uint[] vals = val as uint[];
                //暂时先考虑单个值,多值有待完善
                byte[] data = BitConverter.GetBytes(vals[0]);
                if (syntax.isBE)
                    Array.Reverse(data);
                return data;
            }
            throw new NotSupportedException();
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(uint)) //判断是否请求的是对应的uint类型
            {
                //暂时先考虑单个值,多值有待完善
                uint[] val = new uint[1];
                int idx = 0;
                if (syntax.isBE)
                    val[0] = (uint)((data[idx] << 24) + (data[idx + 1] << 16) + (data[idx + 2] << 8) + data[idx + 3]);
                else
                    val[0] = (uint)((data[idx + 3] << 24) + (data[idx + 2] << 16) + (data[idx + 1] << 8) + data[idx]);
                return val as T[];
            }
            throw new NotSupportedException();
        }

    }
    public class US : VR
    {
        public US(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            uint idx = 0;
            ushort value;
            if (!syntax.isBE)
            {
                value = BitConverter.ToUInt16(data, (int)idx);
            }
            else
            {
                Array.Reverse(data);
                value = BitConverter.ToUInt16(data, (int)idx);

            }
            idx += 2;
            return value.ToString();
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(UInt16)) //判断是否请求的是对应的特定类型
            {
                //暂时先考虑单个值,多值有待完善
                UInt16[] val = new UInt16[1];
                int idx = 0;
                if (syntax.isBE)
                    val[0] = (UInt16)((data[idx] << 24) + (data[idx + 1] << 16));
                else
                    val[0] = (UInt16)((data[idx + 1] << 8) + data[idx]);
                return val as T[];
            }
            else           //抛出异常，提示应用程序读取的类型不对。
            throw new NotSupportedException();
            
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(uint)) //判断是否请求的是对应的特定类型
            {
                uint[] vals = val as uint[];
                //暂时先考虑单个值,多值有待完善
                byte[] data = BitConverter.GetBytes(vals[0]);
                if (syntax.isBE)
                    Array.Reverse(data);
                return data;
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }

    }
    public class SS : VR
    {
        public SS(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            uint idx = 0;
            short numss;
            string vaule;
            if (syntax.isBE)
            {
                numss = (short)((data[idx] << 8) + data[idx + 1]);
                idx += 2;
                vaule = numss.ToString();
                return vaule;
            }
            else
            {
                numss = (short)(data[idx] + (data[idx + 1] << 8));
                idx += 2;
                vaule = numss.ToString();

            }
            return vaule;
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(short)) //判断是否请求的是对应的特定类型，例如uint
            {
                short[] val = new short[1];
                int idx = 0;
                if (syntax.isBE)
                    val[0] = (short)((data[idx] << 8) + data[idx + 1]);
                else
                    val[0] = (short)(data[idx] + (data[idx + 1] << 8));
                return val as T[];
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(short)) //判断是否请求的是对应的特定类型，例如uint
            {
                short [] vals = val as short[];
                //暂时先考虑单个值,多值有待完善
                byte[] data = BitConverter.GetBytes(vals[0]);
                if (syntax.isBE)
                    Array.Reverse(data);
                return data;
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }
    }
    public class SL  : VR
    {
        public SL(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            uint idx = 0;
            string vaule;
            if (syntax.isBE)
            {

                Array.Reverse(data);
                int numsl = BitConverter.ToInt32(data, (int)idx);
                idx += 4;
                vaule = numsl.ToString();

            }
            else
            {
                int numsl = BitConverter.ToInt32(data, (int)idx);
                idx += 4;
                vaule = numsl.ToString();

            }
            return vaule;
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(short)) //判断是否请求的是对应的特定类型，例如uint
            {
                uint idx = 0;
                short[] val = new short[1];
                if (syntax.isBE)
                {
                    Array.Reverse(data);
                    val[0] = (short)BitConverter.ToInt32(data, (int)idx);

                }
                else
                {
                    val[0] = (short)BitConverter.ToInt32(data, (int)idx);

                }
                return val as T[];
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(short)) //判断是否请求的是对应的特定类型，例如uint
            {
                short[] vals = val as short[];
                //暂时先考虑单个值,多值有待完善
                byte[] data = BitConverter.GetBytes(vals[0]);
                if (syntax.isBE)
                    Array.Reverse(data);
                return data;
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }
    }
    public class DA : VR
    {
        public DA(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            string s = Encoding.Default.GetString(data);
            uint idx = 0;
            try
            {
                int year = int.Parse(s.Substring(0, 4));
                int month = int.Parse(s.Substring(4, 2));
                int day = int.Parse(s.Substring(6, 2));
                DateTime dt = new DateTime(year, month, day);
                idx += 8;
                string value = dt.ToString();
                return value.ToString();
            }
            catch(Exception)
            {
                return"NULL";
            }
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(DateTime)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(DateTime)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }

    }
    public class DT : VR
    {
        public DT(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            string s = Encoding.Default.GetString(data);
            uint idx = 0;
            int year = int.Parse(s.Substring(0, 4));
            int month = int.Parse(s.Substring(4, 2));
            int day = int.Parse(s.Substring(6, 2));
            int hour = int.Parse(s.Substring(8, 2));
            int minute = int.Parse(s.Substring(10, 2));
            int second = int.Parse(s.Substring(12, 2));
            int milli = int.Parse(s.Substring(14, 6));
            DateTime ts = new DateTime(year, month, day, hour, minute, second, milli);
            idx += 20;
            string value = ts.ToString();
            return value.ToString();
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(DateTime)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(DateTime)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }

    }
    public class TM : VR
    {
        public TM(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            string s = Encoding.Default.GetString(data);
            uint idx = 0;
            try
            {
                int hour = int.Parse(s.Substring(0, 2));
                int minute = int.Parse(s.Substring(2, 2));
                int second = int.Parse(s.Substring(4, 2));
                // int milli = int.Parse(s.Substring(7, 6));
                TimeSpan ts = new TimeSpan(0, hour, minute, second);
                idx += 13;
                string value = ts.ToString();
                return value.ToString();
            }
            catch (Exception)
            {
                return "NULL";
            }

        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(TimeSpan)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(TimeSpan)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }

    }
    public class FL : VR
    {
        public FL(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            uint idx = 0;
            string value;
            if (syntax.isBE)
            {
                Array.Reverse(data);
                Single numfs = BitConverter.ToSingle(data, (int)idx);
                idx += 4;
                value = numfs.ToString();

            }
            else
            {
                Single numfs = BitConverter.ToSingle(data, (int)idx);
                idx += 4;
                value = numfs.ToString();

            }
            return value;
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(Single)) //判断是否请求的是对应的特定类型，例如uint
            {
                uint idx = 0;
                Single []val = new Single[1];
                if (syntax.isBE)
                {
                    Array.Reverse(data);
                    val[0] = BitConverter.ToSingle(data, (int)idx);
                }
                else
                    val[0] = BitConverter.ToSingle(data, (int)idx);
                return val as T[];
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(Single)) //判断是否请求的是对应的特定类型，例如uint
            {
                Single[] vals = val as Single[];
                //暂时先考虑单个值,多值有待完善
                byte[] data = BitConverter.GetBytes(vals[0]);
                if (syntax.isBE)
                    Array.Reverse(data);
                return data;//相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }
    }
    public class FD : VR
    {
        public FD(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            uint idx = 0;
            string value;
            if (syntax.isBE)
            {
                Array.Reverse(data);
                Double numfd = BitConverter.ToDouble(data, (int)idx);
                idx += 8;
                value = numfd.ToString();

            }
            else
            {
                Double numfd = BitConverter.ToDouble(data, (int)idx);
                idx += 8;
                value = numfd.ToString();

            }
            return value;
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(Double)) //判断是否请求的是对应的特定类型，例如uint
            {
                uint idx = 0;
                Double[] val = new Double[1];
                if (syntax.isBE)
                {
                    Array.Reverse(data);
                    val[0]= BitConverter.ToDouble(data, (int)idx);
                }
                else
                val[0] = BitConverter.ToDouble(data, (int)idx);

                return val as T[];
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(Double)) //判断是否请求的是对应的特定类型，例如uint
            {
                Double[] vals = val as Double[];
                //暂时先考虑单个值,多值有待完善
                byte[] data = BitConverter.GetBytes(vals[0]);
                if (syntax.isBE)
                    Array.Reverse(data);
                return data;
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }
    }
    //特殊文本类型
    public class ST : VR
    {
        public ST(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            string s = Encoding.Default.GetString(data);

            uint idx = 0;
            string value = s;
            idx += (uint)(data.Length);
            return value.ToString();
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(string)) //判断是否请求的是对应的特定类型
            {
                string[] val = new string[1];
                int idx = 0;
                val[0]= Encoding.Default.GetString(data);
                return val as T[];
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(string)) //判断是否请求的是对应的特定类型，例如uint
            {
                string[] vals = val as string[];
                byte[] data = Encoding.Default.GetBytes(vals[0]);


            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }
    }
    public class OB : LongVR //1byte str
    {
        public OB(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            uint idx = 0;
            string value;
            if (syntax.isBE)
            {
                Array.Reverse(data);
                Byte[] numob = data;
                idx += (uint)data.Length;
                value = numob.ToString();
            }
            else
            {
                Byte[] numob = data;
                idx += (uint)data.Length;
                value = numob.ToString();

            }
            return value;
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(Byte)) //判断是否请求的是对应的特定类型，例如uint
            {
                Byte[] val = new Byte[data.Length];
                uint idx = 0;
                val = data;
                idx += (uint)data.Length;
                return val as T[];
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(Byte)) //判断是否请求的是对应的特定类型，例如uint
            {
                Byte[] vals = val as Byte[];
                byte[] data = new byte[val.Length ];
                for (int i = 0; i < val.Length; i++)
                {
                    byte[] ss = BitConverter.GetBytes(vals[i]);
                    if (syntax.isBE) Array.Reverse(ss);
                    data[i ] = ss[0];
                    data[i + 1] = ss[1];
                }
                return data;
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }
    }
    public class OF : LongVR //4byte str
    {
        public OF(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            uint idx = 0;
            string value;
            if (syntax.isBE)
            {
                Array.Reverse(data);
                Single[] numof = Array.ConvertAll<byte, Single>(data, c => Convert.ToSingle(c));
                idx += (uint)data.Length;
                value = numof.ToString();

            }
            else
            {
                Single[] numof = Array.ConvertAll<byte, Single>(data, c => Convert.ToSingle(c));
                idx += (uint)data.Length;
                value = numof.ToString();

            }
            return value;
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(Single)) //判断是否请求的是对应的特定类型
            {
                    Single[] val = new Single[data.Length / 4];
                    uint idx = 0;
                    for (int i = 0; i < val.Length; i++)
                    val[i] = GetUInt(data, ref idx);
                    return val as T[];
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(Single)) //判断是否请求的是对应的特定类型
            {
                Single[] vals = val as Single[];
                byte[] data = new byte[val.Length * 4];
                for (int i = 0; i < val.Length; i++)
                {
                    byte[] ss = BitConverter.GetBytes(vals[i]);
                    if (syntax.isBE) Array.Reverse(ss);
                    data[i * 4] = ss[0];
                    data[i * 4 + 1] = ss[1];
                }
                return data;
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
        }
    }
    public class OW : LongVR //16bit str 2byte
    {
        public OW(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            uint idx = 0;
            string value;
            if (syntax.isBE)
            {
                Array.Reverse(data);
                UInt16[] numow = Array.ConvertAll<byte, UInt16>(data, c => Convert.ToUInt16(c));
                idx += (uint)data.Length;
                value = numow.ToString();

            }
            else
            {
                UInt16[] numow = Array.ConvertAll<byte, UInt16>(data, c => Convert.ToUInt16(c));
                idx += (uint)data.Length;
                value = numow.ToString();

            }
            return value;
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(ushort) || typeof(T) == typeof(short))
            {
                ushort[] val = new ushort[data.Length / 2];
                uint idx = 0;
                for (int i = 0; i < val.Length; i++)
                    val[i] = GetGroupTag(data, ref idx);
                return val as T[];
            }
            throw new NotSupportedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(ushort))
            {
                ushort[] vals = val as ushort[];
                byte[] data = new byte[val.Length * 2];
                for (int i = 0; i < val.Length; i++)
                {
                    byte[] ss = BitConverter.GetBytes(vals[i]);
                    if (syntax.isBE) Array.Reverse(ss);
                    data[i * 2] = ss[0];
                    data[i * 2 + 1] = ss[1];
                }
                return data;
            }
            throw new NotSupportedException();
        }

    }
    public class SQ : LongVR
    {
        public SQ(TransferSyntax syntax) : base(syntax) { }

        public override byte[] GetValue(byte[] data, ref uint idx, uint length)
        {
            uint offset = idx;
            //为每一层Item增加同层顺序编号
            DCMDataSequence sq = new DCMDataSequence(syntax);
            while (idx - offset < length)
            {
                //解码SQ值
                DCMAbstractType item = syntax.Decode(data, ref idx);
                if (length == 0xffffffff && item.gtag == 0xfffe && item.etag == 0xe0dd)
                    break;
                else
                    sq.items.Add((DCMDataItem)item);
            }
            GCHandle handle = GCHandle.Alloc(sq);
            IntPtr ptr = GCHandle.ToIntPtr(handle);
            return BitConverter.GetBytes(ptr.ToInt64());
        }

        public override string GetString(byte[] data, string head)
        {
            IntPtr ptr = new IntPtr(BitConverter.ToInt64(data, 0));
            GCHandle handle = GCHandle.FromIntPtr(ptr);
            DCMDataSequence sq = (DCMDataSequence)handle.Target;
            return sq.ToString(head + ">");
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(DCMDataSequence))
            {
                IntPtr ptr = new IntPtr(BitConverter.ToInt64(data, 0));
                GCHandle handle = GCHandle.FromIntPtr(ptr);
                DCMDataSequence[] sq = new DCMDataSequence[1];
                sq[0] = (DCMDataSequence)handle.Target;
                return sq as T[];
            }
            else
                throw new NotSupportedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T)==typeof(DCMDataSequence))
            {
                DCMDataSequence[] sq = val as DCMDataSequence[];
                GCHandle handle = GCHandle.Alloc(sq[0]);
                IntPtr ptr = GCHandle.ToIntPtr(handle);
                return BitConverter.GetBytes(ptr.ToInt64());
            }
            else
                throw new NotSupportedException();
        }



    }
    public class UT : LongVR
    {
        public UT(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            uint idx = 0;
            Int32 value = (Int32)GetLength(data, ref idx);
            return value.ToString();
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(Int32)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(Int32)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }
    }
    public class UN : LongVR
    {
        public UN(TransferSyntax syntax) : base(syntax) { }
        public override string GetString(byte[] data, string head)
        {
            uint idx = 0;
            Int32 value = (Int32)GetLength(data, ref idx);
            return value.ToString();
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(Int32)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(Int32)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }
    }


    public abstract class LongVR : VR
    {

        public LongVR(TransferSyntax syntax) : base(syntax) { }
        public override uint GetLength(byte[] data, ref uint idx)
        {
            if (syntax.isExplicit)
            {
                idx += 2;
                return GetUInt(data, ref idx);
            }
            else
                return GetGroupTag(data, ref idx);
        }
        public override T[] ReadValue<T>(byte[] data)
        {
            if (typeof(T) == typeof(uint)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }

        public override byte[] WriteValue<T>(T[] val)
        {
            if (typeof(T) == typeof(uint)) //判断是否请求的是对应的特定类型，例如uint
            {
                //相应转换处理
            }
            else           //抛出异常，提示应用程序读取的类型不对。
                throw new NotSupportedException();
            throw new NotImplementedException();
        }
    }
}

