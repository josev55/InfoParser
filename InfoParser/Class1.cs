using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InfoParser
{
    class Class1
    {
        public static int Main(String[] args)
        {
            XMLParser parser = new XMLParser();

            
            parser.ExtractXSN("C:\\Form1\\Prueba_1_Ingreso_LCU_Mobil.xsn", "C:\\Form1");
            foreach (String archivo in Directory.GetFiles("C:\\Form1"))
            {
                FileInfo fileInfo = new FileInfo(archivo);
                if (fileInfo.Extension.Equals(".xsl"))
                {
                    parser.StyleSheetFile = archivo;
                }
                if (fileInfo.Name.Contains("template") && fileInfo.Extension.Equals(".xml"))
                {
                    parser.XmlFile = archivo;
                }
            }
            Console.WriteLine("XML File: {0}", parser.XmlFile);
            Console.WriteLine("Stylesheet File: {0}", parser.StyleSheetFile);
            parser.XslToHTML();
            parser.ParseXSLConnections();
            Console.ReadKey();
            return 0;
        }
    }
}
