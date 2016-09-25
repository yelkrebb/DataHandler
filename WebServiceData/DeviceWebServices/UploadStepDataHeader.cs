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
	public class UploadStepDataHeaderRequest
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
		[JsonProperty(PropertyName = "syncid")]
		public int SynchronizationID;
		[JsonProperty(PropertyName = "data")]
		public StepDataHeaderData stepDataHeaderData;

		public class StepDataHeaderData
		{
			[JsonProperty(PropertyName = "dtid")]
			public int DataTransmissionID;
			[JsonProperty(PropertyName = "hdate")]
			public string DeviceHeaderDate;
			[JsonProperty(PropertyName = "hdat")]
			public long DeviceHeaderData;
			[JsonProperty(PropertyName = "rdate")]
			public string CommandReadDate;
			[JsonProperty(PropertyName = "rshr")]
			public int CommandReadHour;
			[JsonProperty(PropertyName = "rehr")]
			public int CommandReadEnd;

			internal StepDataHeaderData()
			{

			}
		}

		[JsonIgnore]
		public string requestJSON;


	}

	public class UploadStepDataHeaderResponse
	{
		[JsonProperty(PropertyName = "status")]
		public string ResponseStatus;
		[JsonProperty(PropertyName = "msg")]
		public string ResponseMessage;

		[JsonIgnore]
		public string responseJSON;


		internal UploadStepDataHeaderResponse()
		{
		}
	}

	public class UploadStepDataHeader
	{
		const string METHOD_NAME = "UploadStepDataHeader";
		public UploadStepDataHeaderRequest request = new UploadStepDataHeaderRequest();
		public UploadStepDataHeaderResponse response = new UploadStepDataHeaderResponse();

		public UploadStepDataHeader()
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
				this.response = JsonConvert.DeserializeObject<UploadStepDataHeaderResponse>(responseStr, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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

			if (this.request.DeviceModel != 0 && this.request.DeviceSerial != 0 && this.request.MemberDeviceID != 0 && this.request.ApplicationID != 0
			   && this.request.PlatformID != 0 && this.request.SynchronizationID != 0)
			{
				isValid = true;
			}
			return isValid;
		}

		private void initResponseObjects()
		{
			this.request.stepDataHeaderData = new UploadStepDataHeaderRequest.StepDataHeaderData();
		}
	}
}

