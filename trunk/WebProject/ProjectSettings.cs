using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JazCms.Kernel;

namespace JazCms.WebProject
{
    public class ProjectSettings: ISettingOwner, ISettingCollection
    {
        private string rootNameSpace;
        private List<string> insertedJazFiles;

        public ProjectSettings()
        {
         insertedJazFiles = new List<string>();
        }

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

          public Guid Identity { get; set; }
          public string SystemName { get; set; }
          public bool IsSetted { get; set; }
    }
}
