using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JazCms.Kernel
{
	/// <summary>
	/// This is root
	/// </summary>
	public interface IApplication:IStructureElement, ISettingOwner, IRequestProcessor, IStructureInstance<IApplication>
	{
	}
}
