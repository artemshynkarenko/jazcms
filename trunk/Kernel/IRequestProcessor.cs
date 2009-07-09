using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace JazCms.Kernel
{
	public interface IRequestProcessor:IJazCmsObject
	{
		bool IsMyRequest { get; }
		void ProcessRequest(HttpRequest request);
		IRequestProcessor GetNextProcessor();
	}
}
