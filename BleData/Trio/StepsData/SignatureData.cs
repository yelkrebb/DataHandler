using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.StepsData
{
	public class SignatureData
	{

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_READ = 0x26;


		public int SDYear { get; set; }
		public int SDMonth { get; set; }
		public int SDDay { get; set; }
		public int SDHour { get; set; }
		public int SDMin { get; set; }
		public int Frequency { get; set; }
		public int BlockNumber { get; set; }
		public int DurationTime { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }


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
			
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				/*this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);
				this.IsReadCommand = true;

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

					*/
			});

			return parsingStatus;
		}
	}
}

