using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Text;

namespace DotNetPonies.Exceptions
{

	[Serializable]
	public class PonyTownForbiddenException : Exception
	{
		public PonyTownForbiddenException() : base("Can't make the request to pony town, the result is Forbidden") { }
		public PonyTownForbiddenException(string message) : base(message) { }
		public PonyTownForbiddenException(string message, Exception inner) : base(message, inner) { }
		protected PonyTownForbiddenException(
          System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
