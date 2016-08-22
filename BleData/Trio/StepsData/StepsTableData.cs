using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.StepsData
{
	public class StepsTableParameters
	{

		public int dbYear { get; set; }
		public int dbMonth { get; set; }
		public int dbDay { get; set; }
		public int dbHourNumber { get; set; }
		public int sentHourFlag { get; set; }
		public int signatureGenerated { get; set; }
		public int signatureSent { get; set; }
		public int fraudTable { get; set; }

		internal StepsTableParameters()
		{

		}
	}

	public class StepsTableData
	{
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_READ = 0x22;
		const int COMMAND_ID_WRITE = 0x25;
		const int INDEX_ZERO = 0;


		const int YEAR_DATA_BYTE_WRITE_LOC = 2;
		const int MONTH_DATA_BYTE_WRITE_LOC = 3;
		const int DAY_DATA_BYTE_WRITE_LOC = 4;
		const int SENTHOUR_BYTE_WRITE_LOC = 5;
		const int FRAUD_BYTE_WRITE_LOC = 8;

		const int PACKET_NUM_BYTE_LOC = 2;
		const int YEAR_DATA_BYTE_LOC = 3;
		const int MONTH_DATA_BYTE_LOC = 4;
		const int DAY_DATA_BYTE_LOC = 5;
		const int HOUR_NUM_BYTE_LOC = 6;
		const int SENT_HOUR_BYTE_LOC = 7;
		const int PROFILE_GENERATED_BYTE_LOC = 10;

		const int PACKET_NUM_BYTE_SIZE = 1;
		const int YEAR_DATA_BYTE_SIZE = 1;
		const int MONTH_DATA_BYTE_SIZE = 1;
		const int DAY_DATA_BYTE_SIZE = 1;
		const int HOUR_NUM_BYTE_SIZE = 1;
		const int SENT_HOUR_BYTE_SIZE = 3;
		const int PROFILE_GENERATED_BYTE_SIZE = 1;
		const int FRAUD_TABLE_BYTE_SIZE = 1;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public List<StepsTableParameters> stepsDataTable;
		public int PacketHandshakeReadCommandValue { get; set; }
		public int FraudTableCommandValue { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }


		/* #### Equavalent RAW data per field #####*/
		byte[] yrDataRaw;
		byte[] monthDataRaw;
		byte[] dayDataRaw;
		byte[] hrNumberRaw;
		byte[] sentHourFlagRaw;
		byte[] profileGeneratedRaw;
		byte[] fraudTableRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */


		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public StepsTableData(TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData();
			this.ClearStepsData();
		}

		private void ClearData()
		{
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			Array.Clear(this.yrDataRaw, INDEX_ZERO, this.yrDataRaw.Length);
			Array.Clear(this.monthDataRaw, INDEX_ZERO, this.monthDataRaw.Length);
			Array.Clear(this.dayDataRaw, INDEX_ZERO, this.dayDataRaw.Length);
			Array.Clear(this.hrNumberRaw, INDEX_ZERO, this.hrNumberRaw.Length);
			Array.Clear(this.sentHourFlagRaw, INDEX_ZERO, this.sentHourFlagRaw.Length);
			Array.Clear(this.profileGeneratedRaw, INDEX_ZERO, this.profileGeneratedRaw.Length);
			Array.Clear(this.fraudTableRaw, INDEX_ZERO, this.fraudTableRaw.Length);
			Array.Clear(this.writeCommandResponseCodeRaw, INDEX_ZERO, this.writeCommandResponseCodeRaw.Length);
		}

		private void ClearStepsData()
		{
			this.stepsDataTable.Clear();
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				Array.Copy(rawData, this._rawData, rawData.Length);


				this.IsReadCommand = true;
				if (rawData[1] == 0x25)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];

					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);


					this.fraudTableRaw = new byte[FRAUD_TABLE_BYTE_SIZE];
					Array.Copy(this._rawData, 3, this.fraudTableRaw, INDEX_ZERO, FRAUD_TABLE_BYTE_SIZE);
					this.FraudTableCommandValue = BitConverter.ToInt32(this.fraudTableRaw, INDEX_ZERO);

				}
				else //(rawData[1] == 0x22)
				{
					this.stepsDataTable = new List<StepsTableParameters>();
					int byteTotalPerResponse = 19;

					if ((this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f))
						byteTotalPerResponse = 20;



					int len = (this._rawData.Length / byteTotalPerResponse) * 2;
					int currentIndex = 0;
					for (int i = 0; i < len; i++)
					{


						StepsTableParameters stepParams = new StepsTableParameters();

						int dataLen = 8;
						if (i % 2 == 0)
						{
							currentIndex = currentIndex + YEAR_DATA_BYTE_LOC;

						}

						if ((i % 2 != 0) && (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f))
						{
							dataLen = 9;
						}


						this.yrDataRaw = new byte[YEAR_DATA_BYTE_SIZE];
						this.monthDataRaw = new byte[MONTH_DATA_BYTE_SIZE];
						this.dayDataRaw = new byte[DAY_DATA_BYTE_SIZE];
						this.hrNumberRaw = new byte[HOUR_NUM_BYTE_SIZE];
						this.sentHourFlagRaw = new byte[SENT_HOUR_BYTE_SIZE];
						this.profileGeneratedRaw = new byte[PROFILE_GENERATED_BYTE_SIZE];


						Array.Copy(this._rawData, currentIndex, this.yrDataRaw, INDEX_ZERO, YEAR_DATA_BYTE_SIZE);
						Array.Copy(this._rawData, currentIndex + YEAR_DATA_BYTE_SIZE, this.monthDataRaw, INDEX_ZERO, MONTH_DATA_BYTE_SIZE);
						Array.Copy(this._rawData, currentIndex + MONTH_DATA_BYTE_SIZE, this.dayDataRaw, INDEX_ZERO, DAY_DATA_BYTE_SIZE);
						Array.Copy(this._rawData, currentIndex + DAY_DATA_BYTE_SIZE, this.hrNumberRaw, INDEX_ZERO, HOUR_NUM_BYTE_SIZE);
						Array.Copy(this._rawData, currentIndex + HOUR_NUM_BYTE_SIZE, this.sentHourFlagRaw, INDEX_ZERO, SENT_HOUR_BYTE_SIZE);
						Array.Copy(this._rawData, currentIndex + SENT_HOUR_BYTE_SIZE, this.profileGeneratedRaw, INDEX_ZERO, PROFILE_GENERATED_BYTE_SIZE);


						stepParams.dbYear = BitConverter.ToInt32(this.yrDataRaw, INDEX_ZERO);
						stepParams.dbMonth = BitConverter.ToInt32(this.monthDataRaw, INDEX_ZERO);
						stepParams.dbDay = BitConverter.ToInt32(this.dayDataRaw, INDEX_ZERO);
						stepParams.dbHourNumber = BitConverter.ToInt32(this.hrNumberRaw, INDEX_ZERO);
						stepParams.sentHourFlag = BitConverter.ToInt32(this.sentHourFlagRaw, INDEX_ZERO);
						int flagValue = BitConverter.ToInt32(this.profileGeneratedRaw, INDEX_ZERO);
						stepParams.signatureGenerated = Convert.ToInt32(flagValue & 0xFF);
						stepParams.signatureSent = Convert.ToInt32(flagValue & 0xF0);

						if (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f)
						//stepsDataTable
						{
							this.fraudTableRaw = new byte[FRAUD_TABLE_BYTE_SIZE];
							Array.Copy(this._rawData, currentIndex + PROFILE_GENERATED_BYTE_SIZE, this.fraudTableRaw, INDEX_ZERO, FRAUD_TABLE_BYTE_SIZE);
							stepParams.fraudTable = BitConverter.ToInt32(this.fraudTableRaw, INDEX_ZERO);
						}

						currentIndex = currentIndex + dataLen;
						stepsDataTable.Add(stepParams);
					}

					parsingStatus = BLEParsingStatus.SUCCESS;
				}



			});

			return parsingStatus;
		}


		public async Task<byte[]> GetReadStepTableDataCommand()
		{
			await Task.Run(() =>
			{
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ);

				if (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f)
				{
					byte[] packetHandshake = BitConverter.GetBytes(this.PacketHandshakeReadCommandValue);
					byte[] fraudTable = BitConverter.GetBytes(this.FraudTableCommandValue);
					Buffer.BlockCopy(packetHandshake, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 2, 1);
					Buffer.BlockCopy(fraudTable, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 3, 1);
				}



				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
			});
			return this._readCommandRawData;

		}

		public async Task<byte[]> GetWriteStepTableDataCommand(StepsTableParameters stepsParamWrite)
		{
			await Task.Run(() =>
			{

				this.yrDataRaw = BitConverter.GetBytes(stepsParamWrite.dbYear);
				this.monthDataRaw = BitConverter.GetBytes(stepsParamWrite.dbMonth);
				this.dayDataRaw = BitConverter.GetBytes(stepsParamWrite.dbDay);
				this.sentHourFlagRaw = BitConverter.GetBytes(stepsParamWrite.sentHourFlag);
				this.fraudTableRaw = BitConverter.GetBytes(stepsParamWrite.fraudTable);

				Buffer.BlockCopy(this.yrDataRaw, INDEX_ZERO, this._rawData, YEAR_DATA_BYTE_WRITE_LOC, 1);
				Buffer.BlockCopy(this.monthDataRaw, INDEX_ZERO, this._rawData, MONTH_DATA_BYTE_WRITE_LOC, 1);
				Buffer.BlockCopy(this.dayDataRaw, INDEX_ZERO, this._rawData, DAY_DATA_BYTE_WRITE_LOC, 1);
				Buffer.BlockCopy(this.sentHourFlagRaw, INDEX_ZERO, this._rawData, SENTHOUR_BYTE_WRITE_LOC, 3);
				Buffer.BlockCopy(this.fraudTableRaw, INDEX_ZERO, this._rawData, FRAUD_BYTE_WRITE_LOC, 1);


				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}

	}
}

