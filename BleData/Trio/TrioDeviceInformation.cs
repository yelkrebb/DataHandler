using System;
using System.Threading.Tasks;

namespace Motion.Core.Data.BleData.Trio
{
	public class TrioDeviceInformation : ITrioDataHandler
	{

		const int COMMAND_PREFIX = 0x1B;
		const int COMMAND_SIZE_READ = 1;
		const int COMMAND_ID_READ = 0x40;

		const int INDEX_ZERO = 0;

		const int SERIAL_NUMBER_BYTE_LOC = 0;
		const int MODEL_NUMBER_BYTE_LOC = 10;
		const int FIRMWARE_VERSION_BYTE_LOC = 12;
		const int BROADCAST_TYPE_BYTE_LOC = 13;
		const int HARDWARE_VERSION_BYTE_LOC = 14;
		const int BATTERY_LEVEL_BYTE_LOC = 15;
		const int BOOTLOADER_VERSION_BYTE_LOC = 16;

		const int SERIAL_NUMBER_BYTE_SIZE = 10;
		const int MODEL_NUMBER_BYTE_SIZE = 2;
		const int FIRMWARE_VERSION_BYTE_SIZE = 1;
		const int BROADCAST_TYPE_BYTE_SIZE = 1;
		const int HARDWARE_VERSION_BYTE_SIZE = 1;
		const int BATTERY_LEVEL_BYTE_SIZE = 1;
		const int BOOTLOADER_VERSION_BYTE_SIZE = 1;

		public int ModelNumber { get; set; }
		public int ModelName { get; set; }
		public long SerialNumber { get; set; }

		public float FirmwareVersion { get; set; }
		public int BatteryLevel { get; set; }
		public string BroadcastType { get; set; }
		public float HardwareVersion { get; set; }
		public float BootloaderVersion { get; set; }

		public byte[] _rawData;
		public byte[] _readCommandRawData;

		/* #### Equavalent RAW data per field #####*/
		byte[] serialNumberRaw;
		byte[] modelNumberRaw;
		byte[] firmwareVersionRaw;
		byte[] broadcastTypeRaw;
		byte[] hardwareVersionRaw;
		byte[] batteryLevelRaw;
		byte[] bootloaderVersionRaw;
		/* ### End Raw data per field ### */

		private void ClearData()
		{
			if (this._rawData != null && this._rawData.Length > 0)
				Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			if (this._readCommandRawData != null && this._readCommandRawData.Length > 0)
				Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);
			if (this.serialNumberRaw != null && this.serialNumberRaw.Length > 0)
				Array.Clear(this.serialNumberRaw, INDEX_ZERO, this.serialNumberRaw.Length);
			if (this.modelNumberRaw != null && this.modelNumberRaw.Length > 0)
				Array.Clear(this.modelNumberRaw, INDEX_ZERO, this.modelNumberRaw.Length);
			if (this.firmwareVersionRaw != null && this.firmwareVersionRaw.Length > 0)
				Array.Clear(this.firmwareVersionRaw, INDEX_ZERO, this.firmwareVersionRaw.Length);
			if (this.broadcastTypeRaw != null && this.broadcastTypeRaw.Length > 0)
				Array.Clear(this.broadcastTypeRaw, INDEX_ZERO, this.broadcastTypeRaw.Length);
			if (this.hardwareVersionRaw != null && this.hardwareVersionRaw.Length > 0)
				Array.Clear(this.hardwareVersionRaw, INDEX_ZERO, this.hardwareVersionRaw.Length);
			if (this.batteryLevelRaw != null && this.batteryLevelRaw.Length > 0)
				Array.Clear(this.batteryLevelRaw, INDEX_ZERO, this.batteryLevelRaw.Length);
			if (this.bootloaderVersionRaw != null && this.bootloaderVersionRaw.Length > 0)
				Array.Clear(this.bootloaderVersionRaw, INDEX_ZERO, this.bootloaderVersionRaw.Length);
		}

		public TrioDeviceInformation()
		{
			this.ClearData();
		}



		public async Task<BLEParsingStatus> ParseData(byte[] rawData)
		{
			throw new NotImplementedException();
		}

		public async Task<byte[]> GetReadCommand()
		{
			throw new NotImplementedException();
		}

		public async Task<byte[]> GetWriteCommand()
		{
			throw new NotImplementedException();
		}
	}
}

