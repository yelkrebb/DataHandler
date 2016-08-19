using System;
using System.Threading.Tasks;

namespace Motion.Core.Data.BleData.FT900
{
	public class FT900DeviceInformation:FT900DataHandler
	{
		public FT900DeviceInformation()
		{
		}

		public Task<byte[]> GetReadCommand()
		{
			throw new NotImplementedException();
		}

		public Task<byte[]> GetWriteCommand()
		{
			throw new NotImplementedException();
		}

		public Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			throw new NotImplementedException();
		}
	}
}

