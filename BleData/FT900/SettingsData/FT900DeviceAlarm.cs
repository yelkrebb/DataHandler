using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.FT900;

namespace Motion.Core.Data.BleData.FT900.SettingsData
{
	public class FT900DeviceAlarm:FT900DataHandler
	{	
		const int COMMAND_PREFIX_WRITE = 0x03;
		const int COMMAND_PREFIX_READ = 0x01;
		const int COMMAND_ID_WRITE = 0x2A;
		const int COMMAND_ID_READ = 0x2B;

		const int COMMAND_SIZE_READ = 2;
		const int COMMAND_SIZE_WRITE = 2;

		const int INDEX_ZERO = 0;

		const int ALARM_DURATION_LOC = 2;
		const int ALARM_BEEP_TYPE_LOC = 3;

		const int ALARM_DURATION_BYTE_SIZE = 1;
		const int ALARM_BEEP_TYPE_BYTE_SIZE = 1;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int AlarmDuration { get; set; }
		public int BeepType { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }


		/* #### Equavalent RAW data per field #####*/
		byte[] alarmDurationRaw;
		byte[] alarmBeepTypeRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */


		public byte[] _rawData;
		public byte[] _readCommandRawData;

		FT900DeviceInformation ft900DevInfo;
		
		public FT900DeviceAlarm(FT900DeviceInformation devInfo)
		{
			this.ft900DevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{
			if (this._rawData != null && this._rawData.Length > 0)
				Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			if (this._readCommandRawData != null && this._readCommandRawData.Length > 0)
				Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			if (this.alarmDurationRaw != null && this.alarmDurationRaw.Length > 0)
				Array.Clear(this.alarmDurationRaw, INDEX_ZERO, this.alarmDurationRaw.Length);
			if (this.alarmBeepTypeRaw != null && this.alarmBeepTypeRaw.Length > 0)
				Array.Clear(this.alarmBeepTypeRaw, INDEX_ZERO, this.alarmBeepTypeRaw.Length);

		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);
				this.IsReadCommand = true;
				if (rawData[1] == 0x2A)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);
					parsingStatus = BLEParsingStatus.SUCCESS;
				}

				else
				{ 
					this.alarmDurationRaw = new byte[ALARM_DURATION_BYTE_SIZE];
					this.alarmBeepTypeRaw = new byte[ALARM_BEEP_TYPE_BYTE_SIZE];
					Array.Copy(this._rawData, ALARM_DURATION_LOC, this.alarmDurationRaw, INDEX_ZERO, ALARM_DURATION_BYTE_SIZE);
					Array.Copy(this._rawData, ALARM_BEEP_TYPE_LOC, this.alarmBeepTypeRaw, INDEX_ZERO, ALARM_BEEP_TYPE_BYTE_SIZE);

					this.AlarmDuration = BitConverter.ToInt32(this.alarmDurationRaw, INDEX_ZERO);
					this.BeepType = BitConverter.ToInt32(this.alarmBeepTypeRaw, INDEX_ZERO);
				}


			});

			return parsingStatus;
		}

		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{
				this._readCommandRawData = new byte[COMMAND_SIZE_READ];
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX_READ);
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

				this._rawData = new byte[COMMAND_SIZE_WRITE + 2];
				Buffer.BlockCopy(this.alarmDurationRaw, 0, this._rawData, ALARM_DURATION_LOC, ALARM_DURATION_BYTE_SIZE);
				Buffer.BlockCopy(this.alarmBeepTypeRaw, 0, this._rawData, ALARM_BEEP_TYPE_LOC, ALARM_BEEP_TYPE_BYTE_SIZE);
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX_WRITE);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

