using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Data;
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
using JazCms.WebProject;
using JazCms.Kernel;
using Microsoft.Win32;
using JazCms.StoreProviders.XmlStore;
using System.Windows.Forms.Integration;

namespace JazCms.WebProject.WpfEditor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        protected string docName;
        protected string jazNamespace;
        protected string jazClassName;
        protected ProjectSettings progectSettings;

        const string OldFileExtension = ".aspx.cs";
        const string NewFileExtension = ".aspx.jaz.cs";

        public Window1()
        {
            docName = string.Empty;
            jazNamespace = string.Empty;
            jazClassName = string.Empty;
            progectSettings = new ProjectSettings();
            InitializeComponent();
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

            if (ofd.ShowDialog() == true)
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
                    string exportFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(docName), "ExportSetting.xml");
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
                        jazClassName = System.IO.Path.GetFileName(location).Replace(OldFileExtension, "");
                        string directory = System.IO.Path.GetDirectoryName(location).Replace("\\", ".");
                        string jazNamespaceNode = jazNamespace;
                        if (!string.IsNullOrEmpty(directory))
                            jazNamespaceNode = jazNamespace + "." + directory;

                        dataRow.Namespace = jazNamespaceNode;
                        dataRow.ClassName = jazClassName;

                        string exFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(docName), "ExportSetting.xml");
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
                    basePageListTable.Columns.Add("BasePage", typeof(string));

                    foreach (string page in progectSettings.BasePageCollection)
                    {
                        basePageListTable.Rows.Add(page);
                    }

                    List<string> hiddenColumns = new List<string>();

                    foreach (PropertyInfo property in typeof(DataRowComponents).GetProperties())
                    {
                        Type type = property.PropertyType;
                        DisplayNameAttribute[] propertyArrey = (DisplayNameAttribute[])
                            (property.GetCustomAttributes(typeof(DisplayNameAttribute), true));

                        HidePropertyAttribute[] hidePropertyArrey = (HidePropertyAttribute[])
                            (property.GetCustomAttributes(typeof(HidePropertyAttribute), true));

                        DataColumn col;
                        if (propertyArrey.Count() != 0)
                            col = new DataColumn(propertyArrey.First().DisplayName, type);
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
                    SortedList<string, DataRelation> relCollection = new SortedList<string, DataRelation>();

                    foreach (DataRelation rel in relationCollection)
                    {
                        if (rel.ChildTable.TableName == "DataRowComponentsCollection")
                            relCollection.Add(rel.ChildColumns.First().ColumnName, rel);
                    }

                    GridView gridview = new GridView();
                    listview.DataContext = dataTable;
                    Binding bind = new Binding();
                    listview.ItemsSource = dataTable as IEnumerable;

                    foreach (DataColumn col in
                        dataTable.Columns)
                    {
          
                        GridViewColumn gvcolumn = new GridViewColumn();
                        gvcolumn.Header = col.ColumnName;
                        if (hiddenColumns.Contains(col.ColumnName))
                            gvcolumn.Width = 0;
                      
                        if (relCollection.Keys.Contains(col.ColumnName))
                        {
                            DataColumn parentColumn = relCollection.Where(p => p.Key == col.ColumnName)
                                .Select(p => p.Value).First().ParentColumns.First();

                            System.Windows.Forms.DataGridViewComboBoxColumn comboBoxColumn = 
                                new System.Windows.Forms.DataGridViewComboBoxColumn();
                            comboBoxColumn.Name = col.ColumnName;

                            List<string> sourceList = new List<string>();
                            foreach (DataRow dgvRow in parentColumn.Table.Rows)
                                sourceList.Add(dgvRow[parentColumn.ColumnName].ToString());

                            comboBoxColumn.DataSource = sourceList.ToArray();
                            dataGridNodesTable.Columns.Add(comboBoxColumn);

                            if (hiddenColumns.Contains(col.ColumnName))
                                comboBoxColumn.Visible = false;

                            DataTemplate dtCombo = new DataTemplate();
                            dtCombo.DataType = typeof(String);
                            FrameworkElementFactory fefCombo = new FrameworkElementFactory(typeof(ComboBox));
                            Binding bdCombo = new Binding(col.ColumnName);

                            List<string> basePagesList = new List<string>();

                            foreach (DataRow comboRow in basePageListTable.Rows)
                            {
                                basePagesList.Add(comboRow.ItemArray.First().ToString());
                            }
                            ComboBox templateComboBox = new ComboBox();
                            templateComboBox.FontSize = 12;
                            fefCombo.SetValue(ComboBox.ItemsSourceProperty, basePagesList);
                            fefCombo.SetValue(ComboBox.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
                            fefCombo.SetValue(ComboBox.ForegroundProperty, new SolidColorBrush(Colors.Goldenrod));
                            fefCombo.SetValue(ComboBox.BorderBrushProperty, new SolidColorBrush(Colors.Transparent));
                            fefCombo.SetValue(ComboBox.FontSizeProperty, templateComboBox.FontSize);
                            fefCombo.SetBinding(ComboBox.SelectedItemProperty, bdCombo);
                            dtCombo.VisualTree = fefCombo;
                            gvcolumn.CellTemplate = dtCombo;
                            gridview.Columns.Add(gvcolumn);
                        }
                        else
                            if (col.DataType == typeof(bool))
                            {
                                System.Windows.Forms.DataGridViewCheckBoxColumn checkBoxColumn = 
                                    new System.Windows.Forms.DataGridViewCheckBoxColumn();
                                checkBoxColumn.Name = col.ColumnName;
                                dataGridNodesTable.Columns.Add(checkBoxColumn);

                                if (hiddenColumns.Contains(col.ColumnName))
                                    checkBoxColumn.Visible = false;

                                DataTemplate dtCheckbox = new DataTemplate();
                                dtCheckbox.DataType = typeof(Boolean);
                                FrameworkElementFactory fefCheckbox = new FrameworkElementFactory(typeof(CheckBox));
                                Binding bdCheckbox = new Binding(col.ColumnName);
                                fefCheckbox.SetBinding(CheckBox.IsCheckedProperty, bdCheckbox);
                                dtCheckbox.VisualTree = fefCheckbox;
                                gvcolumn.CellTemplate = dtCheckbox;
                                gridview.Columns.Add(gvcolumn);
                            }
                            else
                                if (col.DataType == typeof(string))
                                {
                                    System.Windows.Forms.DataGridViewTextBoxColumn textBoxColumn = 
                                        new System.Windows.Forms.DataGridViewTextBoxColumn();
                                    textBoxColumn.Name = col.ColumnName;
                                    textBoxColumn.ReadOnly = false;
                                    dataGridNodesTable.Columns.Add(textBoxColumn);

                                    if (hiddenColumns.Contains(col.ColumnName))
                                        textBoxColumn.Visible = false;


                                    DataTemplate dtTextBox = new DataTemplate();
                                    dtTextBox.DataType = typeof(string);
                                    FrameworkElementFactory fefTextBox = new FrameworkElementFactory(typeof(TextBox));
                                    Binding bdTextBox = new Binding(col.ColumnName);
                                    fefTextBox.SetBinding(TextBox.TextProperty, bdTextBox);
                                    TextBox templateTB = new TextBox();
                                    templateTB.FontSize = 12;

                                    if (col.ColumnName == "Path to file")
                                       fefTextBox.SetValue(TextBox.IsReadOnlyProperty, true);

                                    fefTextBox.SetValue(TextBox.BackgroundProperty, new SolidColorBrush(Colors.Transparent));
                                    fefTextBox.SetValue(TextBox.ForegroundProperty, new SolidColorBrush(Colors.Goldenrod));
                                    fefTextBox.SetValue(TextBox.FontSizeProperty, templateTB.FontSize);
                                    fefTextBox.SetValue(TextBox.BorderBrushProperty, new SolidColorBrush(Colors.Transparent));
                                    dtTextBox.VisualTree = fefTextBox;
                                    gvcolumn.CellTemplate = dtTextBox;
                                    gridview.Columns.Add(gvcolumn);
                                }
                                else
                                {
                                    System.Windows.Forms.DataGridViewTextBoxColumn column =
                                        new System.Windows.Forms.DataGridViewTextBoxColumn();
                                    column.Name = col.ColumnName;
                                    column.ReadOnly = false;
                                    dataGridNodesTable.Columns.Add(column);
                                    column.Visible = false;

                                    if (hiddenColumns.Contains(col.ColumnName))
                                        column.Visible = false;

                                    gvcolumn.Width = 0;
                                    gridview.Columns.Add(gvcolumn);
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

                    dataGridNodesTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    dataGridNodesTable.Columns["Path to file"].ReadOnly = true;
                    dataGridNodesTable.Columns["Tag"].Visible = false;

                    System.Windows.Forms.DataGridViewButtonColumn buttonColumn = 
                        new System.Windows.Forms.DataGridViewButtonColumn();
                    buttonColumn.Name = "Details";
                    dataGridNodesTable.Columns.Add(buttonColumn);

                    GridViewColumn buttonDetailColumn = new GridViewColumn();

                    DataTemplate dtButtonDetail = new DataTemplate();
                    dtButtonDetail.DataType = typeof(String);

                    Button templateButton = new Button();
                    templateButton.Width = 50;

                    FrameworkElementFactory fefForButton = new FrameworkElementFactory(typeof(Button));
                    fefForButton.SetValue(Button.ContentProperty, "...");
                    fefForButton.SetValue(Button.WidthProperty, templateButton.Width);
                    Binding bdButton = new Binding(dataTable.Columns["DataSource"].ColumnName);
                    fefForButton.SetBinding(Button.TagProperty, bdButton);
                    fefForButton.AddHandler(Button.ClickEvent, new RoutedEventHandler(innerButtonInListView_Click));

                    Binding bindButtonDetail = new Binding();
                    fefForButton.SetBinding(TextBox.NameProperty, bindButtonDetail);
                    dtButtonDetail.VisualTree = fefForButton;

                    buttonDetailColumn.CellTemplate = dtButtonDetail;

                    #region DataGrid menu

                    ContextMenu contextMenuVisibleColumns = new ContextMenu();
                    MenuItem existingJazFiles = new MenuItem() { Header = "Existing Jaz files", IsEnabled = false };
                    MenuItem path = new MenuItem() { Header = "Path to page", IsEnabled = false };
                    MenuItem classNameMenuItem = new MenuItem(){Header = "Class name"};
                    MenuItem namespaceMenuItem = new MenuItem() {Header = "Namespace"};
                    MenuItem basePageMenuItem = new MenuItem() { Header = "Base page" }; 
                    contextMenuVisibleColumns.Items.Add(existingJazFiles);
                    contextMenuVisibleColumns.Items.Add(path);
                    contextMenuVisibleColumns.Items.Add(classNameMenuItem);
                    contextMenuVisibleColumns.Items.Add(namespaceMenuItem);
                    contextMenuVisibleColumns.Items.Add(basePageMenuItem);
                    foreach (MenuItem contextItem in contextMenuVisibleColumns.Items)
                    {
                        contextItem.IsCheckable = true;
                        contextItem.IsChecked = true;

                         contextItem.Checked +=
                            new RoutedEventHandler(contextMenuStripDataGridView_CheckedChanged);
                         contextItem.Unchecked +=
                            new RoutedEventHandler(contextMenuStripDataGridView_CheckedChanged);
                        switch (contextItem.Header.ToString())
                        {
                            case "Class name": contextItem.Tag = "Guessed class name";
                                break;
                            case "Namespace": contextItem.Tag = "Guessed namespace";
                                break;
                            case "Base page": contextItem.Tag = "Custom base page";
                                break;
                       }
                    }

                    #endregion

                    FrameworkElementFactory fefHeaderMenuButton = new FrameworkElementFactory(typeof(Button));
                    fefHeaderMenuButton.SetValue(Button.ContentProperty, "Details");
                    fefHeaderMenuButton.SetValue(Button.ContextMenuProperty, contextMenuVisibleColumns);
                    DataTemplate dtHeaderMenuButton = new DataTemplate();
                    dtHeaderMenuButton.VisualTree = fefHeaderMenuButton;
                    buttonDetailColumn.HeaderTemplate = dtHeaderMenuButton;

                    gridview.Columns.Add(buttonDetailColumn);
                    listview.View = gridview;
                    listview.SetBinding(ListView.ItemsSourceProperty, bind);
                    listview.Focus();
  
                    foreach (System.Windows.Forms.DataGridViewRow row in dataGridNodesTable.Rows)
                    {
                        if ((bool)row.Cells["Existing jaz files"].Value)
                            {
                                row.Cells["Guessed class name"].ReadOnly = true;
                                row.Cells["Guessed class name"].Style.BackColor = System.Drawing.Color.LightGray;
                                row.Cells["Guessed class name"].Style.ForeColor = System.Drawing.Color.Gray;
                                row.Cells["Guessed namespace"].ReadOnly = true;
                                row.Cells["Guessed namespace"].Style.BackColor = System.Drawing.Color.LightGray;
                                row.Cells["Guessed namespace"].Style.ForeColor = System.Drawing.Color.Gray;
                            }

                            row.Cells["Details"].Value = "...";
                    }

                    #endregion

                    //Content = listview;

                    //end list view
                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridNodesTable_CellContentClick(object sender, System.Windows.Forms.DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                System.Windows.Forms.DataGridViewCell cell = dataGridNodesTable.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (cell.Value != null && cell.Value.ToString() == "...")
                {
                    SettingsPage psForm = new SettingsPage();
                    psForm.Title = "FILE SETTINGS";
                    System.Windows.Forms.PropertyGrid propertyGridProjectSettings = psForm.propertyGridProjectSettings;
                    DataRowComponents source = (DataRowComponents)dataGridNodesTable.Rows[e.RowIndex].Cells["DataSource"].Value;
                    propertyGridProjectSettings.SelectedObject = source;
                    psForm.Show();

                }
            }
        }

        private void contextMenuStripDataGridView_CheckedChanged(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;

            if (!item.IsChecked)
                ((GridView)listview.View).Columns.Where(c => c.Header.ToString() == item.Tag.ToString()).First().Width = 0;
            else
                ((GridView)listview.View).Columns.Where(c => c.Header.ToString() == item.Tag.ToString()).First().Width = double.NaN;

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

                    foreach (DataRow row in ((DataView)listview.ItemsSource).Table.Rows)
                    {
                        DataRowComponents rowData = (DataRowComponents)row["DataSource"];
                        XmlNode insertedNode = rowData.Tag as XmlNode;
                        Uri uri = new Uri(docName);
                        XmlElement newNode = insertedNode.OwnerDocument.CreateElement("Compile", insertedNode.NamespaceURI);

                        string insertedNodeName = insertedNode.Attributes.GetNamedItem("Include").Value;
                        string parsedInsertedNodeName = insertedNodeName;

                        parsedInsertedNodeName = System.IO.Path.GetFileName(insertedNodeName);

                        newNode.SetAttribute("Include", insertedNodeName.Replace("aspx.cs", "aspx.jaz.cs"));

                        XmlElement dependentUponNode =
                            insertedNode.OwnerDocument.CreateElement("DependentUpon", insertedNode.NamespaceURI);
                        dependentUponNode.InnerText = parsedInsertedNodeName.Replace(".aspx.cs", ".aspx");
                        newNode.AppendChild(dependentUponNode);

                        if ((bool)row["Existing jaz files"])
                        {
                            if (insertedNode.ParentNode.SelectNodes("*[@Include='"+insertedNodeName.Replace(OldFileExtension,NewFileExtension)+"']").Count == 0)
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
                                string ns = row["Guessed namespace"].ToString();
                                string cn = row["Guessed class name"].ToString();
                                FileGenerator.GenerateCode
                                    (
                                     provider,
                                     FileGenerator.BuildJAZContent(ns, cn, insertedNodeName),
                                     fileFullPath
                                    );
                                string exportFilePath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(docName), "ExportSetting.xml");
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
                    Host.Visibility = Visibility.Hidden;

                    if (((DataView)listview.ItemsSource).Table.Rows.Count != 0)
                    {
                        MessageBox.Show("Modify proccess successed", "executed", MessageBoxButton.OK);
                    }
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
            Host.Visibility = Visibility.Hidden;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.buttonCreate_Click(sender, e);
        }

        private void projectSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsPage settingForm = new SettingsPage();
            settingForm.Title = "PROJECT SETTINGS";
            System.Windows.Forms.PropertyGrid propertyGridProjectSettings = settingForm.propertyGridProjectSettings;
            propertyGridProjectSettings.SelectedObject = this.progectSettings;
            settingForm.Show();
        }


        private void innerButtonInListView_Click(object sender, RoutedEventArgs e)
        {
            SettingsPage psForm = new SettingsPage();
            psForm.Title = "FILE SETTINGS";
            System.Windows.Forms.PropertyGrid propertyGridProjectSettings = psForm.propertyGridProjectSettings;
            DataRowComponents source = (DataRowComponents)((Button)sender).Tag;
            propertyGridProjectSettings.SelectedObject = source;
            psForm.Show();
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
