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
	public class SendApplicationInformationRequest
	{
		[JsonProperty(PropertyName = "aname")]
		public string ApplicationName;
		[JsonProperty(PropertyName = "aver")]
		public string ApplicationVersion;
		[JsonProperty(PropertyName = "os")]
		public string OperatingSystem;
		[JsonProperty(PropertyName = "osver")]
		public string OperatingSystemVersion;
		[JsonProperty(PropertyName = "ptype")]
		public string PlatformType;
		[JsonProperty(PropertyName = "brnd")]
		public string Brand;
		[JsonProperty(PropertyName = "mdl")]
		public string Model;
		[JsonProperty(PropertyName = "mfg")]
		public string Manufacturer;
		[JsonProperty(PropertyName = "comm")]
		public string CommunicationType;

		[JsonIgnore]
		public string requestJSON;


		internal SendApplicationInformationRequest()
		{
		}
	}

	public class SendApplicationInformationResponse
	{
		[JsonProperty(PropertyName = "status")]
		public string ResponseStatus;
		[JsonProperty(PropertyName = "msg")]
		public string ResponseMessage;
		[JsonProperty(PropertyName = "aid")]
		public int ApplicationID;
		[JsonProperty(PropertyName = "pid")]
		public int PlatformID;
		[JsonProperty(PropertyName = "app")]
		public AppUpdateInfo appUpdateInfo;


		[JsonIgnore]
		public string responseJSON;

		public class AppUpdateInfo
		{
			[JsonProperty(PropertyName = "url")]
			public string DownloadUrl;
			[JsonProperty(PropertyName = "updt")]
			public bool UpdateFlag;
			[JsonProperty(PropertyName = "type")]
			public int UpdateType;
			[JsonProperty(PropertyName = "ver")]
			public string VersionOfNewApp;

			internal AppUpdateInfo()
			{

			}
		}


		internal SendApplicationInformationResponse()
		{
		}
	}

	public class SendApplicationInformation
	{
		const string METHOD_NAME = "SendApplicationInfo";
		public SendApplicationInformationRequest request = new SendApplicationInformationRequest();
		public SendApplicationInformationResponse response = new SendApplicationInformationResponse();

		public SendApplicationInformation()
		{
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
				this.response = JsonConvert.DeserializeObject<SendApplicationInformationResponse>(responseStr, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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

			if (this.request.ApplicationName != String.Empty && this.request.ApplicationVersion != String.Empty && this.request.OperatingSystem != String.Empty &&
			    this.request.OperatingSystemVersion != String.Empty && this.request.PlatformType != String.Empty)
			{
				isValid = true;
			}
			return isValid;
		}

		private void initResponseObjects()
		{
			this.response.appUpdateInfo = new SendApplicationInformationResponse.AppUpdateInfo();
		}
	}
}

