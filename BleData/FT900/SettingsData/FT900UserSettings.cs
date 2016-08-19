using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.FT900;

namespace Motion.Core.Data.BleData.FT900.SettingsData
{
	public class FT900UserSettings: FT900DataHandler
	{

		const int COMMAND_SIZE_WRITE_ORIG = 7;
		const int COMMAND_SIZE_WRITE_WITH_DOB = 11;
		const int COMMAND_SIZE_WRITE_WITH_DOB_AND_SCREEN = 12;
		const int COMMAND_SIZE_WRITE_900 = 10;
		const int COMMAND_SIZE_READ = 1;

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x18;
		const int COMMAND_ID_READ = 0x19;

		const int INDEX_ZERO = 0;

		const int STRIDE_BYTE_LOC = 2;
		const int WEIGHT_WHOLE_BYTE_LOC = 3;
		const int WEIGHT_DECIMAL_BYTE_LOC = 5;
		const int RMR_BYTE_LOC = 6;
		const int UNIT_OF_MEASURE_BYTE_LOC = 8;
		const int DOB_YEAR_BYTE_LOC = 9;
		const int DOB_MONTH_BYTE_LOC = 10;
		const int DOB_DAY_BYTE_LOC = 11;
		const int AGE_BYTE_LOC = 12;
		const int SCREEN_BYTE_LOC = 13;

		const int STRIDE_BYTE_SIZE = 1;
		const int WEIGHT_WHOLE_BYTE_SIZE = 2;
		const int WEIGHT_DECIMAL_BYTE_SIZE = 1;
		const int RMR_BYTE_SIZE = 2;
		const int UNIT_OF_MEASURE_BYTE_SIZE = 1;
		const int DOB_YEAR_BYTE_SIZE = 1;
		const int DOB_MONTH_BYTE_SIZE = 1;
		const int DOB_DAY_BYTE_SIZE = 1;
		const int AGE_BYTE_SIZE = 1;
		const int SCREEN_BYTE_SIZE = 1;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int Stride { get; set; }
		public float Weight { get; set; }
		public int RestMetabolicRate { get; set; }
		public bool UnitOfMeasure { get; set; }
		public string DateOfBirth { get; set; } //yy-MM-dd format
		public int Age { get; set; }
		public bool AutoRotateEnable { get; set; }
		public bool VerticalOrientationEnable { get; set; }
		public bool WristPreference { get; set; } // 0-left wrist; 1-right wrist
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }


		/* #### Equavalent RAW data per field #####*/
		private byte[] strideRaw;
		private byte[] weightWholeRaw;
		private byte[] weightDecimalRaw;
		private byte[] rmrRaw;
		private byte[] unitOfMeasureRaw;
		private byte[] dobYearRaw;
		private byte[] dobMonthRaw;
		private byte[] dobDayRaw;
		private byte[] ageRaw;
		private byte[] screenOrientationRaw;
		private byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		private byte[] _rawData { get; set; }
		private byte[] _readCommandRawData { get; set; }
		private byte[] _writeCommandRawData { get; set; }

		FT900DeviceInformation ft900DevInfo;

		public FT900UserSettings(FT900DeviceInformation devInfo)
		{
			this.ft900DevInfo = devInfo;
			this.ClearData();
		}

		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() => { 
			
				this.strideRaw = BitConverter.GetBytes(this.Stride);

				string[] weightValues = this.Weight.ToString(CultureInfo.InvariantCulture).Split('.');
				this.weightWholeRaw = BitConverter.GetBytes(int.Parse(weightValues[0]));
				this.weightDecimalRaw = BitConverter.GetBytes(int.Parse(weightValues[1]));

				this.rmrRaw = BitConverter.GetBytes(this.RestMetabolicRate);
				this.unitOfMeasureRaw = BitConverter.GetBytes(Convert.ToInt32(this.UnitOfMeasure));

				Buffer.BlockCopy(this.strideRaw, 0, this._rawData, STRIDE_BYTE_LOC, STRIDE_BYTE_SIZE);
				Buffer.BlockCopy(this.weightWholeRaw, 0, this._rawData, WEIGHT_WHOLE_BYTE_LOC, WEIGHT_WHOLE_BYTE_SIZE);
				Buffer.BlockCopy(this.weightDecimalRaw, 0, this._rawData, WEIGHT_DECIMAL_BYTE_LOC, WEIGHT_DECIMAL_BYTE_SIZE);
				Buffer.BlockCopy(this.rmrRaw, 0, this._rawData, RMR_BYTE_LOC, RMR_BYTE_SIZE);
				Buffer.BlockCopy(this.unitOfMeasureRaw, 0, this._rawData, UNIT_OF_MEASURE_BYTE_LOC, UNIT_OF_MEASURE_BYTE_SIZE);


				if (this._rawData.Length == COMMAND_SIZE_WRITE_900)
				{
					string[] dateValues = this.DateOfBirth.Split('-');
					this.dobYearRaw = BitConverter.GetBytes(int.Parse(dateValues[0]));
					this.dobMonthRaw = BitConverter.GetBytes(int.Parse(dateValues[1]));
					this.dobDayRaw = BitConverter.GetBytes(int.Parse(dateValues[2]));

					Buffer.BlockCopy(this.dobYearRaw, 0, this._rawData, DOB_YEAR_BYTE_LOC, DOB_YEAR_BYTE_SIZE);
					Buffer.BlockCopy(this.dobMonthRaw, 0, this._rawData, DOB_MONTH_BYTE_LOC, DOB_MONTH_BYTE_SIZE);
					Buffer.BlockCopy(this.dobDayRaw, 0, this._rawData, DOB_DAY_BYTE_LOC, DOB_DAY_BYTE_SIZE);
				}
				//Add the two prefix bytes needed for the commands
				//For 93x and 96x, 1st byte is the command prefix 0x1B while 2nd byte is the command ID
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_SIZE_WRITE_900);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);


			});

			return this._rawData;
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parseStatus = BLEParsingStatus.ERROR;
			await Task.Run(() => { 

				Array.Copy(rawData, this._rawData, rawData.Length);

				this.IsReadCommand = true;
				if (rawData[1] == 0x18)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);
				}

				else
				{ 
					this.strideRaw = new byte[STRIDE_BYTE_SIZE];
					this.weightWholeRaw = new byte[WEIGHT_WHOLE_BYTE_SIZE];
					this.weightDecimalRaw = new byte[WEIGHT_DECIMAL_BYTE_SIZE];
					this.rmrRaw = new byte[RMR_BYTE_SIZE];
					this.unitOfMeasureRaw = new byte[UNIT_OF_MEASURE_BYTE_SIZE];

					Array.Copy(this._rawData, STRIDE_BYTE_LOC, this.strideRaw, INDEX_ZERO, STRIDE_BYTE_SIZE);
					Array.Copy(this._rawData, WEIGHT_WHOLE_BYTE_LOC, this.weightWholeRaw, INDEX_ZERO, WEIGHT_WHOLE_BYTE_SIZE);
					Array.Copy(this._rawData, WEIGHT_DECIMAL_BYTE_LOC, this.weightDecimalRaw, INDEX_ZERO, WEIGHT_DECIMAL_BYTE_SIZE);
					Array.Copy(this._rawData, RMR_BYTE_LOC, this.rmrRaw, INDEX_ZERO, RMR_BYTE_SIZE);
					Array.Copy(this._rawData, UNIT_OF_MEASURE_BYTE_LOC, this.unitOfMeasureRaw, INDEX_ZERO, UNIT_OF_MEASURE_BYTE_SIZE);

					this.Stride = BitConverter.ToInt32(this.strideRaw, INDEX_ZERO);

					string weightValueStr = Convert.ToString(BitConverter.ToUInt16(this.weightWholeRaw, INDEX_ZERO)) + "." +
						Convert.ToString(BitConverter.ToUInt16(this.weightDecimalRaw, INDEX_ZERO));
					this.Weight = float.Parse(weightValueStr);
					this.RestMetabolicRate = BitConverter.ToInt32(this.rmrRaw, INDEX_ZERO);
					this.UnitOfMeasure = Convert.ToBoolean(BitConverter.ToUInt16(this.unitOfMeasureRaw, INDEX_ZERO) & 0x01);


					this.dobYearRaw = new byte[DOB_YEAR_BYTE_SIZE];
					this.dobMonthRaw = new byte[DOB_MONTH_BYTE_SIZE];
					this.dobDayRaw = new byte[DOB_DAY_BYTE_SIZE];

					Array.Copy(this._rawData, DOB_YEAR_BYTE_LOC, this.dobYearRaw, INDEX_ZERO, DOB_YEAR_BYTE_SIZE);
					Array.Copy(this._rawData, DOB_MONTH_BYTE_LOC, this.dobMonthRaw, INDEX_ZERO, DOB_MONTH_BYTE_SIZE);
					Array.Copy(this._rawData, DOB_DAY_BYTE_LOC, this.dobDayRaw, INDEX_ZERO, DOB_DAY_BYTE_SIZE);

					this.DateOfBirth = Convert.ToString(BitConverter.ToUInt16(this.dobYearRaw, INDEX_ZERO)) + "-" +
						Convert.ToString(BitConverter.ToUInt16(this.dobMonthRaw, INDEX_ZERO)) + "-" +
						Convert.ToString(BitConverter.ToUInt16(this.dobDayRaw, INDEX_ZERO));
				}


				parseStatus = BLEParsingStatus.SUCCESS;
			});

			return parseStatus;
		}


		private void ClearData()
		{
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);

			Array.Clear(this.strideRaw, INDEX_ZERO, this.strideRaw.Length);
			Array.Clear(this.weightWholeRaw, INDEX_ZERO, this.weightWholeRaw.Length);
			Array.Clear(this.weightDecimalRaw, INDEX_ZERO, this.weightDecimalRaw.Length);
			Array.Clear(this.rmrRaw, INDEX_ZERO, this.rmrRaw.Length);
			Array.Clear(this.unitOfMeasureRaw, INDEX_ZERO, this.unitOfMeasureRaw.Length);
			Array.Clear(this.dobYearRaw, INDEX_ZERO, this.dobYearRaw.Length);
			Array.Clear(this.dobMonthRaw, INDEX_ZERO, this.dobMonthRaw.Length);
			Array.Clear(this.dobDayRaw, INDEX_ZERO, this.dobDayRaw.Length);
			Array.Clear(this.ageRaw, INDEX_ZERO, this.ageRaw.Length);
			Array.Clear(this.screenOrientationRaw, INDEX_ZERO, this.screenOrientationRaw.Length);

		}

		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_SIZE_READ);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
			});

			return this._readCommandRawData;
		}
	}
}

