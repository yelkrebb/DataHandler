using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.StepsData
{
	public class TalliesData
	{

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_READ = 0x5D;


		const int HOURS_USED_LOC = 3;
		const int BEACONS_CONNECTED_LOC = 5;
		const int BEACONS_FAILED_LOC = 8;
		const int ON_DEMAND_CONNECTED_LOC = 11;
		const int ON_DEMAND_FAILED_LOC = 13;
		const int NUMBER_OF_CHARGE_LOC = 15;
		const int HARD_RESET_LOC = 17;
		const int SCREEN_ON_DURATION_LOC = 19;
		const int SEIZ_OR_PROF_RECORDING_COMPLETED_LOC = 23;
		const int SEIZ_OR_PROF_RECORDING_ABORTED_LOC = 25;
		const int NUM_OF_ALARMS_VIBRATIONS_LOC = 27;


		const int HOURS_USED_BYTE_SIZE = 2;
		const int BEACONS_CONNECTED_BYTE_SIZE = 3;
		const int BEACONS_FAILED_BYTE_SIZE = 3;
		const int ON_DEMAND_CONNECTED_BYTE_SIZE = 2;
		const int ON_DEMAND_FAILED_BYTE_SIZE = 2;
		const int NUMBER_OF_CHARGE_BYTE_SIZE = 2;
		const int HARD_RESET_BYTE_SIZE = 2;
		const int SCREEN_ON_DURATION_BYTE_SIZE = 4;
		const int SEIZ_OR_PROF_RECORDING_COMPLETED_BYTE_SIZE = 2;
		const int SEIZ_OR_PROF_RECORDING_ABORTED_BYTE_SIZE = 2;
		const int NUM_OF_ALARMS_VIBRATIONS_BYTE_SIZE = 2;

		public int HoursUsed { get; set; }
		public int NumberOfBeaconsConnected { get; set; }
		public int NumberOfBeaconsFailed { get; set; }
		public int NumberOfOnDemandConnected { get; set; }
		public int NumberOfOnDemandFailed { get; set; }
		public int NumberOfCharge { get; set; }
		public int NumberOfHardReset { get; set; }
		public int ScreenOnDuration { get; set; }
		public int SeizureOrProfileRecordingCompleted { get; set; }
		public int SeizureOrProfileRecordingAborted { get; set; }
		public int NumberOfAlarmsOrVibrations { get; set; }

		/* #### Equavalent RAW data per field #####*/

		byte[] hoursUsedRaw;
		byte[] beaconsConnectedRaw;
		byte[] beaconsFailedRaw;
		byte[] onDemandConnectedRaw;
		byte[] onDemandFailedRaw;
		byte[] numberOfChargeRaw;
		byte[] numberOfHardResetRaw;
		byte[] screenOnDurationRaw;
		byte[] seizOrProfRecordingCompletedRaw;
		byte[] seizOrProfRecordingAbortedRaw;
		byte[] numOfAlarmsVibrationsRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */


		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public TalliesData(TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{
			
		}
	}
}

