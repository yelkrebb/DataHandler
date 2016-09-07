using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Motion.Mobile.Utilities;
using Motion.Core.WSHandler;
using Motion.Core.Data.WebServiceData;
using Motion.Core.Data.BleData.Trio.SettingsData;
using Motion.Core.Data.BleData.Trio;

namespace Motion.Core.Data.WebServiceData.DeviceWebServices
{
	public class ActivateDeviceWithMemberRequest
	{
		[JsonProperty(PropertyName = "mdl")]
		public int TrackerModel;
		[JsonProperty(PropertyName = "serno")]
		public long TrackerSerialNumber;
		[JsonProperty(PropertyName = "btadd")]
		public string TrackerBtAdd;
		[JsonProperty(PropertyName = "aid")]
		public int ApplicationID;
		[JsonProperty(PropertyName = "drmid")]
		public int MemberDermID;
		[JsonProperty(PropertyName = "fwver")]
		public float TrackerFirmwareVersion;
		[JsonIgnore]
		public string requestJSON;

		internal ActivateDeviceWithMemberRequest()
		{

		}
	}

	public class ActivateDeviceWithMemberResponse
	{
		[JsonProperty(PropertyName = "status")]
		public string ResponseStatus;
		[JsonProperty(PropertyName = "msg")]
		public string ResponseMessage;
		[JsonProperty(PropertyName = "mid")]
		public int MemberID;
		[JsonProperty(PropertyName = "mdid")]
		public int MemberDeviceID;
		[JsonProperty(PropertyName = "did")]
		public int DeviceID;
		[JsonProperty(PropertyName = "syncd")]
		public long LastSyncSettingDateTime;
		[JsonProperty(PropertyName = "lmno")]
		public int LastMinuteSlotNumber;
		[JsonProperty(PropertyName = "lmho")]
		public int LastHourSlotNumber;
		[JsonProperty(PropertyName = "lsdt")]
		public string LastStepDate;
		[JsonProperty(PropertyName = "svrdtm")]
		public string ServerDateTime;
		[JsonProperty(PropertyName = "fwver")]
		public float TrackerFirmwareVersion;
		[JsonProperty(PropertyName = "tdif")]
		public int AllowableTimeDifference;
		[JsonProperty(PropertyName = "uflag")]
		public long UpdateFlag;
		[JsonProperty(PropertyName = "clrx")]
		public bool ClearTrackerDataFlag;
		[JsonProperty(PropertyName = "tsens")]
		public int TrackerSensitivity;//Tracker Sensitivity. Possible values are 0-normal, 1-medium, 2-high
		[JsonProperty(PropertyName = "dpstat")]
		public int DevicePairingStatus;
		[JsonProperty(PropertyName = "tmf")]
		public bool DeviceTimeFormat;
		[JsonProperty(PropertyName = "snans")]
		public List<int> SurveyNotAnswered;
		[JsonProperty(PropertyName = "url")]
		public string ClientUrl;//Client URL
		[JsonProperty(PropertyName = "sdata")]
		public List<string> SignatureUploadedDates;
		[JsonProperty(PropertyName = "szdata")]
		public LastSeizureDataUploadInfo lastSeizureDataUploadInfo;
		[JsonProperty(PropertyName = "uset")]
		public UserSettings userSettings;
		[JsonProperty(PropertyName = "cset")]
		public ExerciseSettings exerciseSettings;
		[JsonProperty(PropertyName = "eset")]
		public CompanySettings companySettings;
		[JsonProperty(PropertyName = "sset")]
		public SignatureSettings signatureSettings;
		[JsonProperty(PropertyName = "app")]
		public AppUpdateInfo appUpdateInfo;
		[JsonProperty(PropertyName = "sz")]
		public SeizureSettings seizureSettings;
		[JsonProperty(PropertyName = "sflow")]
		public List<int> ScreenFlow;
		[JsonProperty(PropertyName = "fw")]
		public DeviceFirmwareUpdateInfo deviceFwUpdateInfo;
		[JsonProperty(PropertyName = "syncid")]
		public int SynchronizationID;
		[JsonProperty(PropertyName = "amode")]
		public int AdvertiseMode;
		[JsonProperty(PropertyName = "lfdtm")]
		public string LatestFallSyncDateTime;
		[JsonProperty(PropertyName = "senset")]
		public SensitivitySettings sensitivitySettings;

		[JsonIgnore]
		public string responseJSON;

		public class LastSeizureDataUploadInfo
		{
			[JsonProperty(PropertyName = "date")]
			public string SeizureDate;
			[JsonProperty(PropertyName = "blkno")]
			public int BlockNumber;

			internal LastSeizureDataUploadInfo()
			{
			}
		}

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

		public class DeviceFirmwareUpdateInfo
		{
			[JsonProperty(PropertyName = "mfid")]
			public int ModelFirmwareID;
			[JsonProperty(PropertyName = "ver")]
			public float VersionOfNewFirmware;

			internal DeviceFirmwareUpdateInfo()
			{

			}
		}

		internal ActivateDeviceWithMemberResponse()
		{

		}
	}

	public class ActivateDeviceWithMember
	{
		const string METHOD_NAME = "ActivateDeviceWithMember";

		public ActivateDeviceWithMemberRequest request = new ActivateDeviceWithMemberRequest();
		public ActivateDeviceWithMemberResponse response = new ActivateDeviceWithMemberResponse();

		private TrioDeviceInformation _devInfo;

		public ActivateDeviceWithMember(TrioDeviceInformation devInfo)
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
				this.response = JsonConvert.DeserializeObject<ActivateDeviceWithMemberResponse>(responseStr, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

				if(string.Equals("Member & Device is already active.",this.response.ResponseMessage))
				   status = WebServiceRequestStatus.ERROR_REQUEST_DATA_INVALID;
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

			if (this.request.TrackerModel != 0 && this.request.TrackerSerialNumber != 0 && this.request.TrackerFirmwareVersion >= 0.0f && this.request.ApplicationID != 0)
			{
				isValid = true;
			}
			return isValid;
		}

		private void initResponseObjects()
		{
			this.response.lastSeizureDataUploadInfo = new ActivateDeviceWithMemberResponse.LastSeizureDataUploadInfo();
			this.response.userSettings = new UserSettings(this._devInfo);
			this.response.companySettings = new CompanySettings(this._devInfo);
			this.response.exerciseSettings = new ExerciseSettings(this._devInfo);
			this.response.signatureSettings = new SignatureSettings(this._devInfo);
			this.response.seizureSettings = new SeizureSettings(this._devInfo);
			this.response.sensitivitySettings = new SensitivitySettings(this._devInfo);
			this.response.appUpdateInfo = new ActivateDeviceWithMemberResponse.AppUpdateInfo();
			this.response.deviceFwUpdateInfo = new ActivateDeviceWithMemberResponse.DeviceFirmwareUpdateInfo();

		}
	}
}

