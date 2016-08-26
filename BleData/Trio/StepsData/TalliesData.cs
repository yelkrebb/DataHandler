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
		const int COMMAND_SIZE_READ = 2;

		const int INDEX_ZERO = 0;

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
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

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
			if (this._rawData != null && this._rawData.Length > 0)
				Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			if (this._readCommandRawData != null && this._readCommandRawData.Length > 0)
				Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			if (this.hoursUsedRaw != null && this.hoursUsedRaw.Length > 0)
				Array.Clear(this.hoursUsedRaw, INDEX_ZERO, this.hoursUsedRaw.Length);
			if (this.beaconsConnectedRaw != null && this.beaconsConnectedRaw.Length > 0)
				Array.Clear(this.beaconsConnectedRaw, INDEX_ZERO, this.beaconsConnectedRaw.Length);
			if (this.beaconsFailedRaw != null && this.beaconsFailedRaw.Length > 0)
				Array.Clear(this.beaconsFailedRaw, INDEX_ZERO, this.beaconsFailedRaw.Length);
			if (this.onDemandConnectedRaw != null && this.onDemandConnectedRaw.Length > 0)
				Array.Clear(this.onDemandConnectedRaw, INDEX_ZERO, this.onDemandConnectedRaw.Length);
			if (this.onDemandFailedRaw != null && this.onDemandFailedRaw.Length > 0)
				Array.Clear(this.onDemandFailedRaw, INDEX_ZERO, this.onDemandFailedRaw.Length);
			if (this.numberOfChargeRaw != null && this.numberOfChargeRaw.Length > 0)
				Array.Clear(this.numberOfChargeRaw, INDEX_ZERO, this.numberOfChargeRaw.Length);
			if (this.numberOfHardResetRaw != null && this.numberOfHardResetRaw.Length > 0)
				Array.Clear(this.numberOfHardResetRaw, INDEX_ZERO, this.numberOfHardResetRaw.Length);

			if (this.screenOnDurationRaw != null && this.screenOnDurationRaw.Length > 0)
				Array.Clear(this.screenOnDurationRaw, INDEX_ZERO, this.screenOnDurationRaw.Length);
			if (this.seizOrProfRecordingCompletedRaw != null && this.seizOrProfRecordingCompletedRaw.Length > 0)
				Array.Clear(this.seizOrProfRecordingCompletedRaw, INDEX_ZERO, this.seizOrProfRecordingCompletedRaw.Length);
			if (this.seizOrProfRecordingAbortedRaw != null && this.seizOrProfRecordingAbortedRaw.Length > 0)
				Array.Clear(this.seizOrProfRecordingAbortedRaw, INDEX_ZERO, this.seizOrProfRecordingAbortedRaw.Length);
			if (this.numOfAlarmsVibrationsRaw != null && this.numOfAlarmsVibrationsRaw.Length > 0)
				Array.Clear(this.numOfAlarmsVibrationsRaw, INDEX_ZERO, this.numOfAlarmsVibrationsRaw.Length);
			if (this.writeCommandResponseCodeRaw != null && this.writeCommandResponseCodeRaw.Length > 0)
				Array.Clear(this.writeCommandResponseCodeRaw, INDEX_ZERO, this.writeCommandResponseCodeRaw.Length);
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				
				this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);
				this.IsReadCommand = true;

				this.hoursUsedRaw = new byte[HOURS_USED_BYTE_SIZE];
				this.beaconsConnectedRaw = new byte[BEACONS_CONNECTED_BYTE_SIZE];
				this.beaconsFailedRaw = new byte[BEACONS_FAILED_BYTE_SIZE];
				this.onDemandConnectedRaw = new byte[ON_DEMAND_CONNECTED_BYTE_SIZE];
				this.onDemandFailedRaw = new byte[ON_DEMAND_FAILED_BYTE_SIZE];
				this.numberOfChargeRaw = new byte[NUMBER_OF_CHARGE_BYTE_SIZE];
				this.numberOfHardResetRaw = new byte[HARD_RESET_BYTE_SIZE];
				this.screenOnDurationRaw = new byte[SCREEN_ON_DURATION_BYTE_SIZE];
				this.seizOrProfRecordingCompletedRaw = new byte[SEIZ_OR_PROF_RECORDING_COMPLETED_BYTE_SIZE];
				this.seizOrProfRecordingAbortedRaw = new byte[SEIZ_OR_PROF_RECORDING_ABORTED_BYTE_SIZE];


				Array.Copy(this._rawData, HOURS_USED_LOC, this.hoursUsedRaw, INDEX_ZERO, HOURS_USED_BYTE_SIZE);
				Array.Copy(this._rawData, BEACONS_CONNECTED_LOC, this.beaconsConnectedRaw, INDEX_ZERO, BEACONS_CONNECTED_BYTE_SIZE);
				Array.Copy(this._rawData, BEACONS_FAILED_LOC, this.beaconsFailedRaw, INDEX_ZERO, BEACONS_FAILED_BYTE_SIZE);
				Array.Copy(this._rawData, ON_DEMAND_CONNECTED_LOC, this.onDemandConnectedRaw, INDEX_ZERO, ON_DEMAND_CONNECTED_BYTE_SIZE);
				Array.Copy(this._rawData, ON_DEMAND_FAILED_LOC, this.onDemandFailedRaw, INDEX_ZERO, ON_DEMAND_FAILED_BYTE_SIZE);
				Array.Copy(this._rawData, NUMBER_OF_CHARGE_LOC, this.numberOfChargeRaw, INDEX_ZERO, NUMBER_OF_CHARGE_BYTE_SIZE);

				Array.Copy(this._rawData, HARD_RESET_LOC, this.numberOfHardResetRaw, INDEX_ZERO, HARD_RESET_BYTE_SIZE);
				Array.Copy(this._rawData, SCREEN_ON_DURATION_LOC, this.screenOnDurationRaw, INDEX_ZERO, SCREEN_ON_DURATION_BYTE_SIZE);
				Array.Copy(this._rawData, SEIZ_OR_PROF_RECORDING_COMPLETED_LOC, this.seizOrProfRecordingCompletedRaw, INDEX_ZERO, SEIZ_OR_PROF_RECORDING_COMPLETED_BYTE_SIZE);
				Array.Copy(this._rawData, SEIZ_OR_PROF_RECORDING_ABORTED_LOC, this.seizOrProfRecordingAbortedRaw, INDEX_ZERO, SEIZ_OR_PROF_RECORDING_ABORTED_BYTE_SIZE);


				this.HoursUsed = Convert.ToInt32(Utils.getDecimalValue(this.hoursUsedRaw));
				this.NumberOfBeaconsConnected = Convert.ToInt32(Utils.getDecimalValue(this.beaconsConnectedRaw));
				this.NumberOfBeaconsFailed = Convert.ToInt32(Utils.getDecimalValue(this.beaconsFailedRaw));
				this.NumberOfOnDemandConnected = Convert.ToInt32(Utils.getDecimalValue(this.onDemandConnectedRaw));
				this.NumberOfOnDemandFailed = Convert.ToInt32(Utils.getDecimalValue(this.onDemandFailedRaw));
				this.NumberOfCharge = Convert.ToInt32(Utils.getDecimalValue(this.numberOfChargeRaw));

				this.NumberOfHardReset = Convert.ToInt32(Utils.getDecimalValue(this.numberOfHardResetRaw));
				this.ScreenOnDuration = Convert.ToInt32(Utils.getDecimalValue(this.screenOnDurationRaw));
				this.SeizureOrProfileRecordingCompleted = Convert.ToInt32(Utils.getDecimalValue(this.seizOrProfRecordingCompletedRaw));
				this.SeizureOrProfileRecordingAborted = Convert.ToInt32(Utils.getDecimalValue(this.seizOrProfRecordingAbortedRaw));


				if (this.trioDevInfo.FirmwareVersion > 2.9f)
				{

					this.numOfAlarmsVibrationsRaw = new byte[NUM_OF_ALARMS_VIBRATIONS_BYTE_SIZE];
					Array.Copy(this._rawData, NUM_OF_ALARMS_VIBRATIONS_LOC, this.numOfAlarmsVibrationsRaw, INDEX_ZERO, NUM_OF_ALARMS_VIBRATIONS_BYTE_SIZE);
					this.NumberOfAlarmsOrVibrations = Convert.ToInt32(Utils.getDecimalValue(this.numOfAlarmsVibrationsRaw));
				}

				parsingStatus = BLEParsingStatus.SUCCESS;

			});

			return parsingStatus;
		}

		public async Task<byte[]> GetReadStepTableDataCommand()
		{
			await Task.Run(() =>
			{
				this._readCommandRawData = new byte[COMMAND_SIZE_READ];
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
			});
			return this._readCommandRawData;
		}
	}
}

