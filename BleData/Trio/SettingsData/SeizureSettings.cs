using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

using Newtonsoft.Json;

namespace Motion.Core.Data.BleData.Trio.SettingsData
{
	public class SeizureSettings:ITrioDataHandler
	{
		const int COMMAND_SIZE_WRITE_OLD = 2;
		const int COMMAND_SIZE_WRITE = 3;
		const int COMMAND_SIZE_WRITE_WITH_ON_OFF = 3;
		const int COMMAND_SIZE_READ = 2;

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x31;
		const int COMMAND_ID_READ = 0x32;

		const int INDEX_ZERO = 0;

		const int SEIZURE_NO_OF_REC_BYTE_LOC = 2;
		const int SEIZURE_SAMPLING_FREQUENCY_BYTE_LOC = 3;
		const int SEIZURE_SETTINGS_BYTE_LOC = 4;


		const int SEIZURE_NO_OF_REC_BYTE_SIZE = 1;
		const int SEIZURE_SAMPLING_FREQUENCY_BYTE_SIZE = 1;
		const int SEIZURE_SETTINGS_BYTE_SIZE = 1;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		[JsonProperty(PropertyName = "szt")]
		public int SeizureSamplingTime { get; set; }
		[JsonProperty(PropertyName = "szrec")]
		public int SeizureNumberOfRecords { get; set; }
		[JsonProperty(PropertyName = "freq")]
		public int SeizureSamplingFrequency { get; set; }
		[JsonProperty(PropertyName = "szflag")]
		public bool SeizureSettingsEnable { get; set; }
		[JsonIgnore]
		public int WriteCommandResponseCode { get; set; }
		[JsonIgnore]
		public bool IsReadCommand { get; set; }

		/* #### Equavalent RAW data per field #####*/
		byte[] seizureNumberOfRecordsRaw;
		byte[] seizureSamplingFrequencyRaw;
		byte[] seizureSettingsRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		private byte[] _rawData;
		private byte[] _readCommandRawData;

		TrioDeviceInformation trioDevInfo;

		public SeizureSettings (TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData ();
		}

		private void ClearData()
		{
			if (this._rawData != null && this._rawData.Length > 0)
				Array.Clear (this._rawData,INDEX_ZERO, this._rawData.Length);
			if (this._readCommandRawData != null && this._readCommandRawData.Length > 0)
				Array.Clear (this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			if (this.seizureNumberOfRecordsRaw != null && this.seizureNumberOfRecordsRaw.Length > 0)
				Array.Clear (this.seizureNumberOfRecordsRaw, INDEX_ZERO, this.seizureNumberOfRecordsRaw.Length);
			if (this.seizureSamplingFrequencyRaw != null && this.seizureSamplingFrequencyRaw.Length > 0)
				Array.Clear (this.seizureSamplingFrequencyRaw, INDEX_ZERO, this.seizureSamplingFrequencyRaw.Length);
			if (this.seizureSettingsRaw != null && this.seizureSettingsRaw.Length > 0)
				Array.Clear (this.seizureSettingsRaw, INDEX_ZERO, this.seizureSettingsRaw.Length);

		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{

			BLEParsingStatus parseStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{ 
				this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);
				this.IsReadCommand = true;
				if (rawData[1] == 0x31)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = Convert.ToInt32(Utils.getDecimalValue(this.writeCommandResponseCodeRaw));
				}

				else
				{ 
					this.seizureNumberOfRecordsRaw = new byte[SEIZURE_NO_OF_REC_BYTE_SIZE];
					this.seizureSamplingFrequencyRaw = new byte[SEIZURE_SAMPLING_FREQUENCY_BYTE_SIZE];
					this.seizureSettingsRaw = new byte[SEIZURE_SETTINGS_BYTE_SIZE];

					Array.Copy(this._rawData, SEIZURE_NO_OF_REC_BYTE_LOC, this.seizureNumberOfRecordsRaw, INDEX_ZERO, SEIZURE_NO_OF_REC_BYTE_SIZE);
					Array.Copy(this._rawData, SEIZURE_SAMPLING_FREQUENCY_BYTE_LOC, this.seizureSamplingFrequencyRaw, INDEX_ZERO, SEIZURE_SAMPLING_FREQUENCY_BYTE_SIZE);

					this.SeizureNumberOfRecords = Convert.ToInt32(Utils.getDecimalValue(this.seizureNumberOfRecordsRaw)); 
					this.SeizureSamplingFrequency = Convert.ToInt32(Utils.getDecimalValue(this.seizureSamplingFrequencyRaw)); 

					if (!((this.trioDevInfo.ModelNumber == 936 && this.trioDevInfo.FirmwareVersion < 2.2f) ||
						(this.trioDevInfo.ModelNumber == 939 && this.trioDevInfo.FirmwareVersion < 0.3f) ||
						(this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion < 1.5f)))
					{
						Array.Copy(this._rawData, SEIZURE_SETTINGS_BYTE_LOC, this.seizureSettingsRaw, INDEX_ZERO, SEIZURE_SETTINGS_BYTE_SIZE);
						this.SeizureSettingsEnable = Convert.ToBoolean(Convert.ToInt32(Utils.getDecimalValue(this.seizureSettingsRaw)));
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
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
			});

			return this._readCommandRawData;
		}

		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() =>
			{ 
				if (!((this.trioDevInfo.ModelNumber == 936 && this.trioDevInfo.FirmwareVersion < 2.2f) ||
					(this.trioDevInfo.ModelNumber == 939 && this.trioDevInfo.FirmwareVersion < 0.3f) ||
					(this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion < 1.5f)))
				
					this._rawData = new byte[COMMAND_SIZE_WRITE + 2];
				else
					this._rawData = new byte[COMMAND_SIZE_WRITE_OLD + 2];

				this.seizureNumberOfRecordsRaw = BitConverter.GetBytes(this.SeizureNumberOfRecords);
				this.seizureSamplingFrequencyRaw = BitConverter.GetBytes(this.SeizureSamplingFrequency);

				Buffer.BlockCopy(this.seizureNumberOfRecordsRaw, INDEX_ZERO, this._rawData, SEIZURE_NO_OF_REC_BYTE_LOC, SEIZURE_NO_OF_REC_BYTE_SIZE);
				Buffer.BlockCopy(this.seizureSamplingFrequencyRaw, INDEX_ZERO, this._rawData, SEIZURE_SAMPLING_FREQUENCY_BYTE_LOC, SEIZURE_SAMPLING_FREQUENCY_BYTE_SIZE);
				if (!((this.trioDevInfo.ModelNumber == 936 && this.trioDevInfo.FirmwareVersion < 2.2f) ||
					(this.trioDevInfo.ModelNumber == 939 && this.trioDevInfo.FirmwareVersion < 0.3f) ||
					(this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion < 1.5f)))
				{
					this.seizureSettingsRaw = BitConverter.GetBytes(Convert.ToInt32(this.SeizureSettingsEnable));
					Buffer.BlockCopy(this.seizureSettingsRaw, INDEX_ZERO, this._rawData, SEIZURE_SETTINGS_BYTE_LOC, SEIZURE_SETTINGS_BYTE_SIZE);
				}

				//Add the two prefix bytes needed for the commands
				//For 93x and 96x, 1st byte is the command prefix 0x1B while 2nd byte is the command ID
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);


			});

			return this._rawData;
		}
	}
}

