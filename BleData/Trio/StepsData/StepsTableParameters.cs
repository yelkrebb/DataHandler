using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.Trio;
namespace Motion.Core.Data.BleData.Trio.StepsData
{
	public class StepsTableParameters
	{
		public int dbYear { get; set; }
		public int  dbMonth { get; set; }
		public int dbDay { get; set; }
		public int dbHourNumber { get; set; }
		public int sentHourFlag { get; set; }
		public int profileGenerated { get; set; }

		public StepsTableParameters()
		{
			
		}
	}
}

