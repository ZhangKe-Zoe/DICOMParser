using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DICOM
{
    //保存dicom
    public class DicomDictionary
    {
        public List<DicomDictonaryEntry> list = new List<DicomDictonaryEntry>();

        public void Read(string path)
        {
            //读取文件 
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            //文件按数据属性存入entry，entry存入list
            while ((line = sr.ReadLine()) != null)
            {
                string[] array = line.Split('\t');
                DicomDictonaryEntry entry = new DicomDictonaryEntry();
                entry.Tag = array[0];
                entry.Name = array[1];
                entry.Keyword = array[2];
                entry.Vr = array[3];
                entry.Vm = array[4];
                list.Add(entry);
                Console.WriteLine("{0},{1},{2},{3},{4}", entry.Tag, entry.Name, entry.Keyword, entry.Vr, entry.Vm);
            }
        }

        //根据TAG查询
        public DicomDictonaryEntry Find(string TAG)
        { 
            //设置flag
            bool t = false;
            int i;
            int j = 0;
            //遍历查找
            for (i = 0; i < list.Count; i++)
            {
                while(list[i].Tag == TAG)
                {
                    //查找成功，修改flag
                    t = true;
                    j = i;
                    
                    break;
                }

            }
            //查找失败报错
            if (t != true)
            {
                //"查询失败"        
            }
            return list[j];

        }
        public DicomDictonaryEntry GetEntry(ushort gtag,ushort etag)
        {
            string TAG = "(" +gtag.ToString("X4") + "," +etag.ToString("X4") + ")";

            //设置flag
            bool t = false;
            int i;
            int j = 0;
            //遍历查找
            for (i = 0; i < list.Count; i++)
            {
                while (list[i].Tag == "\"" + TAG + "\"")
                {
                    //查找成功，修改flag
                    t = true;
                    j = i;

                    break;
                }

            }
            //查找失败报错
            if (t != true)
            {
                //"查询失败"        
            }
            return list[j];

        }


        class Program
        {
            static void Main(string[] args)
            {
                DicomDictionary dicomDictionary = new DicomDictionary();
                dicomDictionary.Read(@"C:\Users\macbook\Desktop\Homework\DICOM\dicom.txt");
                Console.WriteLine("请输入要查询的TAG:");
                string TAG = Console.ReadLine();
                dicomDictionary.Find("\"" + TAG + "\"");
                Console.ReadLine();
            }
        }
    }
}
