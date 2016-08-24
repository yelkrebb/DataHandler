using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data
{

	public class SeizureBlocksParameters
	{

		public int Year { get; set; }
		public int dbMonth { get; set; }
		public int dbDay { get; set; }
		public int dbHourNumber { get; set; }
		public int db { get; set; }
		public int signatureGenerated { get; set; }
		public int signatureSent { get; set; }
		public int fraudTable { get; set; }

		internal SeizureBlocksParameters()
		{

		}
	}

	public class SeizureBlocksData
	{

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x2A;
		const int COMMAND_ID_READ = 0x2B;

		public SeizureBlocksData()
		{
		}
	}
}

