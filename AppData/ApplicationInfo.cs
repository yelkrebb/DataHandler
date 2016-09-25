using System;
namespace Motion.Core.Data.AppData
{
	public class ApplicationInfo
	{
		public int ApplicationID { get; set; }
		public int PlatformID { get; set; }
		public string ApplicationName { get; set; }
		public string ApplicationVersion { get; set; }
		public string OperatingSystem { get; set; }
		public string OperatingSystemVersion { get; set; }
		public string PlatformType { get; set; }
		public string Brand { get; set; }
		public string Model { get; set; }
		public string Manufacturer { get; set; }
		public string CommunicationType { get; set; }


		public ApplicationInfo()
		{
		}
	}
}

