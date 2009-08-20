using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JazCms.Kernel;
using System.Xml;
using System.IO;

namespace JazCms.StoreProviders.XmlStore
{
	public class XmlStoreProvider:ISettingStoreProvider, IStructureStoreProvider
	{
		#region ISettingStoreProvider Members

        private SettingConstructor settingConstructor;
        public SettingConstructor Constructor
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
            SettingConstructor sc = (SettingConstructor)owner.SettingCollection;
            string filePath = sc.FilePath;
            string fileName = sc.FileName;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlElement node = (XmlElement)xmlDoc.SelectSingleNode("//ExportFile[@FileName='" + fileName +"']");

            if (node != null)
            {
                string nameSpace = node.GetAttribute("Namespace");
                string className = node.GetAttribute("ClassName");
                sc = new SettingConstructor(filePath, fileName, nameSpace, className);
                sc.IsSetted = true;
                settingConstructor = sc;
            }
            else
            {
                sc.IsSetted = false;
                settingConstructor = sc;
            }
		}

		public void SaveSettings(ISettingOwner owner)
		{
            SettingConstructor sc = (SettingConstructor)owner.SettingCollection;
            string filePath = sc.FilePath;
            string fileName = sc.FileName;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlElement node = (XmlElement)xmlDoc.SelectSingleNode("//Settings");
            XmlElement existingNode = (XmlElement)xmlDoc.SelectSingleNode("//ExportFile[@FileName='" + fileName + "']");
            if (existingNode != null)
            {
                existingNode.SetAttribute("Namespace", sc.NameSpace);
                existingNode.SetAttribute("ClassName", sc.ClassName);
            }
            else
            {
                XmlElement child = node.OwnerDocument.CreateElement("ExportFile");
                child.SetAttribute("Namespace", sc.NameSpace);
                child.SetAttribute("ClassName", sc.ClassName);
                child.SetAttribute("FileName", sc.FileName);
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

    public class SettingConstructor : ISettingOwner, ISettingCollection
    {
        private string filePathSetting;
        private string nameSpaceSetting;
        private string classNameSetting;
        private string fileNameSetting;

        public string FilePath
        {
            get { return filePathSetting; }
        }

        public string NameSpace
        {
            get { return nameSpaceSetting; }
        }

        public string ClassName
        {
            get { return classNameSetting; }
        }

        public string FileName
        {
            get { return fileNameSetting; }
        }

        public SettingConstructor(string filePath, string fileName, string nameSpace, string className)
        {
            this.fileNameSetting = fileName;
            this.filePathSetting = filePath;
            this.nameSpaceSetting = nameSpace;
            this.classNameSetting = className;
        }

        public SettingConstructor( string filePath, string fileName)
        {
             this.filePathSetting = filePath;
             this.fileNameSetting = fileName;
             this.nameSpaceSetting = null;
             this.classNameSetting = null;
        }

          public ISettingCollection SettingCollection 
          {
              get
              {
                  return new SettingConstructor(filePathSetting, fileNameSetting, nameSpaceSetting, classNameSetting);
              }
          }

          public ISettingOwner SettingOwner
          {
              get
              {
                  return (ISettingOwner)this.SettingCollection;
              }
          }
          public Guid Identity { get; set; }
          public string SystemName { get; set; }
          public bool IsSetted { get; set; }
    }

    public class SettingCollectionConstructor 
    {
      
    }

    
}
