using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using Motion.Mobile.Utilities;
using Motion.Core.WSHandler;
using Motion.Core.Data.WebServiceData;
using Motion.Core.Data.BleData.Trio;
using Motion.Core.Data.BleData.Trio.AccelData;

using Newtonsoft.Json;

namespace Motion.Core.Data.WebServiceData.DeviceWebServices
{
	public class UploadSignatureDataRequest
	{

		[JsonProperty(PropertyName = "mid")]
		public long MemberID;
		[JsonProperty(PropertyName = "mdid")]
		public long MemberDeviceID;
		[JsonProperty(PropertyName = "syncd")]
		public long LastSyncSettingDateTime;
		[JsonProperty(PropertyName = "btadd")]
		public string TrackerBtAdd;
		[JsonProperty(PropertyName = "mdl")]
		public int TrackerModel;
		[JsonProperty(PropertyName = "serno")]
		public long TrackerSerialNumber;
		[JsonProperty(PropertyName = "aid")]
		public int ApplicationID;
		[JsonProperty(PropertyName = "pid")]
		public int PlatformID;
		[JsonProperty(PropertyName = "rmr")]
		public int MemberRMR;
		[JsonProperty(PropertyName = "mdt")]
		public int MinuteDataType;
		[JsonProperty(PropertyName = "pc")]
		public string HostName;
		[JsonProperty(PropertyName = "fwver")]
		public float TrackerFirmwareVersion;
		[JsonProperty(PropertyName = "stype")]
		public string SyncType;
		[JsonProperty(PropertyName = "balvl")]
		public int BatteryLevel;
		[JsonProperty(PropertyName = "fmet")]
		public bool FrequencyMet;
		[JsonProperty(PropertyName = "imet")]
		public bool IntensityMet;
		[JsonProperty(PropertyName = "ddtm")]
		public DateTime? DeviceDateTime = null;
		[JsonProperty(PropertyName = "etdm")]
		public long DataExtractDurationTime;
		[JsonProperty(PropertyName = "syncid")]
		public long SynchronizationID;
		[JsonProperty(PropertyName = "trgr")]
		public long TriggerType;
		[JsonProperty(PropertyName = "data")]
		public Dictionary<string, Dictionary<string, List<string>>> AccelData;

		[JsonIgnore]
		public string requestJSON;

	
		public UploadSignatureDataRequest()
		{
		}
	}

	public class UploadSignatureDataResponse
	{
		[JsonProperty(PropertyName = "status")]
		public string ResponseStatus;
		[JsonProperty(PropertyName = "msg")]
		public string ResponseMessage;
		[JsonProperty(PropertyName = "dtid")]
		public int DataTransmissionID;

		[JsonIgnore]
		public string responseJSON;
		public UploadSignatureDataResponse()
		{
		}
	}
	public class UploadSignatureData
	{
		const string METHOD_NAME = "UploadSignData";

		public SignatureData SignData;

		public UploadSignatureDataRequest request = new UploadSignatureDataRequest();
		public UploadSignatureDataResponse response = new UploadSignatureDataResponse();
		public UploadSignatureData()
		{
			this.initRequestObjects();
		}

		public async Task<WebServiceRequestStatus> PostRequest()
		{
			WebServiceRequestStatus status = WebServiceRequestStatus.SUCCESS;
			this.prepareAccelData();

			if (this.isRequestDataValid())
			{
				WebService ws = new WebService();
				string jsonString = JsonConvert.SerializeObject(request);
				System.Diagnostics.Debug.WriteLine(jsonString);
				this.request.requestJSON = jsonString;
				string responseStr = await ws.PostData(Utils.GetDeviceServicesURL() + METHOD_NAME, jsonString);
				System.Diagnostics.Debug.WriteLine(responseStr);
				this.response.responseJSON = responseStr;
				this.response = JsonConvert.DeserializeObject<UploadSignatureDataResponse>(responseStr, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
			}
			else
			{
				status = WebServiceRequestStatus.ERROR_REQUEST_DATA_INVALID;
			}
			return status;
		}

		private bool isRequestDataValid()
		{
			bool isValid = false;

			if (this.request.TrackerModel != 0 && this.request.TrackerSerialNumber != 0 && this.request.PlatformID != 0 && this.request.ApplicationID != 0
				&& this.request.MemberID != 0 && this.request.MemberDeviceID != 0 && this.request.LastSyncSettingDateTime != 0 && this.request.DeviceDateTime.HasValue)
			{
				isValid = true;
			}
			return isValid;
		}

		private void initRequestObjects()
		{
			this.request.AccelData = new Dictionary<string, Dictionary<string, List<string>>>();
		}

		private void prepareAccelData()
		{
			Dictionary<string, List<string>> data = new Dictionary<string, List<string>>(); 
			List<string> xyzData = new List<string>();
			for (int i = 0; i < this.SignData.X_Axis.Count; i++)
			{
				xyzData.Add(this.SignData.X_Axis[i].ToString());
				xyzData.Add(this.SignData.Y_Axis[i].ToString());
				xyzData.Add(this.SignData.Z_Axis[i].ToString());
			}
			data.Add(this.SignData.SDHour.ToString() + ":" + this.SignData.SDMin.ToString(), xyzData);
			this.request.AccelData.Add(this.SignData.SDYear.ToString() + "-" + this.SignData.SDMonth.ToString() + "-" + this.SignData.SDDay.ToString(), data);
		}

	}
}

