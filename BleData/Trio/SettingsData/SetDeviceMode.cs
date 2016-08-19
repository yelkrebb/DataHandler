using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.SettingsData
{


	public class SetDeviceMode :ITrioDataHandler
	{

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x29;

		const int INDEX_ZERO = 0;

		const int DEVICEMODE_SETTING_LOC = 2;

		const int DEVICEMODE_SETTING_BYTE_SIZE = 1;

		public bool ShipmentBootUpFlag { get; set; }
		public int EnableBroadcastAlways { get; set; }


		/* #### Equavalent RAW data per field #####*/
		byte[] deviceModeSettingRaw;

		/* ### End Raw data per field ### */

		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public SetDeviceMode(TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);

			Array.Clear(this.deviceModeSettingRaw, INDEX_ZERO, this.deviceModeSettingRaw.Length);

		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() => 
			{ 
				
				this.deviceModeSettingRaw = new byte[DEVICEMODE_SETTING_BYTE_SIZE];
				Array.Copy(this._rawData, DEVICEMODE_SETTING_LOC, this.deviceModeSettingRaw, INDEX_ZERO, DEVICEMODE_SETTING_BYTE_SIZE);

				if (this.trioDevInfo.ModelNumber == 932 || this.trioDevInfo.ModelNumber == 939)
				{
					int flagValue = BitConverter.ToInt32(this.deviceModeSettingRaw, INDEX_ZERO);
					this.EnableBroadcastAlways = Convert.ToInt32(flagValue);
				}
				else
				{
					int flagValue = BitConverter.ToInt32(this.deviceModeSettingRaw, INDEX_ZERO);
					this.ShipmentBootUpFlag = Convert.ToBoolean((flagValue >> 7) & 0x01);
					this.EnableBroadcastAlways = flagValue & 0x7F;
				}

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
				if (this.trioDevInfo.ModelNumber == 932 || this.trioDevInfo.ModelNumber == 939 || this.trioDevInfo.ModelNumber == 936)
				{
					this.deviceModeSettingRaw = BitConverter.GetBytes(this.EnableBroadcastAlways);
				}

				else
				{
					int flagValue = 0x00;
					flagValue |= (this.ShipmentBootUpFlag ? 0x01 : 0x00) << 7;
					flagValue |= this.EnableBroadcastAlways;
					this.deviceModeSettingRaw = BitConverter.GetBytes(flagValue);
				}

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

