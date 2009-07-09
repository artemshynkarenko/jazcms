using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JazCms.Kernel
{
	public interface IModuleInstance:IStructureInstance<IModule>, IModule
	{
	}
}
