using System.Collections.Generic;

using Autodesk.Revit.DB;


namespace AOTools.AppSettings.SchemaSettings
{
	public static class SchemaUnitUtil
	{
		// the number of bogus entries to make
		private const int DEFAULT_COUNT_USER = 3;
		private const int DEFAULT_COUNT_STYLES = 2;
		private static readonly List<string> UserNames 
			= new List<string>() {"alpha", "beta", "delta", "gamma", "pi", "nu", "zeta", "iota"};

		public static void MakeDefaultUnitStyles()
		{
			RsuUsr.Clear();

			for (int i = 0; i < DEFAULT_COUNT_USER; i++)
			{
				for (int j = 0; j < DEFAULT_COUNT_STYLES; j++)
				{
					SchemaDictionaryUsr unitStyle = DefaultSchemaUsr((i * 10) + j);
					unitStyle[SchemaUsrKey.USER_NAME].Value = UserNames[i];

					RsuUsrSetg.Add(unitStyle);
				}
			}
		}

		public static List<SchemaDictionaryUsr> DefaultSchemaListUsr(int quantity)
		{
			List<SchemaDictionaryUsr> SettingList = new List<SchemaDictionaryUsr>(quantity);

			for (int i = 0; i < quantity; i++)
			{
				SettingList.Add(DefaultSchemaUsr(i));
			}

			return SettingList;
		}

		public static SchemaDictionaryUsr DefaultSchemaUsr(int itemNumber)
		{
			SchemaDictionaryUsr def = SchemaUnitUsr.SchemaUnitUsrDefault.Clone();

			def[SchemaUsrKey.STYLE_NAME].Value =
				string.Format(SchemaUnitUsr.SchemaUnitUsrDefault[SchemaUsrKey.STYLE_NAME].Value, itemNumber);

			def[SchemaUsrKey.UNIT_SYSTEM].Value = (int) UnitSystem.Imperial;
			def[SchemaUsrKey.UNIT_TYPE].Value = (int) UnitType.UT_Length;
			def[SchemaUsrKey.ACCURACY].Value = (1.0 / 12.0) / 16.0;
			def[SchemaUsrKey.DUT].Value = (int) DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES;
			def[SchemaUsrKey.UST].Value = (int) UnitSymbolType.UST_NONE;

			return def;
		}


		public static SchemaDictionaryApp GetSchemaUnitAppDefault()
		{
			return SchemaUnitApp.SchemaUnitAppDefault.Clone();
		}

		// the guid for each sub-schema and the 
		// field that holds the sub-schema - both must match
		// the guid here is missing the last (2) digits.
		// fill in for each sub-schema 
		// unit type is number is a filler
		public static readonly SchemaFieldUnit SubSchemaFieldInfo =
			new SchemaFieldUnit(SchemaAppKey.UNDEFINED, "LocalUnitStyle{0:D2}",
				"subschema for the local unit style",
				null, RevitUnitType.UT_UNDEFINED, "B2788BC0-381E-4F4F-BE0B-93A93B9470{0:x2}");
	}
}