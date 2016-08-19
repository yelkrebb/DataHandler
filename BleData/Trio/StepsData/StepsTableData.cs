using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Motion.Core.Data.BleData.Trio;
namespace Motion.Core.Data.BleData.Trio.StepsData
{
	public class StepsTableData : ITrioDataHandler
	{
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_READ = 0x22;
		const int INDEX_ZERO = 0;


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

		public List<StepsTableParameters> stepsDataTable;
		public bool IsReadCommand { get; set; }


		/* #### Equavalent RAW data per field #####*/
		byte[] yrDataRaw;
		byte[] monthDataRaw;
		byte[] dayDataRaw;
		byte[] hrNumberRaw;
		byte[] sentHourFlagRaw;
		byte[] profileGeneratedRaw;
		/* ### End Raw data per field ### */


		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public StepsTableData(TrioDeviceInformation  devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);

		}

		private void ClearStepsData()
		{ 
			
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				Array.Copy(rawData, this._rawData, rawData.Length);
				stepsDataTable = new List<StepsTableParameters>();
				this.IsReadCommand = true;
				if (rawData[1] == 0x22)
				{

					int len = this._rawData.Length / 8;

					for (int i = 0; i < len; i++)
					{
						
						StepsTableParameters stepParams = new StepsTableParameters();

						this.yrDataRaw = new byte[YEAR_DATA_BYTE_SIZE]; 
						this.monthDataRaw = new byte[MONTH_DATA_BYTE_SIZE];
						this.dayDataRaw = new byte[DAY_DATA_BYTE_SIZE];
						this.hrNumberRaw = new byte[HOUR_NUM_BYTE_SIZE];
						this.sentHourFlagRaw = new byte[SENT_HOUR_BYTE_SIZE];
						this.profileGeneratedRaw = new byte[PROFILE_GENERATED_BYTE_SIZE];

						Array.Copy(this._rawData, YEAR_DATA_BYTE_LOC, this.yrDataRaw, INDEX_ZERO, YEAR_DATA_BYTE_SIZE);
						Array.Copy(this._rawData, MONTH_DATA_BYTE_LOC, this.yrDataRaw, INDEX_ZERO, MONTH_DATA_BYTE_SIZE);
						Array.Copy(this._rawData, DAY_DATA_BYTE_LOC, this.yrDataRaw, INDEX_ZERO, DAY_DATA_BYTE_SIZE);
						Array.Copy(this._rawData, HOUR_NUM_BYTE_LOC, this.yrDataRaw, INDEX_ZERO, HOUR_NUM_BYTE_SIZE);
						Array.Copy(this._rawData, SENT_HOUR_BYTE_LOC, this.yrDataRaw, INDEX_ZERO, SENT_HOUR_BYTE_SIZE);
						Array.Copy(this._rawData, PROFILE_GENERATED_BYTE_LOC, this.yrDataRaw, INDEX_ZERO, PROFILE_GENERATED_BYTE_SIZE);

						stepParams.dbYear = BitConverter.ToInt32(this.yrDataRaw, INDEX_ZERO);
						stepParams.dbMonth = BitConverter.ToInt32(this.monthDataRaw, INDEX_ZERO);
						stepParams.dbDay = BitConverter.ToInt32(this.dayDataRaw, INDEX_ZERO);
						stepParams.dbHourNumber = BitConverter.ToInt32(this.hrNumberRaw, INDEX_ZERO);
						stepParams.sentHourFlag = BitConverter.ToInt32(this.sentHourFlagRaw, INDEX_ZERO);
						stepParams.profileGenerated = BitConverter.ToInt32(this.profileGeneratedRaw, INDEX_ZERO);

						//stepsDataTable

					}

					parsingStatus = BLEParsingStatus.SUCCESS;
				}



			});

			return parsingStatus;
		}

		public Task<byte[]> GetReadCommand()
		{
			throw new NotImplementedException();
		}

		public Task<byte[]> GetWriteCommand()
		{
			throw new NotImplementedException();
		}
	}
}

