﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JazCms.Kernel
{
	public interface IPage : IStructureElement, ISettingOwner
	{
		IPageContentBuilder ContentBuilder { get; set; }
	}
}
