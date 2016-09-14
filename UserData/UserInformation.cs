using System;
namespace Motion.Core.Data.UserData
{
	public class UserInformation
	{
		public int DermID { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public int MemberDeviceId { get; set; }
		public int MemberID { get; set; }
		public int ApplicationID { get; set; }
		public int PlatformID { get; set; }

		public UserInformation()
		{
		}
	}
}

