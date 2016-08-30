using System;
using System.Globalization;
using System.Threading.Tasks;
using Motion.Mobile.Utilities;
using Motion.Core.Data.BleData.FT900;

namespace Motion.Core.Data.BleData.FT900.SettingsData
{
	public class FT900StepAlgorithmSetting:FT900DataHandler
	{
		const int COMMAND_PREFIX_WRITE = 0x12;
		const int COMMAND_ID_WRITE = 0x3C;
		const int COMMAND_ID_READ = 0x3D;
		const int COMMAND_PREFIX_READ = 0x01;
		const int COMMAND_SIZE_READ = 2;
		const int COMMAND_SIZE_WRITE = 17;

		const int INDEX_ZERO = 0;

		const int GRAVITY_LOC = 2;
		const int NOISE_THRESHOLD_LOC = 4;
		const int PEAK_FACTOR_LOC = 6;
		const int DISCOUNT_RATE_LOC = 7;
		const int UPPER_FALL_THRESHOLD_LOC = 9;
		const int LOWER_FALL_THRESHOLD_LOC = 11;
		const int SAMPLE_PERCENT_LOC = 13;
		const int QUALITY_THRESHOLD_LOC = 14;
		const int RELATIVE_LIMIT_LOC = 15;
		const int RELATIVE_SENSITIVITY_LOC = 16;
		const int STEP_DELAY_LOC = 18;

		const int GRAVITY_BYTE_SIZE = 2;
		const int NOISE_THRESHOLD_BYTE_SIZE = 2;
		const int PEAK_FACTOR_BYTE_SIZE = 1;
		const int DISCOUNT_RATE_BYTE_SIZE = 2;
		const int UPPER_FALL_THRESHOLD_BYTE_SIZE = 2;
		const int LOWER_FALL_THRESHOLD_BYTE_SIZE = 2;
		const int SAMPLE_PERCENT_BYTE_SIZE = 1;
		const int QUALITY_THRESHOLD_BYTE_SIZE = 1;
		const int RELATIVE_LIMIT_BYTE_SIZE = 1;
		const int RELATIVE_SENSITIVITY_BYTE_SIZE = 2;
		const int STEP_DELAY_BYTE_SIZE = 1;

		const int WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE = 1;

		public int Gravity { get; set; }
		public int NoiseThreshold { get; set; }
		public int PeakFactor { get; set; }
		public int DiscountRateWhole { get; set; }
		public int DiscountRateDecimal { get; set; }
		public int UpperThresholdWhole { get; set; }
		public int UpperThresholdDecimal { get; set; }
		public int LowerThresholdWhole { get; set; }
		public int LowerThresholdDecimal { get; set; }
		public int SamplePercent { get; set; }
		public int QualityThreshold { get; set; }
		public int RelativeLimit { get; set; }
		public int RelativeSensitivity { get; set; }
		public int StepDelay { get; set; }
		public int WriteCommandResponseCode { get; set; }
		public bool IsReadCommand { get; set; }



		byte[] _rawData;
		byte[] _readCommandRawData;


		/* #### Equavalent RAW data per field #####*/
		byte[] gravityRaw;
		byte[] noiseThresholdRaw;
		byte[] peakFactorRaw;
		byte[] discountRateRaw;
		byte[] upperThresholdRaw;
		byte[] lowerThresholdRaw;
		byte[] samplePercentRaw;
		byte[] qualityThresholdRaw;
		byte[] relativeLimitRaw;
		byte[] relativeSensitivityRaw;
		byte[] stepDelayRaw;
		byte[] writeCommandResponseCodeRaw;
		/* ### End Raw data per field ### */

		FT900DeviceInformation ft900DevInfo;

		public FT900StepAlgorithmSetting(FT900DeviceInformation devInfo)
		{
			this.ft900DevInfo = devInfo;
			this.ClearData();
		}
		private void ClearData()
		{
			if (this._rawData != null && this._rawData.Length > 0)
				Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			if (this._readCommandRawData != null && this._readCommandRawData.Length > 0)
				Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			if (this.gravityRaw != null && this.gravityRaw.Length > 0)
				Array.Clear(this.gravityRaw, INDEX_ZERO, this.gravityRaw.Length);
			if (this.noiseThresholdRaw != null && this.noiseThresholdRaw.Length > 0)
				Array.Clear(this.noiseThresholdRaw, INDEX_ZERO, this.noiseThresholdRaw.Length);
			if (this.peakFactorRaw != null && this.peakFactorRaw.Length > 0)
				Array.Clear(this.peakFactorRaw, INDEX_ZERO, this.peakFactorRaw.Length);
			if (this.discountRateRaw != null && this.discountRateRaw.Length > 0)
				Array.Clear(this.discountRateRaw, INDEX_ZERO, this.discountRateRaw.Length);
			if (this.upperThresholdRaw != null && this.upperThresholdRaw.Length > 0)
				Array.Clear(this.upperThresholdRaw, INDEX_ZERO, this.upperThresholdRaw.Length);
			if (this.lowerThresholdRaw != null && this.lowerThresholdRaw.Length > 0)
				Array.Clear(this.lowerThresholdRaw, INDEX_ZERO, this.lowerThresholdRaw.Length);
			if (this.samplePercentRaw != null && this.samplePercentRaw.Length > 0)
				Array.Clear(this.samplePercentRaw, INDEX_ZERO, this.samplePercentRaw.Length);
			if (this.qualityThresholdRaw != null && this.qualityThresholdRaw.Length > 0)
				Array.Clear(this.qualityThresholdRaw, INDEX_ZERO, this.qualityThresholdRaw.Length);
			if (this.relativeLimitRaw != null && this.relativeLimitRaw.Length > 0)
				Array.Clear(this.relativeLimitRaw, INDEX_ZERO, this.relativeLimitRaw.Length);
			if (this.relativeSensitivityRaw != null && this.relativeSensitivityRaw.Length > 0)
				Array.Clear(this.relativeSensitivityRaw, INDEX_ZERO, this.relativeSensitivityRaw.Length);
			if (this.stepDelayRaw != null && this.stepDelayRaw.Length > 0)
				Array.Clear(this.stepDelayRaw, INDEX_ZERO, this.stepDelayRaw.Length);
			if (this.writeCommandResponseCodeRaw != null && this.writeCommandResponseCodeRaw.Length > 0)
				Array.Clear(this.writeCommandResponseCodeRaw, INDEX_ZERO, this.writeCommandResponseCodeRaw.Length);
		}

		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			BLEParsingStatus parseStatus = BLEParsingStatus.ERROR;
			await Task.Run(() =>
			{

				this._rawData = new byte[rawData.Length];
				Array.Copy(rawData, this._rawData, rawData.Length);

				this.IsReadCommand = true;
				if (rawData[1] == 0x3C)
				{
					this.IsReadCommand = false;
					this.writeCommandResponseCodeRaw = new byte[WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE];
					Array.Copy(this._rawData, 2, this.writeCommandResponseCodeRaw, INDEX_ZERO, WRITE_COMMAND_RESPONSE_CODE_BYTE_SIZE);
					this.WriteCommandResponseCode = BitConverter.ToInt32(this.writeCommandResponseCodeRaw, INDEX_ZERO);
				}

				else
				{
					this.gravityRaw = new byte[GRAVITY_BYTE_SIZE];
					this.noiseThresholdRaw = new byte[NOISE_THRESHOLD_BYTE_SIZE];
					this.peakFactorRaw = new byte[PEAK_FACTOR_BYTE_SIZE];
					this.discountRateRaw = new byte[DISCOUNT_RATE_BYTE_SIZE];
					this.upperThresholdRaw = new byte[UPPER_FALL_THRESHOLD_BYTE_SIZE];
					this.lowerThresholdRaw = new byte[LOWER_FALL_THRESHOLD_BYTE_SIZE];
					this.samplePercentRaw = new byte[SAMPLE_PERCENT_BYTE_SIZE];
					this.qualityThresholdRaw = new byte[QUALITY_THRESHOLD_BYTE_SIZE];
					this.relativeLimitRaw = new byte[RELATIVE_LIMIT_BYTE_SIZE];
					this.relativeSensitivityRaw = new byte[RELATIVE_SENSITIVITY_BYTE_SIZE];
					this.stepDelayRaw = new byte[STEP_DELAY_BYTE_SIZE];

					Array.Copy(this._rawData, GRAVITY_LOC, this.gravityRaw, INDEX_ZERO, GRAVITY_BYTE_SIZE);
					Array.Copy(this._rawData, NOISE_THRESHOLD_LOC, this.noiseThresholdRaw, INDEX_ZERO, NOISE_THRESHOLD_BYTE_SIZE);
					Array.Copy(this._rawData, PEAK_FACTOR_LOC, this.peakFactorRaw, INDEX_ZERO, PEAK_FACTOR_BYTE_SIZE);
					Array.Copy(this._rawData, DISCOUNT_RATE_LOC, this.discountRateRaw, INDEX_ZERO, DISCOUNT_RATE_BYTE_SIZE);
					Array.Copy(this._rawData, UPPER_FALL_THRESHOLD_LOC, this.upperThresholdRaw, INDEX_ZERO, UPPER_FALL_THRESHOLD_BYTE_SIZE);
					Array.Copy(this._rawData, LOWER_FALL_THRESHOLD_LOC, this.lowerThresholdRaw, INDEX_ZERO, LOWER_FALL_THRESHOLD_BYTE_SIZE);
					Array.Copy(this._rawData, SAMPLE_PERCENT_LOC, this.samplePercentRaw, INDEX_ZERO, SAMPLE_PERCENT_BYTE_SIZE);
					Array.Copy(this._rawData, QUALITY_THRESHOLD_LOC, this.qualityThresholdRaw, INDEX_ZERO, QUALITY_THRESHOLD_BYTE_SIZE);
					Array.Copy(this._rawData, RELATIVE_LIMIT_LOC, this.relativeLimitRaw, INDEX_ZERO, RELATIVE_LIMIT_BYTE_SIZE);
					Array.Copy(this._rawData, RELATIVE_SENSITIVITY_LOC, this.relativeSensitivityRaw, INDEX_ZERO, RELATIVE_SENSITIVITY_BYTE_SIZE);
					Array.Copy(this._rawData, STEP_DELAY_LOC, this.stepDelayRaw, INDEX_ZERO, STEP_DELAY_BYTE_SIZE);

					this.Gravity = Convert.ToInt32(Utils.getDecimalValue(this.gravityRaw));
					this.NoiseThreshold = Convert.ToInt32(Utils.getDecimalValue(this.noiseThresholdRaw));
					this.PeakFactor = Convert.ToInt32(Utils.getDecimalValue(this.peakFactorRaw));

					int byteValue = Convert.ToInt32(Utils.getDecimalValue(this.discountRateRaw));
					this.DiscountRateWhole = Convert.ToInt32(byteValue >> 8);
					this.DiscountRateDecimal = Convert.ToInt32(byteValue & 0xFF);

					byteValue = Convert.ToInt32(Utils.getDecimalValue(this.upperThresholdRaw));
					this.UpperThresholdWhole = Convert.ToInt32(byteValue >> 8);
					this.UpperThresholdDecimal = Convert.ToInt32(byteValue & 0xFF);

					byteValue = Convert.ToInt32(Utils.getDecimalValue(this.lowerThresholdRaw));
					this.LowerThresholdWhole = Convert.ToInt32(byteValue >> 8);
					this.LowerThresholdDecimal = Convert.ToInt32(byteValue & 0xFF);

					this.SamplePercent = Convert.ToInt32(Utils.getDecimalValue(this.samplePercentRaw));
					this.QualityThreshold = Convert.ToInt32(Utils.getDecimalValue(this.qualityThresholdRaw));
					this.RelativeLimit = Convert.ToInt32(Utils.getDecimalValue(this.relativeLimitRaw));
					this.RelativeSensitivity = Convert.ToInt32(Utils.getDecimalValue(this.relativeSensitivityRaw));
					this.StepDelay = Convert.ToInt32(Utils.getDecimalValue(this.stepDelayRaw));

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
				byte[] commandPrefix = BitConverter.GetBytes(COMMAND_ID_READ);
				byte[] commandID = BitConverter.GetBytes(COMMAND_PREFIX_READ);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandPrefix, INDEX_ZERO, this._readCommandRawData, INDEX_ZERO, 1);
			});
			return this._readCommandRawData;
		}

		public async Task<byte[]> GetWriteCommand()
		{
			await Task.Run(() =>
			{
				this._rawData = new byte[COMMAND_SIZE_WRITE + 2];

				this.gravityRaw = BitConverter.GetBytes(this.Gravity);
				this.noiseThresholdRaw = BitConverter.GetBytes(this.NoiseThreshold);
				this.peakFactorRaw = BitConverter.GetBytes(this.PeakFactor);

				int discountRate = 0x00;
				discountRate |= (this.DiscountRateWhole << 8);
				discountRate |= this.DiscountRateDecimal;

				this.discountRateRaw = BitConverter.GetBytes(discountRate);

				int upperThreshold = 0x00;
				upperThreshold |= (this.UpperThresholdWhole << 8);
				upperThreshold |= this.UpperThresholdDecimal;

				this.upperThresholdRaw = BitConverter.GetBytes(upperThreshold);

				int lowerThreshold = 0x00;
				lowerThreshold |= (this.LowerThresholdWhole << 8);
				lowerThreshold |= this.LowerThresholdDecimal;

				this.samplePercentRaw = BitConverter.GetBytes(this.SamplePercent);
				this.qualityThresholdRaw = BitConverter.GetBytes(this.QualityThreshold);
				this.relativeLimitRaw = BitConverter.GetBytes(this.RelativeLimit);
				this.relativeSensitivityRaw = BitConverter.GetBytes(this.RelativeSensitivity);
				this.stepDelayRaw = BitConverter.GetBytes(this.StepDelay);


				Buffer.BlockCopy(this.gravityRaw, INDEX_ZERO, this._rawData, GRAVITY_LOC, GRAVITY_BYTE_SIZE);
				Buffer.BlockCopy(this.noiseThresholdRaw, INDEX_ZERO, this._rawData, NOISE_THRESHOLD_LOC, NOISE_THRESHOLD_BYTE_SIZE);
				Buffer.BlockCopy(this.peakFactorRaw, INDEX_ZERO, this._rawData, PEAK_FACTOR_LOC, PEAK_FACTOR_BYTE_SIZE);
				Buffer.BlockCopy(this.discountRateRaw, INDEX_ZERO, this._rawData, DISCOUNT_RATE_LOC, DISCOUNT_RATE_BYTE_SIZE);
				Buffer.BlockCopy(this.upperThresholdRaw, INDEX_ZERO, this._rawData, UPPER_FALL_THRESHOLD_LOC, UPPER_FALL_THRESHOLD_BYTE_SIZE);
				Buffer.BlockCopy(this.lowerThresholdRaw, INDEX_ZERO, this._rawData, LOWER_FALL_THRESHOLD_LOC, LOWER_FALL_THRESHOLD_BYTE_SIZE);
				Buffer.BlockCopy(this.samplePercentRaw, INDEX_ZERO, this._rawData, SAMPLE_PERCENT_LOC, SAMPLE_PERCENT_BYTE_SIZE);
				Buffer.BlockCopy(this.qualityThresholdRaw, INDEX_ZERO, this._rawData, QUALITY_THRESHOLD_LOC, QUALITY_THRESHOLD_BYTE_SIZE);
				Buffer.BlockCopy(this.relativeLimitRaw, INDEX_ZERO, this._rawData, RELATIVE_LIMIT_LOC, RELATIVE_LIMIT_BYTE_SIZE);
				Buffer.BlockCopy(this.relativeSensitivityRaw, INDEX_ZERO, this._rawData, RELATIVE_SENSITIVITY_LOC, RELATIVE_SENSITIVITY_BYTE_SIZE);
				Buffer.BlockCopy(this.stepDelayRaw, INDEX_ZERO, this._rawData, RELATIVE_SENSITIVITY_LOC, RELATIVE_SENSITIVITY_BYTE_SIZE);

				//Add the two prefix bytes needed for the commands
				//For 900, 1st byte is the command size while 2nd byte is the command ID
				byte[] commandSize = BitConverter.GetBytes(COMMAND_PREFIX_WRITE);
				byte[] commandID = BitConverter.GetBytes(COMMAND_ID_WRITE);
				Buffer.BlockCopy(commandID, INDEX_ZERO, this._rawData, INDEX_ZERO + 1, 1);
				Buffer.BlockCopy(commandSize, INDEX_ZERO, this._rawData, INDEX_ZERO, 1);

			});

			return this._rawData;
		}
	}
}

