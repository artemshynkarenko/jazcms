using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

namespace JazCms.WebProject
{
    public static class AssemblyRefCreator
    {
        public static bool IsAssemblyRefAdded(Assembly assembly, string docName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(docName);
            XmlNode root = doc.DocumentElement;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("ns", root.NamespaceURI);

            XmlNode refSelectedNode = root.SelectSingleNode("//ns:ItemGroup/ns:Reference[@Include='" +
                          assembly.FullName + "']", nsmgr);

            if (refSelectedNode == null)
                return false;

            return true;
        }

        public static void AssemblyRefCreateator(Assembly assembly, string docName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(docName);
            XmlNode root = doc.DocumentElement;
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("ns", root.NamespaceURI);
            
            XmlNode refSelectedNode = root.SelectSingleNode("//ns:ItemGroup/ns:Reference[@Include='" +
                          assembly.FullName + "']", nsmgr);
            if (refSelectedNode == null)
            {
                XmlNode refNode = root.SelectSingleNode("//ns:ItemGroup/ns:Reference[@Include]", nsmgr);
                XmlElement insertingRefNode = refNode.OwnerDocument.CreateElement("Reference", refNode.NamespaceURI);
                insertingRefNode.SetAttribute("Include", assembly.FullName);
                XmlElement specVerNode = refNode.OwnerDocument.CreateElement("SpecificVersion", refNode.NamespaceURI);
                specVerNode.InnerText = "False";
                XmlElement hintPathNode = refNode.OwnerDocument.CreateElement("HintPath", refNode.NamespaceURI);
                hintPathNode.InnerText = assembly.Location;
                insertingRefNode.AppendChild(specVerNode);
                insertingRefNode.AppendChild(hintPathNode);
                refNode.ParentNode.InsertBefore(insertingRefNode, refNode);
                refNode.OwnerDocument.Save(docName);
            }
        }
    }
}
