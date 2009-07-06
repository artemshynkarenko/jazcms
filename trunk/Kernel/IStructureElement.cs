using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace JazCms.Kernel
{
	[Serializable]
	public interface IStructureElement:IJazCmsObject
	{
		Collection<IStructureElement> ChildElements { get; }
		IStructureElement ParentElement { get; }
		Collection<IStructureExtension> Extensions { get; }
	}
}
