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
		const int COMMAND_ID_READ = 0x53;

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

		public Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			throw new NotImplementedException();
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

