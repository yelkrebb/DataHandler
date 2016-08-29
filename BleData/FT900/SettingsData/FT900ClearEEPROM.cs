using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.FT900;

namespace Motion.Core.Data.BleData.FT900.SettingsData
{
	public class FT900ClearEEPROM:FT900DataHandler
	{
		const int COMMAND_PREFIX = 0x02;
		const int COMMAND_ID_WRITE = 0x28;

		const int INDEX_ZERO = 0;

		const int EEPROM_DATA_LOC = 2;

		const int EEPROM_DATA_BYTE_SIZE = 1;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public bool ExerciseData { get; set; }
		public bool SettingsData { get; set; }
		public bool Tallies { get; set; }
		public bool ClearProfile { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }


		/* #### Equavalent RAW data per field #####*/
		byte[] eepRomDataRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		byte[] _rawData;
		byte[] _readCommandRawData;


		FT900DeviceInformation ft900DevInfo;

		public FT900ClearEEPROM(FT900DeviceInformation devInfo)
		{
			this.ft900DevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			Array.Clear(this.eepRomDataRaw, INDEX_ZERO, this.eepRomDataRaw.Length);
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				Array.Copy(rawData, this._rawData, rawData.Length);
				this.IsReadCommand = true;
				if (rawData[1] == 0x28)
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

				int flagValue = 0x00;
				flagValue |= this.ExerciseData ? 0x01 : 0x00;
				flagValue |= this.SettingsData ? 0x01 : 0x00;
				flagValue |= this.Tallies ? 0x01 : 0x00;
				flagValue |= this.ClearProfile ? 0x01 : 0x00;

				this.eepRomDataRaw = BitConverter.GetBytes(flagValue);

				Buffer.BlockCopy(this.eepRomDataRaw, 0, this._rawData, EEPROM_DATA_LOC, EEPROM_DATA_BYTE_SIZE);


				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

