using System;
namespace Motion.Core.Data.BleData
{
	public enum BLEParsingStatus
	{
		SUCCESS,
		ERROR,
		ERROR_NO_COMMAND_SENT,
		ERROR_HAS_MISSING_DATA_PACKET,
		ERROR_INVALID_STEP_DATE_COMPONENTS,
	}
}

