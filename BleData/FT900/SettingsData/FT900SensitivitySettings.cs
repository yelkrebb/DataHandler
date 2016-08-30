using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.FT900;
namespace Motion.Core.Data.BleData.FT900.SettingsData
{
	public class FT900SensitivitySettings: FT900DataHandler
	{
		const int COMMAND_PREFIX_WRITE = 0x02;
		const int COMMAND_PREFIX_READ = 0x01;
		const int COMMAND_ID_WRITE = 0x58;
		const int COMMAND_ID_READ = 0x59;

		const int INDEX_ZERO = 0;

		const int RELATIVE_LIMIT_LOC = 2;
		const int SENSITIVITY_OLD_LOC = 2;
		const int RELATIVE_SENSITIVITY_LOC = 3;
		const int SLEEP_THRESHOLD_LOC = 5;

		const int RELATIVE_LIMIT_SIZE = 1;
		const int SENSITIVITY_OLD_SIZE = 1;
		const int RELATIVE_SENSITIVITY_SIZE = 2;
		const int SLEEP_THRESHOLD_SIZE = 1;

		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int SensitivityOld { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		/* #### Equavalent RAW data per field #####*/
		byte[] sensitivityOldRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		public byte[] _rawData;
		public byte[] _readCommandRawData;

		FT900DeviceInformation ft900DevInfo;

		public FT900SensitivitySettings(FT900DeviceInformation devInfo)
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
			if (this.sensitivityOldRaw != null && this.sensitivityOldRaw.Length > 0)
				Array.Clear(this.sensitivityOldRaw, INDEX_ZERO, this.sensitivityOldRaw.Length);
			if (this.writeCommandResponseCodeRaw != null && this.writeCommandResponseCodeRaw.Length > 0)
				Array.Clear(this.writeCommandResponseCodeRaw, INDEX_ZERO, this.writeCommandResponseCodeRaw.Length);
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				this.IsReadCommand = true;
				if (rawData[1] == 0x58)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);

				}

				else
				{
					this.sensitivityOldRaw = new byte[SENSITIVITY_OLD_SIZE];
					Array.Copy(this._rawData, SENSITIVITY_OLD_LOC, this.sensitivityOldRaw, INDEX_ZERO, SENSITIVITY_OLD_SIZE);
					this.SensitivityOld = BitConverter.ToInt32(this.sensitivityOldRaw, INDEX_ZERO);


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
				this.sensitivityOldRaw = BitConverter.GetBytes(this.SensitivityOld);
				Buffer.BlockCopy(this.sensitivityOldRaw, 0, this._rawData, SENSITIVITY_OLD_LOC, SENSITIVITY_OLD_SIZE);

				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX_WRITE);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

