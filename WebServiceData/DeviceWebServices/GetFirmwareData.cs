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

	public class GetFirmwareDataResponse
	{
		[JsonProperty(PropertyName = "status")]
		public string ResponseStatus;
		[JsonProperty(PropertyName = "msg")]
		public string ResponseMessage;
		[JsonProperty(PropertyName = "csum")]
		public int CheckSum;
		[JsonProperty(PropertyName = "data")]
		public string FirmwareData;

		[JsonIgnore]
		public string responseJSON;

		internal GetFirmwareDataResponse()
		{
		}
	}

	public class GetFirmwareData
	{
		const string METHOD_NAME = "GetFirmware?mfid=";
		public int mfid;
		public GetOnlinePortalDataResponse response = new GetOnlinePortalDataResponse();

		public GetFirmwareData()
		{
		}

		public async Task<WebServiceRequestStatus> PostRequest()
		{
			WebServiceRequestStatus status = WebServiceRequestStatus.SUCCESS;

			if (this.isRequestDataValid())
			{
				WebService ws = new WebService();
				string responseStr = await ws.PostData(Utils.GetDeviceServicesURL() + METHOD_NAME);
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

			if (this.mfid > 0)
			{
				isValid = true;
			}
			return isValid;
		}
	}
}

