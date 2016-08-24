using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.Others
{
	public class LCDTestDisplay
	{
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x2C;
		const int COMMAND_SIZE_WRITE = 2;
		const int COMMAND_SIZE_WRITE_OLD = 7;

		const int INDEX_ZERO = 0;

		const int TEST_TYPE_LOC = 2;
		const int TYPE_SEQUENCE_LOC = 3;
		const int CHAR1_LOC = 2;
		const int CHAR2_LOC = 3;
		const int CHAR3_LOC = 4;
		const int CHAR4_LOC = 5;
		const int CHAR5_LOC = 6;
		const int ICONS_LOC = 7;

		const int TEST_TYPE_BYTE_SIZE = 1;
		const int TYPE_SEQUENCE_BYTE_SIZE = 1;
		const int CHARACTER_BYTE_SIZE = 1;
		const int ICONS_BYTE_SIZE = 2;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;


		public int TestType { get; set; }
		public int TypeSequence { get; set; }
		public int Character1 { get; set; }
		public int Character2 { get; set; }
		public int Character3 { get; set; }
		public int Character4 { get; set; }
		public int Character5 { get; set; }
		public int Icons { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		/* #### Equavalent RAW data per field #####*/
		byte[] testTypeRaw;
		byte[] typeSequenceRaw;
		byte[] character1Raw;
		byte[] character2Raw;
		byte[] character3Raw;
		byte[] character4Raw;
		byte[] character5Raw;
		byte[] iconsRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */


		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public LCDTestDisplay(TrioDeviceInformation devInfo)
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
			if (this.testTypeRaw != null && this.testTypeRaw.Length > 0)
				Array.Clear(this.testTypeRaw, INDEX_ZERO, this.testTypeRaw.Length);
			if (this.typeSequenceRaw != null && this.typeSequenceRaw.Length > 0)
				Array.Clear(this.typeSequenceRaw, INDEX_ZERO, this.typeSequenceRaw.Length);
			if (this.character1Raw != null && this.character1Raw.Length > 0)
				Array.Clear(this.character1Raw, INDEX_ZERO, this.character1Raw.Length);
			if (this.character2Raw != null && this.character2Raw.Length > 0)
				Array.Clear(this.character2Raw, INDEX_ZERO, this.character2Raw.Length);
			if (this.character3Raw != null && this.character3Raw.Length > 0)
				Array.Clear(this.character3Raw, INDEX_ZERO, this.character3Raw.Length);
			if (this.character4Raw != null && this.character4Raw.Length > 0)
				Array.Clear(this.character4Raw, INDEX_ZERO, this.character4Raw.Length);
			if (this.character5Raw != null && this.character5Raw.Length > 0)
				Array.Clear(this.character5Raw, INDEX_ZERO, this.character5Raw.Length);
			if (this.writeCommandResponseCodeRaw != null && this.writeCommandResponseCodeRaw.Length > 0)
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
					this.WriteCommandResponseCode = Convert.ToInt32(Utils.getDecimalValue(this.writeCommandResponseCodeRaw));
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
				
				if (trioDevInfo.ModelNumber != 932)
				{
					this._rawData = new byte[COMMAND_SIZE_WRITE + 2];
					this.testTypeRaw = BitConverter.GetBytes(this.TestType);
					this.typeSequenceRaw = BitConverter.GetBytes(this.TypeSequence);
					Buffer.BlockCopy(this.testTypeRaw, 0, this._rawData, TEST_TYPE_LOC, TEST_TYPE_BYTE_SIZE);
					Buffer.BlockCopy(this.typeSequenceRaw, 0, this._rawData, TYPE_SEQUENCE_LOC, TYPE_SEQUENCE_BYTE_SIZE);
					byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
					byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
					Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
					Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);
				}

				else
				{
					this._rawData = new byte[COMMAND_SIZE_WRITE_OLD + 2];
					this.character1Raw = BitConverter.GetBytes(this.Character1);
					this.character2Raw = BitConverter.GetBytes(this.Character2);
					this.character3Raw = BitConverter.GetBytes(this.Character3);
					this.character4Raw = BitConverter.GetBytes(this.Character4);
					this.character5Raw = BitConverter.GetBytes(this.Character5);
					this.iconsRaw = BitConverter.GetBytes(this.Icons);
					Buffer.BlockCopy(this.character1Raw, 0, this._rawData, CHAR1_LOC, CHARACTER_BYTE_SIZE);
					Buffer.BlockCopy(this.character2Raw, 0, this._rawData, CHAR2_LOC, CHARACTER_BYTE_SIZE);
					Buffer.BlockCopy(this.character3Raw, 0, this._rawData, CHAR3_LOC, CHARACTER_BYTE_SIZE);
					Buffer.BlockCopy(this.character4Raw, 0, this._rawData, CHAR4_LOC, CHARACTER_BYTE_SIZE);
					Buffer.BlockCopy(this.character5Raw, 0, this._rawData, CHAR5_LOC, CHARACTER_BYTE_SIZE);
					Buffer.BlockCopy(this.iconsRaw, 0, this._rawData, ICONS_LOC, ICONS_BYTE_SIZE);
					byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
					byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
					Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
					Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);
				}

			});

			return this._rawData;
		}
	}
}

