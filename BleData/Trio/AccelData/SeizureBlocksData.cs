using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.StepsData
{

	public class SeizureBlocksParameters
	{

		public int sbYear { get; set; }
		public int sbMonth { get; set; }
		public int sbDay { get; set; }
		public int sbHour { get; set; }
		public int sbMinute { get; set; }
		public int sbRecordStatus { get; set; }
		public int sbSeizureBlock { get; set; }
		public bool sbIsNewest { get; set; }

		internal SeizureBlocksParameters()
		{

		}
	}

	public class SeizureBlocksParametersOld
	{

		public int sbSeizureBlock { get; set; }
		public bool sbIsNewest { get; set; }

		internal SeizureBlocksParametersOld()
		{

		}
	}

	public class SeizureBlocksData
	{

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x35;
		const int COMMAND_ID_READ = 0x34;
		const int COMMAND_SIZE_READ = 2;
		const int COMMAND_SIZE_WRITE = 5;

		const int MAX_RECORD = 2;
		const int INDEX_ZERO = 0;

		const int SEIZURE_REC_TO_READ_LOC = 2;
		const int SEIZURE_BLOCK_LOC = 3;

		const int YEAR_DATA_BYTE_SIZE = 1;
		const int MONTH_DATA_BYTE_SIZE = 1;
		const int DAY_DATA_BYTE_SIZE = 1;
		const int HOUR_NUM_BYTE_SIZE = 1;
		const int MIN_NUM_BYTE_SIZE = 1;
		const int RECORD_STATUS_BYTE_SIZE = 1;
		const int SEIZURE_BLOCK_BYTE_SIZE = 4;
		const int SEIZURE_REC_TO_READ_BYTE_SIZE = 1;

		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }


		public List<SeizureBlocksParameters> seizureBlocksData;
		public List<SeizureBlocksParametersOld> seizureBlocksDataOld;


		/* #### Equavalent RAW data per field #####*/
		byte[] yrDataRaw;
		byte[] monthDataRaw;
		byte[] dayDataRaw;
		byte[] hourDataRaw;
		byte[] recordStatusDataRaw;
		byte[] seizureBlockRaw;
		byte[] seizureRecordToUpdateRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public SeizureBlocksData(TrioDeviceInformation devInfo)
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
			if (this.yrDataRaw != null && this.yrDataRaw.Length > 0)
				Array.Clear(this.yrDataRaw, INDEX_ZERO, this.yrDataRaw.Length);
			if (this.monthDataRaw != null && this.monthDataRaw.Length > 0)
				Array.Clear(this.monthDataRaw, INDEX_ZERO, this.monthDataRaw.Length);
			if (this.dayDataRaw != null && this.dayDataRaw.Length > 0)
				Array.Clear(this.dayDataRaw, INDEX_ZERO, this.dayDataRaw.Length);
			if (this.hourDataRaw != null && this.hourDataRaw.Length > 0)
				Array.Clear(this.hourDataRaw, INDEX_ZERO, this.hourDataRaw.Length);
			if (this.recordStatusDataRaw != null && this.recordStatusDataRaw.Length > 0)
				Array.Clear(this.recordStatusDataRaw, INDEX_ZERO, this.recordStatusDataRaw.Length);
			if (this.seizureBlockRaw != null && this.seizureBlockRaw.Length > 0)
				Array.Clear(this.seizureBlockRaw, INDEX_ZERO, this.seizureBlockRaw.Length);
			if (this.seizureRecordToUpdateRaw != null && this.seizureRecordToUpdateRaw.Length > 0)
				Array.Clear(this.seizureRecordToUpdateRaw, INDEX_ZERO, this.seizureRecordToUpdateRaw.Length);
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
				this.IsReadCommand = true;
				if (rawData[1] == 0x35)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = Convert.ToInt32(Utils.getDecimalValue(this.writeCommandResponseCodeRaw));
				}
				else //(rawData[1] == 0x22)
				{
					this.seizureBlocksData = new List<SeizureBlocksParameters>();
					if (this.trioDevInfo.ModelNumber == 961)
					{
						int dataLen = 9;
						int currentIndex = 2;
						for (int i = 0; i < 2; i++)
						{

							SeizureBlocksParameters stepParams = new SeizureBlocksParameters();


							this.yrDataRaw = new byte[YEAR_DATA_BYTE_SIZE];
							this.monthDataRaw = new byte[MONTH_DATA_BYTE_SIZE];
							this.dayDataRaw = new byte[DAY_DATA_BYTE_SIZE];
							this.hourDataRaw = new byte[HOUR_NUM_BYTE_SIZE];
							this.recordStatusDataRaw = new byte[RECORD_STATUS_BYTE_SIZE];
							this.seizureBlockRaw = new byte[SEIZURE_BLOCK_BYTE_SIZE];

							Array.Copy(this._rawData, currentIndex, this.yrDataRaw, INDEX_ZERO, YEAR_DATA_BYTE_SIZE);
							Array.Copy(this._rawData, currentIndex + YEAR_DATA_BYTE_SIZE, this.monthDataRaw, INDEX_ZERO, MONTH_DATA_BYTE_SIZE);
							Array.Copy(this._rawData, currentIndex + MONTH_DATA_BYTE_SIZE, this.dayDataRaw, INDEX_ZERO, DAY_DATA_BYTE_SIZE);
							Array.Copy(this._rawData, currentIndex + DAY_DATA_BYTE_SIZE, this.hourDataRaw, INDEX_ZERO, HOUR_NUM_BYTE_SIZE);
							Array.Copy(this._rawData, currentIndex + HOUR_NUM_BYTE_SIZE, this.recordStatusDataRaw, INDEX_ZERO, RECORD_STATUS_BYTE_SIZE);
							Array.Copy(this._rawData, currentIndex + RECORD_STATUS_BYTE_SIZE, this.seizureBlockRaw, INDEX_ZERO, SEIZURE_BLOCK_BYTE_SIZE);


							stepParams.sbYear = Convert.ToInt32(Utils.getDecimalValue(this.yrDataRaw));
							stepParams.sbMonth = Convert.ToInt32(Utils.getDecimalValue(this.monthDataRaw));
							stepParams.sbDay = Convert.ToInt32(Utils.getDecimalValue(this.dayDataRaw));
							stepParams.sbHour = Convert.ToInt32(Utils.getDecimalValue(this.hourDataRaw));
							stepParams.sbSeizureBlock = Convert.ToInt32(Utils.getDecimalValue(this.seizureBlockRaw));

							int byteValue = Convert.ToInt32(Utils.getDecimalValue(this.recordStatusDataRaw));
							stepParams.sbMinute = Convert.ToInt32(byteValue & 0x3F);
							stepParams.sbRecordStatus = Convert.ToInt32(byteValue >> 6);




							currentIndex = currentIndex + dataLen;
							seizureBlocksData.Add(stepParams);
						}

						parsingStatus = BLEParsingStatus.SUCCESS;
					}

					else 
					{

						if (rawData[2] > 0)
						{
							int max = rawData[2];
							int dataLen = 4;
							int currentIndex = 3;
							for (int i = 0; i < max; i++)
							{

								SeizureBlocksParametersOld stepParams = new SeizureBlocksParametersOld();


								this.seizureBlockRaw = new byte[SEIZURE_BLOCK_BYTE_SIZE];

								Array.Copy(this._rawData, currentIndex , this.seizureBlockRaw, INDEX_ZERO, SEIZURE_BLOCK_BYTE_SIZE);

								stepParams.sbSeizureBlock = Convert.ToInt32(Utils.getDecimalValue(this.seizureBlockRaw));

								currentIndex = currentIndex + dataLen;
								seizureBlocksDataOld.Add(stepParams);
							}

							parsingStatus = BLEParsingStatus.SUCCESS;

						}

					}

				}

			});

			return parsingStatus;
		}


		public async Task<byte[]> GetReadStepTableDataCommand()
		{
			await Task.Run(() =>
			{
				this._readCommandRawData = new byte[COMMAND_SIZE_READ];
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
			});
			return this._readCommandRawData;
		}

		public async Task<byte[]> GetWriteStepTableDataCommand(SeizureBlocksParameters seizureBlocksParamWrite)
		{
			await Task.Run(() =>
			{
				this._rawData = new byte[COMMAND_SIZE_WRITE + 2];

				int seizureBlockToUpdate = 1;
				if (!seizureBlocksParamWrite.sbIsNewest)
					seizureBlockToUpdate = 2;
					
				this.seizureRecordToUpdateRaw = BitConverter.GetBytes(seizureBlockToUpdate);
				this.seizureBlockRaw = BitConverter.GetBytes(seizureBlocksParamWrite.sbSeizureBlock);

				Buffer.BlockCopy(this.seizureRecordToUpdateRaw, INDEX_ZERO, this._rawData, SEIZURE_REC_TO_READ_LOC, SEIZURE_REC_TO_READ_BYTE_SIZE);
				Buffer.BlockCopy(this.seizureBlockRaw, INDEX_ZERO, this._rawData, SEIZURE_BLOCK_LOC, SEIZURE_BLOCK_BYTE_SIZE);

				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

