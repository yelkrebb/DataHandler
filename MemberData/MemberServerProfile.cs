using System;
namespace Motion.Core.Data.MemberData
{
	public class MemberServerProfile
	{
		public int MemberDeviceID { get; set; }
		public long UpdateFlag { get; set; }
		public long UpdateFlagChanges { get; set; }
		public int SynchronizationID { get; set; }
		public string ServerDateTime { get; set; }

		public MemberServerProfile()
		{
		}
	}
}

