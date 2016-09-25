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
	public class NotifySettingsUpdateRequest
	{
		[JsonProperty(PropertyName = "mid")]
		public int MemberID;
		[JsonProperty(PropertyName = "mdid")]
		public long MemberDeviceID;
		[JsonProperty(PropertyName = "syncd")]
		public long LastSyncSettingDateTime;
		[JsonProperty(PropertyName = "serno")]
		public long TrackerSerialNumber;
		[JsonProperty(PropertyName = "btadd")]
		public string TrackerBtAdd;
		[JsonProperty(PropertyName = "mdl")]
		public int TrackerModel;
		[JsonProperty(PropertyName = "dpstat")]
		public int DevicePairingStatus;
		[JsonProperty(PropertyName = "uflag")]
		public int UpdateFlag;
		[JsonProperty(PropertyName = "stype")]
		public int SettingType;
		[JsonProperty(PropertyName = "ddtm")]
		public DateTime? DeviceDateTime = null;
		[JsonProperty(PropertyName = "balvl")]
		public int BatteryLevel;
		[JsonProperty(PropertyName = "aid")]
		public int ApplicationID;
		[JsonProperty(PropertyName = "pid")]
		public int PlatformID;
		[JsonIgnore]
		public string requestJSON;

		internal NotifySettingsUpdateRequest()
		{
			
		}	
	}

	public class NotifySettingsUpdateResponse
	{
		[JsonProperty(PropertyName = "status")]
		public string ResponseStatus;
		[JsonProperty(PropertyName = "msg")]
		public string ResponseMessage;

		[JsonIgnore]
		public string responseJSON;

		internal NotifySettingsUpdateResponse()
		{

		}
	}


	public class NotifySettingsUpdate
	{
		const string METHOD_NAME = "NotifySettingsUpdate";

		public NotifySettingsUpdateRequest request = new NotifySettingsUpdateRequest();
		public NotifySettingsUpdateResponse response = new NotifySettingsUpdateResponse();

		public NotifySettingsUpdate()
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
				this.response = JsonConvert.DeserializeObject<NotifySettingsUpdateResponse>(responseStr, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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

			if (this.request.TrackerModel != 0 && this.request.TrackerSerialNumber != 0 && this.request.PlatformID != 0 && this.request.ApplicationID != 0 
			    && this.request.MemberID !=0 && this.request.MemberDeviceID !=0 && this.request.LastSyncSettingDateTime !=0 && this.request.UpdateFlag !=0 
			    && this.request.SettingType !=0 && this.request.DeviceDateTime.HasValue)
			{
				isValid = true;
			}
			return isValid;
		}
	}
}

