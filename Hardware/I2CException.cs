using System;
using Microsoft.SPOT;

namespace Technobotts.Hardware
{
	public class I2CException : ApplicationException
	{
		public I2CException(String message = "Lost communication with the I2C device") : base(message) { }
	}
}
