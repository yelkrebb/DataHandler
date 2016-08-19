﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.FT900;

namespace Motion.Core.Data.BleData.FT900.SettingsData
{
	public class FT900DeviceAlarm:FT900DataHandler
	{	
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x2A;

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


		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		FT900DeviceInformation ft900DevInfo;
		
		public FT900DeviceAlarm(FT900DeviceInformation devInfo)
		{
			this.ft900DevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			Array.Clear(this.alarmDurationRaw, INDEX_ZERO, this.alarmDurationRaw.Length);
			Array.Clear(this.alarmBeepTypeRaw, INDEX_ZERO, this.alarmBeepTypeRaw.Length);

		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
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


			});

			return parsingStatus;
		}

		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{
				
			});

			throw new NotImplementedException();
		}

		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() =>
			{


				Buffer.BlockCopy(this.alarmDurationRaw, 0, this._rawData, ALARM_DURATION_LOC, ALARM_DURATION_BYTE_SIZE);
				Buffer.BlockCopy(this.alarmBeepTypeRaw, 0, this._rawData, ALARM_BEEP_TYPE_LOC, ALARM_BEEP_TYPE_BYTE_SIZE);
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

