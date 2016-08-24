using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.FT900;

namespace Motion.Core.Data.BleData.FT900.StepsData
{
	public class StepsTableParameters
	{

		public int dbYear { get; set; }
		public int dbMonth { get; set; }
		public int dbDay { get; set; }
		public int dbHourNumber { get; set; }
		public int sentHourFlag { get; set; }
		public int signatureGenerated { get; set; }
		public int signatureSent { get; set; }
		public int fraudTable { get; set; }

		internal StepsTableParameters()
		{

		}
	}

	public class FT900StepsTableData:FT900DataHandler
	{

		const int COMMAND_PREFIX_READ = 0x03;
		const int COMMAND_ID_READ = 0x22;
		const int COMMAND_ID_WRITE = 0x25;
		const int INDEX_ZERO = 0;

		public FT900StepsTableData()
		{
		}

		public Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			throw new NotImplementedException();
		}

		public Task<byte[]> GetReadCommand()
		{
			throw new NotImplementedException();
		}

		public Task<byte[]> GetWriteCommand()
		{
			throw new NotImplementedException();
		}
	}
}

