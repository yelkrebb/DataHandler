using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

using Newtonsoft.Json;

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

		[JsonProperty(PropertyName = "gstp")]
		public int TenacitySteps { get; set; }
		[JsonProperty(PropertyName = "istp")]
		public int IntensitySteps { get; set; }
		[JsonProperty(PropertyName = "istm")]
		public int IntensityTime { get; set; }
		[JsonProperty(PropertyName = "ist")]
		public int IntensityMinuteThreshold { get; set; }
		[JsonProperty(PropertyName = "irm")]
		public int IntensityRestMinuteAllowed { get; set; }
		[JsonProperty(PropertyName = "fstp")]
		public int FrequencySteps { get; set; }
		[JsonProperty(PropertyName = "freqt")]
		public int FrequencyCycleTime { get; set; }
		[JsonProperty(PropertyName = "freqc")]
		public int FrequencyCycle { get; set; }
		[JsonProperty(PropertyName = "freqi")]
		public int FrequencyCycleInterval { get; set; }
		[JsonProperty(PropertyName = "isc")]
		public int IntensityCycle { get; set; }
		[JsonIgnore]
		public int WriteCommandResponseCode { get; set; }
		[JsonIgnore]
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
				this.intensityStepsRaw = BitConverter.GetBytes(BitConverter.ToInt16(BitConverter.GetBytes(this.IntensitySteps), 0));
				this.intensityTimeRaw = BitConverter.GetBytes(this.IntensityTime);
				this.intensityMinuteThresholdRaw = BitConverter.GetBytes(this.IntensityMinuteThreshold);
				this.intensityRestMinuteAllowedRaw = BitConverter.GetBytes(this.IntensityRestMinuteAllowed);
				this.frequencyStepsRaw = BitConverter.GetBytes(BitConverter.ToInt16(BitConverter.GetBytes(this.FrequencySteps), 0));
				this.frequencyCycleTimeRaw = BitConverter.GetBytes(this.FrequencyCycleTime);
				this.frequencyCycleAndIntervalRaw = BitConverter.GetBytes(this.FrequencyCycleInterval | (this.FrequencyCycle << 4));

				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(this.tenacityStepsRaw);
					Array.Reverse(this.intensityStepsRaw);
					Array.Reverse(this.frequencyStepsRaw);
				}

				Buffer.BlockCopy(this.tenacityStepsRaw, Utils.INDEX_ONE, this._rawData, TENACITY_STEPS_BYTE_LOC, TENACITY_STEPS_BYTE_SIZE);
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
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = Convert.ToInt32(Utils.getDecimalValue(this.writeCommandResponseCodeRaw));
				}

				else
				{
					this.tenacityStepsRaw = new byte[TENACITY_STEPS_BYTE_SIZE];
					this.frequencyStepsRaw = new byte[FREQUENCY_STEPS_BYTE_SIZE];
					this.frequencyCycleTimeRaw = new byte[FREQUENCY_CYCLE_TIME_BYTE_SIZE];
					this.frequencyCycleAndIntervalRaw = new byte[FREQUENCY_CYCLE_AND_INTERVAL_BYTE_SIZE];



					this.intensityStepsRaw = new byte[INTENSITY_STEPS_BYTE_SIZE];
					this.intensityTimeRaw = new byte[INTENSITY_TIME_BYTE_SIZE];
					this.intensityMinuteThresholdRaw = new byte[INTENSITY_MINUTE_THRESHOLD_BYTE_SIZE];
					this.intensityRestMinuteAllowedRaw = new byte[INTENSITY_REST_MINUTE_ALLOWED_BYTE_SIZE];
					this.intensityCycleRaw = new byte[INTENSITY_CYCLE_BYTE_SIZE];

					Array.Copy(this._rawData, TENACITY_STEPS_BYTE_LOC, this.tenacityStepsRaw, INDEX_ZERO, TENACITY_STEPS_BYTE_SIZE);
					Array.Copy(this._rawData, INTENSITY_STEPS_BYTE_LOC, this.intensityStepsRaw, INDEX_ZERO, INTENSITY_STEPS_BYTE_SIZE);
					Array.Copy(this._rawData, INTENSITY_TIME_BYTE_LOC, this.intensityTimeRaw, INDEX_ZERO, INTENSITY_TIME_BYTE_SIZE);
					Array.Copy(this._rawData, INTENSITY_MINUTE_THRESHOLD_BYTE_LOC, this.intensityMinuteThresholdRaw, INDEX_ZERO, INTENSITY_MINUTE_THRESHOLD_BYTE_SIZE);
					Array.Copy(this._rawData, INTENSITY_REST_MINUTE_ALLOWED_BYTE_LOC, this.intensityRestMinuteAllowedRaw, INDEX_ZERO, INTENSITY_REST_MINUTE_ALLOWED_BYTE_SIZE);
					Array.Copy(this._rawData, FREQUENCY_STEPS_BYTE_LOC, this.frequencyStepsRaw, INDEX_ZERO, FREQUENCY_STEPS_BYTE_SIZE);
					Array.Copy(this._rawData, FREQUENCY_CYCLE_TIME_BYTE_LOC, this.frequencyCycleTimeRaw, INDEX_ZERO, FREQUENCY_CYCLE_TIME_BYTE_SIZE);
					Array.Copy(this._rawData, FREQUENCY_CYCLE_AND_INTERVAL_BYTE_LOC, this.frequencyCycleAndIntervalRaw, INDEX_ZERO, FREQUENCY_CYCLE_AND_INTERVAL_BYTE_SIZE);
					Array.Copy(this._rawData, INTENSITY_CYCLE_BYTE_LOC, this.intensityCycleRaw, INDEX_ZERO, INTENSITY_CYCLE_BYTE_SIZE);

					this.TenacitySteps = Convert.ToInt32(Utils.getDecimalValue(this.tenacityStepsRaw));
					this.IntensitySteps = Convert.ToInt32(Utils.getDecimalValue(this.intensityStepsRaw)); 
					this.IntensityTime = Convert.ToInt32(Utils.getDecimalValue(this.intensityTimeRaw)); 
					this.IntensityMinuteThreshold = Convert.ToInt32(Utils.getDecimalValue(this.intensityMinuteThresholdRaw)); 
					this.IntensityRestMinuteAllowed = Convert.ToInt32(Utils.getDecimalValue(this.intensityRestMinuteAllowedRaw)); 
					this.FrequencySteps = Convert.ToInt32(Utils.getDecimalValue(this.frequencyStepsRaw)); 
					this.FrequencyCycleTime = Convert.ToInt32(Utils.getDecimalValue(this.frequencyCycleTimeRaw)); 
					this.FrequencyCycleInterval = Convert.ToInt32(Utils.getDecimalValue(this.frequencyCycleAndIntervalRaw)); 
					this.FrequencyCycle = (Convert.ToInt32(Utils.getDecimalValue(this.frequencyCycleAndIntervalRaw))) & 0x0F;

					if ((this.trioDevInfo.ModelNumber == 961 || this.trioDevInfo.ModelNumber == 962 ||
						(this.trioDevInfo.ModelNumber == 939 && this.trioDevInfo.FirmwareVersion >= 0.3f)) || this._rawData.Length > 14)
					{
						//This field was added in the firmware starting Trio model PE939 and latter models.
						this.IntensityCycle = Convert.ToInt32(Utils.getDecimalValue(this.intensityCycleRaw));
					}
				}
				parseStatus = BLEParsingStatus.SUCCESS;

				
			});
			return parseStatus;
		}


	}
}

