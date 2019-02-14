using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DCMLIB;
using System.IO;

namespace DicomParser
{
    public partial class DPUI : Form
    {
        public DPUI()
        {
            InitializeComponent();
        }

        private void DPUI_Load(object sender, EventArgs e)
        {
            //TSs数组初始化cbTransferSyntax控件的items
            TransferSyntax.TransferSyntaxes tsfactory = new TransferSyntax.TransferSyntaxes();
            cbTransferSyntax.Items.Clear();
            foreach (TransferSyntax syntax in tsfactory.TSs)
            {
                cbTransferSyntax.Items.Add(syntax);
            }

        }

        private void btnparse_Click(object sender, EventArgs e)
        {
            byte[] data = HexStringToByteArray(RtxtInput.Text);
            //解码到数据集对象
            //传输语法由构造函数传入
            DCMDataSet ds = new DCMDataSet((TransferSyntax)cbTransferSyntax.SelectedItem);
            //初始化索引
            uint idx = 0;
            //DCMDataSet解码
            ds.Decode(data, ref idx);
            //数据集转换为字符串显示
            string str = ds.ToString("");
            string[] lines = str.Split('\n');
            lvOutput.Items.Clear();
            for (int i = 0; i < lines.Length; i++)
            {
                ListViewItem item = new ListViewItem(lines[i].Split('\t'));
                lvOutput.Items.Add(item);                
            }
            //根据内容设置listview大小
            for (int i =0;i<5;i++)
            {
                lvOutput.Columns[i].Width = -1;               
            }        
        }
        //将txtInput控件中的输入十六进制串转换为byte数组
        private byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            return buffer;

        } 
        //OpenFileDialog openFile = new OpenFileDialog();
            //openFile.ShowDialog();
            //string path = openFile.FileName;
            //RtxtInput.Text = File.ReadAllText(path);
        //从文件读取内容到文本框
        private void btnfile_Click(object sender, EventArgs e)
        {  
            OpenFileDialog openFileDlg = new OpenFileDialog();
            openFileDlg.Filter = "Dicom文件|*.dcm";
            if (openFileDlg.ShowDialog() == DialogResult.OK)
            {
                RtxtInput.Text = File.ReadAllText(openFileDlg.FileName);
                DCMFile dcm = new DCMFile(openFileDlg.FileName);
                uint idx = 0;
                dcm.Decode(null, ref idx);
                string str = dcm.ToString("");
                string[] lines = str.Split('\n');
                lvOutput.Items.Clear();
                for (int i = 0; i < lines.Length; i++)
                {
                    ListViewItem item = new ListViewItem(lines[i].Split('\t'));
                    lvOutput.Items.Add(item);
                }
                //DCMDataSet对象实例化FrmImage，并show
                frmImage frmImage = new frmImage(dcm);
                frmImage.Show();
            }

        }

        private void lvOutput_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
