using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.SettingsData
{
	public class DeviceSettings: ITrioDataHandler
	{
		const int COMMAND_SIZE_WRITE_WITH_OFFSET = 9;
		const int COMMAND_SIZE_WRITE_ORIG = 7;
		const int COMMAND_SIZE_WRITE_WITH_DST = 13;
		const int COMMAND_SIZE_READ = 2;

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

		const int DATA_BYTE_SIZE = 1;

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

		private byte[] _rawData;
		private byte[] _readCommandRawData;


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

		TrioDeviceInformation trioDevInfo;

		public DeviceSettings (TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;

			this.ClearData ();
		}

		private void ClearData()
		{
			if (this._rawData != null && this._rawData.Length > 0)
				Array.Clear (this._rawData, INDEX_ZERO, this._rawData.Length);
			if (this._readCommandRawData != null && this._readCommandRawData.Length > 0)
				Array.Clear (this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			if (this.hourRaw != null && this.hourRaw.Length > 0)
				Array.Clear (this.hourRaw, INDEX_ZERO, this.hourRaw.Length);
			if (this.minuteRaw != null && this.minuteRaw.Length > 0)
				Array.Clear (this.minuteRaw, INDEX_ZERO, this.minuteRaw.Length);
			if (this.secondRaw != null && this.secondRaw.Length > 0)
				Array.Clear (this.secondRaw, INDEX_ZERO, this.secondRaw.Length);
			if (this.hourTypeRaw != null && this.hourTypeRaw.Length > 0)
				Array.Clear (this.hourTypeRaw, INDEX_ZERO, this.hourTypeRaw.Length);
			if (this.yearRaw != null && this.yearRaw.Length > 0)
				Array.Clear (this.yearRaw, INDEX_ZERO, this.yearRaw.Length);
			if (this.monthRaw != null && this.monthRaw.Length > 0)
				Array.Clear (this.monthRaw, INDEX_ZERO, this.monthRaw.Length);
			if (this.dayRaw != null && this.dayRaw.Length > 0)
				Array.Clear (this.dayRaw, INDEX_ZERO, this.dayRaw.Length);
			if (this.hourOffsetRaw != null && this.hourOffsetRaw.Length > 0)
				Array.Clear (this.hourOffsetRaw, INDEX_ZERO, this.hourOffsetRaw.Length);
			if (this.minuteOffsetRaw != null && this.minuteOffsetRaw.Length > 0)
				Array.Clear (this.minuteOffsetRaw, INDEX_ZERO, this.minuteOffsetRaw.Length);
			if (this.dstStartRaw != null && this.dstStartRaw.Length > 0)
				Array.Clear (this.dstStartRaw, INDEX_ZERO, this.dstStartRaw.Length);
			if (this.dstEndRaw != null && this.dstEndRaw.Length > 0)
				Array.Clear (this.dstEndRaw, INDEX_ZERO, this.dstEndRaw.Length);
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parseStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{

				this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);


				this.IsReadCommand = true;
				if (rawData[1] == 0x16)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = Convert.ToInt32(Utils.getDecimalValue(this.writeCommandResponseCodeRaw));

				}

				else
				{ 
					this.hourRaw = new byte[DATA_BYTE_SIZE];
					this.minuteRaw = new byte[DATA_BYTE_SIZE];
					this.secondRaw = new byte[DATA_BYTE_SIZE];
					this.hourTypeRaw = new byte[DATA_BYTE_SIZE];
					this.yearRaw = new byte[DATA_BYTE_SIZE];
					this.monthRaw = new byte[DATA_BYTE_SIZE];
					this.dayRaw = new byte[DATA_BYTE_SIZE];
					this.hourOffsetRaw = new byte[DATA_BYTE_SIZE];
					this.minuteOffsetRaw = new byte[DATA_BYTE_SIZE];
					this.dstStartRaw = new byte[DATA_BYTE_SIZE];
					this.dstEndRaw = new byte[DATA_BYTE_SIZE];

					Array.Copy(this._rawData, HOUR_BYTE_LOC, this.hourRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, MINUTE_BYTE_LOC, this.minuteRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, SECOND_BYTE_LOC, this.secondRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, HOUR_TYPE_BYTE_LOC, this.hourTypeRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, YEAR_BYTE_LOC, this.yearRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, MONTH_BYTE_LOC, this.monthRaw, INDEX_ZERO, 1);
					Array.Copy(this._rawData, DAY_BYTE_LOC, this.dayRaw, INDEX_ZERO, 1);

					this.Hour = Convert.ToInt32(Utils.getDecimalValue(this.hourRaw)); 
					this.Minute = Convert.ToInt32(Utils.getDecimalValue(this.minuteRaw)); 
					this.Second = Convert.ToInt32(Utils.getDecimalValue(this.secondRaw)); 
					this.HourType = Convert.ToBoolean(Utils.getDecimalValue(this.hourTypeRaw)); 
					this.Year = Convert.ToInt32(Utils.getDecimalValue(this.yearRaw)); 
					this.Month = Convert.ToInt32(Utils.getDecimalValue(this.monthRaw)); 
					this.Day = Convert.ToInt32(Utils.getDecimalValue(this.dayRaw));

					/*if ((this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion < 5.0f))
					{
						Array.Copy(this._rawData, HOUR_OFFSET_BYTE_LOC, this.hourOffsetRaw, INDEX_ZERO, 1);
						Array.Copy(this._rawData, MINUTE_OFFSET_BYTE_LOC, this.minuteOffsetRaw, INDEX_ZERO, 1);

						int flagValue = Convert.ToInt32(Utils.getDecimalValue(this.hourOffsetRaw)); 
						this.OffestType = Convert.ToBoolean((flagValue >> 7) & 0x01);
						this.HourOffset = flagValue & 0x3F;
						this.MinuteOffset = Convert.ToInt32(Utils.getDecimalValue(this.minuteOffsetRaw)); 
					}*/
					if (this.trioDevInfo.ModelNumber == 962 || (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f))
					{
						Array.Copy(this._rawData, HOUR_OFFSET_BYTE_LOC, this.hourOffsetRaw, INDEX_ZERO, 1);
						Array.Copy(this._rawData, MINUTE_OFFSET_BYTE_LOC, this.minuteOffsetRaw, INDEX_ZERO, 1);
						Array.Copy(this._rawData, DST_START_BYTE_LOC, this.dstStartRaw, INDEX_ZERO, 1);
						Array.Copy(this._rawData, DST_END_BYTE_LOC, this.dstEndRaw, INDEX_ZERO, 1);

						int flagValue = Convert.ToInt32(Utils.getDecimalValue(this.hourOffsetRaw)); BitConverter.ToInt32(this.hourOffsetRaw, INDEX_ZERO);
						this.OffestType = Convert.ToBoolean((flagValue >> 7) & 0x01);
						this.DstApplicable = Convert.ToBoolean((flagValue >> 6) & 0x01);
						this.HourOffset = flagValue & 0x3F;
						this.MinuteOffset = BitConverter.ToInt32(this.minuteOffsetRaw, INDEX_ZERO);

						int dstStartValues = BitConverter.ToInt32(this.dstStartRaw, INDEX_ZERO);
						this.DstStartHour = dstStartValues & 0x1F;
						this.DstStartDay = (dstStartValues >> 5) & 0x1F;
						this.DstStartMonth = (dstStartValues >> 10) & 0x0F;

						int dstEndValues = BitConverter.ToInt32(this.dstEndRaw, INDEX_ZERO);
						this.DstEndHour = dstEndValues & 0x1F;
						this.DstEndDay = (dstEndValues >> 5) & 0x1F;
						this.DstEndMonth = (dstEndValues >> 10) & 0x0F;
					}
				}

				parseStatus = BLEParsingStatus.SUCCESS;


			});

			return parseStatus;

		}

		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{
				this._readCommandRawData = new byte[COMMAND_SIZE_READ];
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO+1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
			});
			return this._readCommandRawData;
		}

		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() =>
			{

				if (this.trioDevInfo.ModelNumber == 962 || (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f))
					this._rawData = new byte[COMMAND_SIZE_WRITE_WITH_DST + 2];
				else
					this._rawData = new byte[COMMAND_SIZE_WRITE_WITH_OFFSET + 2];

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


				if (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion < 5.0f)
				{
					//Add the two prefix bytes needed for the commands
					//For 93x and 96x, 1st byte is the command prefix 0x1B while 2nd byte is the command ID
					byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
					byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
					Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO+1, 1);
					Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);
				}
				else if (this.trioDevInfo.ModelNumber == 962 || (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f))
				{
					//Add the two prefix bytes needed for the commands
					//For 93x and 96x, 1st byte is the command prefix 0x1B while 2nd byte is the command ID
					byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
					byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
					Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);
					Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

					Buffer.BlockCopy(this.dstStartRaw, INDEX_ZERO, this._rawData, DST_START_BYTE_LOC, 2);
					Buffer.BlockCopy(this.dstEndRaw, INDEX_ZERO, this._rawData, DST_END_BYTE_LOC, 2);
				}
				else
				{
					//Add the two prefix bytes needed for the commands
					//For 93x and 96x, 1st byte is the command prefix 0x1B while 2nd byte is the command ID
					byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
					byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
					Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO+1, 1);
					Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);
				}

			});

			return this._rawData;
		}
	}
}

