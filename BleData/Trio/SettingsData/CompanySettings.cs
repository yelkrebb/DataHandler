using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.SettingsData
{
	public class CompanySettings : ITrioDataHandler
	{
		const int COMMAND_SIZE_WRITE_ORIG = 12;
		const int COMMAND_SIZE_WRITE_WITH_MULTI_INT = 13;
		const int COMMAND_SIZE_READ = 2;


		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x1D;
		const int COMMAND_ID_READ = 0x1E;


		const int INDEX_ZERO = 0;

		const int TENACITY_STEPS_BYTE_LOC = 2;
		const int INTENSITY_STEPS_BYTE_LOC = 5;
		const int INTENSITY_TIME_BYTE_LOC = 7;
		const int INTENSITY_MINUTE_THRESHOLD_BYTE_LOC = 8;
		const int INTENSITY_REST_MINUTE_ALLOWED_BYTE_LOC = 9;
		const int FREQUENCY_STEPS_BYTE_LOC = 10;
		const int FREQUENCY_CYCLE_TIME_BYTE_LOC = 12;
		const int FREQUENCY_CYCLE_AND_INTERVAL_BYTE_LOC = 13;
		const int INTENSITY_CYCLE_BYTE_LOC = 14; // LAST BYTE

		const int TENACITY_STEPS_BYTE_SIZE = 3;
		const int INTENSITY_STEPS_BYTE_SIZE = 2;
		const int INTENSITY_TIME_BYTE_SIZE = 1;
		const int INTENSITY_MINUTE_THRESHOLD_BYTE_SIZE = 1;
		const int INTENSITY_REST_MINUTE_ALLOWED_BYTE_SIZE = 1;
		const int INTENSITY_CYCLE_BYTE_SIZE = 1;
		const int FREQUENCY_STEPS_BYTE_SIZE = 2;
		const int FREQUENCY_CYCLE_TIME_BYTE_SIZE = 1;
		const int FREQUENCY_CYCLE_AND_INTERVAL_BYTE_SIZE = 1;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int TenacitySteps { get; set; }
		public int IntensitySteps { get; set; }
		public int IntensityTime { get; set; }
		public int IntensityMinuteThreshold { get; set; }
		public int IntensityRestMinuteAllowed { get; set; }
		public int FrequencySteps { get; set; }
		public int FrequencyCycleTime { get; set; }
		public int FrequencyCycle { get; set; }
		public int FrequencyCycleInterval { get; set; }
		public int IntensityCycle { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		/* #### Equavalent RAW data per field #####*/
		byte[] tenacityStepsRaw;
		byte[] intensityStepsRaw;
		byte[] intensityTimeRaw;
		byte[] intensityMinuteThresholdRaw;
		byte[] intensityRestMinuteAllowedRaw;
		byte[] frequencyStepsRaw;
		byte[] frequencyCycleTimeRaw;
		byte[] frequencyCycleAndIntervalRaw;
		byte[] intensityCycleRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */


		private byte[] _rawData;
		private byte[] _readCommandRawData;

		TrioDeviceInformation trioDevInfo;


		private void ClearData()
		{
			if (this._rawData != null && this._rawData.Length > 0)
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			if (this._readCommandRawData != null && this._readCommandRawData.Length > 0)
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			if (this.tenacityStepsRaw != null && this.tenacityStepsRaw.Length > 0)
			Array.Clear(tenacityStepsRaw, INDEX_ZERO, tenacityStepsRaw.Length);
			if (this.intensityStepsRaw != null && this.intensityStepsRaw.Length > 0)
			Array.Clear(intensityStepsRaw, INDEX_ZERO, intensityStepsRaw.Length);
			if (this.intensityTimeRaw != null && this.intensityTimeRaw.Length > 0)
			Array.Clear(intensityTimeRaw, INDEX_ZERO, intensityTimeRaw.Length);
			if (this.intensityMinuteThresholdRaw != null && this.intensityMinuteThresholdRaw.Length > 0)
			Array.Clear(intensityMinuteThresholdRaw, INDEX_ZERO, intensityMinuteThresholdRaw.Length);
			if (this.intensityRestMinuteAllowedRaw != null && this.intensityRestMinuteAllowedRaw.Length > 0)
			Array.Clear(intensityRestMinuteAllowedRaw, INDEX_ZERO, intensityRestMinuteAllowedRaw.Length);
			if (this.frequencyStepsRaw != null && this.frequencyStepsRaw.Length > 0)
			Array.Clear(frequencyStepsRaw, INDEX_ZERO, frequencyStepsRaw.Length);
			if (this.frequencyCycleTimeRaw != null && this.frequencyCycleTimeRaw.Length > 0)
			Array.Clear(frequencyCycleTimeRaw, INDEX_ZERO, frequencyCycleTimeRaw.Length);
			if (this.frequencyCycleAndIntervalRaw != null && this.frequencyCycleAndIntervalRaw.Length > 0)
			Array.Clear(frequencyCycleAndIntervalRaw, INDEX_ZERO, frequencyCycleAndIntervalRaw.Length);
			if (this.intensityCycleRaw != null && this.intensityCycleRaw.Length > 0)
			Array.Clear(intensityCycleRaw, INDEX_ZERO, intensityCycleRaw.Length);

		}



		public CompanySettings(TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData();
		}


		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{
				this._readCommandRawData = new byte[COMMAND_SIZE_READ];
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO+1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
			});
			return this._readCommandRawData;
		}

		public async Task<byte[]> GetWriteCommand()
		{
			
			
			await Task.Run(() =>
			{

				if (this.trioDevInfo.ModelNumber != 932)
					this._rawData = new byte[COMMAND_SIZE_WRITE_WITH_MULTI_INT + 2];
				else
					this._rawData = new byte[COMMAND_SIZE_WRITE_ORIG + 2];

				this.tenacityStepsRaw = BitConverter.GetBytes(this.TenacitySteps);
				this.intensityStepsRaw = BitConverter.GetBytes(this.IntensitySteps);
				this.intensityTimeRaw = BitConverter.GetBytes(this.IntensityTime);
				this.intensityMinuteThresholdRaw = BitConverter.GetBytes(this.IntensityMinuteThreshold);
				this.intensityRestMinuteAllowedRaw = BitConverter.GetBytes(this.IntensityRestMinuteAllowed);
				this.frequencyStepsRaw = BitConverter.GetBytes(this.FrequencySteps);
				this.frequencyCycleTimeRaw = BitConverter.GetBytes(this.FrequencyCycleTime);
				this.frequencyCycleAndIntervalRaw = BitConverter.GetBytes(this.FrequencyCycleInterval | (this.FrequencyCycle << 4));

				Buffer.BlockCopy(this.tenacityStepsRaw, 0, this._rawData, TENACITY_STEPS_BYTE_LOC, TENACITY_STEPS_BYTE_SIZE);
				Buffer.BlockCopy(this.intensityStepsRaw, 0, this._rawData, INTENSITY_STEPS_BYTE_LOC, INTENSITY_STEPS_BYTE_SIZE);
				Buffer.BlockCopy(this.intensityTimeRaw, 0, this._rawData, INTENSITY_TIME_BYTE_LOC, INTENSITY_TIME_BYTE_SIZE);
				Buffer.BlockCopy(this.intensityMinuteThresholdRaw, 0, this._rawData, INTENSITY_MINUTE_THRESHOLD_BYTE_LOC, INTENSITY_MINUTE_THRESHOLD_BYTE_SIZE);
				Buffer.BlockCopy(this.intensityRestMinuteAllowedRaw, 0, this._rawData, INTENSITY_REST_MINUTE_ALLOWED_BYTE_LOC, INTENSITY_REST_MINUTE_ALLOWED_BYTE_SIZE);
				Buffer.BlockCopy(this.frequencyStepsRaw, 0, this._rawData, FREQUENCY_STEPS_BYTE_LOC, FREQUENCY_STEPS_BYTE_SIZE);
				Buffer.BlockCopy(this.frequencyCycleTimeRaw, 0, this._rawData, FREQUENCY_CYCLE_TIME_BYTE_LOC, FREQUENCY_CYCLE_TIME_BYTE_SIZE);
				Buffer.BlockCopy(this.frequencyCycleAndIntervalRaw, 0, this._rawData, FREQUENCY_CYCLE_AND_INTERVAL_BYTE_LOC, FREQUENCY_CYCLE_AND_INTERVAL_BYTE_SIZE);

				//if ((this.trioDevInfo.ModelNumber == 961 || this.trioDevInfo.ModelNumber == 962 ||
				//	(this.trioDevInfo.ModelNumber == 939 && this.trioDevInfo.FirmwareVersion >= 0.3f)) || this._rawData.Length > 14)
				// commented out since intensity cycle is not needed only in 932
				if(this.trioDevInfo.ModelNumber != 932)
				{
					this.intensityCycleRaw = BitConverter.GetBytes(this.IntensityCycle);
					Buffer.BlockCopy(this.intensityCycleRaw, 0, this._rawData, INTENSITY_CYCLE_BYTE_LOC, INTENSITY_CYCLE_BYTE_SIZE);
				}

				//Add the two prefix bytes needed for the commands
				//For 93x and 96x, 1st byte is the command prefix 0x1B while 2nd byte is the command ID
				byte[] commandPrefix =  BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID =  BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);
			});
			return this._rawData;
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parseStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{

				this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);

				this.IsReadCommand = true;
				if (rawData[1] == 0x1D)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[Constants.INT32_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);
				}

				else
				{ 
					this.tenacityStepsRaw = new byte[Constants.INT32_BYTE_SIZE];
					this.frequencyStepsRaw = new byte[Constants.INT32_BYTE_SIZE];
					this.frequencyCycleTimeRaw = new byte[Constants.INT32_BYTE_SIZE];
					this.frequencyCycleAndIntervalRaw = new byte[Constants.INT32_BYTE_SIZE];



					this.intensityStepsRaw = new byte[Constants.INT32_BYTE_SIZE];
					this.intensityTimeRaw = new byte[Constants.INT32_BYTE_SIZE];
					this.intensityMinuteThresholdRaw = new byte[Constants.INT32_BYTE_SIZE];
					this.intensityRestMinuteAllowedRaw = new byte[Constants.INT32_BYTE_SIZE];
					this.intensityCycleRaw = new byte[Constants.INT32_BYTE_SIZE];

					Array.Copy(this._rawData, TENACITY_STEPS_BYTE_LOC, this.tenacityStepsRaw, INDEX_ZERO, TENACITY_STEPS_BYTE_SIZE);
					Array.Copy(this._rawData, INTENSITY_STEPS_BYTE_LOC, this.intensityStepsRaw, INDEX_ZERO, INTENSITY_STEPS_BYTE_SIZE);
					Array.Copy(this._rawData, INTENSITY_TIME_BYTE_LOC, this.intensityTimeRaw, INDEX_ZERO, INTENSITY_TIME_BYTE_SIZE);
					Array.Copy(this._rawData, INTENSITY_MINUTE_THRESHOLD_BYTE_LOC, this.intensityMinuteThresholdRaw, INDEX_ZERO, INTENSITY_MINUTE_THRESHOLD_BYTE_SIZE);
					Array.Copy(this._rawData, INTENSITY_REST_MINUTE_ALLOWED_BYTE_LOC, this.intensityRestMinuteAllowedRaw, INDEX_ZERO, INTENSITY_REST_MINUTE_ALLOWED_BYTE_SIZE);
					Array.Copy(this._rawData, FREQUENCY_STEPS_BYTE_LOC, this.frequencyStepsRaw, INDEX_ZERO, FREQUENCY_STEPS_BYTE_SIZE);
					Array.Copy(this._rawData, FREQUENCY_CYCLE_TIME_BYTE_LOC, this.frequencyCycleTimeRaw, INDEX_ZERO, FREQUENCY_CYCLE_TIME_BYTE_SIZE);
					Array.Copy(this._rawData, FREQUENCY_CYCLE_AND_INTERVAL_BYTE_LOC, this.frequencyCycleAndIntervalRaw, INDEX_ZERO, FREQUENCY_CYCLE_AND_INTERVAL_BYTE_SIZE);
					Array.Copy(this._rawData, INTENSITY_CYCLE_BYTE_LOC, this.intensityCycleRaw, INDEX_ZERO, INTENSITY_CYCLE_BYTE_SIZE);

					this.TenacitySteps = BitConverter.ToInt32(this.tenacityStepsRaw, INDEX_ZERO);
					this.IntensitySteps = BitConverter.ToInt32(this.intensityStepsRaw, INDEX_ZERO);
					this.IntensityTime = BitConverter.ToInt32(this.intensityTimeRaw, INDEX_ZERO);
					this.IntensityMinuteThreshold = BitConverter.ToInt32(this.intensityRestMinuteAllowedRaw, INDEX_ZERO);
					this.IntensityRestMinuteAllowed = BitConverter.ToInt32(this.intensityRestMinuteAllowedRaw, INDEX_ZERO);
					this.FrequencySteps = BitConverter.ToInt32(this.frequencyStepsRaw, INDEX_ZERO);
					this.FrequencyCycleTime = BitConverter.ToInt32(this.frequencyCycleTimeRaw, INDEX_ZERO);
					this.FrequencyCycleInterval = BitConverter.ToInt32(this.frequencyCycleAndIntervalRaw, INDEX_ZERO) & 0x0F;
					this.FrequencyCycle = (BitConverter.ToInt32(this.frequencyCycleAndIntervalRaw, INDEX_ZERO) >> 4) & 0x0F;

					if ((this.trioDevInfo.ModelNumber == 961 || this.trioDevInfo.ModelNumber == 962 ||
						(this.trioDevInfo.ModelNumber == 939 && this.trioDevInfo.FirmwareVersion >= 0.3f)) || this._rawData.Length > 14)
					{
						//This field was added in the firmware starting Trio model PE939 and latter models.
						this.IntensityCycle = BitConverter.ToInt32(this.intensityCycleRaw, INDEX_ZERO);
					}
				}
				parseStatus = BLEParsingStatus.SUCCESS;

				
			});
			return parseStatus;
		}


	}
}

