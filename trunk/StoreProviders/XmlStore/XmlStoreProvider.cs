using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JazCms.Kernel;
using System.Xml;
using System.IO;
using JazCms.WebProject;

namespace JazCms.StoreProviders.XmlStore
{
	public class XmlStoreProvider:ISettingStoreProvider, IStructureStoreProvider
	{
		#region ISettingStoreProvider Members

        private ProjectSettings settingConstructor;
        public ProjectSettings Constructor
        {
            get { return settingConstructor; }
        }


        public XmlStoreProvider(string xmlFilePath)
        {
            if (!File.Exists(xmlFilePath))
            {
                XmlWriterSettings xmlWriterSetting = new XmlWriterSettings();
                xmlWriterSetting.Indent = true;
                xmlWriterSetting.NewLineOnAttributes = true;
                XmlWriter writer = XmlWriter.Create(xmlFilePath, xmlWriterSetting);
                writer.WriteStartDocument();
                writer.WriteStartElement("Settings");
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
        }

		public void LoadSettings(ISettingOwner owner)
		{
            ProjectSettings prjset = (ProjectSettings)owner;
            PageSettings ps = (PageSettings)prjset.SelectedPage;
            string filePath = ps.FilePath;
            string fileName = ps.FileName;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlElement node = (XmlElement)xmlDoc.SelectSingleNode("//ExportFile[@FileName='" + fileName +"']");

            if (node != null)
            {
                string nameSpace = node.GetAttribute("Namespace");
                string className = node.GetAttribute("ClassName");
                PageSettings newPage = new PageSettings(filePath, fileName, nameSpace, className);
                
                prjset.SelectedPage = newPage;
                prjset.IsSetted = true;
                settingConstructor = prjset;
            }
            else
            {
                prjset.IsSetted = false;
                settingConstructor = prjset;
            }
		}

		public void SaveSettings(ISettingOwner owner)
		{
            ProjectSettings prjset = (ProjectSettings)owner;
            PageSettings ps = (PageSettings)prjset.SelectedPage;
            string filePath = ps.FilePath;
            string fileName = ps.FileName;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlElement node = (XmlElement)xmlDoc.SelectSingleNode("//Settings");
            XmlElement existingNode = (XmlElement)xmlDoc.SelectSingleNode("//ExportFile[@FileName='" + fileName + "']");
            if (existingNode != null)
            {
                existingNode.SetAttribute("Namespace", ps.NameSpace);
                existingNode.SetAttribute("ClassName", ps.ClassName);
            }
            else
            {
                XmlElement child = node.OwnerDocument.CreateElement("ExportFile");
                child.SetAttribute("Namespace", ps.NameSpace);
                child.SetAttribute("ClassName", ps.ClassName);
                child.SetAttribute("FileName", ps.FileName);
                node.AppendChild(child);
            }
            xmlDoc.Save(filePath);
		}

		#endregion

		#region IJazCmsObject Members

		public Guid Identity
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string SystemName
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}

}
