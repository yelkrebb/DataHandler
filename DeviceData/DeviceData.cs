using System;
namespace Motion.Core.Data.DeviceData
{
	public class DeviceData
	{
		public int LastHourSlotNumber { get; set; }
		public int LastMinuteSlotNumber { get; set; }
		public int MemberDeviceID { get; set; }
		public int MemberID { get; set; }
		public int TrackerSensitivity { get; set; }
		public int AdvertiseMode { get; set; }

		public DeviceData()
		{
		}
	}
}

