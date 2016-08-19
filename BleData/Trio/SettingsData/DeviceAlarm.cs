﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.SettingsData
{
	public class DeviceAlarm:ITrioDataHandler
	{
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x2A;
		const int COMMAND_ID_READ = 0x2B;

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


		TrioDeviceInformation trioDevInfo;

		public DeviceAlarm(TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
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
				}

				else
				{
					this.alarmDurationRaw = new byte[ALARM_DURATION_BYTE_SIZE];
					this.alarmBeepTypeRaw = new byte[ALARM_BEEP_TYPE_BYTE_SIZE];
					Array.Copy(this._rawData, ALARM_DURATION_LOC, this.alarmDurationRaw, INDEX_ZERO, ALARM_DURATION_BYTE_SIZE);
					Array.Copy(this._rawData, ALARM_BEEP_TYPE_LOC, this.alarmBeepTypeRaw, INDEX_ZERO, ALARM_BEEP_TYPE_BYTE_SIZE);

					this.AlarmDuration =  BitConverter.ToInt32(this.alarmDurationRaw, INDEX_ZERO);
					this.BeepType = BitConverter.ToInt32(this.alarmBeepTypeRaw, INDEX_ZERO);
				}





				parsingStatus = BLEParsingStatus.SUCCESS;

			});

			return parsingStatus;
		}

		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{
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

				this.alarmDurationRaw = BitConverter.GetBytes(this.AlarmDuration);
				this.alarmBeepTypeRaw = BitConverter.GetBytes(this.BeepType);
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

