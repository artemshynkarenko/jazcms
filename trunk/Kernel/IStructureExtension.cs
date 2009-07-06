using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace JazCms.Kernel
{
	[Serializable]
	public interface IStructureExtension:IJazCmsObject
	{
		bool CanExtend(IStructureElement element);
		bool CanExtend(IStructureExtension extension);
		Collection<IStructureExtension> ChildExtensions { get; }
		IStructureElement ParentElement { get; }
		IStructureExtension ParentExtention { get; }
	}
}
