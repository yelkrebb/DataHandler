using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.FT900;

namespace Motion.Core.Data.BleData.FT900.SettingsData
{
	public class CompanySettings:FT900DataHandler
	{
		const int COMMAND_SIZE_WRITE_900 = 0x13;
		const int COMMAND_SIZE_READ_900 = 0x02;
		const int COMMAND_ID_WRITE_FT900 = 0x3A;
		const int COMMAND_ID_READ_FT900 = 0x3B;

		const int INDEX_ZERO = 0;
		const int COMMAND_SIZE_READ = 1;

		const int TENACITY_STEPS_BYTE_LOC = 2;

		const int FREQUENCY_STEPS_BYTE_LOC_FT900 = 5;
		const int FREQUENCY_CYCLE_TIME_BYTE_LOC_FT900 = 7;
		const int FREQUENCY_CYCLE_BYTE_LOC_FT900 = 8;
		const int FREQUENCY_AUTO_SET_BYTE_LOC_FT900 = 9;
		const int ACTIVITY_MAX_BYTE_LOC_FT900 = 11;
		const int ACTIVITY_START_TIME_BYTE_LOC_FT900 = 13;
		const int ACTIVITY_END_TIME_BYTE_LOC_FT900 = 15;
		const int SYNC_TIME_INTERVAL_BYTE_LOC_FT900 = 17;
		const int MIN_STEPS_THRESHOLD_BYTE_LOC_FT900 = 18;
		const int FLAGS_BYTE_LOC_FT900 = 19;

		const int TENACITY_STEPS_BYTE_SIZE = 3;
		const int FREQUENCY_STEPS_BYTE_SIZE = 2;
		const int FREQUENCY_CYCLE_TIME_BYTE_SIZE = 1;
		const int FREQUENCY_CYCLE_AND_INTERVAL_BYTE_SIZE = 1;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		const int FREQUENCY_AUTO_SET_BYTE_SIZE_FT900 = 2;
		const int ACTIVITY_MAX_BYTE_SIZE_FT900 = 2;
		const int ACTIVITY_START_TIME_BYTE_SIZE_FT900 = 2;
		const int ACTIVITY_END_TIME_BYTE_SIZE_FT900 = 2;
		const int SYNC_TIME_INTERVAL_BYTE_SIZE_FT900 = 1;
		const int MIN_STEPS_THRESHOLD_BYTE_SIZE_FT900 = 1;
		const int FLAGS_BYTE_SIZE_FT900 = 1;

		public int TenacitySteps { get; set; }
		public int FrequencySteps { get; set; }
		public int FrequencyCycleTime { get; set; }
		public int FrequencyCycle { get; set; }
		public int FrequencyCycleInterval { get; set; }

		public bool FrequencyAutoSet { get; set; }
		public int ActivityMax { get; set; }
		public int ActivityStartTime { get; set; }
		public int ActivityEndTime { get; set; }
		public int SyncTimeInterval { get; set; }
		public int MinimumStepThreshold { get; set; }
		public bool FrequencyAlarmEnable { get; set; }
		public bool NextDaySet { get; set; }
		public int StepSensitivity { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		byte[] frequencyAutoSetRaw;
		byte[] activityMaxRaw;
		byte[] activityStartTimeRaw;
		byte[] activityEndTimeRaw;
		byte[] syncTimeIntervalRaw;
		byte[] minimumStepThresholdRaw;
		byte[] flagsByteRaw;

		byte[] tenacityStepsRaw;
		byte[] frequencyStepsRaw;
		byte[] frequencyCycleTimeRaw;
		byte[] frequencyCycleAndIntervalRaw;
		byte[] writeCommandResponseCodeRaw;

		private byte[] _rawData;
		private byte[] _readCommandRawData;

		FT900DeviceInformation ft900DevInfo;

		private void ClearData()
		{
			Array.Clear(frequencyAutoSetRaw, INDEX_ZERO, frequencyAutoSetRaw.Length);
			Array.Clear(activityMaxRaw, INDEX_ZERO, activityMaxRaw.Length);
			Array.Clear(activityStartTimeRaw, INDEX_ZERO, activityStartTimeRaw.Length);
			Array.Clear(activityEndTimeRaw, INDEX_ZERO, activityEndTimeRaw.Length);
			Array.Clear(syncTimeIntervalRaw, INDEX_ZERO, syncTimeIntervalRaw.Length);
			Array.Clear(minimumStepThresholdRaw, INDEX_ZERO, minimumStepThresholdRaw.Length);
			Array.Clear(flagsByteRaw, INDEX_ZERO, flagsByteRaw.Length);
		}

		public CompanySettings(FT900DeviceInformation devInfo)
		{
			this.ft900DevInfo = devInfo;
			this.ClearData();
		}

		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_SIZE_READ_900);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ_FT900);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
			});
			return this._readCommandRawData;
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parseStatus = BLEParsingStatus.ERROR;
			await Task.Run(() => 
			{ 
				Array.Copy(rawData, this._rawData, rawData.Length);

				this.IsReadCommand = true;
				if (rawData[1] == 0x3A)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);
				}

				else
				{ 
					this.tenacityStepsRaw = new byte[TENACITY_STEPS_BYTE_SIZE];
					this.frequencyStepsRaw = new byte[FREQUENCY_STEPS_BYTE_SIZE];
					this.frequencyCycleTimeRaw = new byte[FREQUENCY_CYCLE_TIME_BYTE_SIZE];
					this.frequencyCycleAndIntervalRaw = new byte[FREQUENCY_CYCLE_AND_INTERVAL_BYTE_SIZE];

					this.frequencyAutoSetRaw = new byte[FREQUENCY_AUTO_SET_BYTE_SIZE_FT900];
					this.activityMaxRaw = new byte[ACTIVITY_MAX_BYTE_SIZE_FT900];
					this.activityStartTimeRaw = new byte[ACTIVITY_START_TIME_BYTE_SIZE_FT900];
					this.activityEndTimeRaw = new byte[ACTIVITY_END_TIME_BYTE_SIZE_FT900];
					this.syncTimeIntervalRaw = new byte[SYNC_TIME_INTERVAL_BYTE_SIZE_FT900];
					this.minimumStepThresholdRaw = new byte[MIN_STEPS_THRESHOLD_BYTE_SIZE_FT900];
					this.flagsByteRaw = new byte[FLAGS_BYTE_SIZE_FT900];

					Array.Copy(this._rawData, TENACITY_STEPS_BYTE_LOC, this.tenacityStepsRaw, INDEX_ZERO, TENACITY_STEPS_BYTE_SIZE);
					Array.Copy(this._rawData, FREQUENCY_STEPS_BYTE_LOC_FT900, this.frequencyStepsRaw, INDEX_ZERO, FREQUENCY_STEPS_BYTE_SIZE);
					Array.Copy(this._rawData, FREQUENCY_CYCLE_TIME_BYTE_LOC_FT900, this.frequencyCycleTimeRaw, INDEX_ZERO, FREQUENCY_CYCLE_TIME_BYTE_SIZE);
					Array.Copy(this._rawData, FREQUENCY_AUTO_SET_BYTE_LOC_FT900, this.frequencyAutoSetRaw, INDEX_ZERO, 2);
					Array.Copy(this._rawData, ACTIVITY_MAX_BYTE_LOC_FT900, this.activityMaxRaw, INDEX_ZERO, 2);
					Array.Copy(this._rawData, ACTIVITY_START_TIME_BYTE_LOC_FT900, this.activityStartTimeRaw, INDEX_ZERO, 2);
					Array.Copy(this._rawData, ACTIVITY_END_TIME_BYTE_LOC_FT900, this.activityEndTimeRaw, INDEX_ZERO, 2);
					Array.Copy(this._rawData, SYNC_TIME_INTERVAL_BYTE_LOC_FT900, this.syncTimeIntervalRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, MIN_STEPS_THRESHOLD_BYTE_LOC_FT900, this.minimumStepThresholdRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, FLAGS_BYTE_LOC_FT900, this.flagsByteRaw, INDEX_ZERO, 1);

					this.TenacitySteps = BitConverter.ToInt32(this.tenacityStepsRaw, INDEX_ZERO);
					this.FrequencySteps = BitConverter.ToInt32(this.frequencyStepsRaw, INDEX_ZERO);
					this.FrequencyCycleTime = BitConverter.ToInt32(this.frequencyCycleTimeRaw, INDEX_ZERO);

					int autoSetAndFrequencyIntervalValue = BitConverter.ToInt32(this.frequencyAutoSetRaw, INDEX_ZERO);

					this.FrequencyAutoSet = Convert.ToBoolean((autoSetAndFrequencyIntervalValue >> 15) & 0x01);
					this.FrequencyCycleInterval = autoSetAndFrequencyIntervalValue & 0x0FFF;
					this.ActivityMax = BitConverter.ToInt32(this.activityMaxRaw, INDEX_ZERO);
					this.ActivityStartTime = BitConverter.ToInt32(this.activityStartTimeRaw, INDEX_ZERO);
					this.ActivityEndTime = BitConverter.ToInt32(this.activityEndTimeRaw, INDEX_ZERO);
					this.SyncTimeInterval = BitConverter.ToInt32(this.syncTimeIntervalRaw, INDEX_ZERO);
					this.MinimumStepThreshold = BitConverter.ToInt32(this.minimumStepThresholdRaw, INDEX_ZERO);

					int flagsByteValue = BitConverter.ToInt32(this.flagsByteRaw, INDEX_ZERO);

					this.FrequencyAlarmEnable = Convert.ToBoolean((flagsByteValue >> 3) & 0x01);
					this.NextDaySet = Convert.ToBoolean((flagsByteValue >> 2) & 0x01);
					this.StepSensitivity = flagsByteValue & 0x03;
				}

				parseStatus = BLEParsingStatus.SUCCESS;
			});

			return parseStatus;
		}

		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() =>
			{ 
				this.tenacityStepsRaw = BitConverter.GetBytes(this.TenacitySteps);
				this.frequencyStepsRaw = BitConverter.GetBytes(this.FrequencySteps);
				this.frequencyCycleTimeRaw = BitConverter.GetBytes(this.FrequencyCycleTime);
				this.frequencyCycleAndIntervalRaw = BitConverter.GetBytes(this.FrequencyCycle);

				int frequencyAutoSetByteValue = 0x00;
				frequencyAutoSetByteValue |= this.FrequencyAutoSet ? 0x8000 : 0x0000;
				frequencyAutoSetByteValue |= (this.FrequencyCycleInterval & 0x0FFF);
				this.frequencyAutoSetRaw = BitConverter.GetBytes(frequencyAutoSetByteValue);
				this.activityMaxRaw = BitConverter.GetBytes(this.ActivityMax);
				this.activityStartTimeRaw = BitConverter.GetBytes(this.ActivityStartTime);
				this.activityEndTimeRaw = BitConverter.GetBytes(this.ActivityEndTime);
				this.syncTimeIntervalRaw = BitConverter.GetBytes(this.SyncTimeInterval);
				this.minimumStepThresholdRaw = BitConverter.GetBytes(this.MinimumStepThreshold);

				int flagsByteValue = 0x00;
				flagsByteValue |= this.FrequencyAlarmEnable ? 0x08 : 0x00;
				flagsByteValue |= this.NextDaySet ? 0x04 : 0x00;
				flagsByteValue |= this.StepSensitivity;
				this.flagsByteRaw = BitConverter.GetBytes(flagsByteValue);

				Buffer.BlockCopy(this.tenacityStepsRaw, 0, this._rawData, TENACITY_STEPS_BYTE_LOC, TENACITY_STEPS_BYTE_SIZE);
				Buffer.BlockCopy(this.frequencyStepsRaw, 0, this._rawData, FREQUENCY_STEPS_BYTE_LOC_FT900, FREQUENCY_STEPS_BYTE_SIZE);
				Buffer.BlockCopy(this.frequencyCycleTimeRaw, 0, this._rawData, FREQUENCY_CYCLE_TIME_BYTE_LOC_FT900, FREQUENCY_CYCLE_TIME_BYTE_SIZE);
				Buffer.BlockCopy(this.frequencyAutoSetRaw, 0, this._rawData, FREQUENCY_AUTO_SET_BYTE_LOC_FT900, 2);
				Buffer.BlockCopy(this.activityMaxRaw, 0, this._rawData, ACTIVITY_MAX_BYTE_LOC_FT900, 2);
				Buffer.BlockCopy(this.activityStartTimeRaw, 0, this._rawData, ACTIVITY_START_TIME_BYTE_LOC_FT900, 2);
				Buffer.BlockCopy(this.activityEndTimeRaw, 0, this._rawData, ACTIVITY_END_TIME_BYTE_LOC_FT900, 2);
				Buffer.BlockCopy(this.syncTimeIntervalRaw, 0, this._rawData, SYNC_TIME_INTERVAL_BYTE_LOC_FT900, 1);
				Buffer.BlockCopy(this.minimumStepThresholdRaw, 0, this._rawData, MIN_STEPS_THRESHOLD_BYTE_LOC_FT900, 1);
				Buffer.BlockCopy(this.flagsByteRaw, 0, this._rawData, FLAGS_BYTE_LOC_FT900, 1);

				//Add the two prefix bytes needed for the commands
				//For 93x and 96x, 1st byte is the command prefix 0x1B while 2nd byte is the command ID
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_SIZE_WRITE_900);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE_FT900);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);
			});

			return this._rawData;
		}
	}
}

