using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DCMLIB
{
    public class VRFactory
    {
        protected TransferSyntax syntax;
        //定义一个Hashtable用于存储享元对象，实现享元池
        private Hashtable VRs = new Hashtable();
        public VRFactory(TransferSyntax syntax)
        {
            this.syntax = syntax;
        }

        public VR GetVR(string key)
        {
            //如果对象存在，则直接从享元池获取
            if (VRs.ContainsKey(key))
            {
                return (VR)VRs[key];
            }
            //如果对象不存在，先创建一个新的对象添加到享元池中，然后返回
            else
            {
                VR fw = null;
                switch (key)
                {
                    case "SS": fw = new SS(syntax); break;
                    case "US": fw = new US(syntax); break;
                    case "SL": fw = new SL(syntax); break;
                    case "UL": fw = new UL(syntax); break;
                    case "FL": fw = new FL(syntax); break;
                    case "FD": fw = new FD(syntax); break;
                    case "DA": fw = new DA(syntax); break;
                    case "TM": fw = new TM(syntax); break;
                    case "DT": fw = new DT(syntax); break;
                    case "OB": fw = new OB(syntax); break;
                    case "OF": fw = new OF(syntax); break;
                    case "OW": fw = new OW(syntax); break;
                    case "SQ": fw = new SQ(syntax); break;
                    case "UT": fw = new UT(syntax); break;
                    case "UN": fw = new UN(syntax); break;
                    //default for text
                    default: fw = new ST(syntax); break;
                }
                VRs.Add(key, fw);
                return fw;
            }
        }

    }

}
