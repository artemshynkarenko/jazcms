using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JazCms.Kernel;

namespace JazCms.WebProject
{
	public class Configuration
	{
		public string ProjectFile { get; set; }
		public ISettingStoreProvider SettingsStoreProvider { get; set; }
		public IStructureStoreProvider StructureStoreProvider { get; set; }
	}
}
