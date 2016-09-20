using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.AccelData
{
	public class SeizureData
	{
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_READ = 0x33;
		const int COMMAND_SIZE_READ = 4;

		const int INDEX_ZERO = 0;
		const int SEIZ_LEN = 3;

		const int BLOCK_NUMBER_LOC = 3;
		const int YEAR_DATA_LOC = 4;
		const int MONTH_DATA_LOC = 5;
		const int DAY_DATA_LOC = 6;
		const int HOUR_DATA_LOC = 7;
		const int MIN_DATA_LOC = 8;
		const int FREQUENCY_SAMPLING_LOC = 9;
		const int FREQUENCY_SAMPLING_TIME_LOC = 10;
		const int SEIZURE_PROFILE_DATA_LOC_OLD = 10;
		const int SEIZURE_PROFILE_DATA_LOC = 11;

		const int BLOCK_NUM_BYTE_SIZE = 1;
		const int YEAR_DATA_BYTE_SIZE = 1;
		const int MONTH_DATA_BYTE_SIZE = 1;
		const int DAY_DATA_BYTE_SIZE = 1;
		const int HOUR_DATA_BYTE_SIZE = 1;
		const int MIN_DATA_BYTE_SIZE = 1;
		const int FREQUENCY_SAMPLING_BYTE_SIZE = 1;
		const int FREQUENCY_SAMPLING_TIME_BYTE_SIZE = 1;
		const int CHECKSUM_BYTE_SIZE = 4;


		public List<byte> X_Axis;
		public List<byte> Y_Axis;
		public List<byte> Z_Axis;

		public int BlockNumber { get; set; }
		public int SeizureRecToRead { get; set; }
		public int SDYear { get; set; }
		public int SDMonth { get; set; }
		public int SDDay { get; set; }
		public int SDHour { get; set; }
		public int SDMin { get; set; }
		public int FrequencySampling { get; set; }
		public int FrequencySamplingTime { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }


		/* #### Equavalent RAW data per field #####*/

		byte[] blockNumRaw;
		byte[] yrDataRaw;
		byte[] monthDataRaw;
		byte[] dayDataRaw;
		byte[] hourDataRaw;
		byte[] minDataRaw;
		byte[] frequencySamplingRaw;
		byte[] frequencySamplingTimeRaw;
		byte[] seizureRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */


		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public SeizureData(TrioDeviceInformation devInfo)
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
			if (this.blockNumRaw != null && this.blockNumRaw.Length > 0)
				Array.Clear(this.blockNumRaw, INDEX_ZERO, this.blockNumRaw.Length);
			if (this.yrDataRaw != null && this.yrDataRaw.Length > 0)
				Array.Clear(this.yrDataRaw, INDEX_ZERO, this.yrDataRaw.Length);
			if (this.monthDataRaw != null && this.monthDataRaw.Length > 0)
				Array.Clear(this.monthDataRaw, INDEX_ZERO, this.monthDataRaw.Length);
			if (this.dayDataRaw != null && this.dayDataRaw.Length > 0)
				Array.Clear(this.dayDataRaw, INDEX_ZERO, this.dayDataRaw.Length);
			if (this.hourDataRaw != null && this.hourDataRaw.Length > 0)
				Array.Clear(this.hourDataRaw, INDEX_ZERO, this.hourDataRaw.Length);
			if (this.minDataRaw != null && this.minDataRaw.Length > 0)
				Array.Clear(this.minDataRaw, INDEX_ZERO, this.minDataRaw.Length);
			if (this.frequencySamplingRaw != null && this.frequencySamplingRaw.Length > 0)
				Array.Clear(this.frequencySamplingRaw, INDEX_ZERO, this.frequencySamplingRaw.Length);
			if (this.frequencySamplingTimeRaw != null && this.frequencySamplingTimeRaw.Length > 0)
				Array.Clear(this.frequencySamplingTimeRaw, INDEX_ZERO, this.frequencySamplingTimeRaw.Length);
			if (this.seizureRaw != null && this.seizureRaw.Length > 0)
				Array.Clear(this.seizureRaw, INDEX_ZERO, this.seizureRaw.Length);
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

				this.blockNumRaw = new byte[BLOCK_NUM_BYTE_SIZE];
				this.yrDataRaw = new byte[YEAR_DATA_BYTE_SIZE];
				this.monthDataRaw = new byte[MONTH_DATA_BYTE_SIZE];
				this.dayDataRaw = new byte[DAY_DATA_BYTE_SIZE];
				this.hourDataRaw = new byte[HOUR_DATA_BYTE_SIZE];
				this.minDataRaw = new byte[MIN_DATA_BYTE_SIZE];
				this.frequencySamplingRaw = new byte[FREQUENCY_SAMPLING_BYTE_SIZE];

				Array.Copy(this._rawData, BLOCK_NUMBER_LOC, this.blockNumRaw, INDEX_ZERO, BLOCK_NUM_BYTE_SIZE);
				Array.Copy(this._rawData, YEAR_DATA_LOC, this.yrDataRaw, INDEX_ZERO, YEAR_DATA_BYTE_SIZE);
				Array.Copy(this._rawData, MONTH_DATA_LOC, this.monthDataRaw, INDEX_ZERO, MONTH_DATA_BYTE_SIZE);
				Array.Copy(this._rawData, DAY_DATA_LOC, this.dayDataRaw, INDEX_ZERO, DAY_DATA_BYTE_SIZE);
				Array.Copy(this._rawData, HOUR_DATA_LOC, this.hourDataRaw, INDEX_ZERO, HOUR_DATA_BYTE_SIZE);
				Array.Copy(this._rawData, MIN_DATA_LOC, this.minDataRaw, INDEX_ZERO, MIN_DATA_BYTE_SIZE);
				Array.Copy(this._rawData, FREQUENCY_SAMPLING_LOC, this.frequencySamplingRaw, INDEX_ZERO, FREQUENCY_SAMPLING_BYTE_SIZE);

				this.BlockNumber = Convert.ToInt32(Utils.getDecimalValue(this.blockNumRaw));
				this.SDYear = Convert.ToInt32(Utils.getDecimalValue(this.yrDataRaw));
				this.SDMonth = Convert.ToInt32(Utils.getDecimalValue(this.monthDataRaw));
				this.SDDay = Convert.ToInt32(Utils.getDecimalValue(this.dayDataRaw));
				this.SDHour = Convert.ToInt32(Utils.getDecimalValue(this.hourDataRaw));
				this.SDMin = Convert.ToInt32(Utils.getDecimalValue(this.minDataRaw));
				this.FrequencySampling = Convert.ToInt32(Utils.getDecimalValue(this.frequencySamplingRaw));

				/*if (this.trioDevInfo.ModelNumber == 961)
				{

					this.frequencySamplingTimeRaw = new byte[FREQUENCY_SAMPLING_TIME_BYTE_SIZE];
					Array.Copy(this._rawData, FREQUENCY_SAMPLING_TIME_LOC, this.frequencySamplingTimeRaw, INDEX_ZERO, FREQUENCY_SAMPLING_BYTE_SIZE);
					this.FrequencySamplingTime = Convert.ToInt32(Utils.getDecimalValue(this.frequencySamplingTimeRaw));
				}*/


				int startIndexForSeizData = 9;

				if (this.trioDevInfo.ModelNumber == 961)
					startIndexForSeizData = SEIZURE_PROFILE_DATA_LOC;
				else if (this.trioDevInfo.ModelNumber == 939)
					startIndexForSeizData = SEIZURE_PROFILE_DATA_LOC_OLD;
				
				int dataLen = rawData.Length - startIndexForSeizData - CHECKSUM_BYTE_SIZE;
				int seizLen = SEIZ_LEN;

				Array.Copy(this._rawData, startIndexForSeizData, this.seizureRaw, INDEX_ZERO, dataLen);

				int signatureCount = this.seizureRaw.Length / seizLen;

				this.X_Axis = new List<byte>();
				this.Y_Axis = new List<byte>();
				this.Z_Axis = new List<byte>();

				for (int i = 0; i < this.seizureRaw.Length;)
				{
					this.X_Axis.Add(this.seizureRaw[i]);
					this.Y_Axis.Add(this.seizureRaw[i + 1]);
					this.Z_Axis.Add(this.seizureRaw[i + 2]);
					i += seizLen;

					if (i + seizLen > this.seizureRaw.Length)
						break;
				}

				parsingStatus = BLEParsingStatus.SUCCESS;

			});

			return parsingStatus;
		}

		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{
				this._readCommandRawData = new byte[COMMAND_SIZE_READ];

				byte[] blockNumberParam = BitConverter.GetBytes(this.BlockNumber);
				byte[] seizureRecToReadParam = BitConverter.GetBytes(this.SeizureRecToRead);


				Buffer.BlockCopy(blockNumberParam, INDEX_ZERO, this._readCommandRawData, 2, 1);
				Buffer.BlockCopy(seizureRecToReadParam, INDEX_ZERO, this._readCommandRawData, 3, 1);


				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
			});
			return this._readCommandRawData;
		}
	}
}

