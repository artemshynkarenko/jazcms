using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace JazCms.WebProject.WinEditor
{
    public partial class MainForm : Form
    {
        protected string docName;

        public MainForm()
        {
            docName = string.Empty;
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "project files (*.csproj)|*.csproj|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    docName = ofd.FileName.ToString();
                    doc.Load(docName);

                    XmlNode root = doc.DocumentElement;

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("ns", root.NamespaceURI);

                    XmlNodeList nodeList = root.SelectNodes("//ns:Compile", nsmgr);

                    List<XmlNode> nodeCollection = new List<XmlNode>();
                    foreach (XmlNode node in nodeList)
                    {
                        if (node.Attributes.GetNamedItem("Include").Value.Contains(".aspx.cs"))
                        {
                            nodeCollection.Add(node);
                        }
                    }
                    int checkBoxPossition = 40;
                    foreach (XmlNode selectedNodes in nodeCollection)
                    {
                        string location = selectedNodes.Attributes.GetNamedItem("Include").Value;

                        CheckBox checkBoxEditedNodes = new CheckBox();
                        checkBoxEditedNodes.Text = location;
                        string xPath = "//ns:Compile[@Include='" + location.Replace(".aspx.cs", ".aspx.jaz.cs") + "']";
                        XmlNodeList jazNodesList = root.SelectNodes(xPath, nsmgr);
                        if (jazNodesList.Count > 0)
                            checkBoxEditedNodes.Checked = true;
                        checkBoxEditedNodes.Tag = selectedNodes;
                        checkBoxEditedNodes.Top = checkBoxPossition;
                        checkBoxEditedNodes.Left = 50;
                        checkBoxEditedNodes.Width = checkBoxEditedNodes.Text.Length * 12;
                        checkBoxPossition += 40;
                        groupBoxEditedNodes.Height = checkBoxPossition;
                        groupBoxEditedNodes.Controls.Add(checkBoxEditedNodes);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(docName))
            {
                try
                {
                    #region nodes cration
                    foreach (CheckBox cBSelectedNodes in groupBoxEditedNodes.Controls)
                    {
                        XmlNode insertedNode = cBSelectedNodes.Tag as XmlNode;
                        Uri uri = new Uri(docName);
                        XmlElement newNode = insertedNode.OwnerDocument.CreateElement("Compile", insertedNode.NamespaceURI);

                        string insertedNodeName = insertedNode.Attributes.GetNamedItem("Include").Value;
                        string parsedInsertedNodeName = insertedNodeName;

                        parsedInsertedNodeName = Path.GetFileName(insertedNodeName);

                        newNode.SetAttribute("Include", insertedNodeName.Replace("aspx.cs", "aspx.jaz.cs"));

                        XmlElement dependentUponNode =
                            insertedNode.OwnerDocument.CreateElement("DependentUpon", insertedNode.NamespaceURI);
                        dependentUponNode.InnerText = parsedInsertedNodeName.Replace(".aspx.cs", ".aspx");
                        newNode.AppendChild(dependentUponNode);

                        if (cBSelectedNodes.Checked)
                        {
                            if (insertedNode.ParentNode.SelectNodes("*[@Include='" +
                                insertedNodeName.Replace("aspx.cs", "aspx.jaz.cs") + "']").Count == 0)
                            {
                                insertedNode.ParentNode.InsertAfter(newNode, insertedNode);
                                insertedNode.OwnerDocument.Save(docName);
                                string fileFullPath = docName.Replace(uri.Segments[uri.Segments.Length - 1].ToString(), "") +
                                         insertedNode.Attributes.GetNamedItem("Include").Value
                                         .Replace("aspx.cs", "aspx.jaz.cs");
                                FileInfo newFile = new FileInfo(fileFullPath);
                                FileStream fs = newFile.Create();
                                fs.Dispose();
                            }
                        }
                        else
                        {
                            XmlNode removedNode = insertedNode.ParentNode.SelectSingleNode("*[@Include='" +
                                insertedNodeName.Replace("aspx.cs", "aspx.jaz.cs") + "']");
                            if (removedNode != null)
                            {

                                insertedNode.ParentNode.RemoveChild(removedNode);
                                insertedNode.OwnerDocument.Save(docName);

                                FileInfo newFile =
                                new FileInfo(docName.Replace(uri.Segments[uri.Segments.Length - 1].ToString(), "") +
                                             insertedNode.Attributes.GetNamedItem("Include").Value
                                             .Replace("aspx.cs", "aspx.jaz.cs"));
                                newFile.Delete();
                            }

                        }
                    }
                    MessageBox.Show("Modify proccess successed", "executed", MessageBoxButtons.OK);
                    #endregion

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {

                }
            }

        }
    }
}
