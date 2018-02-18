#region Using directives

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using static UtilityLibrary.MessageUtilities;

#endregion

// itemname:	UnitStylesDelete
// username:	jeffs
// created:		1/14/2018 7:19:43 PM


namespace AOTools
{
	[Transaction(TransactionMode.Manual)]
	class UnitStylesDelete : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			output = outputLocation.debug;

			logMsgDbLn2("delete unit styles", "before");
			RevitSettingsBase.ListRevitSchema();

			if (!RsMgr.DeleteSchema())
			{
				return Result.Failed;
			}

			logMsg("");
			logMsgDbLn2("delete unit styles", "after");
			RevitSettingsBase.ListRevitSchema();

			logMsg("");
			return Result.Succeeded;
		}
	}
}
