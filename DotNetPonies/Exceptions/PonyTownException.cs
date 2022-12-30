using System;

namespace DotNetPonies.Exceptions
{

	[Serializable]
	public class PonyTownException : Exception
	{
		public PonyTownException() { }
		public PonyTownException(string message) : base(message) { }
		public PonyTownException(string message, Exception inner) : base(message, inner) { }
		protected PonyTownException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
