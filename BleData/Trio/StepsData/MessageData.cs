using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.StepsData
{
	public class MessageProperty
	{

		public int type { get; set; }
		public int reserved { get; set; }
		public int text { get; set; }
		public int scroll { get; set; }
		public int fontSize { get; set; }
		public int color { get; set; }
		public int xCoor { get; set; }
		public int yCoor { get; set; }
		public int bgColor { get; set; }
		public byte[] msgData { get; set; }

		internal MessageProperty()
		{

		}
	}


	public class MessageData
	{
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x4B;
		const int COMMAND_SIZE_WRITE = 2;


		const int INDEX_ZERO = 0;

		const int PACKET_NO_LOC = 2;
		const int TOTAL_MESSAGE_LOC = 4;
		const int CHECK_SUM_LOC = 7;
		const int SCROLL_DELAY_LOC = 9;
		const int MESSAGE_TYPE_LOC = 9;
		const int MESSAGE_CODE_LOC = 10;
		const int SLOT_NUM_LOC = 12;

		const int PACKET_NO_BYTE_SIZE = 2;
		const int TOTAL_MESSAGE_BYTE_SIZE = 3;
		const int CHECK_SUM_BYTE_SIZE = 2;
		const int SCROLL_DELAY_BYTE_SIZE = 1;
		const int MESSAGE_TYPE_BYTE_SIZE = 1;
		const int MESSAGE_CODE_BYTE_SIZE = 2;
		const int SLOT_NUM_BYTE_SIZE = 1;
		const int MSG_PROPERTY_BYTE_SIZE = 1;
		const int X_COOR_BYTE_SIZE = 1;
		const int Y_COOR_BYTE_SIZE = 1;
		const int BG_COLOR_BYTE_SIZE = 2;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;


		public int PacketNum { get; set; }
		public int TotalSurveyAlerts { get; set; }
		public int CheckSum { get; set; }
		public int ScrollingDelay { get; set; }
		public int MessageType { get; set; }
		public int MessageCode { get; set; }
		public int SlotNumber { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }
		public List<MessageProperty> messagePropList;

		/* #### Equavalent RAW data per field #####*/
		byte[] packetNumRaw;
		byte[] totalSurveyAlertsRaw;
		byte[] checkSumRaw;
		byte[] scrollingDelayRaw;
		byte[] messageTypeRaw;
		byte[] messageCodeRaw;
		byte[] slotNumberRaw;
		byte[] messagePropertyRaw;
		byte[] xCoorRaw;
		byte[] yCoorRaw;
		byte[] bgColorRaw;
 		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */


		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public MessageData(TrioDeviceInformation devInfo)
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

				if (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f)
				{
					this.packetNumRaw = new byte[PACKET_NO_BYTE_SIZE];
					Array.Copy(this._rawData, 3, this.packetNumRaw, INDEX_ZERO, PACKET_NO_BYTE_SIZE);
					this.PacketNum = Convert.ToInt32(Utils.getDecimalValue(this.packetNumRaw));
				}


				parsingStatus = BLEParsingStatus.SUCCESS;

			});

			return parsingStatus;
		}


		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() =>
			{
				int msgsByteCount = 0;
				int msgPropCount = 0;
				for (int i = 0; i < messagePropList.Count; i++)
				{
					MessageProperty messageProp = messagePropList[i];
					msgsByteCount += messageProp.msgData.Length;
					msgPropCount += 5; // byte size for the rest of the fixed sized properties
				}

				this._rawData = new byte[COMMAND_SIZE_WRITE + 2];

				this.packetNumRaw = BitConverter.GetBytes(this.PacketNum);
				this.totalSurveyAlertsRaw = BitConverter.GetBytes(this.TotalSurveyAlerts);
				this.checkSumRaw = BitConverter.GetBytes(this.CheckSum);
				this.messageCodeRaw = BitConverter.GetBytes(this.MessageCode);
				this.slotNumberRaw = BitConverter.GetBytes(this.SlotNumber);



				Buffer.BlockCopy(this.packetNumRaw, INDEX_ZERO, this._rawData, PACKET_NO_LOC, PACKET_NO_BYTE_SIZE);
				Buffer.BlockCopy(this.totalSurveyAlertsRaw, INDEX_ZERO, this._rawData, TOTAL_MESSAGE_LOC, TOTAL_MESSAGE_BYTE_SIZE);
				Buffer.BlockCopy(this.checkSumRaw, INDEX_ZERO, this._rawData, CHECK_SUM_LOC, CHECK_SUM_BYTE_SIZE);
				Buffer.BlockCopy(this.messageCodeRaw, INDEX_ZERO, this._rawData, CHECK_SUM_LOC, CHECK_SUM_BYTE_SIZE);
				Buffer.BlockCopy(this.slotNumberRaw, INDEX_ZERO, this._rawData, CHECK_SUM_LOC, CHECK_SUM_BYTE_SIZE);


				if (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f)
				{
					this.scrollingDelayRaw = BitConverter.GetBytes(this.ScrollingDelay);
					Buffer.BlockCopy(this.scrollingDelayRaw, INDEX_ZERO, this._rawData, SCROLL_DELAY_LOC, SCROLL_DELAY_BYTE_SIZE);
				}
				else
				{ 
					this.messageTypeRaw = BitConverter.GetBytes(this.MessageType);
					Buffer.BlockCopy(this.messageTypeRaw, INDEX_ZERO, this._rawData, MESSAGE_TYPE_LOC, MESSAGE_TYPE_BYTE_SIZE);
				}

				int indexPos = SLOT_NUM_LOC + SLOT_NUM_BYTE_SIZE;
				for (int i = 0; i < messagePropList.Count; i++)
				{
					MessageProperty messageProp = messagePropList[i];

					if (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f)
					{
						int flagValue = 0x00;
						flagValue |= messageProp.type << 13;
						flagValue |= messageProp.scroll << 6;
						flagValue |= messageProp.fontSize << 4;
						flagValue |= messageProp.color > 7 ? 7 : messageProp.color;
						this.messagePropertyRaw = BitConverter.GetBytes(flagValue);

						this.xCoorRaw = BitConverter.GetBytes(messageProp.xCoor);
						this.yCoorRaw = BitConverter.GetBytes(messageProp.yCoor);
						this.bgColorRaw = BitConverter.GetBytes(messageProp.bgColor);

						Buffer.BlockCopy(this.messagePropertyRaw, INDEX_ZERO, this._rawData, indexPos, MSG_PROPERTY_BYTE_SIZE);
						Buffer.BlockCopy(this.xCoorRaw, INDEX_ZERO, this._rawData, indexPos + MSG_PROPERTY_BYTE_SIZE, X_COOR_BYTE_SIZE);
						Buffer.BlockCopy(this.yCoorRaw, INDEX_ZERO, this._rawData, indexPos + X_COOR_BYTE_SIZE, Y_COOR_BYTE_SIZE);
						Buffer.BlockCopy(this.bgColorRaw, INDEX_ZERO, this._rawData, indexPos + Y_COOR_BYTE_SIZE, BG_COLOR_BYTE_SIZE);
						Buffer.BlockCopy(messageProp.msgData, INDEX_ZERO, this._rawData, indexPos + BG_COLOR_BYTE_SIZE, messageProp.msgData.Length);

					}

					else
					{ 
						int flagValue = 0x00;
						flagValue |= messageProp.scroll << 6;
						flagValue |= messageProp.fontSize << 4;
						flagValue |= messageProp.color > 7 ? 7 : messageProp.color;
						this.messagePropertyRaw = BitConverter.GetBytes(flagValue);

						this.xCoorRaw = BitConverter.GetBytes(messageProp.xCoor);
						this.yCoorRaw = BitConverter.GetBytes(messageProp.yCoor);
						this.bgColorRaw = BitConverter.GetBytes(messageProp.bgColor);

						Buffer.BlockCopy(this.messagePropertyRaw, INDEX_ZERO, this._rawData, indexPos, MSG_PROPERTY_BYTE_SIZE);
						Buffer.BlockCopy(this.xCoorRaw, INDEX_ZERO, this._rawData, indexPos + MSG_PROPERTY_BYTE_SIZE, X_COOR_BYTE_SIZE);
						Buffer.BlockCopy(this.yCoorRaw, INDEX_ZERO, this._rawData, indexPos + X_COOR_BYTE_SIZE, Y_COOR_BYTE_SIZE);
						Buffer.BlockCopy(this.bgColorRaw, INDEX_ZERO, this._rawData, indexPos + Y_COOR_BYTE_SIZE, BG_COLOR_BYTE_SIZE);
						Buffer.BlockCopy(messageProp.msgData, INDEX_ZERO, this._rawData, indexPos + BG_COLOR_BYTE_SIZE, messageProp.msgData.Length);
					}


				}


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

