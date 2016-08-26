using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.Trio;
using Motion.Mobile.Utilities;
namespace Motion.Core.Data.BleData.Trio.SettingsData
{
	public class MedicineAlarm:ITrioDataHandler
	{
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x52;
		const int COMMAND_SIZE_WRITE = 13;

		const int INDEX_ZERO = 0;

		const int YEAR_START_LOC = 2;
		const int MONTH_START_LOC = 3;
		const int DAY_START_LOC = 4;
		const int HOUR_START_LOC = 5;
		const int PRESCRIPTION_TYPE_LOC = 6;
		const int HOUR_INTERVAL_LOC = 7;
		const int TOTAL_ALARMS_LOC = 8;
		const int MEDICINE_CODE1_LOC = 9;
		const int MEDICINE_CODE2_LOC = 10;
		const int MEDICINE_CODE3_LOC = 11;
		const int MEDICINE_CODE4_LOC = 12;
		const int MEDICINE_CODE5_LOC = 13;
		const int MEDICINE_CODE6_LOC = 14;


		const int MEDICINE_ALARM_BYTE_SIZE = 1;
		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int YearStart { get; set; }
		public int MonthStart { get; set; }
		public int DayStart { get; set; }
		public int HourStart { get; set; }
		public bool PrescriptionTypeRoundTheClock { get; set; }
		public bool PrescriptionTypeFixedBreakfast { get; set; }
		public bool PrescriptionTypeFixedLunch { get; set; }
		public bool PrescriptionTypeFixedDinner { get; set; }
		public int HourInterval { get; set; }
		public int TotalAlarm { get; set; }
		public int MedicineCode1 { get; set; }
		public int MedicineCode2 { get; set; }
		public int MedicineCode3 { get; set; }
		public int MedicineCode4 { get; set; }
		public int MedicineCode5 { get; set; }
		public int MedicineCode6 { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }


		/* #### Equavalent RAW data per field #####*/
		byte[] yearStartRaw;
		byte[] monthStartRaw;
		byte[] dayStartRaw;
		byte[] hourStartRaw;
		byte[] prescriptionTypeRaw;
		byte[] hourIntervalRaw;
		byte[] totalAlarmsRaw;
		byte[] medicineCode1Raw;
		byte[] medicineCode2Raw;
		byte[] medicineCode3Raw;
		byte[] medicineCode4Raw;
		byte[] medicineCode5Raw;
		byte[] medicineCode6Raw;

		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }

		TrioDeviceInformation trioDevInfo;

		public MedicineAlarm(TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			Array.Clear(this.yearStartRaw, INDEX_ZERO, this.yearStartRaw.Length);
			Array.Clear(this.monthStartRaw, INDEX_ZERO, this.monthStartRaw.Length);
			Array.Clear(this.dayStartRaw, INDEX_ZERO, this.dayStartRaw.Length);
			Array.Clear(this.hourStartRaw, INDEX_ZERO, this.hourStartRaw.Length);
			Array.Clear(this.totalAlarmsRaw, INDEX_ZERO, this.totalAlarmsRaw.Length);
			Array.Clear(this.medicineCode1Raw, INDEX_ZERO, this.medicineCode1Raw.Length);
			Array.Clear(this.medicineCode2Raw, INDEX_ZERO, this.medicineCode2Raw.Length);
			Array.Clear(this.medicineCode3Raw, INDEX_ZERO, this.medicineCode3Raw.Length);
			Array.Clear(this.medicineCode4Raw, INDEX_ZERO, this.medicineCode4Raw.Length);
			Array.Clear(this.medicineCode5Raw, INDEX_ZERO, this.medicineCode5Raw.Length);
			Array.Clear(this.medicineCode6Raw, INDEX_ZERO, this.medicineCode6Raw.Length);
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{

			BLEParsingStatus parseStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);
				this.IsReadCommand = true;
				if (rawData[1] == 0x52)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = Convert.ToInt32(Utils.getDecimalValue(this.writeCommandResponseCodeRaw));

					parseStatus = BLEParsingStatus.SUCCESS;
				}

			});

			return parseStatus;
		}

		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{

			});

			throw new NotImplementedException();
		}

		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() =>
			{

				this._rawData = new byte[COMMAND_SIZE_WRITE + 2];


				this.yearStartRaw = BitConverter.GetBytes(this.YearStart);
				this.monthStartRaw = BitConverter.GetBytes(this.MonthStart);
				this.dayStartRaw = BitConverter.GetBytes(this.DayStart);
				this.hourStartRaw = BitConverter.GetBytes(this.HourStart);


				int flagValue = 0x00;
				flagValue |= (Convert.ToInt32(this.PrescriptionTypeRoundTheClock)) << 6;
				flagValue |= (Convert.ToInt32(this.PrescriptionTypeFixedBreakfast)) << 5;
				flagValue |= (Convert.ToInt32(this.PrescriptionTypeFixedLunch)) << 4;
				flagValue |= (Convert.ToInt32(this.PrescriptionTypeFixedDinner)) << 3;


				this.prescriptionTypeRaw = BitConverter.GetBytes(flagValue);
				this.hourIntervalRaw = BitConverter.GetBytes(this.HourInterval);
				this.totalAlarmsRaw = BitConverter.GetBytes(this.TotalAlarm);
				this.medicineCode1Raw = BitConverter.GetBytes(this.MedicineCode1);
				this.medicineCode2Raw = BitConverter.GetBytes(this.MedicineCode2);
				this.medicineCode3Raw = BitConverter.GetBytes(this.MedicineCode3);
				this.medicineCode4Raw = BitConverter.GetBytes(this.MedicineCode4);
				this.medicineCode5Raw = BitConverter.GetBytes(this.MedicineCode5);
				this.medicineCode6Raw = BitConverter.GetBytes(this.MedicineCode6);

				Buffer.BlockCopy(this.yearStartRaw, INDEX_ZERO, this._rawData, YEAR_START_LOC, 1);
				Buffer.BlockCopy(this.monthStartRaw, INDEX_ZERO, this._rawData, MONTH_START_LOC, 1);
				Buffer.BlockCopy(this.dayStartRaw, INDEX_ZERO, this._rawData, DAY_START_LOC, 1);
				Buffer.BlockCopy(this.hourStartRaw, INDEX_ZERO, this._rawData, HOUR_START_LOC, 1);
				Buffer.BlockCopy(this.prescriptionTypeRaw, INDEX_ZERO, this._rawData, PRESCRIPTION_TYPE_LOC, 1);
				Buffer.BlockCopy(this.hourIntervalRaw, INDEX_ZERO, this._rawData, HOUR_INTERVAL_LOC, 1);
				Buffer.BlockCopy(this.totalAlarmsRaw, INDEX_ZERO, this._rawData, TOTAL_ALARMS_LOC, 1);
				Buffer.BlockCopy(this.medicineCode1Raw, INDEX_ZERO, this._rawData, MEDICINE_CODE1_LOC, 1);
				Buffer.BlockCopy(this.medicineCode2Raw, INDEX_ZERO, this._rawData, MEDICINE_CODE2_LOC, 1);
				Buffer.BlockCopy(this.medicineCode3Raw, INDEX_ZERO, this._rawData, MEDICINE_CODE3_LOC, 1);
				Buffer.BlockCopy(this.medicineCode4Raw, INDEX_ZERO, this._rawData, MEDICINE_CODE4_LOC, 1);
				Buffer.BlockCopy(this.medicineCode5Raw, INDEX_ZERO, this._rawData, MEDICINE_CODE5_LOC, 1);
				Buffer.BlockCopy(this.medicineCode6Raw, INDEX_ZERO, this._rawData, MEDICINE_CODE6_LOC, 1);


				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

