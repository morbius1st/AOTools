#region Using directives

using System.Collections.Generic;
using AOTools.AppSettings.ConfigSettings;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using UtilityLibrary;
using static AOTools.AppSettings.ConfigSettings.SettingsUsr;
using static AOTools.AppSettings.ConfigSettings.SettingsApp;
using static UtilityLibrary.MessageUtilities;

#endregion

// itemname:	UnitStylesCommand
// username:	jeffs
// created:		1/6/2018 3:55:08 PM


namespace AOTools
{
	[Transaction(TransactionMode.Manual)]
	class UnitStylesCommand : IExternalCommand
	{
		private const int testVal = 45;

		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			output = outputLocation.debug;

			SmAppInit();
			SmUsrInit();
			RsMgr.Read();

			RevitSettingsUnitApp a = RsuApp;
			SchemaDictionaryApp b = RsuAppSetg;

			RevitSettingsUnitUsr c = RsuUsr;
			List<SchemaDictionaryUsr> d = RsuUsrSetg;

			SettingsMgr<SettingsAppBase> e = SmApp;
			SettingsAppBase g = SmAppSetg;

			SettingsMgr<SettingsUsrBase> h = SmUsr;
			SettingsUsrBase j = SmUsrSetg;
			List<SchemaDictionaryUsr> k = SmuUsrSetg;

			test111();
			logMsg("");
			test112();

			return Result.Succeeded;
		}

		// test config setting functions
		private void test111()
		{
			logMsgDbLn2("config Settings", "before");
			logMsg("");

			ListConfigSettings();

			logMsg("");
			logMsgDbLn2("config Settings", "reset");
			SmUsr.Reset();
			SmApp.Reset();

			ListConfigSettings();

			logMsg("");
			logMsgDbLn2("config Settings", "save");
			SmUsr.Save();
			SmApp.Save();

			
		}

		// test revit setting functions
		private void test112()
		{
			logMsgDbLn2("revit Settings", "before");
			logMsg("");

			logMsgDbLn2("revit setting initalized", RvtSetgInitalized.ToString());

			ListRevitSettings();

			logMsg("");
			logMsgDbLn2("revit Settings", "reset");
			RsMgr.Reset();
			ListRevitSettings();

			logMsg("");
			logMsgDbLn2("revit Settings", "save");
			RsMgr.Save();
			ListRevitSettings();

		}


		// test transfer settings from user to revit and visa versa
		private void test101()
		{
			logMsgDbLn2("Settings", "before");
			logMsg("");

			logMsgDbLn2("config Settings", "before");
			logMsg("");

			ListConfigSettings();

			logMsgDbLn2("revit Settings", "before");
			logMsg("");

			logMsgDbLn2("revit setting initalized", RvtSetgInitalized.ToString());

			ListRevitSettings();
		}

		// enum test
		private void test22()
		{
			logMsgDbLn2("enum test");
			logMsgDbLn2("unit type", UnitType.UT_Undefined.ToString());

			UnitType u = (UnitType) (int) UnitType.UT_Undefined;

			logMsgDbLn2("unit type", u.ToString());
		}

		// test user settings 
		// read and list current values
		// change the names of the current unit styles
		// save

		private void test21()
		{
			logMsg("");
			logMsgDbLn2("user settings file", "before");
			logMsg("");
			ListUnitDictionary<SchemaDictionaryUsr, SchemaUsrKey>(SmUsrSetg.UnitStylesList, 4);

			ModifyConfigSettings("test21");

			SmUsr.Save();

			logMsgDbLn2("user settings file", "after");
			logMsg("");
			ListUnitDictionary<SchemaDictionaryUsr, SchemaUsrKey>(SmUsrSetg.UnitStylesList, 4);
		}



		// test set to the generic list with user names
		private void test4()
		{
			// first read and display the current settings
			RsMgr.Read();

			logMsg("");
			logMsgDbLn2("revit settings", "before");
			RevitSettingsBase.ListRevitSettingInfo();
			logMsg("");

			SchemaUnitUtil.MakeDefaultUnitStyles();

			if (!RsMgr.Save())
			{
				logMsgDbLn2("revit save settings", "failed");
				return;
			}

			logMsg("");
			logMsgDbLn2("revit settings", "after");
			RevitSettingsBase.ListRevitSettingInfo();
			logMsg("");
		}

		// test update settings
		private void test3()
		{
			// first read and display the current settings
			RsMgr.Read();

			logMsg("");
			logMsgDbLn2("revit settings", "before");
			RevitSettingsBase.ListRevitSettingInfo();
			logMsg("");

			RsMgr.Update();

			logMsg("");
			logMsgDbLn2("revit settings", "after");
			RevitSettingsBase.ListRevitSettingInfo();
			logMsg("");
		}

		// test reset settings to default
		private void test2()
		{
//			RsMgr.InitalizeRevitSettings();

			// first read and display the current settings
			RsMgr.Read();

			logMsg("");
			logMsgDbLn2("revit settings", "before");
			RevitSettingsBase.ListRevitSettingInfo();
			logMsg("");
			
			RsuAppSetg[VERSION_BASIC].Value = (testVal * 10.00).ToString();

			RsuAppSetg[AUTO_RESTORE].Value = (testVal / 10) % 2 == 0;

			int i = 0;

			foreach (SchemaDictionaryUsr unitStyle in RsuUsrSetg)
			{
				unitStyle[VERSION_UNIT].Value = "sub version " + (testVal + i++ + 1);
			}

			RsMgr.Reset();

			logMsg("");
			logMsgDbLn2("settings", "after");
			RevitSettingsBase.ListRevitSettingInfo();
			logMsg("");
		}

		// this is just a basic read, modify, save, re-read test
		// of revit settings
		private void test1()
		{
			if (!RsMgr.Read())
			{
				TaskDialog.Show("AO Tools", "Could not read Revit Settings or " + nl + "is not Initalized");
				return;
			}

			logMsg("");
			logMsgDbLn2("revit saved settings", "before");
			RevitSettingsBase.ListRevitSettingInfo(4);
			logMsg("");

			ModifyRevitSettings("test1");

			logMsg("");
			if (!RsMgr.Save())
			{
				logMsgDbLn2("revit save settings", "failed");
				return;
			}

			RsMgr.Read();

			logMsgDbLn2("revit saved settings", "after");
			RevitSettingsBase.ListRevitSettingInfo(4);
		}

		// simple test - read and list revit settings
		private void test0()
		{
			if (!RsMgr.Read())
			{
				TaskDialog.Show("AO Tools", "Could not read Revit Settings or " + nl + "is not Initalized");
				return;
			}

			logMsg("");
			logMsgDbLn2("revit saved settings");
			RevitSettingsBase.ListRevitSettingInfo();
		}

		private void ModifyRevitSettings(string msg, int count = 3)
		{
			RsuAppSetg[VERSION_BASIC].Value = (testVal * 11.00).ToString();

			RsuAppSetg[AUTO_RESTORE].Value = false;

			for (int i = 0; i < count; i++)
			{
				RsuUsrSetg[i][VERSION_UNIT].Value = "(" + msg + ") sub version " + (testVal + i + 1);
				RsuUsrSetg[i][STYLE_NAME].Value = "(" + msg + ") style name " + (testVal + i + 1);
				RsuUsrSetg[i][STYLE_DESC].Value = "(" + msg + ") style description " + (testVal + i + 1);
			}

		}

		private void ModifyConfigSettings(string msg, int count = 3)
		{
			SmUsrSetg.FormMeasurePointsLocation = new System.Drawing.Point(100 + testVal, 100 + testVal);
			SmUsrSetg.MeasurePointsShowWorkplane = false;

			for (int i = 0; i < count; i++)
			{
				SmuUsrSetg[i][VERSION_UNIT].Value = "(" + msg + ") sub version " + (testVal + i + 1);
				SmuUsrSetg[i][STYLE_NAME].Value = "(" + msg + ") Style Name " + i;
				SmuUsrSetg[i][STYLE_DESC].Value = "(" + msg + ") style description " + (testVal + i + 1);
			}
		}

	}

}