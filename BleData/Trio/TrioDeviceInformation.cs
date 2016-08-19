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
		public char BroadcastType { get; set; }
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
			Array.Clear(this._rawData, INDEX_ZERO, this._rawData.Length);
			Array.Clear(this._readCommandRawData, INDEX_ZERO, this._readCommandRawData.Length);

			Array.Clear(this.serialNumberRaw, INDEX_ZERO, this.serialNumberRaw.Length);
			Array.Clear(this.modelNumberRaw, INDEX_ZERO, this.modelNumberRaw.Length);
			Array.Clear(this.firmwareVersionRaw, INDEX_ZERO, this.firmwareVersionRaw.Length);
			Array.Clear(this.broadcastTypeRaw, INDEX_ZERO, this.broadcastTypeRaw.Length);
			Array.Clear(this.hardwareVersionRaw, INDEX_ZERO, this.hardwareVersionRaw.Length);
			Array.Clear(this.batteryLevelRaw, INDEX_ZERO, this.batteryLevelRaw.Length);
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

