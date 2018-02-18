#region Using directives

using System.Runtime.Serialization;
using Point = System.Drawing.Point;

using UtilityLibrary;

#endregion

// itemname:	SettingsUsr
// username:	jeffs
// created:		1/3/2018 8:04:40 PM

namespace AOTools.AppSettings.ConfigSettings
{
	[DataContract(Namespace = Header.NSpace)]
//	[DataContract]
	public static class SettingsUsr
	{
		public static SettingsMgr<SettingsUsrBase> SmUsr { get; private set; }

		public static SettingsUsrBase SmUsrSetg { get; private set; }

		public static void Init()
		{
			SmUsr = new SettingsMgr<SettingsUsrBase>();
			SmUsrSetg = SmUsr.Settings;
//			SmUsrSetg.Heading = new Header(SettingsUsrBase.USERSETTINGFILEVERSION);
		}
		public static bool IsValid()
		{
			return SmUsr != null;
		}
	}

	// sample Settings User
	[DataContract(Namespace = Header.NSpace)]
	public class SettingsUsrBase : SettingsPathFileUserBase
	{
		public override string FileVersion { get; set; } = "1.1";

		[DataMember]
		public Point FormMeasurePointsLocation = new Point(0, 0);
		[DataMember]
		public bool MeasurePointsShowWorkplane = false;

	}

}
