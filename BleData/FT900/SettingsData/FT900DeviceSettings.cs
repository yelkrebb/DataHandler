using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.FT900;

namespace Motion.Core.Data.BleData.FT900.SettingsData
{
	public class FT900DeviceSettings: FT900DataHandler
	{
		const int COMMAND_SIZE_WRITE_WITH_OFFSET = 9;
		const int COMMAND_SIZE_WRITE_ORIG = 7;
		const int COMMAND_SIZE_WRITE_WITH_DST = 13;
		const int COMMAND_SIZE_READ = 1;

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x16;
		const int COMMAND_ID_READ = 0x17;

		const int INDEX_ZERO = 0;

		const int HOUR_BYTE_LOC = 2;
		const int MINUTE_BYTE_LOC = 3;
		const int SECOND_BYTE_LOC = 4;
		const int HOUR_TYPE_BYTE_LOC = 5;
		const int YEAR_BYTE_LOC = 6;
		const int MONTH_BYTE_LOC = 7;
		const int DAY_BYTE_LOC = 8;
		const int HOUR_OFFSET_BYTE_LOC = 9;
		const int MINUTE_OFFSET_BYTE_LOC = 10;
		const int DST_START_BYTE_LOC = 11;
		const int DST_END_BYTE_LOC = 13;

		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		//Predefine flag values for booleans bytes
		const int OFFSET_TYPE = 128;
		const int DST_APPLICABLE = 64;

		public int Hour { get; set; }
		public int Minute { get; set; }
		public int Second { get; set; }
		public bool HourType { get; set; }
		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }
		public bool OffestType { get; set; }
		public bool DstApplicable { get; set; }
		public int HourOffset { get; set; }
		public int MinuteOffset { get; set; }
		public int DstStartMonth { get; set; }
		public int DstStartDay { get; set; }
		public int DstStartHour { get; set; }
		public int DstEndMonth { get; set; }
		public int DstEndDay { get; set; }
		public int DstEndHour { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }


		/* #### Equavalent RAW data per field #####*/
		byte[] hourRaw;
		byte[] minuteRaw;
		byte[] secondRaw;
		byte[] hourTypeRaw;
		byte[] yearRaw;
		byte[] monthRaw;
		byte[] dayRaw;
		byte[] hourOffsetRaw;
		byte[] minuteOffsetRaw;
		byte[] dstStartRaw;
		byte[] dstEndRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		FT900DeviceInformation ft900DevInfo;

		public FT900DeviceSettings(FT900DeviceInformation devInfo)
		{
			this.ft900DevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);

			Array.Clear(this.hourRaw, INDEX_ZERO, this.hourRaw.Length);
			Array.Clear(this.minuteRaw, INDEX_ZERO, this.minuteRaw.Length);
			Array.Clear(this.secondRaw, INDEX_ZERO, this.secondRaw.Length);
			Array.Clear(this.hourTypeRaw, INDEX_ZERO, this.hourTypeRaw.Length);
			Array.Clear(this.yearRaw, INDEX_ZERO, this.yearRaw.Length);
			Array.Clear(this.monthRaw, INDEX_ZERO, this.monthRaw.Length);
			Array.Clear(this.dayRaw, INDEX_ZERO, this.dayRaw.Length);
			Array.Clear(this.hourOffsetRaw, INDEX_ZERO, this.hourOffsetRaw.Length);
			Array.Clear(this.minuteOffsetRaw, INDEX_ZERO, this.minuteOffsetRaw.Length);
			Array.Clear(this.dstStartRaw, INDEX_ZERO, this.dstStartRaw.Length);
			Array.Clear(this.dstEndRaw, INDEX_ZERO, this.dstEndRaw.Length);
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parseStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				Array.Copy(rawData, this._rawData, rawData.Length);

				this.IsReadCommand = true;
				if (rawData[1] == 0x16)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);
				}

				else
				{ 
					this.hourRaw = new byte[1];
					this.minuteRaw = new byte[1];
					this.secondRaw = new byte[1];
					this.hourTypeRaw = new byte[1];
					this.yearRaw = new byte[1];
					this.monthRaw = new byte[1];
					this.dayRaw = new byte[1];
					this.hourOffsetRaw = new byte[1];
					this.minuteOffsetRaw = new byte[1];
					this.dstStartRaw = new byte[1];
					this.dstEndRaw = new byte[1];

					Array.Copy(this._rawData, HOUR_BYTE_LOC, this.hourRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, MINUTE_BYTE_LOC, this.minuteRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, SECOND_BYTE_LOC, this.secondRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, HOUR_TYPE_BYTE_LOC, this.hourTypeRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, YEAR_BYTE_LOC, this.yearRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, MONTH_BYTE_LOC, this.monthRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, DAY_BYTE_LOC, this.dayRaw, INDEX_ZERO, 1);

					this.Hour = BitConverter.ToInt32(this.hourRaw, INDEX_ZERO);
					this.Minute = BitConverter.ToInt32(this.minuteRaw, INDEX_ZERO);
					this.Second = BitConverter.ToInt32(this.secondRaw, INDEX_ZERO);
					this.HourType = BitConverter.ToBoolean(this.hourTypeRaw, INDEX_ZERO);
					this.Year = BitConverter.ToInt32(this.yearRaw, INDEX_ZERO);
					this.Month = BitConverter.ToInt32(this.monthRaw, INDEX_ZERO);
					this.Day = BitConverter.ToInt32(this.dayRaw, INDEX_ZERO);


					Array.Copy(this._rawData, HOUR_OFFSET_BYTE_LOC, this.hourOffsetRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, MINUTE_OFFSET_BYTE_LOC, this.minuteOffsetRaw, INDEX_ZERO, 1);

					int flagValue = BitConverter.ToInt32(this.hourOffsetRaw, INDEX_ZERO);
					this.OffestType = Convert.ToBoolean((flagValue >> 7) & 0x01);
					this.HourOffset = flagValue & 0x3F;
					this.MinuteOffset = BitConverter.ToInt32(this.minuteOffsetRaw, INDEX_ZERO);
				}


				parseStatus = BLEParsingStatus.SUCCESS;

			});

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

		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() =>
			{
				this.hourRaw = BitConverter.GetBytes(this.Hour);
				this.minuteRaw = BitConverter.GetBytes(this.Minute);
				this.secondRaw = BitConverter.GetBytes(this.Second);
				this.hourTypeRaw = BitConverter.GetBytes(this.HourType);
				this.yearRaw = BitConverter.GetBytes(this.Year);
				this.monthRaw = BitConverter.GetBytes(this.Month);
				this.dayRaw = BitConverter.GetBytes(this.Day);

				int flagValue = 0x00;
				flagValue |= this.OffestType ? OFFSET_TYPE : 0x00;
				flagValue |= this.DstApplicable ? DST_APPLICABLE : 0x00;
				flagValue |= this.HourOffset;

				this.hourOffsetRaw = BitConverter.GetBytes(flagValue);
				this.minuteOffsetRaw = BitConverter.GetBytes(this.MinuteOffset);

				int startDstByteValue = 0x00;
				startDstByteValue |= (this.DstStartMonth << 10);
				startDstByteValue |= (this.DstStartDay << 5);
				startDstByteValue |= (this.DstStartHour);

				this.dstStartRaw = BitConverter.GetBytes(startDstByteValue);

				int endDstByteValue = 0x00;
				endDstByteValue |= (this.DstEndMonth << 10);
				endDstByteValue |= (this.DstEndDay << 5);
				endDstByteValue |= (this.DstEndHour);

				this.dstEndRaw = BitConverter.GetBytes(endDstByteValue);

				Buffer.BlockCopy(this.hourRaw, INDEX_ZERO, this._rawData, HOUR_BYTE_LOC, 1);
				Buffer.BlockCopy(this.minuteRaw, INDEX_ZERO, this._rawData, MINUTE_BYTE_LOC, 1);
				Buffer.BlockCopy(this.secondRaw, INDEX_ZERO, this._rawData, SECOND_BYTE_LOC, 1);
				Buffer.BlockCopy(this.hourTypeRaw, INDEX_ZERO, this._rawData, HOUR_TYPE_BYTE_LOC, 1);
				Buffer.BlockCopy(this.yearRaw, INDEX_ZERO, this._rawData, YEAR_BYTE_LOC, 1);
				Buffer.BlockCopy(this.monthRaw, INDEX_ZERO, this._rawData, MONTH_BYTE_LOC, 1);
				Buffer.BlockCopy(this.dayRaw, INDEX_ZERO, this._rawData, DAY_BYTE_LOC, 1);
				Buffer.BlockCopy(this.hourOffsetRaw, INDEX_ZERO, this._rawData, HOUR_OFFSET_BYTE_LOC, 1);
				Buffer.BlockCopy(this.minuteOffsetRaw, INDEX_ZERO, this._rawData, MINUTE_OFFSET_BYTE_LOC, 1);

				//Add the two prefix bytes needed for the commands
				//For 900, 1st byte is the command size while 2nd byte is the command ID
				byte[] commandSize = BitConverter.GetBytes(COMMAND_SIZE_WRITE_WITH_OFFSET);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandSize, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

