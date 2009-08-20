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
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Collections;
using System.Diagnostics;
using Microsoft.CSharp;
using System.Reflection;
using System.ComponentModel.Design;
using JazCms.Kernel;
using JazCms.WebProject;
using JazCms.StoreProviders.XmlStore;

namespace JazCms.WebProject.WinEditor
{
     
    public partial class MainForm : Form
    {
        protected string docName;
        protected string jazNamespace;
        protected string jazClassName;

        public MainForm()
        {
            docName = string.Empty;
            jazNamespace = string.Empty;
            jazClassName = string.Empty;
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
                    #region CheckBox list creator and inserting reference

                    XmlDocument doc = new XmlDocument();
                    docName = ofd.FileName.ToString();
                    doc.Load(docName);
                    XmlDocument docExport = new XmlDocument();
                    string exportFilePath = Path.Combine(Path.GetDirectoryName(docName), "ExportSetting.xml");

                    XmlNode root = doc.DocumentElement;

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("ns", root.NamespaceURI);

                    #region assembly reference creation

                    Assembly assembly = typeof(IPageInstance).Module.Assembly;
                    AssemblyRefCreator.AssemblyRefCreateator(assembly, docName);

                    #endregion

                    XmlNode nameSpace = root.SelectSingleNode("//ns:RootNamespace", nsmgr);
                    jazNamespace = nameSpace.InnerText;
                    
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
                        else
                        {
                            jazClassName = Path.GetFileName(location).Replace(".aspx.cs","");
                            string directory = Path.GetDirectoryName(location).Replace("//",".");
                            string jazNamespaceNode = jazNamespace;
                            if (!string.IsNullOrEmpty(directory))
                                jazNamespaceNode = jazNamespace + "." + directory;

                            TextBox tbClassName = new TextBox() 
                            { 
                                Name = "tbClassName"+location, 
                                Text = jazClassName 
                            };
                            tbClassName.Top = checkBoxPossition;
                                tbClassName.Left = 50 + location.Length * 11;
                            TextBox tbNameSpace = new TextBox() 
                            {
                                Name = "tbNameSpace" + location,
                                Text = jazNamespaceNode 
                            };
                            tbNameSpace.Left = tbClassName.Width +location.Length * 11 + 70;
                            tbNameSpace.Top = checkBoxPossition;

                            string exFilePath = Path.Combine(Path.GetDirectoryName(docName), "ExportSetting.xml");
                            XmlStoreProvider storeProvider = new XmlStoreProvider(exFilePath);
                            SettingConstructor setting = new SettingConstructor(exFilePath, location);
                            storeProvider.LoadSettings(setting.SettingOwner);
                            SettingConstructor sc = storeProvider.Constructor;

                            if (sc.IsSetted)
                            {
                                tbNameSpace.Text = sc.NameSpace;
                                tbClassName.Text = sc.ClassName;
                            }
                            groupBoxEditedNodes.Controls.Add(tbNameSpace);
                            groupBoxEditedNodes.Controls.Add(tbClassName);
                        }
                        checkBoxEditedNodes.Tag = selectedNodes;
                        checkBoxEditedNodes.Top = checkBoxPossition;
                        checkBoxEditedNodes.Left = 50;
                        checkBoxEditedNodes.Width = checkBoxEditedNodes.Text.Length * 11;
                        checkBoxPossition += 40;
                        groupBoxEditedNodes.Height = checkBoxPossition;
                        groupBoxEditedNodes.Controls.Add(checkBoxEditedNodes);
                        
                    }

                    if (groupBoxEditedNodes.Controls.Count > 0)
                    {
                        labelGuess.Visible = true;
                        labelGuess.Enabled = true;
                        labelGuess.Text = "guessed namespaces and class names:";
                    }
                    else
                        labelGuess.Visible = false;

                    #endregion
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

                    foreach (Control SelectedNodes in groupBoxEditedNodes.Controls)
                    {
                        
                        if (SelectedNodes.Tag != null)
                        {
                            CheckBox cBSelectedNodes = (CheckBox)SelectedNodes;

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
                                    CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                                    TextBox tbNS = (TextBox)groupBoxEditedNodes.Controls.Find("tbNameSpace" + insertedNodeName, true).First();
                                    TextBox tbCN = (TextBox)groupBoxEditedNodes.Controls.Find("tbClassName" + insertedNodeName, true).First();
                                    FileGenerator.GenerateCode
                                        (
                                         provider, 
                                         FileGenerator.BuildJAZContent(tbNS.Text, tbCN.Text, insertedNodeName),
                                         fileFullPath
                                        );
                                    string exportFilePath = Path.Combine(Path.GetDirectoryName(docName), "ExportSetting.xml");
                                    XmlStoreProvider storeProvider = new XmlStoreProvider(exportFilePath);
                                    SettingConstructor setting = new SettingConstructor(exportFilePath, insertedNodeName,tbNS.Text, tbCN.Text);
                                    storeProvider.SaveSettings(setting.SettingOwner);


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
                   groupBoxEditedNodes.Controls.Clear();
                   labelGuess.Visible = false;
                }
            }

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            groupBoxEditedNodes.Controls.Clear();
            labelGuess.Visible = false;
        }
    }
}
