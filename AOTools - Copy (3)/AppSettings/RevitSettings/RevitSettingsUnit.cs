﻿#region Using directives

using System;
using System.Collections.Generic;
using AOTools.AppSettings.Schema;
using AOTools.Settings;
using Autodesk.Revit.DB;

#endregion

// itemname:	RevitUserSettingsMgr
// username:	jeffs
// created:		1/27/2018 10:26:23 AM


namespace AOTools.AppSettings.RevitSettings
{
	public class RevitSettingsUnitApp : SchemaUnitApp
	{
		// scheme description info
		public string SchemaName => SCHEMA_NAME;
		public string SchemaDesc => SCHEMA_DESC;
		public Guid SchemaGuid => Schemaguid;

		public static RevitSettingsUnitApp RsuApp = new RevitSettingsUnitApp();
		public SchemaDictionaryApp RsuAppSetg { get; private set; }

		public SchemaDictionaryApp DefAppSchema => SchemaUnitAppDefault;

		public bool Initalized { get; } = false;

		private RevitSettingsUnitApp()
		{
			Initalize();

			Initalized = true;

		}

		public void Initalize()
		{
			RsuAppSetg = GetSchemaUnitAppDefault();
		}
	}

	// defined here as this file will be revit specific
	public enum RevitUnitType
	{
		UT_UNDEFINED = (int) UnitType.UT_Undefined,
		UT_NUMBER = (int) UnitType.UT_Number
	}

	public class RevitSettingsUnitUsr : SchemaUnitUsr
	{
		public string UnitSchemaName => SCHEMA_NAME;
		public string SchemaDesc => SCHEMA_DESC;

		public static RevitSettingsUnitUsr RsuUsr = new RevitSettingsUnitUsr();
		public List<SchemaDictionaryUsr> RsuUsrSetg { get; private set; }

		private RevitSettingsUnitUsr()
		{
			if (RevitSettingsUnitApp.RsuApp.Initalized == false)
			{
				throw new Exception("Basic Manager Must be Initalized First");
			}

			Initalize();
		}

		public void Initalize()
		{
			RsuUsrSetg = SchemaUnitUtil.CreateDefaultSchemaList(RevitSettingsUnitApp.RsuApp.RsuAppSetg[SchemaAppKey.COUNT].Value);
		}

		public void Resize(int quantity)
		{
			RsuUsrSetg = SchemaUnitUtil.CreateDefaultSchemaList(quantity);
		}
	}
}