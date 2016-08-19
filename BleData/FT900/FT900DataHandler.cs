using System;
using Motion.Core.Data.BleData;
using System.Threading.Tasks;


namespace Motion.Core.Data.BleData.FT900
{
	public interface FT900DataHandler
	{
		Task<BLEParsingStatus> ParseData(byte[] rawData);
		Task<byte[]> GetReadCommand();
		Task<byte[]> GetWriteCommand();
	}
}

