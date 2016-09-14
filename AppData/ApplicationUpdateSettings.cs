using System;
namespace Motion.Core.Data.AppData
{
	public class ApplicationUpdateSettings
	{
		public bool UpdateFlag { get; set; }
		public int UpdateType { get; set; }
		public string VersionOfNewApp { get; set; }

		public ApplicationUpdateSettings()
		{
		}
	}
}

