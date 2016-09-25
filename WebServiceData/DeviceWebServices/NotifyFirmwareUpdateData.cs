using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Motion.Mobile.Utilities;
using Motion.Core.WSHandler;
using Motion.Core.Data.WebServiceData;


namespace Motion.Core.Data.WebServiceData.DeviceWebServices
{
	public class NotifyFirmwareUpdateDataRequest
	{
		[JsonProperty(PropertyName = "mdid")]
		public int MemberDeviceID;
		[JsonProperty(PropertyName = "aid")]
		public int ApplicationID;
		[JsonProperty(PropertyName = "pid")]
		public int PlatformID;
		[JsonProperty(PropertyName = "old")]
		public OldDeviceInfo oldDeviceInfo;
		[JsonProperty(PropertyName = "new")]
		public NewDeviceInfo newDeviceInfo;

		public class OldDeviceInfo
		{
			[JsonProperty(PropertyName = "mdl")]
			public int DeviceModel;
			[JsonProperty(PropertyName = "serno")]
			public long DeviceSerial;
			[JsonProperty(PropertyName = "fwver")]
			public float FirmwareVersion;

			internal OldDeviceInfo()
			{

			}
		}

		public class NewDeviceInfo
		{
			[JsonProperty(PropertyName = "mdl")]
			public int DeviceModel;
			[JsonProperty(PropertyName = "serno")]
			public long DeviceSerial;
			[JsonProperty(PropertyName = "fwver")]
			public float FirmwareVersion;

			internal NewDeviceInfo()
			{

			}
		}

		[JsonIgnore]
		public string requestJSON;


		internal NotifyFirmwareUpdateDataRequest()
		{
		}
	}

	public class NotifyFirmwareUpdateDataResponse
	{
		[JsonProperty(PropertyName = "status")]
		public string ResponseStatus;
		[JsonProperty(PropertyName = "msg")]
		public string ResponseMessage;

		[JsonIgnore]
		public string responseJSON;


		internal NotifyFirmwareUpdateDataResponse()
		{
		}
	}

	public class NotifyFirmwareUpdateData
	{
		const string METHOD_NAME = "NotifyFirmwareUpdate";
		public NotifyFirmwareUpdateDataRequest request = new NotifyFirmwareUpdateDataRequest();
		public NotifyFirmwareUpdateDataResponse response = new NotifyFirmwareUpdateDataResponse();

		public NotifyFirmwareUpdateData()
		{
			this.initResponseObjects();
		}

		public async Task<WebServiceRequestStatus> PostRequest()
		{
			WebServiceRequestStatus status = WebServiceRequestStatus.SUCCESS;

			if (this.isRequestDataValid())
			{
				WebService ws = new WebService();
				string jsonString = JsonConvert.SerializeObject(request);
				System.Diagnostics.Debug.WriteLine(jsonString);
				this.request.requestJSON = jsonString;
				string responseStr = await ws.PostData(Utils.GetDeviceServicesURL() + METHOD_NAME, jsonString);
				System.Diagnostics.Debug.WriteLine(responseStr);
				this.response.responseJSON = responseStr;
				this.response = JsonConvert.DeserializeObject<NotifyFirmwareUpdateDataResponse>(responseStr, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
				if (string.Equals(this.response.ResponseStatus, "FAIL"))
					status = WebServiceRequestStatus.FAIL;
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

			if (this.request.MemberDeviceID != 0 && this.request.ApplicationID != 0 && this.request.PlatformID != 0)
			{
				isValid = true;
			}
			return isValid;
		}

		private void initResponseObjects()
		{
			this.request.oldDeviceInfo = new NotifyFirmwareUpdateDataRequest.OldDeviceInfo();
			this.request.newDeviceInfo = new NotifyFirmwareUpdateDataRequest.NewDeviceInfo();
		}
	}
}

