using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;

using Newtonsoft.Json;

namespace Motion.Core.Data.BleData.Trio.StepsData
{
	public class StepsDailyData
	{
		public int stepsYear;
		public int stepsMonth;
		public int stepsDay;
		public List<StepsHourlyData> hourlyData;

		public int totalDailySteps;
		public int totalDailyCal;

		internal StepsDailyData()
		{
		}
	}

	public class StepsHourlyData
	{
		public int hourNumber;
		public List<StepsMinuteData> minuteData;

		public int totalHourlySteps;
		public int totalHourlyCal;

		internal StepsHourlyData()
		{
		}
	}

	public class StepsMinuteData
	{
		[JsonProperty(PropertyName = "sdate")]
		public string StepDate;
		[JsonProperty(PropertyName = "hrsno")]
		public int HourNumber;
		[JsonProperty(PropertyName = "mslot")]
		public int MinuteNumber;
		[JsonProperty(PropertyName = "step")]
		public int Steps;
		[JsonProperty(PropertyName = "kcal")]
		public int Calories;

		internal StepsMinuteData()
		{
		}
	}


	public class StepsData
	{

		private enum LastCommandSent
		{
			NO_COMMAND_SENT,
			HOUR_RANGE_COMMAND,
			CURRENT_HOUR_COMMAND
		}

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_READ_CURRENT_HOUR = 0x27;
		const int COMMAND_ID_READ_HOUR_RANGE = 0x24;
		const int INDEX_ZERO = 0;

		const int READ_HOUR_RANGE_SIZE = 7;
		const int READ_CURRENT_HOUR_SIZE = 2;
		const int OLD_HOURLY_DATA_SIZE = 124;
		const int NEW_HOURLY_DATA_SIZE = 64;

		const int CHECKSUM_AND_TERMINATOR_DATA_SIZE = 6;

		const int YEAR_LOC = 0;
		const int MONTH_LOC = 1;
		const int DAY_LOC = 2;
		const int HOUR_LOC = 3;

		public List<StepsDailyData> dailyData;


		private byte[] _rawData;
		private byte[] _readCurrentHourCommandRawData;
		private byte[] _readHourRangeCommandRawData;

		private LastCommandSent lastCommand;

		TrioDeviceInformation trioDevInfo;

		public StepsData(TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.lastCommand = LastCommandSent.NO_COMMAND_SENT;
			this.ClearData();
		}

		private void ClearData()
		{
			int rawSize = (this.trioDevInfo.ModelNumber == (int) Constants.TrioDeviceModel.FT965 || this.trioDevInfo.ModelNumber == (int)Constants.TrioDeviceModel.FT969 || this.trioDevInfo.ModelNumber == (int)Constants.TrioDeviceModel.FT905) ? NEW_HOURLY_DATA_SIZE : OLD_HOURLY_DATA_SIZE;
			this._rawData = new byte[rawSize];
			this._readCurrentHourCommandRawData = new byte[READ_CURRENT_HOUR_SIZE];
			this._readHourRangeCommandRawData = new byte[READ_HOUR_RANGE_SIZE];
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCurrentHourCommandRawData, INDEX_ZERO, this._readCurrentHourCommandRawData.Length);
			Array.Clear(this._readHourRangeCommandRawData, INDEX_ZERO, this._readHourRangeCommandRawData.Length);
		}

		public async Task<byte[]> getReadCurrentHourCommand()
		{
			await Task.Run(() =>
			{
				this._readCurrentHourCommandRawData = new byte[READ_CURRENT_HOUR_SIZE];
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ_CURRENT_HOUR);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCurrentHourCommandRawData, INDEX_ZERO, 1);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCurrentHourCommandRawData, INDEX_ZERO + 1, 1);

			});
			lastCommand = LastCommandSent.CURRENT_HOUR_COMMAND;
			return this._readCurrentHourCommandRawData;
		}

		public async Task<byte[]> getReadHourRangeCommand(DateTime stepDate, int startHour, int endHour)
		{
			await Task.Run(() =>
			{
				this._readHourRangeCommandRawData = new byte[READ_HOUR_RANGE_SIZE];
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_READ_HOUR_RANGE);
				byte[] year = BitConverter.GetBytes(stepDate.Year | 0xC0);
				byte[] month = BitConverter.GetBytes(stepDate.Month | 0xC0);
				byte[] day = BitConverter.GetBytes(stepDate.Day | 0xC0);
				byte[] start = BitConverter.GetBytes(startHour | 0xC0);
				byte[] end = BitConverter.GetBytes(endHour | 0xC0);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readHourRangeCommandRawData, INDEX_ZERO, 1);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readHourRangeCommandRawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(year, INDEX_ZERO, this._readHourRangeCommandRawData, INDEX_ZERO + 2, 1);
				Buffer.BlockCopy(month, INDEX_ZERO, this._readHourRangeCommandRawData, INDEX_ZERO + 3, 1);
				Buffer.BlockCopy(day, INDEX_ZERO, this._readHourRangeCommandRawData, INDEX_ZERO + 4, 1);
				Buffer.BlockCopy(start,INDEX_ZERO, this._readHourRangeCommandRawData, INDEX_ZERO + 5, 1);
				Buffer.BlockCopy(end, INDEX_ZERO, this._readHourRangeCommandRawData, INDEX_ZERO + 6, 1);
			});
			lastCommand = LastCommandSent.HOUR_RANGE_COMMAND;
			return this._readHourRangeCommandRawData;
		}

		public async Task<BLEParsingStatus> ParseData(byte[] stepsDataRaw)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR_NO_COMMAND_SENT;
			await Task.Run(() =>
			{
				if (this.lastCommand != LastCommandSent.NO_COMMAND_SENT)
				{
					this._rawData = new byte[stepsDataRaw.Length];
					Array.Copy(stepsDataRaw, this._rawData, stepsDataRaw.Length);

					if (!this.hasMissingDataPacket())
					{
						this.dailyData = new List<StepsDailyData>();
						int hourlyDataLength = this.getHourlyDataSize();
						StepsDailyData daily = new StepsDailyData();

						for (int i = 0; i < this._rawData.Length;)
						{
							if ((i + hourlyDataLength) <= this._rawData.Length)
							{
								byte[] temp_hourData = new byte[hourlyDataLength];
								byte[] yearNumberRaw = new byte[Constants.CHAR_BYTE_SIZE];
								byte[] monthNumberRaw = new byte[Constants.CHAR_BYTE_SIZE];
								byte[] dayNumberRaw = new byte[Constants.CHAR_BYTE_SIZE];
								byte[] hourNumberRaw = new byte[Constants.CHAR_BYTE_SIZE];

								Array.Clear(temp_hourData, INDEX_ZERO, temp_hourData.Length);
								Buffer.BlockCopy(this._rawData, i, temp_hourData, INDEX_ZERO, hourlyDataLength);

								Array.Copy(temp_hourData, YEAR_LOC, yearNumberRaw, INDEX_ZERO, Constants.CHAR_BYTE_SIZE);
								Array.Copy(temp_hourData, MONTH_LOC, monthNumberRaw, INDEX_ZERO, Constants.CHAR_BYTE_SIZE);
								Array.Copy(temp_hourData, DAY_LOC, dayNumberRaw, INDEX_ZERO, Constants.CHAR_BYTE_SIZE);
								Array.Copy(temp_hourData, HOUR_LOC, hourNumberRaw, INDEX_ZERO, Constants.CHAR_BYTE_SIZE);

								int year = (int)Utils.getDecimalValue(yearNumberRaw) & 0x3F;
								int month = (int)Utils.getDecimalValue(monthNumberRaw) & 0x3F;
								int day = (int)Utils.getDecimalValue(dayNumberRaw) & 0x3F;
								int hour = (int)Utils.getDecimalValue(hourNumberRaw) & 0x3F;

								if (Utils.isValidYear(year+2000) && Utils.isValidMonth(month) && Utils.isValidDay(year+2000, month, day))
								{

									if (daily.stepsYear != year || daily.stepsMonth != month || daily.stepsDay != day)
									{
										if (daily.stepsYear != 0 && daily.stepsMonth != 0 && daily.stepsDay != 0)
										{
											dailyData.Add(daily);
										}
										daily = new StepsDailyData();
										daily.hourlyData = new List<StepsHourlyData>();
										daily.stepsYear = year;
										daily.stepsMonth = month;
										daily.stepsDay = day;
									}
									StepsHourlyData hourly = new StepsHourlyData();
									hourly.hourNumber = hour;
									hourly.minuteData = new List<StepsMinuteData>();

									int minuteNumber = 1;
									for (int j = 4; j < temp_hourData.Length;)
									{
										int minutePacketSize = temp_hourData.Length == OLD_HOURLY_DATA_SIZE ? 2 : 1;
										byte[] minutePacketRawData = new byte[minutePacketSize];
										Array.Clear(minutePacketRawData, INDEX_ZERO, minutePacketRawData.Length);
										Array.Copy(temp_hourData, j, minutePacketRawData, INDEX_ZERO, minutePacketSize);

										int stepValue = minutePacketRawData[0];
										int kCalValue = 0;
										if (minutePacketSize == 2)
										{
											kCalValue = minutePacketRawData[1];
										}

										hourly.totalHourlySteps += stepValue;
										hourly.totalHourlyCal += kCalValue;

										StepsMinuteData minuteData = new StepsMinuteData();
										minuteData.StepDate = year.ToString() + "-" + month.ToString() + "-" + day.ToString();
										minuteData.HourNumber = hour;
										minuteData.MinuteNumber = minuteNumber++;
										minuteData.Steps = stepValue;
										minuteData.Calories = kCalValue;

										hourly.minuteData.Add(minuteData);

										j += minutePacketSize;
									}

									daily.totalDailySteps += hourly.totalHourlySteps;
									daily.totalDailyCal += hourly.totalHourlyCal;
									daily.hourlyData.Add(hourly);
								}
								else
								{
									parsingStatus = BLEParsingStatus.ERROR_INVALID_STEP_DATE_COMPONENTS;
									break;
								}
							}
							else
							{
								break;
							}
							i += hourlyDataLength;
						}

						if (!this.dailyData.Contains(daily))
						{
							dailyData.Add(daily);
						}
						parsingStatus = BLEParsingStatus.SUCCESS;
					}
					else
					{
						parsingStatus = BLEParsingStatus.ERROR_HAS_MISSING_DATA_PACKET;
					}
				}
				else
				{
					parsingStatus = BLEParsingStatus.ERROR_NO_COMMAND_SENT;
				}
			});
			return parsingStatus;
		}

		private bool hasMissingDataPacket()
		{
			bool hasMissingPacket = false;

			switch (this.lastCommand)
			{
				case LastCommandSent.CURRENT_HOUR_COMMAND:
					hasMissingPacket = this._rawData.Length < (this.getHourlyDataSize() + CHECKSUM_AND_TERMINATOR_DATA_SIZE) ? true : false;
					break;
				case LastCommandSent.HOUR_RANGE_COMMAND:
					hasMissingPacket = this.hourRangeHouseMissingPacket();
					break;	
			}

			return hasMissingPacket;
		}

		private bool hourRangeHouseMissingPacket()
		{
			int totalHoursRead = (this._readHourRangeCommandRawData[6] - this._readHourRangeCommandRawData[5]) + 1;
			System.Diagnostics.Debug.WriteLine("totalHoursRead " + totalHoursRead);
			System.Diagnostics.Debug.WriteLine("rawDataLength " + _rawData.Length);
			return this._rawData.Length < ((this.getHourlyDataSize() * totalHoursRead) + CHECKSUM_AND_TERMINATOR_DATA_SIZE) ? true : false;
		}

		private int getHourlyDataSize()
		{
			int hourlyDataSize = OLD_HOURLY_DATA_SIZE;

			if (this.trioDevInfo.ModelNumber == (int)Constants.TrioDeviceModel.FT965 || this.trioDevInfo.ModelNumber == (int)Constants.TrioDeviceModel.FT969
							|| this.trioDevInfo.ModelNumber == (int)Constants.TrioDeviceModel.FT905 || (this.trioDevInfo.ModelNumber == (int)Constants.TrioDeviceModel.PE961 && this.trioDevInfo.FirmwareVersion > 4.3f))
			{
				hourlyDataSize = NEW_HOURLY_DATA_SIZE;
			}

			return hourlyDataSize;
		}
	}


}

