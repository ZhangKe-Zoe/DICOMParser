using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using DCMLIB;


namespace DicomParser
{
    public partial class frmImage : Form
    {
        short[] owpixels;    //OW像素缓冲区
        byte[] obpixels;    //OB像素缓冲区
        DCMDataSet items;
        //传入解码得到的DCMDataSet数据集对象并保存
        public frmImage(DCMDataSet items)
        {
            InitializeComponent();
            this.items = items;

        }
            
        private void frmImage_Load(object sender, EventArgs e)
        {
            //读取相关数据元素的值，并设置窗体、窗宽、窗位
            this.Width = items[DicomTags.Columns].ReadValue<ushort>()[0];
            this.Height = items[DicomTags.Rows].ReadValue<ushort>()[0];
            tsWindow.Text = items[DicomTags.WindowWidth].ReadValue<string>()[0];
            tsLevel.Text = items[DicomTags.WindowCenter].ReadValue<string>()[0];

            //获取图像像素IOM各数据元素的值
            ushort bs = items[DicomTags.BitsStored].ReadValue<ushort>()[0];
            ushort hb = items[DicomTags.HighBit].ReadValue<ushort>()[0];
            if (items[DicomTags.BitsAllocated].ReadValue<ushort>()[0] == 16)  //OW
            {
                OW ow = new OW(items[DicomTags.PixelData].vrparser.syntax);
                items[DicomTags.PixelData].vrparser = ow;
                owpixels = items[DicomTags.PixelData].ReadValue<short>();
                for (int i = 0; i < owpixels.Length; i++)
                {
                    //BitsAllocated右位移至最低位
                    owpixels[i] = (short)(owpixels[i] >> (hb - (bs - 1)));
                    //逐像素单元转换处理得到像素值，放入owpixels
                }
            }
            else
            {
                OB ob = new OB(items[DicomTags.PixelData].vrparser.syntax);
                items[DicomTags.PixelData].vrparser = ob;
                obpixels = items[DicomTags.PixelData].ReadValue<byte>();
                for (int i = 0; i < obpixels.Length; i++)
                {
                    //BitsAllocated右位移至最低位
                    obpixels[i] = (byte)(obpixels[i] >> (hb - (bs-1)));
                    //逐像素单元转换处理得到像素值，放入obpixels
                }
            }

        }

        private void frmImage_Paint(object sender, PaintEventArgs e)
        {
            //获取窗宽窗位
            double c, w;
            c = double.Parse(tsLevel.Text);
            w = double.Parse(tsWindow.Text);
            //获取绘图对象
            Bitmap bmp = new Bitmap(this.Width, this.Height, e.Graphics);
            //窗宽窗位变换与显示
            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                {
                    int idx = i * Width + j;
                    int pixel;
                    if (owpixels != null) //ow
                        pixel = owpixels[idx];
                    else  //ob
                        pixel = obpixels[idx];
                    //窗宽窗位变换
                    pixel = (int)(((pixel - c) / w + 0.5) * 255);
                    //显示为灰度值
                    Color p = Color.FromArgb(pixel, pixel, pixel);
                    bmp.SetPixel(j, i, p);
                    e.Graphics.DrawImage(bmp, 0, 0);
                }

        }

        private void tsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = tsList.Text;
            switch (s)
            {
                case "脑窗": { tsWindow.Text = "60"; tsLevel.Text = "35"; } break;
                case "肺窗": { tsWindow.Text = "700"; tsLevel.Text = "-600"; } break;
                case "骨窗": { tsWindow.Text = "1400"; tsLevel.Text = "600"; } break;
                default: { tsWindow.Text = "2000"; tsLevel.Text = "0"; } break;
            }

            
        }

        private void tsRefresh_Click(object sender, EventArgs e)
        {
            //刷新
        }

        

    }
}
