using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.Others
{
	public class DisplayOnScreenData
	{
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x54;

		const int COMMAND_SIZE_WRITE = 13;

		const int INDEX_ZERO = 0;

		const int PACKET_NO_LOC = 2;
		const int TOTAL_MESSAGE_LOC = 4;
		const int CHECK_SUM_LOC = 7;
		const int PROPERTY_LOC = 9;
		const int XCOOR_LOC = 10;
		const int YCOOR_LOC = 11;
		const int BG_COLOR_LOC = 12;
		const int MSG_BYTES_LOC = 14;

		const int TERMINATOR_SIZE = 1;
		const int PACKET_NO_BYTE_SIZE = 2;
		const int TOTAL_MESSAGE_BYTE_SIZE = 3;
		const int CHECK_SUM_BYTE_SIZE = 2;
		const int PROPERTY_BYTE_SIZE = 1;
		const int XCOOR_BYTE_SIZE = 1;
		const int YCOOR_BYTE_SIZE = 1;
		const int BG_COLOR_BYTE_SIZE = 2;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int PacketNum { get; set; }
		public int TotalMessage { get; set; }
		public int CheckSum { get; set; }
		public int PropertyText { get; set; }
		public bool PropertyScroll { get; set; }
		public int PropertyFont { get; set; }
		public int PropertyColor { get; set; }
		public int XCoordinate { get; set; }
		public int YCoordinate { get; set; }
		public int BackgroundColor { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		/* #### Equavalent RAW data per field #####*/
		byte[] packetNumRaw;
		byte[] totalMessageRaw;
		byte[] checkSumRaw;
		byte[] propertyRaw;
		byte[] xCoorRaw;
		byte[] yCoorRaw;
		byte[] bgColorRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		byte[] _rawData { get; set; }
		byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public DisplayOnScreenData(TrioDeviceInformation devInfo)
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
			if (this.packetNumRaw != null && this.packetNumRaw.Length > 0)
				Array.Clear(this.packetNumRaw, INDEX_ZERO, this.packetNumRaw.Length);
			if (this.totalMessageRaw != null && this.totalMessageRaw.Length > 0)
				Array.Clear(this.totalMessageRaw, INDEX_ZERO, this.totalMessageRaw.Length);
			if (this.checkSumRaw != null && this.checkSumRaw.Length > 0)
				Array.Clear(this.checkSumRaw, INDEX_ZERO, this.checkSumRaw.Length);
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

		public async Task<byte[]> GetWriteCommand(byte[] messageData)
		{
			await Task.Run(() =>
			{
				// preset data
				this.PacketNum = 0;
				this.PropertyText = 0;
				this.PropertyScroll = false;
				this.PropertyFont = 0;
				this.PropertyColor = 7;
				this.XCoordinate = 45;
				int terminatorVal = 255;



				this._rawData = new byte[COMMAND_SIZE_WRITE + messageData.Length + 2];

				byte[] terminator = BitConverter.GetBytes(terminatorVal);
				int mesageCommandForCheckSumSize = PROPERTY_BYTE_SIZE + XCOOR_BYTE_SIZE + YCOOR_BYTE_SIZE + BG_COLOR_BYTE_SIZE + messageData.Length + 1;
				byte[] messageCommandForCheckSum = new byte[mesageCommandForCheckSumSize];

				this.TotalMessage = mesageCommandForCheckSumSize + 2;


				this.packetNumRaw = BitConverter.GetBytes(BitConverter.ToInt16(BitConverter.GetBytes(this.PacketNum), 0));
				this.totalMessageRaw = BitConverter.GetBytes(this.TotalMessage);

				int flagValue = 0x00;
				flagValue |= this.PropertyText << 7;
				flagValue |=( Convert.ToInt32(this.PropertyScroll)) << 6;
				flagValue |= this.PropertyFont << 4;
				flagValue |= this.PropertyColor;
				this.propertyRaw = BitConverter.GetBytes(flagValue);

				this.xCoorRaw = BitConverter.GetBytes(this.XCoordinate);
				this.yCoorRaw = BitConverter.GetBytes(this.YCoordinate);
				this.bgColorRaw = BitConverter.GetBytes(BitConverter.ToInt16(BitConverter.GetBytes(this.BackgroundColor), 0));

				if (BitConverter.IsLittleEndian)
				{
					Array.Reverse(this.packetNumRaw);
					Array.Reverse(this.bgColorRaw);
					Array.Reverse(this.totalMessageRaw);
				}



				Buffer.BlockCopy(this.packetNumRaw, INDEX_ZERO, this._rawData, PACKET_NO_LOC, PACKET_NO_BYTE_SIZE);
				Buffer.BlockCopy(this.totalMessageRaw, Utils.INDEX_ONE, this._rawData, TOTAL_MESSAGE_LOC, TOTAL_MESSAGE_BYTE_SIZE);
				Buffer.BlockCopy(this.propertyRaw, INDEX_ZERO, this._rawData, PROPERTY_LOC, PROPERTY_BYTE_SIZE);
				Buffer.BlockCopy(this.xCoorRaw, INDEX_ZERO, this._rawData, XCOOR_LOC, XCOOR_BYTE_SIZE);
				Buffer.BlockCopy(this.yCoorRaw, INDEX_ZERO, this._rawData, YCOOR_LOC, YCOOR_BYTE_SIZE);
				Buffer.BlockCopy(this.bgColorRaw, INDEX_ZERO, this._rawData, BG_COLOR_LOC, BG_COLOR_BYTE_SIZE);
				Buffer.BlockCopy(messageData, INDEX_ZERO, this._rawData, MSG_BYTES_LOC, messageData.Length);
				Buffer.BlockCopy(terminator, INDEX_ZERO, this._rawData, MSG_BYTES_LOC + messageData.Length, TERMINATOR_SIZE);
				Buffer.BlockCopy(this._rawData, PROPERTY_LOC, messageCommandForCheckSum, INDEX_ZERO, mesageCommandForCheckSumSize);

				this.CheckSum = Utils.GetCheckSumWithBytes(messageCommandForCheckSum);
				this.checkSumRaw =BitConverter.GetBytes(BitConverter.ToInt16(BitConverter.GetBytes(this.CheckSum), 0));

				if(BitConverter.IsLittleEndian)
					Utils.reverseBytesForEndianessHandling(this.checkSumRaw);

				Buffer.BlockCopy(this.checkSumRaw, INDEX_ZERO, this._rawData, CHECK_SUM_LOC, CHECK_SUM_BYTE_SIZE);


				//Add the two prefix bytes needed for the commands
				//For 93x and 96x, 1st byte is the command prefix 0x1B while 2nd byte is the command ID
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);


			});

			return this._rawData;
		}
	}
}

