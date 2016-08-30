using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.FT900;

namespace Motion.Core.Data.BleData.FT900.SettingsData
{
	public class FT900SetDeviceMode:FT900DataHandler
	{

		const int COMMAND_PREFIX = 0x02;
		const int COMMAND_ID_WRITE = 0x29;

		const int INDEX_ZERO = 0;

		const int DEVICEMODE_SETTING_LOC = 2;

		const int DEVICEMODE_SETTING_BYTE_SIZE = 1;

		public bool ShipmentBootUpFlag { get; set; }
		public int EnableBroadcastAlways { get; set; }


		/* #### Equavalent RAW data per field #####*/
		byte[] deviceModeSettingRaw;

		/* ### End Raw data per field ### */

		public byte[] _rawData;
		public byte[] _readCommandRawData;

		FT900DeviceInformation ft900DevInfo;

		public FT900SetDeviceMode(FT900DeviceInformation devInfo)
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
			if (this.deviceModeSettingRaw != null && this.deviceModeSettingRaw.Length > 0)
				Array.Clear(this.deviceModeSettingRaw, INDEX_ZERO, this.deviceModeSettingRaw.Length);

		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{

				this.deviceModeSettingRaw = new byte[DEVICEMODE_SETTING_BYTE_SIZE];
				Array.Copy(this._rawData, DEVICEMODE_SETTING_LOC, this.deviceModeSettingRaw, INDEX_ZERO, DEVICEMODE_SETTING_BYTE_SIZE);

				int flagValue = BitConverter.ToInt32(this.deviceModeSettingRaw, INDEX_ZERO);
				this.ShipmentBootUpFlag = Convert.ToBoolean((flagValue >> 7) & 0x01);
				this.EnableBroadcastAlways = flagValue & 0x7F;

				parsingStatus = BLEParsingStatus.SUCCESS;

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
				int flagValue = 0x00;
				flagValue |= (this.ShipmentBootUpFlag ? 0x01 : 0x00) << 7;
				flagValue |= this.EnableBroadcastAlways;
				this.deviceModeSettingRaw = BitConverter.GetBytes(flagValue);
				Buffer.BlockCopy(this.deviceModeSettingRaw, 0, this._rawData, DEVICEMODE_SETTING_LOC, DEVICEMODE_SETTING_BYTE_SIZE);

				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}


	}
}

