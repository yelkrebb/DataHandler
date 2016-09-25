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
	public class UnpairMemberDeviceRequest
	{
		[JsonProperty(PropertyName = "mid")]
		public long MemberID;
		[JsonProperty(PropertyName = "mdid")]
		public long MemberDeviceID;
		[JsonProperty(PropertyName = "did")]
		public long DeviceID;

		[JsonIgnore]
		public string requestJSON;

		internal UnpairMemberDeviceRequest()
		{
		}
	}

	public class UnpairMemberDeviceResponse
	{
		[JsonProperty(PropertyName = "status")]
		public string ResponseStatus;
		[JsonProperty(PropertyName = "msg")]
		public string ResponseMessage;

		[JsonIgnore]
		public string responseJSON;

		internal UnpairMemberDeviceResponse()
		{

		}
	}

	public class UnpairMemberDevice
	{
		const string METHOD_NAME = "UnpairMemberDevice";

		public UnpairMemberDeviceRequest request = new UnpairMemberDeviceRequest();
		public UnpairMemberDeviceResponse response = new UnpairMemberDeviceResponse();

		public UnpairMemberDevice()
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
				this.response = JsonConvert.DeserializeObject<UnpairMemberDeviceResponse>(responseStr, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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

			if (this.request.MemberID != 0 && this.request.MemberDeviceID != 0 && this.request.DeviceID != 0)
			{
				isValid = true;
			}
			return isValid;
		}
	}
}

