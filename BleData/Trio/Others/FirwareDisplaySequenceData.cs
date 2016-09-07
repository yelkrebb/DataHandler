using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.Others
{
	public class FirmwareDisplaySequenceData
	{

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x30;

		const int COMMAND_SIZE_WRITE = 2;

		const int INDEX_ZERO = 0;

		const int DATA_LOC = 2;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;


		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		/* #### Equavalent RAW data per field #####*/
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		byte[] _rawData { get; set; }
		byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public FirmwareDisplaySequenceData(TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{
			if (this._rawData != null && this._rawData.Length > 0)
				Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			if (this._readCommandRawData != null && this._readCommandRawData.Length > 0)
				Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			if (this.writeCommandResponseCodeRaw != null && this.writeCommandResponseCodeRaw.Length > 0)
				Array.Clear(this.writeCommandResponseCodeRaw, INDEX_ZERO, this.writeCommandResponseCodeRaw.Length);
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{

				this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);

				this.IsReadCommand = false;
				this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
				Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
				this.WriteCommandResponseCode = Convert.ToInt32(Utils.getDecimalValue(this.writeCommandResponseCodeRaw));

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


		public async Task<byte[]> GetWriteCommand( List<int> screenflow )
		{
			await Task.Run(() =>
			{
				
				byte screenCommandByte = 0;
				int dataSizeInBytes = 0;

				if (screenflow.Count % 2 != 0)
					screenflow.Add(0);

				dataSizeInBytes = screenflow.Count / 2;
				byte[] screenFlowData = new byte[dataSizeInBytes];

				this._rawData = new byte[COMMAND_SIZE_WRITE + dataSizeInBytes];

				for (int i = 0, j = 0; i < screenflow.Count;i++)
				{
					int screenNumber = screenflow[i];

					if ((i % 2) == 0)
					{
						screenCommandByte = (byte)screenNumber;

					}

					else
					{
						screenCommandByte = (byte)((int)screenCommandByte | (screenNumber << 4));
						screenFlowData[j++] = screenCommandByte;

					}
				}



				Buffer.BlockCopy(screenFlowData, INDEX_ZERO, this._rawData, DATA_LOC, dataSizeInBytes);
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

