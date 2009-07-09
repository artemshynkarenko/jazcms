using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JazCms.Kernel
{
	public interface ISettingStoreProvider:IStoreProvider
	{
		void LoadSettings(ISettingOwner owner);
		void SaveSettings(ISettingOwner owner);
	}
}
