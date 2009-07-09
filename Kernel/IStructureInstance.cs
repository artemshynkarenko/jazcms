using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JazCms.Kernel
{
	public interface IStructureInstance<T> where T:IStructureElement
	{
		T Element { get; set; }
	}
}
