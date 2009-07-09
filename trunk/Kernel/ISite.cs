using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JazCms.Kernel
{
	public interface ISite : IStructureElement, ISettingOwner, IRequestProcessor
	{
		IPageContentBuilder ContentBuilder { get; set; }
	}
}
