using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Motion.Mobile.Utilities;
using Motion.Core.WSHandler;
using Motion.Core.Data.WebServiceData;
using Motion.Core.Data.BleData.Trio.StepsData;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.WebServiceData.DeviceWebServices
{
	public class UploadTalliesDataRequest
	{
		[JsonProperty(PropertyName = "mdl")]
		public int DeviceModel;
		[JsonProperty(PropertyName = "serno")]
		public long DeviceSerial;
		[JsonProperty(PropertyName = "mdid")]
		public int MemberDeviceID;
		[JsonProperty(PropertyName = "aid")]
		public int ApplicationID;
		[JsonProperty(PropertyName = "pid")]
		public int PlatformID;
		[JsonProperty(PropertyName = "ddtm")]
		public DateTime? DeviceDateTime = null;
		[JsonProperty(PropertyName = "data")]
		public TalliesData talliesData;

		[JsonIgnore]
		public string requestJSON;


		internal UploadTalliesDataRequest()
		{
		}
	}

	public class UploadTalliesDataResponse
	{
		[JsonProperty(PropertyName = "status")]
		public string ResponseStatus;
		[JsonProperty(PropertyName = "msg")]
		public string ResponseMessage;

		[JsonIgnore]
		public string responseJSON;


		internal UploadTalliesDataResponse()
		{
		}
	}

	public class UploadTalliesData
	{
		const string METHOD_NAME = "UploadTallies";
		public UploadTalliesDataRequest request = new UploadTalliesDataRequest();
		public UploadTalliesDataResponse response = new UploadTalliesDataResponse();

		private TrioDeviceInformation _devInfo;


		public UploadTalliesData(TrioDeviceInformation devInfo)
		{
			this._devInfo = devInfo;
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
				this.response = JsonConvert.DeserializeObject<UploadTalliesDataResponse>(responseStr, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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

			if (this.request.DeviceModel != 0 && this.request.DeviceSerial != 0 && this.request.MemberDeviceID != 0 && this.request.MemberDeviceID != 0
			   &&  this.request.ApplicationID != 0 && this.request.PlatformID != 0 && this.request.DeviceDateTime != null)
			{
				isValid = true;
			}
			return isValid;
		}

		private void initResponseObjects()
		{
			this.request.talliesData = new TalliesData(this._devInfo);
		}
	}
}

