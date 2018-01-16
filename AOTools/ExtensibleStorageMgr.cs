﻿#region Using directives
using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using EnvDTE;
using static AOTools.Util;
using static AOTools.AppRibbon;
using static AOTools.SBasicKey;
using static AOTools.SUnitKey;
using static AOTools.BasicSchema;
using static AOTools.UnitSchema;


using static UtilityLibrary.MessageUtilities;

#endregion

// itemname:	ExtensibleStorageMgr
// username:	jeffs
// created:		1/7/2018 3:37:43 PM


namespace AOTools
{
	public class ExtensibleStorageMgr
	{

		public static bool initalized = false;

		public static Dictionary<SBasicKey, FieldInfo> SchemaFields;
		public static Dictionary<SUnitKey, FieldInfo>[] UnitSchemaFields;

		// ******************************
		// general routines
		// ******************************
		private static void Init()
		{
			if (initalized) return;

			initalized = true;

			SchemaFields = new Dictionary<SBasicKey, FieldInfo>(_schemaFields);

			UnitSchemaFields = GetUnitSchemaFields(_schemaFields[COUNT].Value);
		}

		public static void DeleteCurrentSchema()
		{
			Schema schema = Schema.Lookup(SchemaGUID);
			if (schema != null)
			{

				using (Transaction t = new Transaction(Doc, "Delete old schema"))
				{
					t.Start();
					Element elem = GetProjectBasepoint();
					logMsgDbLn2("delete current schema", elem.DeleteEntity(schema) ? "worked" : "failed");
					Schema.EraseSchemaAndAllEntities(schema, false);
					schema.Dispose();
					t.Commit();

				}
			}
		}

		// ******************************
		// save settings routines
		// ******************************

		// save the basic settings to the revit project base point
		// this saves both the basic and the unit styles
		public static bool SaveRevitSettings()
		{
			if (!initalized) { return false; }
			Element elem = GetProjectBasepoint();

			SchemaBuilder sbld = CreateSchema(SCHEMA_NAME, SCHEMA_DESC, SchemaGUID);
//			SchemaBuilder sbld = new SchemaBuilder(SchemaGUID);
//
//			sbld.SetReadAccessLevel(AccessLevel.Public);
//			sbld.SetWriteAccessLevel(AccessLevel.Vendor);
//			sbld.SetVendorId(Util.GetVendorId());
//			sbld.SetSchemaName(SCHEMA_NAME);
//			sbld.SetDocumentation(SCHEMA_DESC);

			// this makes the basic setting fields
			MakeFields(sbld, SchemaFields);

			Dictionary<string, string> subSchemaFields = 
				new Dictionary<string, string>(SchemaFields[COUNT].Value);

			CreateUnitSchemaFields(sbld, subSchemaFields);

			Schema schema = sbld.Finish();

			Entity entity = new Entity(schema);

			// set the basic fields
			SaveFieldValues(entity, schema, SchemaFields);

			SaveUnitSettings(entity, schema, subSchemaFields);

			using (Transaction t = new Transaction(Doc, "Unit Style Settings"))
			{
				t.Start();
				elem.SetEntity(entity);
				t.Commit();
			}

			schema.Dispose();

			return true;
		}

		private static SchemaBuilder CreateSchema(string name, string description, Guid guid)
		{
			SchemaBuilder sbld = new SchemaBuilder(guid);

			sbld.SetReadAccessLevel(AccessLevel.Public);
			sbld.SetWriteAccessLevel(AccessLevel.Vendor);
			sbld.SetVendorId(Util.GetVendorId());
			sbld.SetSchemaName(name);
			sbld.SetDocumentation(description);

			return sbld;
		}

		private static void CreateUnitSchemaFields(SchemaBuilder sbld, 
			Dictionary<string, string> subSchemaFields)
		{
			// temp - test making ) unit subschemas
			for (int i = 0; i < SchemaFields[COUNT].Value; i++)
			{
				string guid = String.Format(_subSchemaFieldInfo.Guid, i);   // + suffix;
				string fieldName =
					String.Format(_subSchemaFieldInfo.Name, i);
				FieldBuilder fbld =
					sbld.AddSimpleField(fieldName, typeof(Entity));
				fbld.SetDocumentation(_subSchemaFieldInfo.Desc);
				fbld.SetSubSchemaGUID(new Guid(guid));

				subSchemaFields.Add(fieldName, guid);
			}
		}

		private static void SaveUnitSettings(Entity entity, Schema schema, 
			Dictionary<string, string> subSchemaFields)
		{
			int j = 0;

			foreach (KeyValuePair<string, string> kvp in subSchemaFields)
			{
				Field f = schema.GetField(kvp.Key);
				Entity subEntity =
					MakeUnitSchema(kvp.Value, UnitSchemaFields[j++]);
				entity.Set(f, subEntity);
			}
		}


		/// <summary>
		/// general routine to make one schema field for<para/>
		/// each item in a field info list
		/// </summary>
		/// <param name="sbld"></param>
		/// <param name="fieldList"></param>
		private static void MakeFields<T>(SchemaBuilder sbld, 
			Dictionary<T, FieldInfo> fieldList)
		{
			foreach (KeyValuePair<T, FieldInfo> kvp in fieldList)
			{
				MakeField(sbld, kvp.Value);
			}
		}

		/// <summary>
		/// general routine to make a single schema field
		/// </summary>
		/// <param name="sbld"></param>
		/// <param name="fieldInfo"></param>
		private static void MakeField(SchemaBuilder sbld, FieldInfo fieldInfo)
		{
			FieldBuilder fbld = sbld.AddSimpleField(
					fieldInfo.Name, fieldInfo.Value.GetType());

			fbld.SetDocumentation(fieldInfo.Desc);

			if (fieldInfo.UnitType != UnitType.UT_Undefined)
			{
				fbld.SetUnitType(fieldInfo.UnitType);
			}
		}

		/// <summary>
		/// routine to make a unit schema and its fields
		/// </summary>
		/// <param name="guid"></param>
		/// <param name="unitSchemaFields"></param>
		/// <returns>SchemaBuilder</returns>
		private static Entity MakeUnitSchema(string guid, 
			Dictionary<SUnitKey, FieldInfo> unitSchemaFields)
		{
			SchemaBuilder sbld = new SchemaBuilder(new Guid(guid));

			sbld.SetReadAccessLevel(AccessLevel.Public);
			sbld.SetWriteAccessLevel(AccessLevel.Vendor);
			sbld.SetVendorId(Util.GetVendorId());
			sbld.SetSchemaName(UNIT_SCHEMA_NAME);
			sbld.SetDocumentation(UNIT_SCHEMA_DESC);

			MakeFields(sbld, unitSchemaFields);

			Schema schema = sbld.Finish();

			Entity entity = new Entity(schema);

			SaveFieldValues(entity, schema, unitSchemaFields);

			return entity;
		}

		private static void SaveFieldValues<T>(Entity entity, Schema schema, 
			Dictionary<T, FieldInfo> fieldList)
		{
			foreach (KeyValuePair<T, FieldInfo> kvp in fieldList)
			{
				Field field = schema.GetField(kvp.Value.Name);

				if (kvp.Value.UnitType != UnitType.UT_Undefined)
				{
					entity.Set(field, kvp.Value.Value, DisplayUnitType.DUT_GENERAL);
				}
				else
				{
					entity.Set(field, kvp.Value.Value);
				}
			}
		}

		// ******************************
		// read setting routines
		// ******************************


		// routine to read the existing saved settings
		// if none exist, this saves the default settings, then
		// reads them back, and then flags that this is initalized
		public static bool ReadRevitSettings()
		{
			Init();

			if (!ReadAllRevitSettings())
			{
				SaveRevitSettings();

				if (ReadAllRevitSettings())
				{
					return false;
				}
			}

			return true;
		}


		// general routine to read through a saved schema and 
		// get the value from each field 
		// this will work with any field list
		private static bool ReadAllRevitSettings()
		{
			Schema schema = Schema.Lookup(SchemaGUID);

			if (schema == null ||
				schema.IsValidObject == false) { return false; }

			Element elem = GetProjectBasepoint();

			Entity elemEntity = elem.GetEntity(schema);

			if (elemEntity?.Schema == null) { return false; }

			ReadBasicRevitSettings(elemEntity, schema);

			if (!ReadRevitUnitStyles(elemEntity, schema))
			{
				return false;
			}

			schema.Dispose();

			return true;
		}

		private static void ReadBasicRevitSettings(Entity elemEntity, Schema schema)
		{
			foreach (KeyValuePair<SBasicKey, FieldInfo> kvp in SchemaFields)
			{
				Field f = schema.GetField(kvp.Value.Name);
				kvp.Value.Value = kvp.Value.ExtractValue(elemEntity, f);
			}
		}

		// this reads through the basic fields associated with the unit style schema
		// it passes these down to the readsubentity method that then reads
		// through all of the fields in the subschema
		private static bool ReadRevitUnitStyles(Entity elemEntity, Schema schema)
		{
			for (int i = 0; i < SchemaFields[COUNT].Value; i++)
			{
				string subSchemaName = GetSubSchemaName(i);

				Field f = schema.GetField(subSchemaName);

				if (f == null || !f.IsValidObject) { continue; }

				Entity subSchema = elemEntity.Get<Entity>(f);
				

				if (subSchema == null || !subSchema.IsValidObject) { continue; }

				ReadSubSchema(subSchema, subSchema.Schema, UnitSchemaFields[i]);
			}

			return true;
		}

		private static void ReadSubSchema(Entity subSchemaEntity, Schema schema,
			Dictionary<SUnitKey, FieldInfo> unitSchemaField)
		{
			foreach (KeyValuePair<SUnitKey, FieldInfo> kvp
				in unitSchemaField)
			{
				Field f = schema.GetField(kvp.Value.Name);
				kvp.Value.Value =
					kvp.Value.ExtractValue(subSchemaEntity, f);
			}
		}

		// ******************************
		// listing routines
		// ******************************

		public static void ListFieldInfo()
		{
			logMsgDbLn2("basic", "settings");
			ListFieldInfo(SchemaFields);

			for (int i = 0; i < SchemaFields[COUNT].Value; i++)
			{
				logMsg(nl);
				logMsgDbLn2("unit", "settings");
				ListFieldInfo(UnitSchemaFields[i]);
			}
		}

		private static void ListFieldInfo<T>(Dictionary<T, FieldInfo> fieldList)
		{
			int i = 0;

			foreach (KeyValuePair<T, FieldInfo> kvp in fieldList)
			{
				logMsgDbLn2("field #" + i++, kvp.Key.GetType().Name
					+ "  name| " + kvp.Value.Name
					+ "  value| " + kvp.Value.Value);
			}
		}

		//		private static Entity MakeSubSchema()
		//		{
		//			SchemaBuilder sbld = new SchemaBuilder(SubSchemaGuid);
		//
		//			sbld.SetReadAccessLevel(AccessLevel.Public);
		//			sbld.SetWriteAccessLevel(AccessLevel.Vendor);
		//			sbld.SetVendorId(Util.GetVendorId());
		//			sbld.SetDocumentation(SCHEMA_DESC);
		//			sbld.SetSchemaName("subSchema" + seq.ToString("0000"));
		//		}
		//
		//		private const string guid = "59338959-393B-4064-9EE0-ADE4599D";
		//
		//		public static bool SaveRevitSettings2()
		//		{
		//			if (VendorId == null) Initalize();
		//
		//			ListSchemaInMemory();
		//
		//
		////
		////			IDictionary<string, Entity> testDict = new Dictionary<string, Entity>(3);
		////
		////			testDict.Add("name1", MakeSubSchema("a1", 101, 201.0, 1));
		////			testDict.Add("name2", MakeSubSchema("a2", 102, 202.0, 2));
		////			testDict.Add("name3", MakeSubSchema("a3", 103, 203.0, 3));
		//
		//			if (VendorId == null) Initalize();
		//
		//			Element elem = GetProjectBasepoint();
		//
		//			SchemaBuilder sbld = new SchemaBuilder(SchemaGUID);
		//
		//			sbld.SetReadAccessLevel(AccessLevel.Public);
		//			sbld.SetWriteAccessLevel(AccessLevel.Vendor);
		//			sbld.SetVendorId(VendorId);
		//			sbld.SetDocumentation(SCHEMA_DESC);
		//			sbld.SetSchemaName(SCHEMA1_NAME);
		//
		////			FieldBuilder fbld = sbld.AddMapField("dictionary", typeof(string), typeof(Entity));
		////			fbld.SetSubSchemaGUID(new Guid(guid + "0000"));
		//
		//			FieldBuilder fbld1 = sbld.AddSimpleField("field1", typeof(Entity));
		//			fbld1.SetSubSchemaGUID(new Guid(guid + "0001"));
		//
		//			FieldBuilder fbld2 = sbld.AddSimpleField("field2", typeof(Entity));
		//			fbld2.SetSubSchemaGUID(new Guid(guid + "0002"));
		//
		//			FieldBuilder fbld3 = sbld.AddSimpleField("field3", typeof(Entity));
		//			fbld3.SetSubSchemaGUID(new Guid(guid + "0003"));
		//
		//			Schema schema = sbld.Finish();
		//
		//			Entity entity = new Entity(schema);
		//
		//			entity.Set("field1", MakeSubSchema("a1", 101, 201.0, 1));
		//			entity.Set("field2", MakeSubSchema("a2", 102, 202.0, 2));
		//			entity.Set("field3", MakeSubSchema("a3", 103, 203.0, 3));
		//
		//			using (Transaction t = new Transaction(Doc, "unit style test"))
		//			{
		//				t.Start();
		//				elem.SetEntity(entity);
		//				t.Commit();
		//			}
		//
		//			return true;
		//		}
		//
		//		private static Entity MakeSubSchema(string testStr, int testInt, double testDbl, int seq)
		//		{
		//			string g = guid + seq.ToString("0000");
		//			
		//			SchemaBuilder sbld = new SchemaBuilder(new Guid(g));
		//
		//			sbld.SetReadAccessLevel(AccessLevel.Public);
		//			sbld.SetWriteAccessLevel(AccessLevel.Vendor);
		//			sbld.SetVendorId(VendorId);
		//			sbld.SetDocumentation(SCHEMA_DESC);
		//			sbld.SetSchemaName("subSchema" + seq.ToString("0000"));
		//
		//			FieldBuilder fbldStr = sbld.AddSimpleField("fieldStr", typeof(string));
		//			FieldBuilder fbldInt = sbld.AddSimpleField("fieldInt", typeof(int));
		//			FieldBuilder fbldDbl = sbld.AddSimpleField("fieldDbl", typeof(double));
		//			fbldDbl.SetUnitType(UnitType.UT_Number);
		//
		//			Schema schema = sbld.Finish();
		//
		//			Entity entity = new Entity(schema);
		//
		//			Field fldStr = schema.GetField("fieldStr");
		//			Field fldInt = schema.GetField("fieldInt");
		//			Field fldDbl = schema.GetField("fieldDbl");
		//
		//			entity.Set(fldStr, testStr);
		//			entity.Set(fldInt, testInt);
		//			entity.Set(fldDbl, testDbl, DisplayUnitType.DUT_GENERAL);
		//
		//			return entity;
		//		}
		//
		//		public static string ReadRevitSettings()
		//		{
		//
		//			try
		//			{
		//				Element elem = GetProjectBasepoint();
		//
		//				Schema schema = Schema.Lookup(SchemaGUID);
		//
		//				if (schema == null) return null;
		//
		//				Field fld = schema.GetField(FIELD1_NAME);
		//
		//				Entity entity = elem.GetEntity(schema);
		//
		//				string test = entity.Get<string>(fld);
		//
		//				return test;
		//
		//			}
		//			catch { }
		//
		//			return null;
		//		}
		//
		//		public static string ReadRevitSettings2()
		//		{
		//			StringBuilder sb = new StringBuilder();
		//
		//			try
		//			{
		//				Element elem = GetProjectBasepoint();
		//
		//				Schema schema = Schema.Lookup(SchemaGUID);
		//
		//				ListSchemaFields(schema);
		//
		//				if (schema == null) return null;
		//
		//				Entity entElem = elem.GetEntity(schema);
		//
		//				Entity entity;
		//
		//				entity = entElem.Get<Entity>("field1");
		//				sb.Append(ReadSubEntity(entity));
		//
		//				entity = entElem.Get<Entity>("field2");
		//				sb.Append(ReadSubEntity(entity));
		//
		//				entity = entElem.Get<Entity>("field3");
		//				sb.Append(ReadSubEntity(entity));
		//
		//				return sb.ToString();
		//
		//			}
		//			catch { }
		//
		//			return null;
		//		}
		//
		//		private static string ReadSubEntity(Entity entity)
		//		{
		//			StringBuilder sb = new StringBuilder();
		//
		//			string s = entity.Get<string>("fieldStr");
		//			int i = entity.Get<int>("fieldInt");
		//			double d = entity.Get<double>("fieldDbl", DisplayUnitType.DUT_GENERAL);
		//
		//			sb.Append("string is| ").AppendLine(s);
		//			sb.Append("int is| ").AppendLine(i.ToString());
		//			sb.Append("double is| ").AppendLine(d.ToString());
		//
		//			return sb.ToString();
		//		}
		//
		//		private static void ListSchemaFields(Schema schema)
		//		{
		//			foreach (Field  fld in schema.ListFields())
		//			{
		//				logMsgDbLn2("field", fld.FieldName + "  type| " + fld.ValueType);
		//			}
		//		}
		//
		//		private static void ListSchemaInMemory()
		//		{
		//			foreach (Schema schema in Schema.ListSchemas())
		//			{
		//				logMsgDbLn2("schema", schema.SchemaName);
		//			}
		//		}
	}
}
