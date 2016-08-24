using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.SettingsData
{
	public class ExerciseSettings: ITrioDataHandler
	{
		const int COMMAND_SIZE_WRITE = 2;
		const int COMMAND_SIZE_WRITE_OLD = 1;
		const int COMMAND_SIZE_READ = 2;

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x1A;
		const int COMMAND_ID_READ = 0x1C;

		const int INDEX_ZERO = 0;

		const int SYNC_TIME_INTERVAL_BYTE_LOC = 2;
		const int FLAG_BYTE_LOC = 3;

		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		//Predefined Flag values for boolean bytes
		const int DATA_SYNC_ENABLE = 16;
		const int FREQUENCY_ALARM_ENABLE = 32;
		const int MULTIPLE_INTENSITY_ENABLE = 64;

		const int DATA_SYNC_ENABLE_NEW = 1;
		const int FREQUENCY_ALARM_ENABLE_NEW = 2;
		const int MULTIPLE_INTENSITY_ENABLE_NEW = 4;

		public int SyncTimeInterval { get; set; }
		public bool DataSyncEnable { get; set; }
		public bool FrequencyAlarmEnable { get; set; }
		public bool MultipleIntensityEnable { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		/* #### Equavalent RAW data per field #####*/
		byte[] syncTimeIntervalRaw;
		byte[] flagRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		public ExerciseSettings (TrioDeviceInformation devInfo)
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
			if (this.syncTimeIntervalRaw != null && this.syncTimeIntervalRaw.Length > 0)
				Array.Clear (this.syncTimeIntervalRaw, INDEX_ZERO, this.syncTimeIntervalRaw.Length);
			if (this.flagRaw != null && this.flagRaw.Length > 0)
				Array.Clear (this.flagRaw, INDEX_ZERO, this.flagRaw.Length);
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{

			BLEParsingStatus parseStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{ 
				this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);
				this.IsReadCommand = true;
				if (rawData[1] == 0x1A)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[Constants.INT32_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);
				}

				else
				{ 
					this.syncTimeIntervalRaw = new byte[Constants.INT32_BYTE_SIZE];
					this.flagRaw = new byte[Constants.INT32_BYTE_SIZE];

					Array.Copy(this._rawData, SYNC_TIME_INTERVAL_BYTE_LOC, this.syncTimeIntervalRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, FLAG_BYTE_LOC, this.flagRaw, INDEX_ZERO, 1);

					if (this.trioDevInfo.ModelNumber == 932 && this.trioDevInfo.FirmwareVersion <= 3.4f)
					{
						int exerciseSettingsValue = BitConverter.ToInt32(this.syncTimeIntervalRaw, INDEX_ZERO);
						this.SyncTimeInterval = exerciseSettingsValue & 0x0F;
						this.DataSyncEnable = Convert.ToBoolean((exerciseSettingsValue >> 4) & 0x01);
						this.FrequencyAlarmEnable = Convert.ToBoolean((exerciseSettingsValue >> 5) & 0x01);
						this.MultipleIntensityEnable = Convert.ToBoolean((exerciseSettingsValue >> 6) & 0x01);
					}
					else
					{
						this.SyncTimeInterval = BitConverter.ToInt32(this.syncTimeIntervalRaw, INDEX_ZERO);

						int flagValue = BitConverter.ToInt32(this.flagRaw, INDEX_ZERO);
						this.DataSyncEnable = Convert.ToBoolean(flagValue & 0x01);
						this.FrequencyAlarmEnable = Convert.ToBoolean((flagValue >> 1) & 0x01);
						this.MultipleIntensityEnable = Convert.ToBoolean((flagValue >> 2) & 0x01);
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
				byte[] commandPrefix =  BitConverter.GetBytes(COMMAND_PREFIX);
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
				if (this.trioDevInfo.ModelNumber == 932 && this.trioDevInfo.FirmwareVersion <= 3.4f)
					this._rawData = new byte[COMMAND_SIZE_WRITE_OLD + 2];
				else
					this._rawData = new byte[COMMAND_SIZE_WRITE + 2];

				int flagValue = 0x00;
				if (this.trioDevInfo.ModelNumber == 932 && this.trioDevInfo.FirmwareVersion <= 3.4f)
				{
					// if FW ver <= 3.4 and exercise sync time interval is greater than 15
					// then exercise sync time interval should be set to 1hr as default.
					// because the maximum value allowed in v3.4 and below is 15.
					this.SyncTimeInterval = this.SyncTimeInterval > 15 ? 1 : this.SyncTimeInterval;
					flagValue |= this.SyncTimeInterval;
					flagValue |= this.DataSyncEnable ? DATA_SYNC_ENABLE : 0x00;
					flagValue |= this.FrequencyAlarmEnable ? FREQUENCY_ALARM_ENABLE : 0x00;

					this.syncTimeIntervalRaw = BitConverter.GetBytes(flagValue);

					Buffer.BlockCopy(this.syncTimeIntervalRaw, 0, this._rawData, SYNC_TIME_INTERVAL_BYTE_LOC, 1);


					// moving this outside the if condition since it is common on both
					//Add the two prefix bytes needed for the commands
					//For 900, 1st byte is the command size while 2nd byte is the command ID
					//byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
					//byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
					//Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);
					//Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);
				}
				else
				{
					flagValue |= this.DataSyncEnable ? DATA_SYNC_ENABLE_NEW : 0x00;
					flagValue |= this.FrequencyAlarmEnable ? FREQUENCY_ALARM_ENABLE_NEW : 0x00;
					flagValue |= this.MultipleIntensityEnable ? MULTIPLE_INTENSITY_ENABLE_NEW : 0x00;

					this.syncTimeIntervalRaw = BitConverter.GetBytes(this.SyncTimeInterval);
					this.flagRaw = BitConverter.GetBytes(flagValue);

					Buffer.BlockCopy(this.syncTimeIntervalRaw, 0, this._rawData, SYNC_TIME_INTERVAL_BYTE_LOC, 1);
					Buffer.BlockCopy(this.flagRaw, 0, this._rawData, FLAG_BYTE_LOC, 1);

					//Add the two prefix bytes needed for the commands
					//For 93x and 96x, 1st byte is the command prefix 0x1B while 2nd byte is the command ID

				}

				//Add the two prefix bytes needed for the commands
				//For 93x and 96x, 1st byte is the command prefix 0x1B while 2nd byte is the command ID
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO+1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

