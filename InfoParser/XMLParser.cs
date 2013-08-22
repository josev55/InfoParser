using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Shell32;
using System.IO;
using System.Diagnostics;
using HtmlAgilityPack;
using Ionic.Zip;
using System.Web;


namespace InfoParser
{
    public class XMLParser
    {
        private String xmlFile;
        private String styleSheetFile;
        private int idCounter;
        private String fileName;
        private List<ConnectionModel> comboboxConnections;
        private HtmlDocument htmlDoc = new HtmlDocument();
        private int nodeIndex = 0;

        private List<String> supportedTags = new List<string>()
        {
            "input",
            "div",
            "table",
            "span",
            "select",
            "button",
            "label",
            "img"
        };


        public String FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }

        public XMLParser()
        {
            comboboxConnections = new List<ConnectionModel>();
        }

        public XMLParser(String Xml, String Stylesheet)
        {
            xmlFile = Xml;
            styleSheetFile = Stylesheet;
            idCounter = 0;
            comboboxConnections = new List<ConnectionModel>();
        }

        public String XmlFile
        {
            get { return xmlFile; }
            set { xmlFile = value; }
        }

        public String StyleSheetFile
        {
            get { return styleSheetFile; }
            set { styleSheetFile = value; }
        }

        public void XslToHTML()
        {
            XslCompiledTransform transform = new XslCompiledTransform();
            transform.Load(styleSheetFile);
            transform.Transform(xmlFile, HttpContext.Current.Server.MapPath("/tmp") + "/Formulario.html");
            htmlDoc.Load("C:\\Form1\\Formulario.html");
            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//span[@class='xdTextBox']"))
            {
                node.Name = "input";
            }

            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//div[@class='xdDTPicker']"))
            {
                node.Name = "input";
                foreach (HtmlNode node2 in node.ChildNodes)
                {
                    if (node2.Name.Equals("span"))
                    {
                        node.SetAttributeValue("xd:binding", node2.Attributes["xd:binding"].Value);
                        node.SetAttributeValue("onclick", "setFecha(this)");
                    }
                }
                node.RemoveAllChildren();
                HtmlAttribute attr = htmlDoc.CreateAttribute("type", "date");
                node.Attributes.Prepend(attr);
                node.SetAttributeValue("style", "width: 100%;");

                node.SetAttributeValue("onclick", "setFecha(this)");

            }
            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//input | //select"))
            {
                node.SetAttributeValue("id", (idCounter++).ToString());
                if (node.Attributes["type"]!=null &&node.Attributes["type"].Value.Equals("button"))
                {
                    node.SetAttributeValue("onclick", "save()");
                }
            }

            HtmlNode head = htmlDoc.DocumentNode.SelectSingleNode("//head");
            HtmlNode body = htmlDoc.DocumentNode.SelectSingleNode("//body");
            HtmlNode meta = new HtmlNode(HtmlNodeType.Element, htmlDoc, nodeIndex++);
            HtmlNode script = new HtmlNode(HtmlNodeType.Element, htmlDoc, nodeIndex++);

            script = new HtmlNode(HtmlNodeType.Element, htmlDoc, nodeIndex++);
            script.Name = "script";
            script.Attributes.Add("type", "text/javascript");
            script.SetAttributeValue("src", "tmp.js");
            head.ChildNodes.Add(script);

            meta.Name = "meta";
            meta.Attributes.Add("content", "width=device-width");
            head.ChildNodes.Add(meta);
            meta = new HtmlNode(HtmlNodeType.Element, htmlDoc, nodeIndex++);
            meta.Name = "meta";
            meta.Attributes.Add("property", "maxid");
            meta.Attributes.Add("content", idCounter.ToString());
            head.ChildNodes.Add(meta);

            HtmlNode link = new HtmlNode(HtmlNodeType.Element, htmlDoc, nodeIndex++);
            link.Name = "link";
            link.Attributes.Add("href", "common/bootstrap-responsive.min.css");
            head.ChildNodes.Add(link);
            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//body/div"))
            {
                node.Attributes.Add("class", "row-fluid");
            }
            this.addSpan(htmlDoc);
            
            htmlDoc.Save("C:\\Form1\\Formulario.html");
            idCounter = 0;

            
        }

        public void ParseXSLConnections()
        {
            XmlDocument xsl = new XmlDocument();
            xsl.Load("C:\\Form1\\view1.xsl");
            XmlNodeList selects = xsl.GetElementsByTagName("select");
            
            foreach (XmlNode node in selects)
            {
                ConnectionModel model = new ConnectionModel();
                model.CBId = node.Attributes["xd:CtrlId"].Value;
                #region xml parse
                foreach (XmlNode node1 in node.ChildNodes)
                {
                    if (node1.Name.Equals("xsl:choose"))
                    {
                        foreach (XmlNode node2 in node1.ChildNodes)
                        {
                            if (node2.Name.Equals("xsl:when"))
                            {
                                foreach (XmlNode node3 in node2.ChildNodes)
                                {
                                    if (node3.Name.Equals("xsl:for-each"))
                                    {
                                        model.ConnectionName = node3.Attributes["select"].Value;
                                        
                                        
                                        if (model.ConnectionName.Contains("[d:"))
                                        {
                                            model.HasRelationship = true;
                                            String[] relationship = this.LocateRelationship(model.ConnectionName).Split('=');
                                            model.RelationshipMember = relationship[0];
                                            model.RelatedProperty = relationship[1];
                                        }
                                        foreach (XmlNode node4 in node3.ChildNodes)
                                        {
                                            if (node4.Name.Equals("option"))
                                            {
                                                foreach (XmlNode node5 in node4.ChildNodes)
                                                {
                                                    if (node5.Name.Equals("xsl:attribute") && node5.Attributes["name"] != null)
                                                    {
                                                        model.CBValue = node5.FirstChild.Attributes["select"].Value;
                                                    }
                                                    if (node5.Name.Equals("xsl:value-of"))
                                                    {
                                                        model.CBText = node5.Attributes["select"].Value;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        model.cleanInfo();
                        comboboxConnections.Add(model);
                    }
                }
                #endregion
                                        
            }
            this.IncrustOptions();
        }

        private void IncrustOptions()
        {
            foreach (HtmlNode node in htmlDoc.DocumentNode.SelectNodes("//select"))
            {
                foreach (var model in comboboxConnections)
                {
                    if (model.CBId == node.Attributes["xd:CtrlId"].Value)
                    {
                        XmlDocument selectOptions = new XmlDocument();
                        selectOptions.Load("C:\\Form1\\" + model.ConnectionName);
                        XmlNodeList options = selectOptions.GetElementsByTagName("z:row");
                        foreach (XmlNode option in options)
                        {
                            HtmlNode optionNode = new HtmlNode(HtmlNodeType.Element, htmlDoc, nodeIndex++);
                            optionNode.Name = "option";
                            optionNode.Attributes.Add("value", option.Attributes["ows_" + model.CBValue].Value);
                            optionNode.InnerHtml = option.Attributes["ows_" + model.CBText].Value;
                            node.AppendChild(optionNode);
                        }
                    }
                }
            }
            htmlDoc.Save("C:\\Form1\\Formulario.html");
            this.Pack();
        }

        public void ExtractXSN(String pathXSN, String destDir)
        {
            FileInfo XSN = new FileInfo(pathXSN);
            fileName = XSN.Name.Substring(0, XSN.Name.Length - 4);
            String commandText = "/c expand -F:* " + pathXSN + " " + destDir;
            Process proc = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = commandText;
            proc.StartInfo = startInfo;
            proc.Start();
            proc.WaitForExit();
        }

        private void Pack()
        {
            String filepath = String.Format("C:\\Form1\\{0}.zip", fileName);
            FileInfo fileInfo = new FileInfo(filepath);
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }                                                                                  
            ZipFile packedForm = new ZipFile(filepath, Encoding.UTF8);

            List<string> files = new List<String>() { "C:\\Form1\\Formulario.html", "tmp.js", this.createInfoFile().FullName };
            packedForm.AddFiles(files, false, "\\");
            packedForm.Save();

        }

        private String readScript(FileInfo scriptPath)
        {
            String stringFile = "";
            using (StreamReader reader = new StreamReader(scriptPath.FullName))
            {
                stringFile = reader.ReadToEnd();
            }
            return stringFile;
        }

        private void addSpan(HtmlDocument htmlDocument)
        {

        }

        private FileInfo createInfoFile()
        {
            XmlDocument infoXml = new XmlDocument();
            XmlElement root = infoXml.CreateElement("config");
            XmlElement id = infoXml.CreateElement("id");

            id.InnerText = Guid.NewGuid().ToString();
            XmlElement name = infoXml.CreateElement("name");
            name.InnerText = fileName;
            XmlElement version = infoXml.CreateElement("version");
            version.InnerText = "1";
            XmlElement lastCopy = infoXml.CreateElement("lastCopy");
            lastCopy.InnerText = "0";
            root.AppendChild(id);
            root.AppendChild(name);
            root.AppendChild(version);
            root.AppendChild(lastCopy);
            root.SetAttribute("xmlns", "http://cl.colabra.infocloud");
            infoXml.AppendChild(root);
            String xmlOutputPath = "C:\\Form1\\info.xml";
            infoXml.Save(xmlOutputPath);
            return new FileInfo(xmlOutputPath);
        }

        private String LocateRelationship(String relationshipString)
        {
            var startingIndex = relationshipString.IndexOf("[");
            var endingIndex = relationshipString.IndexOf("]");
            var tmpResult = relationshipString.Substring(startingIndex, (endingIndex - startingIndex));
            var relatedRightIndex = tmpResult.LastIndexOf("/");
            var startOfRelatedRight = tmpResult.IndexOf("xdXDocument:get-DOM()");
            var tmpRelated = tmpResult.Replace(tmpResult.Substring(startOfRelatedRight, (relatedRightIndex - startOfRelatedRight)), "");
            tmpRelated = tmpRelated.Replace("[", "").Replace("]","").Replace("my:","").Replace("_x0020_"," ");
            return tmpRelated;
        }
    }
}
