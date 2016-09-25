using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Motion.Mobile.Utilities;
using Motion.Core.WSHandler;
using Motion.Core.Data.WebServiceData;
using Motion.Core.Data.BleData.Trio.StepsData;

namespace Motion.Core.Data.WebServiceData.DeviceWebServices
{
	public class UploadStepsDataRequest
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
		[JsonProperty(PropertyName = "data")]
		public List<StepsMinuteData> minuteData;

		[JsonIgnore]
		public List<StepsDailyData> StepsData;
		[JsonIgnore]
		public string requestJSON;

		internal UploadStepsDataRequest()
		{
		}
	}

	public class UploadStepsDataResponse
	{
		[JsonProperty(PropertyName = "status")]
		public string ResponseStatus;
		[JsonProperty(PropertyName = "msg")]
		public string ResponseMessage;
		[JsonProperty(PropertyName = "syncd")]
		public long LastSyncDataDateTime;
		[JsonProperty(PropertyName = "dtid")]
		public int DataTransmissionID;
		[JsonProperty(PropertyName = "lmno")]
		public int LastMinuteSlotNo;
		[JsonProperty(PropertyName = "lhno")]
		public int LastHourNo;
		[JsonProperty(PropertyName = "lsdt")]
		public string LastStepDate;

		[JsonIgnore]
		public string responseJSON;

		internal UploadStepsDataResponse()
		{

		}
	}

	public class UploadStepsData
	{
		const string METHOD_NAME = "UploadData";

		public UploadStepsDataRequest request = new UploadStepsDataRequest();
		public UploadStepsDataResponse response = new UploadStepsDataResponse();
		public UploadStepsData()
		{
			this.initRequestObjects();
		}

		public async Task<WebServiceRequestStatus> PostRequest()
		{
			WebServiceRequestStatus status = WebServiceRequestStatus.SUCCESS;
			this.preparePerMinuteData();

			if (this.isRequestDataValid())
			{
				WebService ws = new WebService();
				string jsonString = JsonConvert.SerializeObject(request);
				System.Diagnostics.Debug.WriteLine(jsonString);
				this.request.requestJSON = jsonString;
				string responseStr = await ws.PostData(Utils.GetDeviceServicesURL() + METHOD_NAME, jsonString);
				System.Diagnostics.Debug.WriteLine(responseStr);
				this.response.responseJSON = responseStr;
				this.response = JsonConvert.DeserializeObject<UploadStepsDataResponse>(responseStr, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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
				&& this.request.MemberID != 0 && this.request.MemberDeviceID != 0 && this.request.LastSyncSettingDateTime != 0 && this.request.DeviceDateTime.HasValue)
			{
				isValid = true;
			}
			return isValid;
		}

		private void initRequestObjects()
		{
			this.request.StepsData = new List<StepsDailyData>();

		}

		private void preparePerMinuteData()
		{
			this.request.minuteData = new List<StepsMinuteData>();
			foreach (StepsDailyData daily in this.request.StepsData)
			{
				foreach (StepsHourlyData hourly in daily.hourlyData)
				{
					foreach (StepsMinuteData minute in hourly.minuteData)
					{
						this.request.minuteData.Add(minute);
					}
				}
			}
		}
	}
}

