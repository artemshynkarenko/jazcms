using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JazCms.Kernel;

namespace JazCms.StoreProviders.XmlStore
{
	public class XmlStoreProvider:ISettingStoreProvider, IStructureStoreProvider
	{
		#region ISettingStoreProvider Members

		public void LoadSettings(ISettingOwner owner)
		{
			throw new NotImplementedException();
		}

		public void SaveSettings(ISettingOwner owner)
		{
			throw new NotImplementedException();
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
