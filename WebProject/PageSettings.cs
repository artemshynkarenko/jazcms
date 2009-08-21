using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JazCms.Kernel;

namespace JazCms.WebProject
{
    public class PageSettings : ISettingCollection
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

        public PageSettings(string filePath, string fileName, string nameSpace, string className)
        {
            this.fileNameSetting = fileName;
            this.filePathSetting = filePath;
            this.nameSpaceSetting = nameSpace;
            this.classNameSetting = className;
        }

        public PageSettings(string filePath, string fileName)
        {
             this.filePathSetting = filePath;
             this.fileNameSetting = fileName;
             this.nameSpaceSetting = null;
             this.classNameSetting = null;
        }
    }
}
