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
        protected ProjectSettings progectSettings;

        const string OldFileExtension = ".aspx.cs";
        const string NewFileExtension = ".aspx.jaz.cs";

        public MainForm()
        {
            docName = string.Empty;
            jazNamespace = string.Empty;
            jazClassName = string.Empty;
            progectSettings = new ProjectSettings();
            InitializeComponent();
            this.SizeChanged += new EventHandler(page_SizeChanged);
        }

        protected void page_SizeChanged(object sender, EventArgs e)
        {
            dataGridNodesTable.Width = this.Width - 50;
            dataGridNodesTable.Height = this.Height - 200;
            buttonCreate.Top = dataGridNodesTable.Height + 50;
            buttonCreate.Left = dataGridNodesTable.Width - 150;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridNodesTable.Rows.Clear();
            dataGridNodesTable.Columns.Clear();

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = "c:\\";
            ofd.Filter = "project files (*.csproj)|*.csproj|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    #region CheckBox list creator

                    progectSettings = new ProjectSettings();

                    dataGridNodesTable.UseWaitCursor = true;
                    XmlDocument doc = new XmlDocument();
                    docName = ofd.FileName.ToString();
                    doc.Load(docName);
                    XmlDocument docExport = new XmlDocument();
                    string exportFilePath = Path.Combine(Path.GetDirectoryName(docName), "ExportSetting.xml");
                    progectSettings.ExportFileName = exportFilePath;

                    XmlNode root = doc.DocumentElement;

                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                    nsmgr.AddNamespace("ns", root.NamespaceURI);

                    Assembly assembly = typeof(IPageInstance).Module.Assembly;
                    progectSettings.IsRefJCMSAdded = AssemblyRefCreator.IsAssemblyRefAdded(assembly, docName);
                    progectSettings.AddBasePageToCollection("test");
                    progectSettings.AddBasePageToCollection("test2");


                    XmlNode nameSpace = root.SelectSingleNode("//ns:RootNamespace", nsmgr);
                    jazNamespace = nameSpace.InnerText;
                    progectSettings.RootNameSpace = jazNamespace;

                    XmlNodeList nodeList = root.SelectNodes("//ns:Compile", nsmgr);
                    List<XmlNode> nodeCollection = new List<XmlNode>();
                    foreach (XmlNode node in nodeList)
                    {
                        if (node.Attributes.GetNamedItem("Include").Value.Contains(OldFileExtension))
                        {
                            nodeCollection.Add(node);
                        }
                    }

                    List<DataRowComponents> dataRowCollection = new List<DataRowComponents>();

                    foreach (XmlNode selectedNodes in nodeCollection)
                    {
                        DataRowComponents dataRow = new DataRowComponents();
                        dataRow.IsSelected = false;

                        string location = selectedNodes.Attributes.GetNamedItem("Include").Value;

                        dataRow.Text = location;
                        string xPath = "//ns:Compile[@Include='" + location.Replace(OldFileExtension, NewFileExtension) + "']";
                        XmlNodeList jazNodesList = root.SelectNodes(xPath, nsmgr);
                        jazClassName = Path.GetFileName(location).Replace(OldFileExtension, "");
                        string directory = Path.GetDirectoryName(location).Replace("\\", ".");
                        string jazNamespaceNode = jazNamespace;
                        if (!string.IsNullOrEmpty(directory))
                            jazNamespaceNode = jazNamespace + "." + directory;

                        dataRow.Namespace = jazNamespaceNode;
                        dataRow.ClassName = jazClassName;

                        string exFilePath = Path.Combine(Path.GetDirectoryName(docName), "ExportSetting.xml");
                        PageSettings pageSet = new PageSettings(exFilePath, location);
                        XmlStoreProvider storeProvider = new XmlStoreProvider(exFilePath);
                        progectSettings.SelectedPage = pageSet;
                        storeProvider.LoadSettings(progectSettings);

                        if (progectSettings.IsSetted)
                        {
                            dataRow.Namespace = progectSettings.SelectedPage.NameSpace;
                            dataRow.ClassName = progectSettings.SelectedPage.ClassName;
                        }

                        if (jazNodesList.Count > 0)
                        {
                            dataRow.IsSelected = true;
                        }

                        dataRow.Tag = selectedNodes;
                        dataRow.BasePage = "test2";
                        dataRowCollection.Add(dataRow);
                    }


                    dataGridNodesTable.AutoGenerateColumns = false;

                    DataSet dataSet = new DataSet("JazCmsDataSet");
                    DataTable dataTable = new DataTable("DataRowComponentsCollection");
                    DataTable basePageListTable = new DataTable("BasePageListTable");
                    basePageListTable.Columns.Add("BasePage",typeof(string));

                    foreach (string page in progectSettings.BasePageCollection)
                    {
                        basePageListTable.Rows.Add(page);
                    }

                    List<string> hiddenColumns = new List<string>(); 

                    foreach(PropertyInfo property in typeof(DataRowComponents).GetProperties())
                    {
                        Type type = property.PropertyType;
                        DisplayNameAttribute[] propertyArrey  = (DisplayNameAttribute[])
                            (property.GetCustomAttributes(typeof(DisplayNameAttribute), true));

                        HidePropertyAttribute[] hidePropertyArrey = (HidePropertyAttribute[])
                            (property.GetCustomAttributes(typeof(HidePropertyAttribute), true));

                        DataColumn col;
                        if (propertyArrey.Count()!= 0)
                            col = new DataColumn(propertyArrey.First().DisplayName,type);
                        else
                            col = new DataColumn(property.Name, type);

                        dataTable.Columns.Add(col);

                        if (hidePropertyArrey.Count() != 0 && hidePropertyArrey.First().IsHidden)
                            hiddenColumns.Add(col.ColumnName);
                    }

                    DataColumn dataSource = new DataColumn("DataSource", typeof(DataRowComponents));
                    dataTable.Columns.Add(dataSource);

                    foreach (DataRowComponents row in dataRowCollection)
                    {
                        dataTable.Rows.Add(row.IsSelected, row.Text, row.ClassName, row.Namespace, row.BasePage, row.Tag, row);
                    }

                    dataSet.Tables.Add(dataTable);
                    dataSet.Tables.Add(basePageListTable);

                    DataColumn child = dataSet.Tables["DataRowComponentsCollection"].Columns["Custom base page"];
                    DataColumn parent = dataSet.Tables["BasePageListTable"].Columns["BasePage"];
                    parent.Unique = true;
                    ForeignKeyConstraint fk = new ForeignKeyConstraint("FK_BasePage", parent, child);
                    dataSet.Tables["DataRowComponentsCollection"].Constraints.Add(fk);
                    dataSet.Relations.Add("BasePage",
                        parent,
                        child);

                    DataRelationCollection relationCollection = dataSet.Relations;
                    SortedList<string,DataRelation> relCollection = new SortedList<string,DataRelation>();

                    foreach (DataRelation rel in relationCollection)
                    {
                        if (rel.ChildTable.TableName == "DataRowComponentsCollection")
                            relCollection.Add(rel.ChildColumns.First().ColumnName, rel);
                    }

                    foreach (DataColumn col in 
                        dataTable.Columns)
                    {
                        if (relCollection.Keys.Contains(col.ColumnName))
                        {
                            DataColumn parentColumn = relCollection.Where(p => p.Key == col.ColumnName)
                                .Select(p => p.Value).First().ParentColumns.First();

                            DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn();
                            comboBoxColumn.Name = col.ColumnName;

                            List<string> sourceList = new List<string>();
                            foreach (DataRow dgvRow in parentColumn.Table.Rows)
                                sourceList.Add(dgvRow[parentColumn.ColumnName].ToString());

                            comboBoxColumn.DataSource = sourceList.ToArray();
                            dataGridNodesTable.Columns.Add(comboBoxColumn);

                            if(hiddenColumns.Contains(col.ColumnName))
                                comboBoxColumn.Visible = false;
                        }
                        else
                            if (col.DataType == typeof(bool))
                            {
                                DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
                                checkBoxColumn.Name = col.ColumnName;
                                dataGridNodesTable.Columns.Add(checkBoxColumn);

                                if (hiddenColumns.Contains(col.ColumnName))
                                    checkBoxColumn.Visible = false;
                            }
                            else
                                if (col.DataType == typeof(string))
                                {
                                    DataGridViewTextBoxColumn textBoxColumn = new DataGridViewTextBoxColumn();
                                    textBoxColumn.Name = col.ColumnName;
                                    dataGridNodesTable.Columns.Add(textBoxColumn);

                                    if (hiddenColumns.Contains(col.ColumnName))
                                        textBoxColumn.Visible = false;
                                }
                                else
                                {
                                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                                    column.Name = col.ColumnName;
                                    dataGridNodesTable.Columns.Add(column);
                                    column.Visible = false;

                                    if (hiddenColumns.Contains(col.ColumnName))
                                        column.Visible = false;
                                }
                    }

                    foreach (DataRowComponents row in dataRowCollection)
                    {

                        dataGridNodesTable.Rows.Add(
                            row.IsSelected, row.Text, row.ClassName, row.Namespace 
                           , row.BasePage,
                           row.Tag, row
                            );
                    }

                    dataGridNodesTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    dataGridNodesTable.Columns["Path to file"].ReadOnly = true;
                    dataGridNodesTable.Columns["Tag"].Visible = false;
                   
                    DataGridViewButtonColumn buttonColumn = new DataGridViewButtonColumn();
                    buttonColumn.Name = "Details";
                    dataGridNodesTable.Columns.Add(buttonColumn);

                    foreach (DataGridViewRow row in dataGridNodesTable.Rows)
                    {
                        if ((bool)row.Cells["Existing jaz files"].Value)
                        {
                            row.Cells["Guessed class name"].ReadOnly = true;
                            row.Cells["Guessed namespace"].ReadOnly = true;
                        }

                        row.Cells["Details"].Value = "...";
                    }

                    #endregion

                    buttonCreate.Top = dataGridNodesTable.Height + 100;
                    buttonCreate.Left = dataGridNodesTable.Left + dataGridNodesTable.Width - buttonCreate.Width;
                    dataGridNodesTable.UseWaitCursor = false;

                   
                    foreach (ToolStripMenuItem item in contextMenuStripDataGridView.Items)
                    {
                        item.CheckedChanged +=
                            new EventHandler(contextMenuStripDataGridView_CheckedChanged);
                        switch (item.Text)
                        {
                            case "Class name": item.Tag = "Guessed class name";
                                break;
                            case "Namespace": item.Tag = "Guessed namespace";
                                break;
                            case "Base page": item.Tag = "Base page";
                                break;
                        }
                    }
                    #region DataGrid menu

                    dataGridNodesTable.CellMouseClick -= new DataGridViewCellMouseEventHandler(dataGridNodesTable_CellContentClick);
                    dataGridNodesTable.CellMouseClick += new DataGridViewCellMouseEventHandler(dataGridNodesTable_CellContentClick);
                    dataGridNodesTable.Columns["Details"].HeaderCell.ContextMenuStrip
                        = contextMenuStripDataGridView;
                    dataGridNodesTable.EnableHeadersVisualStyles = false;
                    dataGridNodesTable.Columns["Details"].HeaderCell.Style.ForeColor = Color.Blue;
         
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

        private void dataGridNodesTable_CellContentClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                DataGridViewCell cell = dataGridNodesTable.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Value != null && cell.Value.ToString() == "...")
                {
                    ProgectSettingForm psForm = new ProgectSettingForm();
                    PropertyGrid propertyGridProjectSettings = (PropertyGrid)
                    psForm.Controls.Find("propertyGridProjectSettings", true).First();
                    DataRowComponents source = (DataRowComponents) dataGridNodesTable.Rows[e.RowIndex].Cells["DataSource"].Value;
                    propertyGridProjectSettings.SelectedObject = source;
                    psForm.Show();

                }
            }
        }

        private void contextMenuStripDataGridView_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;

            if (!item.Checked)
                dataGridNodesTable.Columns[item.Tag.ToString()].Visible = false;
            else
                dataGridNodesTable.Columns[item.Tag.ToString()].Visible = true;
        
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(docName))
            {
                try
                {
                    Assembly assembly = typeof(IPageInstance).Module.Assembly;
                    AssemblyRefCreator.AssemblyRefCreateator(assembly, docName);

                    #region nodes cration

                    foreach (DataGridViewRow row in dataGridNodesTable.Rows)
                    {
                        DataRowComponents rowData = (DataRowComponents)row.Cells["DataSource"].Value;
                        XmlNode insertedNode = rowData.Tag as XmlNode;
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

                        if ((bool)row.Cells["Existing jaz files"].Value)
                        {
                            if (insertedNode.ParentNode.SelectNodes("*[@Include='" +
                                insertedNodeName.Replace(OldFileExtension, NewFileExtension) + "']").Count == 0)
                            {
                                insertedNode.ParentNode.InsertAfter(newNode, insertedNode);
                                insertedNode.OwnerDocument.Save(docName);
                                string fileFullPath = docName.Replace(uri.Segments[uri.Segments.Length - 1].ToString(), "") +
                                         insertedNode.Attributes.GetNamedItem("Include").Value
                                         .Replace(OldFileExtension, NewFileExtension);
                                progectSettings.AddNewJazFile(fileFullPath);
                                FileInfo newFile = new FileInfo(fileFullPath);
                                FileStream fs = newFile.Create();
                                fs.Dispose();
                                CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
                                string ns = row.Cells["Guessed namespace"].Value.ToString();
                                string cn = row.Cells["Guessed class name"].Value.ToString();
                                FileGenerator.GenerateCode
                                    (
                                     provider,
                                     FileGenerator.BuildJAZContent(ns, cn, insertedNodeName),
                                     fileFullPath
                                    );
                                string exportFilePath = Path.Combine(Path.GetDirectoryName(docName), "ExportSetting.xml");
                                PageSettings pageSet = new PageSettings(exportFilePath, insertedNodeName, ns, cn);
                                XmlStoreProvider storeProvider = new XmlStoreProvider(exportFilePath);
                                progectSettings.SelectedPage = pageSet;
                                storeProvider.SaveSettings(progectSettings);
                            }
                        }
                        else
                        {
                            XmlNode removedNode = insertedNode.ParentNode.SelectSingleNode("*[@Include='" +
                                insertedNodeName.Replace(OldFileExtension, NewFileExtension) + "']");
                            if (removedNode != null)
                            {

                                insertedNode.ParentNode.RemoveChild(removedNode);
                                insertedNode.OwnerDocument.Save(docName);
                                string unselectedFileName = docName.Replace(uri.Segments[uri.Segments.Length - 1].ToString(), "") +
                                             insertedNode.Attributes.GetNamedItem("Include").Value
                                             .Replace(OldFileExtension, NewFileExtension);
                                FileInfo newFile = new FileInfo(unselectedFileName);
                                newFile.Delete();

                                if (progectSettings.GetJazFileCollection().Contains(unselectedFileName))
                                    progectSettings.RemoveJazFileFromCollection(unselectedFileName);
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
                    dataGridNodesTable.Rows.Clear();
                    dataGridNodesTable.Columns.Clear();
                }
            }

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridNodesTable.Rows.Clear();
            dataGridNodesTable.Columns.Clear();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.buttonCreate_Click(sender, e);
        }

        private void projectSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgectSettingForm settingForm = new ProgectSettingForm();
            PropertyGrid propertyGridProjectSettings = (PropertyGrid)
                settingForm.Controls.Find("propertyGridProjectSettings",true).First();
            propertyGridProjectSettings.SelectedObject = this.progectSettings;
            settingForm.Show();
        }
   }

    public class DataRowComponents
    {
        private bool _IsSelected;
        private string _Text;
        private string _ClassName;
        private string _Namespace;
        private string _BasePage;

        [DisplayName("Existing jaz files")]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
            }

        }

        [DisplayName("Path to file")]
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
            }
        }

        [DisplayName("Guessed class name")]
        public string ClassName
        {
            get
            {
                return _ClassName;
            }
            set
            {
                _ClassName = value;
            }
        }

        [DisplayName("Guessed namespace")]
        public string Namespace
        {
            get
            {
                return _Namespace;
            }
            set
            {
                _Namespace = value;
            }
        }

        [DisplayName("Custom base page")]
        public string BasePage
        {
            get
            {
                return _BasePage;
            }
            set
            {
                _BasePage = value;
            }
        }

       
        private object _Tag;
        [HideProperty(true)]
        public object Tag
        {
            get
            {
                return _Tag;
            }
            set
            {
                _Tag = value;
            }

        }

    }

}
