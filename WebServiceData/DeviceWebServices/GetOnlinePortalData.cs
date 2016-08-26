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
	public class GetOnlinePortalDataRequest
	{
		[JsonProperty(PropertyName = "email")]
		public string Email;

		[JsonIgnore]
		public string requestJSON;


		internal GetOnlinePortalDataRequest()
		{
		}
	}


	public class GetOnlinePortalDataResponse
	{
		[JsonProperty(PropertyName = "status")]
		public string ResponseStatus;
		[JsonProperty(PropertyName = "msg")]
		public string ResponseMessage;
		[JsonProperty(PropertyName = "url")]
		public string ClientURL;

		[JsonIgnore]
		public string responseJSON;

		internal GetOnlinePortalDataResponse()
		{
		}
	}

	public class GetOnlinePortalData
	{
		const string METHOD_NAME = "GetOnlinePortal";
		public GetOnlinePortalDataRequest request = new GetOnlinePortalDataRequest();
		public GetOnlinePortalDataResponse response = new GetOnlinePortalDataResponse();

		public GetOnlinePortalData()
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
				this.response = JsonConvert.DeserializeObject<GetOnlinePortalDataResponse>(responseStr, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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

			if (this.request.Email != String.Empty )
			{
				isValid = true;
			}
			return isValid;
		}
	}
}

