using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.FT900;

namespace Motion.Core.Data.BleData.FT900.Others
{
	public class FT900LCDTestDisplay:FT900DataHandler
	{
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x2C;

		const int INDEX_ZERO = 0;

		const int TEST_TYPE_LOC = 2;
		const int TYPE_SEQUENCE_LOC = 3;

		const int TEST_TYPE_BYTE_SIZE = 1;
		const int TYPE_SEQUENCE_BYTE_SIZE = 1;

		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int TestType { get; set; }
		public int TypeSequence { get; set; }

		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }


		/* #### Equavalent RAW data per field #####*/
		byte[] testTypeRaw;
		byte[] typeSequenceRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		FT900DeviceInformation ft900DevInfo;

		public FT900LCDTestDisplay(FT900DeviceInformation devInfo)
		{
			this.ft900DevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			Array.Clear(this.testTypeRaw, INDEX_ZERO, this.testTypeRaw.Length);
			Array.Clear(this.typeSequenceRaw, INDEX_ZERO, this.typeSequenceRaw.Length);
			Array.Clear(this.writeCommandResponseCodeRaw, INDEX_ZERO, this.writeCommandResponseCodeRaw.Length);

		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				Array.Copy(rawData, this._rawData, rawData.Length);
				this.IsReadCommand = true;
				if (rawData[1] == 0x2C)
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

				this.testTypeRaw = BitConverter.GetBytes(this.TestType);
				this.typeSequenceRaw = BitConverter.GetBytes(this.TypeSequence);
				Buffer.BlockCopy(this.testTypeRaw, 0, this._rawData, TEST_TYPE_LOC, TEST_TYPE_BYTE_SIZE);
				Buffer.BlockCopy(this.typeSequenceRaw, 0, this._rawData, TYPE_SEQUENCE_LOC, TYPE_SEQUENCE_BYTE_SIZE);
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

