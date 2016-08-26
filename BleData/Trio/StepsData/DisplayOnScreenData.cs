using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.StepsData
{
	public class DisplayOnScreenData
	{
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x54;

		const int COMMAND_SIZE_WRITE = 7;

		const int INDEX_ZERO = 0;

		const int PACKET_NO_LOC = 2;
		const int TOTAL_MESSAGE_LOC = 4;
		const int CHECK_SUM_LOC = 7;

		const int PACKET_NO_BYTE_SIZE = 2;
		const int TOTAL_MESSAGE_BYTE_SIZE = 3;
		const int CHECK_SUM_BYTE_SIZE = 2;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int PacketNum { get; set; }
		public int TotalMessage { get; set; }
		public int CheckSum { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		/* #### Equavalent RAW data per field #####*/
		byte[] packetNumRaw;
		byte[] totalMessageRaw;
		byte[] checkSumRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public DisplayOnScreenData(TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{

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

		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() =>
			{
				this._rawData = new byte[COMMAND_SIZE_WRITE + 2];

				this.packetNumRaw = BitConverter.GetBytes(this.PacketNum);
				this.totalMessageRaw = BitConverter.GetBytes(this.TotalMessage);
				this.checkSumRaw = BitConverter.GetBytes(this.CheckSum);

				Buffer.BlockCopy(this.packetNumRaw, INDEX_ZERO, this._rawData, PACKET_NO_LOC, PACKET_NO_BYTE_SIZE);
				Buffer.BlockCopy(this.totalMessageRaw, INDEX_ZERO, this._rawData, TOTAL_MESSAGE_LOC, TOTAL_MESSAGE_BYTE_SIZE);
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

