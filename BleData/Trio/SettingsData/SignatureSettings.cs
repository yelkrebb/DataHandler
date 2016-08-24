using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.SettingsData
{
	public class SignatureSettings: ITrioDataHandler
	{
		const int COMMAND_SIZE_WRITE_ORIG = 3;
		const int COMMAND_SIZE_WRITE_NEW = 12;
		const int COMMAND_SIZE_READ = 2;

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE_OLD = 0x1F;
		const int COMMAND_ID_READ_OLD = 0x20;
		const int COMMAND_ID_WRITE_NEW = 0x5F;
		const int COMMAND_ID_READ_NEW = 0x60;

		const int INDEX_ZERO = 0;

		const int SAMPLING_TIME_BYTE_LOC = 2;
		const int SAMPLING_CYCLE_BYTE_LOC_OLD = 2;
		const int SAMPLING_CYCLE_BYTE_LOC_NEW = 4;
		const int SAMPLING_FREQUENCY_BYTE_LOC_OLD = 3;
		const int SAMPLING_FREQUENCY_BYTE_LOC_NEW = 5;
		const int SAMPLING_THRESHOLD_BYTE_LOC_OLD = 4;
		const int SAMPLING_THRESHOLD_BYTE_LOC_NEW = 6;
		const int SAMPLING_RECORDING_PER_DAY_BYTE_LOC = 7;
		//const int SAMPLING_RECORDING_PER_DAY_BYTE_LOC_READ = 8;
		const int SAMPLING_STEPS_LOC = 8;
		const int SAMPLING_MINUTE_RECORDING_LOC = 9;
		const int SAMPLING_MAXIMUM_STEPS_LOC = 11;
		const int SAMPLING_TIME_FRAME_IN_SECONDS_LOC = 12;
		const int SAMPLING_TOTAL_BLOCKS_BYTE_LOC = 13;

		const int SAMPLING_TIME_BYTE_SIZE_OLD = 1;
		const int SAMPLING_TIME_BYTE_SIZE_NEW = 2;
		const int SAMPLING_CYCLE_BYTE_SIZE = 1;
		const int SAMPLING_FREQUENCY_BYTE_SIZE = 1;
		const int SAMPLING_THRESHOLD_BYTE_SIZE = 1;
		const int SAMPLING_RECORDING_PER_DAY_BYTE_SIZE = 1;
		const int SAMPLING_STEPS_SIZE = 1;
		const int SAMPLING_MINUTE_RECORDING_SIZE = 2;
		const int SAMPLING_MAXIMUM_STEPS_SIZE = 1;
		const int SAMPLING_TIME_FRAME_IN_SECONDS_SIZE = 1;
		const int SAMPLING_TOTAL_BLOCKS_BYTE_SIZE = 1;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int SamplingTime { get; set; }
		public int SamplingCycle { get; set; }
		public int SamplingFrequency { get; set; }
		public int SamplingThreshold { get; set; }
		public int SamplingRecordingPerDay { get; set; }
		public int SamplingTotalBlocks { get; set; }
		public int SamplingSteps { get; set; }
		public int MinuteRecordingInterval { get; set; }
		public int MaximumSteps { get; set; }
		public int TimeFrameInSeconds { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		/* #### Equavalent RAW data per field #####*/
		byte[] samplingTimeWithSamplingCycleRaw;
		byte[] samplingTimeRaw;
		byte[] samplingCycleRaw;
		byte[] samplingFrequencyRaw;
		byte[] samplingThresholdRaw;
		byte[] samplingRecordingPerdayRaw;
		byte[] samplingTotalBlocksRaw;
		byte[] samplingStepsRaw;
		byte[] minuteRecordingIntervalRaw;
		byte[] maximumStepsRaw;
		byte[] timeframeInSecondsRaw;
		byte[] samplingTimeCycleRaw;
		byte[] writeCommandResponseCodeRaw;
 		/* #### Equavalent RAW data per field #####*/


		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public SignatureSettings (TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData ();
		}

		private void ClearData()
		{
			if (this._rawData != null && this._rawData.Length > 0)
				Array.Clear (this._rawData, INDEX_ZERO, this._rawData.Length);
			if (this._readCommandRawData != null && this._readCommandRawData.Length > 0)
				Array.Clear (this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			if (this.samplingTimeWithSamplingCycleRaw != null && this.samplingTimeWithSamplingCycleRaw.Length > 0)
				Array.Clear (this.samplingTimeWithSamplingCycleRaw, INDEX_ZERO, this.samplingTimeWithSamplingCycleRaw.Length);
			if (this.samplingTimeRaw != null && this.samplingTimeRaw.Length > 0)
				Array.Clear (this.samplingTimeRaw, INDEX_ZERO, this.samplingTimeRaw.Length);
			if (this.samplingCycleRaw != null && this.samplingCycleRaw.Length > 0)
				Array.Clear (this.samplingCycleRaw, INDEX_ZERO, this.samplingCycleRaw.Length);
			if (this.samplingFrequencyRaw != null && this.samplingFrequencyRaw.Length > 0)
				Array.Clear (this.samplingFrequencyRaw, INDEX_ZERO, this.samplingFrequencyRaw.Length);
			if (this.samplingThresholdRaw != null && this.samplingThresholdRaw.Length > 0)
				Array.Clear (this.samplingThresholdRaw, INDEX_ZERO, this.samplingThresholdRaw.Length);
			if (this.samplingRecordingPerdayRaw != null && this.samplingRecordingPerdayRaw.Length > 0)
				Array.Clear (this.samplingRecordingPerdayRaw, INDEX_ZERO, this.samplingRecordingPerdayRaw.Length);
			if (this.samplingTotalBlocksRaw != null && this.samplingTotalBlocksRaw.Length > 0)
				Array.Clear (this.samplingTotalBlocksRaw, INDEX_ZERO, this.samplingTotalBlocksRaw.Length);

		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{

			BLEParsingStatus parseStatus = BLEParsingStatus.ERROR;
			await Task.Run(() => { 

				this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);
				this.IsReadCommand = true;
				if (rawData[1] == 0x5F || rawData[1] == 0x1F)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[Constants.INT32_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);
				}

				else
				{ 
					if ((this.trioDevInfo.ModelNumber == 932 || this.trioDevInfo.ModelNumber == 939 || this.trioDevInfo.ModelNumber == 936 ||
					this.trioDevInfo.ModelNumber == 905 || (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion < 5.0f)))
					{
						this.samplingTimeWithSamplingCycleRaw = new byte[Constants.INT32_BYTE_SIZE];
						this.samplingFrequencyRaw = new byte[Constants.INT32_BYTE_SIZE];
						this.samplingThresholdRaw = new byte[Constants.INT32_BYTE_SIZE];

						Array.Copy(this._rawData, SAMPLING_TIME_BYTE_LOC, this.samplingTimeWithSamplingCycleRaw, INDEX_ZERO, SAMPLING_TIME_BYTE_SIZE_OLD);
						Array.Copy(this._rawData, SAMPLING_FREQUENCY_BYTE_LOC_OLD, this.samplingFrequencyRaw, INDEX_ZERO, SAMPLING_FREQUENCY_BYTE_SIZE);
						Array.Copy(this._rawData, SAMPLING_THRESHOLD_BYTE_LOC_OLD, this.samplingThresholdRaw, INDEX_ZERO, SAMPLING_THRESHOLD_BYTE_SIZE);

						if (this.trioDevInfo.ModelNumber == 932 || (this.trioDevInfo.ModelNumber == 936 && this.trioDevInfo.FirmwareVersion < 1.4f))
						{
							this.SamplingTime = BitConverter.ToInt32(this.samplingTimeWithSamplingCycleRaw, INDEX_ZERO) & 0x0F;
							this.SamplingCycle = (BitConverter.ToInt32(this.samplingTimeWithSamplingCycleRaw, INDEX_ZERO) >> 4) & 0X0F;
						}
						else
						{
							this.SamplingTime = BitConverter.ToInt32(this.samplingTimeWithSamplingCycleRaw, INDEX_ZERO) & 0x1F;
							this.SamplingCycle = (BitConverter.ToInt32(this.samplingTimeWithSamplingCycleRaw, INDEX_ZERO) >> 5) & 0X07;
						}

						this.SamplingFrequency = BitConverter.ToInt32(this.samplingFrequencyRaw, INDEX_ZERO);
						this.SamplingThreshold = BitConverter.ToInt32(this.samplingThresholdRaw, INDEX_ZERO);
					}
					else
					{
						this.samplingTimeRaw = new byte[Constants.INT32_BYTE_SIZE];
						this.samplingCycleRaw = new byte[Constants.INT32_BYTE_SIZE];
						this.samplingFrequencyRaw = new byte[Constants.INT32_BYTE_SIZE];
						this.samplingThresholdRaw = new byte[Constants.INT32_BYTE_SIZE];
						this.samplingRecordingPerdayRaw = new byte[Constants.INT32_BYTE_SIZE];
						this.samplingStepsRaw = new byte[Constants.INT32_BYTE_SIZE];
						this.minuteRecordingIntervalRaw = new byte[Constants.INT32_BYTE_SIZE];
						this.maximumStepsRaw =  new byte[Constants.INT32_BYTE_SIZE];
						this.timeframeInSecondsRaw = new byte[Constants.INT32_BYTE_SIZE];
						this.samplingTotalBlocksRaw = new byte[Constants.INT32_BYTE_SIZE];

						Array.Copy(this._rawData, SAMPLING_TIME_BYTE_LOC, this.samplingTimeRaw, INDEX_ZERO, SAMPLING_TIME_BYTE_SIZE_NEW);
						Array.Copy(this._rawData, SAMPLING_CYCLE_BYTE_LOC_NEW, this.samplingCycleRaw, INDEX_ZERO, SAMPLING_CYCLE_BYTE_SIZE);
						Array.Copy(this._rawData, SAMPLING_FREQUENCY_BYTE_LOC_NEW, this.samplingFrequencyRaw, INDEX_ZERO, SAMPLING_FREQUENCY_BYTE_SIZE);
						Array.Copy(this._rawData, SAMPLING_THRESHOLD_BYTE_LOC_NEW, this.samplingThresholdRaw, INDEX_ZERO, SAMPLING_THRESHOLD_BYTE_SIZE);
						Array.Copy(this._rawData, SAMPLING_RECORDING_PER_DAY_BYTE_LOC, this.samplingRecordingPerdayRaw, INDEX_ZERO, SAMPLING_RECORDING_PER_DAY_BYTE_SIZE);
						Array.Copy(this._rawData, SAMPLING_STEPS_LOC, this.samplingStepsRaw, INDEX_ZERO, SAMPLING_STEPS_SIZE);
						Array.Copy(this._rawData, SAMPLING_MINUTE_RECORDING_LOC, this.minuteRecordingIntervalRaw, INDEX_ZERO, SAMPLING_MINUTE_RECORDING_SIZE);
						Array.Copy(this._rawData, SAMPLING_MAXIMUM_STEPS_LOC, this.maximumStepsRaw, INDEX_ZERO, SAMPLING_MAXIMUM_STEPS_SIZE);
						Array.Copy(this._rawData, SAMPLING_TIME_FRAME_IN_SECONDS_LOC, this.timeframeInSecondsRaw, INDEX_ZERO, SAMPLING_TIME_FRAME_IN_SECONDS_SIZE);
						Array.Copy(this._rawData, SAMPLING_TOTAL_BLOCKS_BYTE_LOC, this.samplingTotalBlocksRaw, INDEX_ZERO, SAMPLING_TOTAL_BLOCKS_BYTE_SIZE);

						this.SamplingTime = BitConverter.ToInt32(this.samplingTimeRaw, INDEX_ZERO);
						this.SamplingCycle = BitConverter.ToInt32(this.samplingCycleRaw, INDEX_ZERO);
						this.SamplingFrequency = BitConverter.ToInt32(this.samplingFrequencyRaw, INDEX_ZERO);
						this.SamplingThreshold = BitConverter.ToInt32(this.samplingThresholdRaw, INDEX_ZERO);
						this.SamplingRecordingPerDay = BitConverter.ToInt32(this.samplingRecordingPerdayRaw, INDEX_ZERO);
						this.SamplingSteps = BitConverter.ToInt32(this.samplingStepsRaw, INDEX_ZERO);
						this.MinuteRecordingInterval = BitConverter.ToInt32(this.minuteRecordingIntervalRaw, INDEX_ZERO);
						this.MaximumSteps = BitConverter.ToInt32(this.maximumStepsRaw, INDEX_ZERO);
						this.TimeFrameInSeconds = BitConverter.ToInt32(this.timeframeInSecondsRaw, INDEX_ZERO);
						this.SamplingTotalBlocks = BitConverter.ToInt32(this.samplingTotalBlocksRaw, INDEX_ZERO);

					}
				}

				parseStatus = BLEParsingStatus.SUCCESS;
			

			});

			return parseStatus;
		}

		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{ 
				this._readCommandRawData = new byte[COMMAND_SIZE_READ];
				bool willUseNewCommandID = false;
				if (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f)
				{
					willUseNewCommandID = true;
				}
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = willUseNewCommandID ? BitConverter.GetBytes(COMMAND_ID_READ_NEW) : BitConverter.GetBytes(COMMAND_ID_READ_OLD);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);


			});

			return this._readCommandRawData;
		}

		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() =>
			{

				if ((this.trioDevInfo.ModelNumber == 961) && (this.trioDevInfo.FirmwareVersion >= 5.0f))
				{
					this._rawData = new byte[COMMAND_SIZE_WRITE_NEW + 2];

					this.samplingTimeRaw = BitConverter.GetBytes(this.SamplingTime);
					this.samplingCycleRaw = BitConverter.GetBytes(this.SamplingCycle);
					this.samplingFrequencyRaw = BitConverter.GetBytes(this.SamplingFrequency);
					this.samplingThresholdRaw = BitConverter.GetBytes(this.SamplingThreshold);
					this.samplingRecordingPerdayRaw = BitConverter.GetBytes(this.SamplingRecordingPerDay);
					this.samplingStepsRaw = BitConverter.GetBytes(this.SamplingSteps);
					this.minuteRecordingIntervalRaw = BitConverter.GetBytes(this.MinuteRecordingInterval);
					this.maximumStepsRaw = BitConverter.GetBytes(this.MaximumSteps);
					this.timeframeInSecondsRaw = BitConverter.GetBytes(this.TimeFrameInSeconds);
					this.samplingTotalBlocksRaw = BitConverter.GetBytes(this.SamplingTotalBlocks);

					Buffer.BlockCopy(this.samplingTimeRaw, INDEX_ZERO, this._rawData, SAMPLING_TIME_BYTE_LOC, SAMPLING_TIME_BYTE_SIZE_NEW);
					Buffer.BlockCopy(this.samplingCycleRaw, INDEX_ZERO, this._rawData, SAMPLING_CYCLE_BYTE_LOC_NEW, SAMPLING_CYCLE_BYTE_SIZE);
					Buffer.BlockCopy(this.samplingFrequencyRaw, INDEX_ZERO, this._rawData, SAMPLING_FREQUENCY_BYTE_LOC_NEW, SAMPLING_FREQUENCY_BYTE_SIZE);
					Buffer.BlockCopy(this.samplingThresholdRaw, INDEX_ZERO, this._rawData, SAMPLING_THRESHOLD_BYTE_LOC_NEW, SAMPLING_THRESHOLD_BYTE_SIZE);
					Buffer.BlockCopy(this.samplingRecordingPerdayRaw, INDEX_ZERO, this._rawData, SAMPLING_RECORDING_PER_DAY_BYTE_LOC_WRITE, SAMPLING_RECORDING_PER_DAY_BYTE_SIZE);
					Buffer.BlockCopy(this.samplingStepsRaw, INDEX_ZERO, this._rawData, SAMPLING_STEPS_LOC, SAMPLING_STEPS_SIZE);
					Buffer.BlockCopy(this.minuteRecordingIntervalRaw, INDEX_ZERO, this._rawData, SAMPLING_MINUTE_RECORDING_LOC, SAMPLING_MINUTE_RECORDING_SIZE);
					Buffer.BlockCopy(this.maximumStepsRaw, INDEX_ZERO, this._rawData, SAMPLING_MAXIMUM_STEPS_LOC, SAMPLING_MAXIMUM_STEPS_SIZE);
					Buffer.BlockCopy(this.timeframeInSecondsRaw, INDEX_ZERO, this._rawData, SAMPLING_TIME_FRAME_IN_SECONDS_LOC, SAMPLING_TIME_FRAME_IN_SECONDS_SIZE);
					Buffer.BlockCopy(this.samplingTotalBlocksRaw, INDEX_ZERO, this._rawData, SAMPLING_TOTAL_BLOCKS_BYTE_LOC, SAMPLING_TOTAL_BLOCKS_BYTE_SIZE);

					byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
					byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE_NEW);
					Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
					Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);
				}
				/*else if (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion < 5.0f)
				{
					int samplingTimeCycle = 0x00;
					samplingTimeCycle |= (this.SamplingCycle << 4);
					samplingTimeCycle |= (this.SamplingTime);

					this.samplingTimeCycleRaw = BitConverter.GetBytes(samplingTimeCycle);

					this.samplingFrequencyRaw = BitConverter.GetBytes(this.SamplingFrequency);
					this.samplingThresholdRaw = BitConverter.GetBytes(this.SamplingThreshold);

					Buffer.BlockCopy(this.samplingTimeCycleRaw, INDEX_ZERO, this._rawData, SAMPLING_TIME_BYTE_LOC, SAMPLING_TIME_BYTE_SIZE_OLD);
					Buffer.BlockCopy(this.samplingFrequencyRaw, INDEX_ZERO, this._rawData, SAMPLING_FREQUENCY_BYTE_LOC_OLD, SAMPLING_FREQUENCY_BYTE_SIZE);
					Buffer.BlockCopy(this.samplingThresholdRaw, INDEX_ZERO, this._rawData, SAMPLING_THRESHOLD_BYTE_LOC_OLD, SAMPLING_THRESHOLD_BYTE_SIZE);

					byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
					byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE_OLD);
					Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
					Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1)


				}*/
				else
				{
					this._rawData = new byte[COMMAND_SIZE_WRITE_ORIG + 2];
					int samplingTimeCycle = 0x00;
					samplingTimeCycle |= (this.SamplingCycle << 4);
					samplingTimeCycle |= (this.SamplingTime);

					this.samplingTimeCycleRaw = BitConverter.GetBytes(samplingTimeCycle);

					this.samplingFrequencyRaw = BitConverter.GetBytes(this.SamplingFrequency);
					this.samplingThresholdRaw = BitConverter.GetBytes(this.SamplingThreshold);

					Buffer.BlockCopy(this.samplingTimeCycleRaw, INDEX_ZERO, this._rawData, SAMPLING_TIME_BYTE_LOC, SAMPLING_TIME_BYTE_SIZE_OLD);
					Buffer.BlockCopy(this.samplingFrequencyRaw, INDEX_ZERO, this._rawData, SAMPLING_FREQUENCY_BYTE_LOC_OLD, SAMPLING_FREQUENCY_BYTE_SIZE);
					Buffer.BlockCopy(this.samplingThresholdRaw, INDEX_ZERO, this._rawData, SAMPLING_THRESHOLD_BYTE_LOC_OLD, SAMPLING_THRESHOLD_BYTE_SIZE);

					byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
					byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE_OLD);
					Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
					Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);


				}


			});

			return this._rawData;
		}
	}
}

