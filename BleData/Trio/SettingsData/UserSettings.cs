using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData;

using Newtonsoft.Json;

namespace Motion.Core.Data.BleData.Trio.SettingsData
{
	public class UserSettings: ITrioDataHandler
	{
		const int COMMAND_SIZE_WRITE_ORIG = 7;
		const int COMMAND_SIZE_WRITE_WITH_DOB = 11;
		const int COMMAND_SIZE_WRITE_WITH_DOB_AND_SCREEN = 12;
		const int COMMAND_SIZE_READ = 2;

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

		[JsonProperty(PropertyName = "strd")]
		public int Stride { get; set; }
		[JsonProperty(PropertyName = "wt")]
		public float Weight { get; set; }
		[JsonProperty(PropertyName = "rmr")]
		public int RestMetabolicRate { get; set; }
		[JsonProperty(PropertyName = "uom")]
		public bool UnitOfMeasure { get; set; }
		[JsonProperty(PropertyName = "byear")]
		public int DOBYear { get; set; }
		[JsonProperty(PropertyName = "bmon")]
		public int DOBMonth { get; set; }
		[JsonProperty(PropertyName = "bday")]
		public int DOBDay { get; set; }
		[JsonProperty(PropertyName = "age")]
		public int Age { get; set; }
		[JsonIgnore]
		public bool AutoRotateEnable { get; set; }
		[JsonIgnore]
		public bool VerticalOrientationEnable { get; set; }
		[JsonIgnore]
		public bool WristPreference { get; set; } // 0-left wrist; 1-right wrist
		[JsonIgnore]
		public int WriteCommandResponseCode { get; set; }
		[JsonIgnore]
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

		private byte[] _rawData;
		private byte[] _readCommandRawData;
		private byte[] _writeCommandRawData;

		TrioDeviceInformation trioDevInfo;

		public UserSettings (TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData ();
		}

		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() =>
			{

				if (this.trioDevInfo.ModelNumber == 961)
					this._rawData = new byte[COMMAND_SIZE_WRITE_WITH_DOB_AND_SCREEN + 2];
				else
					this._rawData = new byte[COMMAND_SIZE_WRITE_ORIG + 2];

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

				if (this.trioDevInfo.ModelNumber == 961)
				{
					this.dobYearRaw = BitConverter.GetBytes(this.DOBYear);
					this.dobMonthRaw = BitConverter.GetBytes(this.DOBMonth);
					this.dobDayRaw = BitConverter.GetBytes(this.DOBDay);

					this.ageRaw = BitConverter.GetBytes(this.Age);

					int screenOrientationValues = 0x00;
					screenOrientationValues |= this.AutoRotateEnable ? 0x01 : 0x00;
					screenOrientationValues |= this.VerticalOrientationEnable ? 0x02 : 0x00;
					screenOrientationValues |= this.WristPreference ? 0x04 : 0x00;

					this.screenOrientationRaw = BitConverter.GetBytes(screenOrientationValues);

					Buffer.BlockCopy(this.dobYearRaw, 0, this._rawData, DOB_YEAR_BYTE_LOC, DOB_YEAR_BYTE_SIZE);
					Buffer.BlockCopy(this.dobMonthRaw, 0, this._rawData, DOB_MONTH_BYTE_LOC, DOB_MONTH_BYTE_SIZE);
					Buffer.BlockCopy(this.dobDayRaw, 0, this._rawData, DOB_DAY_BYTE_LOC, DOB_DAY_BYTE_SIZE);
					Buffer.BlockCopy(this.ageRaw, 0, this._rawData, AGE_BYTE_LOC, AGE_BYTE_SIZE);
					Buffer.BlockCopy(this.screenOrientationRaw, 0, this._rawData, SCREEN_BYTE_LOC, SCREEN_BYTE_SIZE);
				}
				/*else if (this._rawData.Length == COMMAND_SIZE_WRITE_WITH_DOB)
				{
					string[] dateValues = this.DateOfBirth.Split('-');
					this.dobYearRaw = BitConverter.GetBytes(int.Parse(dateValues[0]));
					this.dobMonthRaw = BitConverter.GetBytes(int.Parse(dateValues[1]));
					this.dobDayRaw = BitConverter.GetBytes(int.Parse(dateValues[2]));

					this.ageRaw = BitConverter.GetBytes(this.Age);

					Buffer.BlockCopy(this.dobYearRaw, 0, this._rawData, DOB_YEAR_BYTE_LOC, DOB_YEAR_BYTE_SIZE);
					Buffer.BlockCopy(this.dobMonthRaw, 0, this._rawData, DOB_MONTH_BYTE_LOC, DOB_MONTH_BYTE_SIZE);
					Buffer.BlockCopy(this.dobDayRaw, 0, this._rawData, DOB_DAY_BYTE_LOC, DOB_DAY_BYTE_SIZE);
					Buffer.BlockCopy(this.ageRaw, 0, this._rawData, AGE_BYTE_LOC, AGE_BYTE_SIZE);
				}*/

				//Add the two prefix bytes needed for the commands
				//For 93x and 96x, 1st byte is the command prefix 0x1B while 2nd byte is the command ID
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
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
			
				this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);
				this.IsReadCommand = true;
				if (rawData[1] == 0x18)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = Convert.ToInt32(Utils.getDecimalValue(this.writeCommandResponseCodeRaw));
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

					this.Stride = Convert.ToInt32(Utils.getDecimalValue(this.strideRaw));

					string weightValueStr = Convert.ToString( Convert.ToInt32(Utils.getDecimalValue(this.weightWholeRaw)))  + "." +
						Convert.ToString(Convert.ToInt32(Utils.getDecimalValue(this.weightDecimalRaw)));
					this.Weight = float.Parse(weightValueStr);
					this.RestMetabolicRate = Convert.ToInt32(Utils.getDecimalValue(this.rmrRaw)); 
					this.UnitOfMeasure = Convert.ToBoolean(Convert.ToInt32(Utils.getDecimalValue(this.unitOfMeasureRaw)) & 0x01);

					if (this._rawData.Length == COMMAND_SIZE_WRITE_WITH_DOB_AND_SCREEN)
					{
						this.dobYearRaw = new byte[DOB_YEAR_BYTE_SIZE];
						this.dobMonthRaw = new byte[DOB_MONTH_BYTE_SIZE];
						this.dobDayRaw = new byte[DOB_DAY_BYTE_SIZE];
						this.ageRaw = new byte[AGE_BYTE_SIZE];
						this.screenOrientationRaw = new byte[SCREEN_BYTE_SIZE];

						Array.Copy(this._rawData, DOB_YEAR_BYTE_LOC, this.dobYearRaw, INDEX_ZERO, DOB_YEAR_BYTE_SIZE);
						Array.Copy(this._rawData, DOB_MONTH_BYTE_LOC, this.dobMonthRaw, INDEX_ZERO, DOB_MONTH_BYTE_SIZE);
						Array.Copy(this._rawData, DOB_DAY_BYTE_LOC, this.dobDayRaw, INDEX_ZERO, DOB_DAY_BYTE_SIZE);
						Array.Copy(this._rawData, AGE_BYTE_LOC, this.ageRaw, INDEX_ZERO, AGE_BYTE_SIZE);
						Array.Copy(this._rawData, SCREEN_BYTE_LOC, this.screenOrientationRaw, INDEX_ZERO, SCREEN_BYTE_SIZE);

						this.DOBYear = (int)Utils.getDecimalValue(this.dobYearRaw);
						this.DOBMonth = (int)Utils.getDecimalValue(this.dobMonthRaw);
						this.DOBDay = (int)Utils.getDecimalValue(this.dobDayRaw);

						this.Age = Convert.ToInt32(Utils.getDecimalValue(this.ageRaw));
						this.AutoRotateEnable = Convert.ToBoolean(Convert.ToInt32(Utils.getDecimalValue(this.screenOrientationRaw)) & 0x01);
						this.VerticalOrientationEnable = Convert.ToBoolean((Convert.ToInt32(Utils.getDecimalValue(this.screenOrientationRaw)) >> 1) & 0x01);
						this.WristPreference = Convert.ToBoolean((Convert.ToInt32(Utils.getDecimalValue(this.screenOrientationRaw)) >> 2) & 0x01);
					}
					else if (this._rawData.Length == COMMAND_SIZE_WRITE_WITH_DOB)
					{
						this.dobYearRaw = new byte[DOB_YEAR_BYTE_SIZE];
						this.dobMonthRaw = new byte[DOB_MONTH_BYTE_SIZE];
						this.dobDayRaw = new byte[DOB_DAY_BYTE_SIZE];
						this.ageRaw = new byte[AGE_BYTE_SIZE];

						Array.Copy(this._rawData, DOB_YEAR_BYTE_LOC, this.dobYearRaw, INDEX_ZERO, DOB_YEAR_BYTE_SIZE);
						Array.Copy(this._rawData, DOB_MONTH_BYTE_LOC, this.dobMonthRaw, INDEX_ZERO, DOB_MONTH_BYTE_SIZE);
						Array.Copy(this._rawData, DOB_DAY_BYTE_LOC, this.dobDayRaw, INDEX_ZERO, DOB_DAY_BYTE_SIZE);
						Array.Copy(this._rawData, AGE_BYTE_LOC, this.ageRaw, INDEX_ZERO, AGE_BYTE_SIZE);

						this.DOBYear = (int)Utils.getDecimalValue(this.dobYearRaw);
						this.DOBMonth = (int)Utils.getDecimalValue(this.dobMonthRaw);
						this.DOBDay = (int)Utils.getDecimalValue(this.dobDayRaw);

						this.Age = Convert.ToInt32(Utils.getDecimalValue(this.ageRaw));
					}
				}
				parseStatus = BLEParsingStatus.SUCCESS;

			});

			return parseStatus;
		}

		private void ClearData()
		{
			if (this._rawData != null && this._rawData.Length > 0)
				Array.Clear (this._rawData,INDEX_ZERO, this._rawData.Length);
			if (this._readCommandRawData != null && this._readCommandRawData.Length > 0)
				Array.Clear (this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			if (this.strideRaw != null && this.strideRaw.Length > 0)
				Array.Clear (this.strideRaw, INDEX_ZERO, this.strideRaw.Length);
			if (this.weightWholeRaw != null && this.weightWholeRaw.Length > 0)
				Array.Clear (this.weightWholeRaw, INDEX_ZERO, this.weightWholeRaw.Length);
			if (this.weightDecimalRaw != null && this.weightDecimalRaw.Length > 0)
				Array.Clear (this.weightDecimalRaw, INDEX_ZERO, this.weightDecimalRaw.Length);
			if (this.rmrRaw != null && this.rmrRaw.Length > 0)
				Array.Clear (this.rmrRaw, INDEX_ZERO, this.rmrRaw.Length);
			if (this.unitOfMeasureRaw != null && this.unitOfMeasureRaw.Length > 0)
				Array.Clear (this.unitOfMeasureRaw, INDEX_ZERO, this.unitOfMeasureRaw.Length);
			if (this.dobYearRaw != null && this.dobYearRaw.Length > 0)
				Array.Clear (this.dobYearRaw, INDEX_ZERO, this.dobYearRaw.Length);
			if (this.dobMonthRaw != null && this.dobMonthRaw.Length > 0)
				Array.Clear (this.dobMonthRaw, INDEX_ZERO, this.dobMonthRaw.Length);
			if (this.dobDayRaw != null && this.dobDayRaw.Length > 0)
				Array.Clear (this.dobDayRaw, INDEX_ZERO, this.dobDayRaw.Length);
			if (this.ageRaw != null && this.ageRaw.Length > 0)
				Array.Clear (this.ageRaw, INDEX_ZERO, this.ageRaw.Length);
			if (this.screenOrientationRaw != null && this.screenOrientationRaw.Length > 0)
				Array.Clear (this.screenOrientationRaw, INDEX_ZERO, this.screenOrientationRaw.Length);

		}


		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() => { 
				this._readCommandRawData = new byte[COMMAND_SIZE_READ];
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
			});

			return this._readCommandRawData;
		}

	}
}

