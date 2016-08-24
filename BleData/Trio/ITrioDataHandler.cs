using System;
using Motion.Core.Data.BleData;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.Trio.StepsData;

namespace Motion.Core.Data.BleData.Trio
{
	public interface ITrioDataHandler
	{
		Task<BLEParsingStatus> ParseData(byte[] rawData);
		Task<byte[]> GetReadCommand();
		Task<byte[]> GetWriteCommand();
	}
}

