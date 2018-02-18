using System;
using System.Collections.Generic;
using AOTools.AppSettings.SchemaSettings;
using static UtilityLibrary.MessageUtilities;

using static AOTools.AppSettings.ConfigSettings.SettingsApp;
using static AOTools.AppSettings.ConfigSettings.SettingsUsr;


namespace AOTools.AppSettings.SettingUtil
{
	public static class SettingsListings
	{
		public static void ListRevitSettings()
		{
			logMsgDbLn2("revit app settings");
			ListRevitAppSettings();
			logMsg("");
			logMsgDbLn2("revit usr settings");
			ListRevitUsrSettings();
			
			logMsg("");
		}

		public static void ListConfigSettings()
		{
			logMsgDbLn2("config app settings");
			ListConfigAppSettings();
			logMsg("");
			logMsgDbLn2("config user settings");
			ListConfigUsrSettings();
			logMsg("");
		}


		public static void ListUnitDictionary<TU, T>(List<TU> u, int count = -1) where TU : SchemaDictionaryBase<T>
		{
			int j = 0;
			foreach (TU sd in u)
			{
				logMsgDbLn2("unit style #", j++.ToString());
				ListFieldInfo(sd, count);
				logMsg("");
			}
		}

		public static void ListFieldInfo<T>(SchemaDictionaryBase<T> fieldList, int count = -1)
		{
			int i = 0;

			foreach (KeyValuePair<T, SchemaFieldUnit> kvp in fieldList)
			{
				if (i == count) return;

				logMsgDbLn2("field #" + i++,
					FormatFieldInfo(kvp.Key as Enum, kvp.Value));
			}
		}

		private static string FormatFieldInfo(Enum key, SchemaFieldUnit fi)
		{
			int len = 28;
			string keyDesc = key?.ToString() ?? "undefined";
			string valueDesc = fi.Value.ToString().PadRight(len).Substring(0, len);
			return $"key| {keyDesc,-20}  name| {fi.Name,-20} value| {valueDesc,-30} unit type| {fi.UnitType}";
		}

		public static void ListRevitUsrSettings()
		{
			ListUnitDictionary<SchemaDictionaryUsr, SchemaUsrKey>(RsuUsrSetg, 4);
		}

		public static void ListRevitAppSettings()
		{
			logMsgDbLn2("data in dictionary");
			foreach (KeyValuePair<SchemaAppKey, SchemaFieldUnit> kvp in RsuAppSetg)
			{
				logMsgDbLn2("data", "key| " + kvp.Key + "  name| " + kvp.Value.Name + "  value| " + kvp.Value.Value);
			}

			logMsg("");
		}

		public static void ListConfigUsrSettings()
		{
			logMsgDbLn2("header");
			logMsgDbLn2("assm version", SmUsrSetg.Heading.AssemblyVersion);
			logMsgDbLn2("save date/time", SmUsrSetg.Heading.SaveDateTime);
			logMsgDbLn2("setting file version", SmUsrSetg.Heading.SettingFileVersion);
			logMsgDbLn2("setting system version", SmUsrSetg.Heading.SettingSystemVersion);

			ListUnitDictionary<SchemaDictionaryUsr, SchemaUsrKey>(SmuUsrSetg, 4);
		}


		public static void ListConfigAppSettings()
		{
			logMsgDbLn2("header");
			logMsgDbLn2("assm version", SmAppSetg.Heading.AssemblyVersion);
			logMsgDbLn2("save date/time", SmAppSetg.Heading.SaveDateTime);
			logMsgDbLn2("setting file version", SmAppSetg.Heading.SettingFileVersion);
			logMsgDbLn2("setting system version", SmAppSetg.Heading.SettingSystemVersion);


			logMsgDbLn2("app inits", SmAppSetg.AppIs[0].ToString()
				+ "  " + SmAppSetg.AppIs[1].ToString() + "  " + SmAppSetg.AppIs[2].ToString());


			logMsgDbLn2("data in dictionary");
			foreach (KeyValuePair<SchemaAppKey, SchemaFieldUnit> kvp in SmAppSetg.SettingsAppData)
			{
				logMsgDbLn2("data", "key| " + kvp.Key + "  name| " + kvp.Value.Name + "  value| " + kvp.Value.Value);
			}

			logMsg("");
		}



	}
}