using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JazCms.Kernel;

namespace JazCms.WebProject
{
	public abstract class ProjectItem
	{
		public abstract bool IsExtended { get; protected set; }
		public abstract IStructureElement Element { get; protected set; }
		public abstract IStructureElement Extend();
	}
}
