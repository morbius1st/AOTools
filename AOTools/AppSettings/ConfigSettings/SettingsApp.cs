#region Using directives

using System.Runtime.Serialization;

#endregion

using UtilityLibrary;

// itemname:	SettingsFileApp
// username:	jeffs
// created:		1/3/2018 8:05:02 PM

namespace AOTools.AppSettings.ConfigSettings
{
	// no app settings yet.
	[DataContract(Namespace = Header.NSpace)]
	//	[DataContract]
	public static class SettingsApp
	{
		public static SettingsMgr<SettingsAppBase> SmApp { get; private set; }

		public static SettingsAppBase SmAppSetg { get; private set; }

		public static void Init()
		{
			SmApp = new SettingsMgr<SettingsAppBase>();
			SmAppSetg = SmApp.Settings;
//			SmAppSetg.Heading = new Header(SettingsAppBase.APPSETTINGFILEVERSION);
		}

		public static bool IsAppSetgValid()
		{
			return SmApp != null;
		}
	}

	[DataContract(Namespace = Header.NSpace)]
	//	[DataContract]
	public class SettingsAppBase : SettingsPathFileAppBase
	{
		public override string FileVersion { get; set; } = "1.1";

		[DataMember]
		public int[] AppIs { get; set; } = new[] {10, 20, 30 };

//		[DataMember]
//		public SchemaDictionaryApp SettingsAppData = RsuApp.DefAppSchema;

		
	}
}
