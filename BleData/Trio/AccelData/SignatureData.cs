using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.AccelData
{
	public class SignatureData
	{

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_READ = 0x26;
		const int COMMAND_SIZE_READ = 5;

		const int INDEX_ZERO = 0;
		const int SIG_LEN = 3;

		const int YEAR_DATA_LOC = 3;
		const int MONTH_DATA_LOC = 4;
		const int DAY_DATA_LOC = 5;
		const int HOUR_DATA_LOC = 6;
		const int MIN_DATA_LOC = 7;
		const int FREQUENCY_SAMPLING_LOC = 8;
		const int FREQUENCY_SAMPLING_TIME_LOC = 9;
		const int SIGNATURE_PROFILE_DATA_LOC_OLD = 9;
		const int SIGNATURE_PROFILE_DATA_LOC = 10;

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

		byte[] yrDataRaw;
		byte[] monthDataRaw;
		byte[] dayDataRaw;
		byte[] hourDataRaw;
		byte[] minDataRaw;
		byte[] frequencySamplingRaw;
		byte[] frequencySamplingTimeRaw;
		byte[] signatureRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */


		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public SignatureData(TrioDeviceInformation devInfo)
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
			if (this.minDataRaw != null && this.minDataRaw.Length > 0)
				Array.Clear(this.minDataRaw, INDEX_ZERO, this.minDataRaw.Length);
			if (this.frequencySamplingRaw != null && this.frequencySamplingRaw.Length > 0)
				Array.Clear(this.frequencySamplingRaw, INDEX_ZERO, this.frequencySamplingRaw.Length);
			if (this.frequencySamplingTimeRaw != null && this.frequencySamplingTimeRaw.Length > 0)
				Array.Clear(this.frequencySamplingTimeRaw, INDEX_ZERO, this.frequencySamplingTimeRaw.Length);
			if (this.signatureRaw != null && this.signatureRaw.Length > 0)
				Array.Clear(this.signatureRaw, INDEX_ZERO, this.signatureRaw.Length);
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

				this.yrDataRaw = new byte[YEAR_DATA_BYTE_SIZE];
				this.monthDataRaw = new byte[MONTH_DATA_BYTE_SIZE];
				this.dayDataRaw = new byte[DAY_DATA_BYTE_SIZE];
				this.hourDataRaw = new byte[HOUR_DATA_BYTE_SIZE];
				this.minDataRaw = new byte[MIN_DATA_BYTE_SIZE];
				this.frequencySamplingRaw = new byte[FREQUENCY_SAMPLING_BYTE_SIZE];


				Array.Copy(this._rawData, YEAR_DATA_LOC, this.yrDataRaw, INDEX_ZERO, YEAR_DATA_BYTE_SIZE);
				Array.Copy(this._rawData, MONTH_DATA_LOC, this.monthDataRaw, INDEX_ZERO, MONTH_DATA_BYTE_SIZE);
				Array.Copy(this._rawData, DAY_DATA_LOC, this.dayDataRaw, INDEX_ZERO, DAY_DATA_BYTE_SIZE);
				Array.Copy(this._rawData, HOUR_DATA_LOC, this.hourDataRaw, INDEX_ZERO, HOUR_DATA_BYTE_SIZE);
				Array.Copy(this._rawData, MIN_DATA_LOC, this.minDataRaw, INDEX_ZERO, MIN_DATA_BYTE_SIZE);
				Array.Copy(this._rawData, FREQUENCY_SAMPLING_LOC, this.frequencySamplingRaw, INDEX_ZERO, FREQUENCY_SAMPLING_BYTE_SIZE);

				this.SDYear = Convert.ToInt32(Utils.getDecimalValue(this.yrDataRaw) & 0x3F);
				this.SDMonth = Convert.ToInt32(Utils.getDecimalValue(this.monthDataRaw) & 0x3F);
				this.SDDay = Convert.ToInt32(Utils.getDecimalValue(this.dayDataRaw) & 0x3F);
				this.SDHour = Convert.ToInt32(Utils.getDecimalValue(this.hourDataRaw) & 0x3F);
				this.SDMin = Convert.ToInt32(Utils.getDecimalValue(this.minDataRaw));
				this.FrequencySampling = Convert.ToInt32(Utils.getDecimalValue(this.frequencySamplingRaw));

				if (this.trioDevInfo.ModelNumber == 961)
				{

					this.frequencySamplingTimeRaw = new byte[FREQUENCY_SAMPLING_TIME_BYTE_SIZE];
					Array.Copy(this._rawData, FREQUENCY_SAMPLING_TIME_LOC, this.frequencySamplingTimeRaw, INDEX_ZERO, FREQUENCY_SAMPLING_BYTE_SIZE);
					this.FrequencySamplingTime = Convert.ToInt32(Utils.getDecimalValue(this.frequencySamplingTimeRaw));
				}


				int startIndexForSigData = (this.trioDevInfo.ModelNumber == 961) ? SIGNATURE_PROFILE_DATA_LOC : SIGNATURE_PROFILE_DATA_LOC_OLD;
				int dataLen = rawData.Length - startIndexForSigData - CHECKSUM_BYTE_SIZE;
				int sigLen = SIG_LEN;
				this.signatureRaw = new byte[dataLen];
				Array.Copy(this._rawData, startIndexForSigData, this.signatureRaw, INDEX_ZERO, dataLen);

				int signatureCount = this.signatureRaw.Length / sigLen;

				this.X_Axis = new List<byte>();
				this.Y_Axis = new List<byte>();
				this.Z_Axis = new List<byte>();

				for (int i = 0; i < this.signatureRaw.Length;)
				{
					this.X_Axis.Add(this.signatureRaw[i]);
					this.Y_Axis.Add(this.signatureRaw[i + 1]);
					this.Z_Axis.Add(this.signatureRaw[i + 2]);
					i += sigLen;

					if (i + sigLen > this.signatureRaw.Length)
						break;
				}

				parsingStatus = BLEParsingStatus.SUCCESS;
					
			});

			return parsingStatus;
		}

		public async Task<byte[]> GetReadSignDataCommand()
		{
			await Task.Run(() =>
			{
				this._readCommandRawData = new byte[COMMAND_SIZE_READ];

				byte[] yearParam = BitConverter.GetBytes(this.SDYear | 0xC0);
				byte[] monthParam = BitConverter.GetBytes(this.SDMonth | 0xC0);
				byte[] dayParam = BitConverter.GetBytes(this.SDDay | 0xC0);

				Buffer.BlockCopy(yearParam, INDEX_ZERO, this._readCommandRawData,2 , 1);
				Buffer.BlockCopy(monthParam, INDEX_ZERO, this._readCommandRawData, 3, 1);
				Buffer.BlockCopy(dayParam, INDEX_ZERO, this._readCommandRawData, 4, 1);

				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 1, 1);

			});
			return this._readCommandRawData;
		}
	}
}

