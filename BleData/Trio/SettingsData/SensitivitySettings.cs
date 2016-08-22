using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.BleData.Trio.SettingsData
{
	public class SensitivitySettings: ITrioDataHandler
	{
		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_ID_WRITE = 0x58;
		const int COMMAND_ID_READ = 0x59;

		const int INDEX_ZERO = 0;

		const int RELATIVE_LIMIT_LOC = 2;
		const int SENSITIVITY_OLD_LOC = 2;
		const int RELATIVE_SENSITIVITY_LOC = 3;
		const int SLEEP_THRESHOLD_LOC = 5;

		const int RELATIVE_LIMIT_SIZE = 1;
		const int SENSITIVITY_OLD_SIZE = 1;
		const int RELATIVE_SENSITIVITY_SIZE = 2;
		const int SLEEP_THRESHOLD_SIZE = 1;

		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;


		public int RelativeLimit { get; set; }
		public int RelativeSensitivity { get; set; }
		public int SleepThreshold { get; set; }
		public int SensitivityOld { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }

		/* #### Equavalent RAW data per field #####*/
		byte[] relativeLimitRaw;
		byte[] relativeSensitivityRaw;
		byte[] sleepThresholdRaw;
		byte[] sensitivityOldRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		public byte[] _rawData { get; set; }
		public byte[] _readCommandRawData { get; set; }


		TrioDeviceInformation trioDevInfo;

		public SensitivitySettings(TrioDeviceInformation devInfo)
		{
			this.trioDevInfo = devInfo;
			this.ClearData();
		}

		private void ClearData()
		{
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			Array.Clear(this.relativeLimitRaw, INDEX_ZERO, this.relativeLimitRaw.Length);
			Array.Clear(this.relativeSensitivityRaw, INDEX_ZERO, this.relativeSensitivityRaw.Length);
			Array.Clear(this.sleepThresholdRaw, INDEX_ZERO, this.sleepThresholdRaw.Length);
			Array.Clear(this.sensitivityOldRaw, INDEX_ZERO, this.sensitivityOldRaw.Length);
			Array.Clear(this.writeCommandResponseCodeRaw, INDEX_ZERO, this.writeCommandResponseCodeRaw.Length);
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parsingStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{
				Array.Copy(rawData, this._rawData, rawData.Length);
				this.IsReadCommand = true;
				if (rawData[1] == 0x58)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);
					parsingStatus = BLEParsingStatus.SUCCESS;
				}

				else
				{
					if (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f)
					{
						this.relativeLimitRaw = new byte[RELATIVE_LIMIT_SIZE];
						this.relativeSensitivityRaw = new byte[RELATIVE_SENSITIVITY_SIZE];
						this.sleepThresholdRaw = new byte[SLEEP_THRESHOLD_SIZE];

						Array.Copy(this._rawData, RELATIVE_LIMIT_LOC, this.relativeLimitRaw, INDEX_ZERO, RELATIVE_LIMIT_SIZE);
						Array.Copy(this._rawData, RELATIVE_SENSITIVITY_LOC, this.relativeSensitivityRaw, INDEX_ZERO, RELATIVE_SENSITIVITY_SIZE);
						Array.Copy(this._rawData, SLEEP_THRESHOLD_LOC, this.sleepThresholdRaw, INDEX_ZERO, SLEEP_THRESHOLD_SIZE);


						this.RelativeLimit = BitConverter.ToInt32(this.relativeLimitRaw, INDEX_ZERO);
						this.RelativeSensitivity = BitConverter.ToInt32(this.relativeSensitivityRaw, INDEX_ZERO);
						this.SleepThreshold = BitConverter.ToInt32(this.sleepThresholdRaw, INDEX_ZERO);
					}
					else
					{
						this.sensitivityOldRaw = new byte[SENSITIVITY_OLD_SIZE];
						Array.Copy(this._rawData, SENSITIVITY_OLD_LOC, this.sensitivityOldRaw, INDEX_ZERO, SENSITIVITY_OLD_SIZE);
						this.SensitivityOld = BitConverter.ToInt32(this.sensitivityOldRaw, INDEX_ZERO);
					}


				}



			});

			return parsingStatus;
		}

		public async Task<byte[]> GetReadCommand()
		{
			await Task.Run(() =>
			{
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
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
				if (this.trioDevInfo.ModelNumber == 961 && this.trioDevInfo.FirmwareVersion >= 5.0f)
				{
					this.relativeLimitRaw = BitConverter.GetBytes(this.RelativeLimit);
					this.relativeSensitivityRaw = BitConverter.GetBytes(this.RelativeSensitivity);
					this.sleepThresholdRaw = BitConverter.GetBytes(this.SleepThreshold);

					Buffer.BlockCopy(this.relativeLimitRaw, 0, this._rawData, RELATIVE_LIMIT_LOC, RELATIVE_LIMIT_SIZE);
					Buffer.BlockCopy(this.relativeSensitivityRaw, 0, this._rawData, RELATIVE_SENSITIVITY_LOC, RELATIVE_SENSITIVITY_SIZE);
					Buffer.BlockCopy(this.relativeLimitRaw, 0, this._rawData, SLEEP_THRESHOLD_LOC, SLEEP_THRESHOLD_SIZE);



				}

				else
				{
					this.sensitivityOldRaw = BitConverter.GetBytes(this.SensitivityOld);
					Buffer.BlockCopy(this.sensitivityOldRaw, 0, this._rawData, SENSITIVITY_OLD_LOC, SENSITIVITY_OLD_SIZE);

				}

				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_PREFIX);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

