using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace DCMLIB
{
    public abstract class DCMAbstractType
    { 
        // 组
        public ushort gtag;    
        // 元素号        
        public ushort etag;        
        // 数据元素名        
        public string name;       
        // 值表示法       
        public string vr;        
        // 多值        
        public string vm;
        // 值长度       
        public uint length;
        // 值
        public byte[] value;
        // 获取解码
        public abstract string ToString(string head);
        //VR
        public VR vrparser;
        //idx
        public uint idx;
        //ReadValue/WriteValue模板方法
        public virtual void WriteValue<T>(T[] val)
        {
            value = vrparser.WriteValue<T>(val);
            length = (uint)value.Length;
        }

        public virtual T[] ReadValue<T>()
        {
            return vrparser.ReadValue<T>(value);
        }

    }

    public class DCMDataElement : DCMAbstractType
    {
        //返回各字段的字符串，比如用”\t”分割的各字段的值。
        public override string ToString(string head)
        {
            string str = head;//头部
            str += gtag.ToString("X4") + "," + etag.ToString("X4") + "\t";
            str += vr + "\t";
            str += name + "\t";
            str += length.ToString();
            str += "\t";
            //value怎么返回字符串需要根据不同VR
            str += vrparser.GetString(value, head);
            //str += “”;
            return str;
        } 
    }

    public class DCMDataSet : DCMAbstractType
    {
        // 容纳数据元素或条目
        public List<DCMAbstractType> items;
        //TransferSyntax的关联
        protected TransferSyntax syntax;
        //传输语法由构造函数传入
        public DCMDataSet(TransferSyntax syntax)   
        {
            this.syntax = syntax;
            items = new List<DCMAbstractType>();//储存完整的一条item
        }
        //返回各字段的字符串，比如用”\t”分割的各字段的值。
        public override string ToString(string head)
        {
            string str = "";
            foreach (DCMAbstractType item in items)
            {
                if (item != null)
                {
                    if (str != "") str += "\n";  //两个数据元素之间用换行符分割
                    str += item.ToString(head);
                }
            }
            return str;
        }
        // 解码
        public virtual List<DCMAbstractType> Decode(byte[] data, ref uint idx)
        {
            while (idx < data.Length)
            {
                DCMAbstractType item = null;
                //此处调用传输语法对象解码一条数据元素
                item = syntax.Decode(data, ref idx);
                //存入容器
                items.Add(item);
            }
            return items;
        }
        //DCMDataSet索引成员
        public virtual DCMAbstractType this[uint idx]
        {
            get
            {
                //拉姆达表达式(匿名函数)比较tag与索引idx是否相等，返回索引结果
                DCMAbstractType item = items.Find(elem => (uint)(elem.gtag << 16) + (elem.etag) == idx);
                return item;
            }
            set
            {
                DCMAbstractType val = (DCMAbstractType)value;
                DCMAbstractType item = items.Find(elem => (uint)(elem.gtag << 16 + (elem.etag)) == idx);
                if (item == null)  //not exists
                    items.Add(val);
                else
                {
                    item.length = val.length;
                    item.value = val.value;
                }
            }
        }

    }

    public class DCMDataItem : DCMDataSet
    {
        public DCMDataItem(TransferSyntax syntax) : base(syntax) { }
    }

    public class DCMDataSequence : DCMDataSet
    {
        public DCMDataSequence(TransferSyntax syntax) : base(syntax) { }
        //为每一层Item增加同层顺序编号
        public override string ToString(string head)
        {
            string str = "";
            int i = 1;
            foreach (DCMAbstractType item in items)
            {
                str += "\n" + head + "ITEM" + i.ToString() + "\n";
                str += item.ToString(head);
                i++;
            }
            return str;
        }
    }

    public class DCMFile : DCMDataSet
    {
        protected string filename;//从构造函数传入待打开的文件名
        protected DCMFileMeta filemeta;//保存解码的头元素对象
        protected TransferSyntax.TransferSyntaxes tsFactory;//查找数据集传输语法
        //将syntax初始化为默认传输语法
        public DCMFile(string filename) : base(new ImplicitVRLittleEndian())
        {
            this.filename = filename;
            tsFactory = new TransferSyntax.TransferSyntaxes();
        }
        //文件解码
        public override List<DCMAbstractType> Decode(byte[] data, ref uint idx)
        {
            //打开filename文件，将文件内容读到缓冲区byte[] data
            data = File.ReadAllBytes(filename);
            //跳过128字节前导符(idx=128)
            idx += 128;
            //读取四字节标识，如不为“DICM”则结束
            string s = Encoding.Default.GetString(data,(int)idx,4);
            if (s == "DICM")
            {
                idx += 4;
                //用ExplicitVRLittleEndian对象实例化filemeta对象
                filemeta = new DCMFileMeta(new ExplicitVRLittleEndian());
                //通过其Decode方法从data中读取头元素
                filemeta.Decode(data, ref idx);
                //读取(0002,0010)头元素  
                string uid = Encoding.Default.GetString( filemeta[DCMLIB.DicomTags.TransferSyntaxUid].value);
                //在tsFactory中找到对应的数据集传输语法对象赋给基类的syntax字段
                foreach (TransferSyntax syn in tsFactory.TSs)
                {
                    if ((syn.uid+"\0") == uid)
                    {
                       base.syntax = syn;
                        break;
                    }
                       
                }
                //调用base.Decode方法解码数据集
                return (base.Decode(data, ref idx));
            }
            else return (base.Decode(data, ref idx));
        }
        public override string ToString(string head)
        {
            /*先调用filemeta.ToString方法，然后调用base.ToString方法，
            将两次调用得到的字符串拼接起来*/
            return (filemeta.ToString(head) + base.ToString(head));
        }
    }

    //处理文件头元素
    public class DCMFileMeta : DCMDataSet
    {
        public DCMFileMeta(TransferSyntax syntax):base(syntax)
        {
            this.syntax = syntax;
        }
        public override List<DCMAbstractType> Decode(byte[] data, ref uint idx)
        {
            while (idx < data.Length)
            {
                DCMAbstractType metaitem = null;
                uint i = idx;//保存原始idx
                //此处调用传输语法对象解码一条数据元素
                metaitem = syntax.Decode(data, ref idx);
                if (metaitem.gtag == 0x0002)//将文件头元素存入容器
                {
                    items.Add(metaitem);
                }
                else//修改idx退回到缓冲区
                {
                    idx = items[items.Count - 1].idx;
                    break;
                }
            }
            return items;
        }
    }
    
  
    
}
