using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.FT900;

namespace Motion.Core.Data.BleData.FT900.SettingsData
{
	public class FT900DeviceStatus:FT900DataHandler
	{

		const int COMMAND_PREFIX_READ = 0x01;
		const int COMMAND_PREFIX_WRITE = 0x02;
		const int COMMAND_ID_WRITE = 0x12;
		const int COMMAND_ID_READ = 0x13;

		const int INDEX_ZERO = 0;

		const int DEVICE_STATUS_SETTING_LOC = 2;

		const int DEVICE_STATUS_SETTING_BYTE_SIZE = 1;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int GoalStatus { get; set; }
		public int UnusedStatus { get; set; }
		public int PairingStatus { get; set; }
		public bool IntensityMet { get; set; }
		public bool FrequencyMet { get; set; }
		public bool TenacityMet { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		/* #### Equavalent RAW data per field #####*/
		byte[] deviceStatusSettingRaw;
		private byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */


		public byte[] _rawData;
		public byte[] _readCommandRawData;

		FT900DeviceInformation ft900DevInfo;

		public FT900DeviceStatus(FT900DeviceInformation devInfo)
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
			if (this.deviceStatusSettingRaw != null && this.deviceStatusSettingRaw.Length > 0)
				Array.Clear(this.deviceStatusSettingRaw, INDEX_ZERO, this.deviceStatusSettingRaw.Length);

		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				Array.Copy(rawData, this._rawData, rawData.Length);

				this.IsReadCommand = true;
				if (rawData[1] == 0x12)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);
				}

				else
				{
					this.deviceStatusSettingRaw = new byte[DEVICE_STATUS_SETTING_BYTE_SIZE];
					Array.Copy(this._rawData, DEVICE_STATUS_SETTING_LOC, this.deviceStatusSettingRaw, INDEX_ZERO, DEVICE_STATUS_SETTING_BYTE_SIZE);

					int flagValue = BitConverter.ToInt32(this.deviceStatusSettingRaw, INDEX_ZERO);
					this.FrequencyMet = Convert.ToBoolean((flagValue >> 7) & 0x01);
					this.IntensityMet = Convert.ToBoolean((flagValue >> 6) & 0x01);
					this.TenacityMet = Convert.ToBoolean((flagValue >> 5) & 0x01);


					this.PairingStatus = flagValue & 0x03;
				}
			
				parsingStatus = BLEParsingStatus.SUCCESS;

			});

			return parsingStatus;
		}

		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{
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
				int flagValue = 0x00;
				flagValue |= this.PairingStatus;
				this.deviceStatusSettingRaw = BitConverter.GetBytes(flagValue);

				Buffer.BlockCopy(this.deviceStatusSettingRaw, 0, this._rawData, DEVICE_STATUS_SETTING_LOC, DEVICE_STATUS_SETTING_BYTE_SIZE);
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX_WRITE);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

