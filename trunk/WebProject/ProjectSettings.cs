using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JazCms.Kernel;
using System.ComponentModel;

namespace JazCms.WebProject
{
    public class ProjectSettings: ISettingOwner, ISettingCollection
    {
        private string rootNameSpace;
        private string exportFileName;
        private List<string> insertedJazFiles;
        private List<string> _BasePageCollection;

        public ProjectSettings()
        {
         insertedJazFiles = new List<string>();
         _BasePageCollection = new List<string>();
        }

        [DisplayName("Path to settings file")]
        /// <summary>
        /// Name of settings file in xml format.
        /// </summary>
        public string ExportFileName
        {
            get 
            {
                return exportFileName; 
            }
            set
            {
                exportFileName = value;
            }
        }

        [DisplayName("Root namespace")]
        /// <summary>
        /// Root namespace found in opening project.
        /// </summary>
        public string RootNameSpace
        {
            get 
            {
                return rootNameSpace;
            }
            set 
            { 
                this.rootNameSpace = value;
            }
        }

        public void AddNewJazFile(string fileName)
        {
            insertedJazFiles.Add(fileName);
        }

        public List<string> GetJazFileCollection()
        {
            return insertedJazFiles;
        }

        
        public void RemoveJazFileFromCollection(string fileName)
        {
            insertedJazFiles.Remove(fileName);
        }

        public void ClearJazFileCollection()
        {
            insertedJazFiles.Clear();
        }

        private PageSettings pageSet;
        
        [DisplayName("Last loaded page")]
        public PageSettings SelectedPage
        {
            get 
            { 
                return pageSet; 
            }
            set
            { 
                pageSet = value; 
            }
        }

        public ISettingCollection SettingCollection 
          {
              get
              {
                  return this;
              }
          }

        protected bool _IsRefJCMSAdded;

        [DisplayName("Ref on JCMS added")]
        public bool IsRefJCMSAdded
        {
            get
            {
                return _IsRefJCMSAdded;
            }
            set
            {
                _IsRefJCMSAdded = value;
            }
        }

        protected bool _IsSiteLevelCreated;


        [DisplayName("Create site layer")]
        public bool IsSiteLevelCreated
        {
            get
            {
                return _IsSiteLevelCreated;
            }
            set
            {
                _IsSiteLevelCreated = value;
            }
        }

          public Guid Identity { get; set; }
          public string SystemName { get; set; }

          [DisplayName("Selected page contein xml settings")] 
          public bool IsSetted { get; set; }


          [DisplayName("Custom base pages")] 
          public List<string> BasePageCollection
          {
              get
              {
                  return _BasePageCollection;
              }

          }

          public void AddBasePageToCollection(string page)
          {
              _BasePageCollection.Add(page);
          }
    }
}
