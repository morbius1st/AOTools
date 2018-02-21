using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using View = Autodesk.Revit.DB.View;

using static UtilityLibrary.MessageUtilities;

using UtilityLibrary;

namespace AOTools.Utility
{
	internal static class Util
	{
		public static RevitAddIns addinManifest;
//		public static readonly string nl = Environment.NewLine;
	

		internal const ObjectSnapTypes snaps =
			ObjectSnapTypes.Centers | ObjectSnapTypes.Endpoints | ObjectSnapTypes.Intersections |
			ObjectSnapTypes.Midpoints | ObjectSnapTypes.Nearest | ObjectSnapTypes.Perpendicular |
			ObjectSnapTypes.Quadrants | ObjectSnapTypes.Tangents;

		public static string FormatLengthNumber(double length, Units units)
		{
			
			return UnitFormatUtils.Format(units,
				UnitType.UT_Length, length, true, false);
		}

		// determine if the supplied list contains the supplied point
		// within tolerance - and only test the X / Y values - set Z to 0

		internal enum VTtypeCat
		{
			OTHER,
			D2_WITHPLANE,
			D2_WITHOUTPLANE,
			D3_WITHPLANE
		}

		internal enum VTypeSub
		{
			OTHER,
			D2_HORIZONTAL,
			D2_VERTICAL,
			D2_DRAFTING,
			D2_SHEET,
			D3_VIEW
		}

		internal struct VType
		{
			internal VTypeSub VTSub { get;} 
			internal VTtypeCat VTCat { get; }
			internal string VTName { get; }

			internal VType(VTypeSub VTSub, VTtypeCat VTCat, string VTName )
			{
				this.VTSub = VTSub;
				this.VTCat = VTCat;
				this.VTName = VTName;
			}
		}

		internal static VType GetViewType(Autodesk.Revit.DB.View v)
		{
			VType vtype = new VType(VTypeSub.OTHER, VTtypeCat.OTHER, "Other View Type");

			switch (v.ViewType)
			{
				case ViewType.AreaPlan:
				case ViewType.CeilingPlan:
				case ViewType.EngineeringPlan:
				case ViewType.FloorPlan:
					vtype = new VType(VTypeSub.D2_HORIZONTAL, 
						VTtypeCat.D2_WITHPLANE, "Plan 2D View");
					break;
				case ViewType.Elevation:
				case ViewType.Section:
					vtype = new VType(VTypeSub.D2_VERTICAL, 
						VTtypeCat.D2_WITHPLANE, "Vertical 2D View");
					break;
				case ViewType.ThreeD:
					vtype = new VType(VTypeSub.D3_VIEW, 
						VTtypeCat.D3_WITHPLANE, "3D View");
					break;
				case ViewType.Detail:
				case ViewType.DraftingView:
					vtype = new VType(VTypeSub.D2_DRAFTING, 
						VTtypeCat.D2_WITHOUTPLANE, "Drafting View");
					break;
				case ViewType.DrawingSheet:
					vtype = new VType(VTypeSub.D2_SHEET, 
						VTtypeCat.D2_WITHOUTPLANE, "Sheet View");
					break;
			}

			return vtype;
		}

		private static void ReadManifest()
		{
			string path = CsUtilities.AssemblyDirectory;

			using (FileStream fs =
				new FileStream(path + "\\" + AppRibbon.APP_NAME + ".addin", FileMode.Open))
			{
				XmlSerializer xs = new XmlSerializer(typeof(RevitAddIns));
				addinManifest = (RevitAddIns) xs.Deserialize(fs);
			}
		}

		public static string GetVendorId()
		{
			if (addinManifest == null)
			{
				ReadManifest();
			}

			return addinManifest?.AddIn[0].VendorId;
		}

		// get reference to the project basepoint
		public static Element GetProjectBasepoint()
		{
			ElementCategoryFilter sitElementCategoryFilter =
				new ElementCategoryFilter(BuiltInCategory.OST_ProjectBasePoint);

			FilteredElementCollector collector =
				new FilteredElementCollector(AppRibbon.Doc);

			IList<Element> siteElements =
				collector.WherePasses(sitElementCategoryFilter).ToElements();

			if (siteElements.Count > 1)
			{
				return null;
			}
			
			return siteElements[0];
		}

		internal static bool IsPlaneOrientationAcceptable(UIDocument uiDoc)
		{
			View v = uiDoc.ActiveGraphicalView;
			SketchPlane sp = uiDoc.ActiveGraphicalView.SketchPlane;
			Plane p = sp?.GetPlane();

			if (p == null) { return false; }

			double dp = Math.Abs(v.ViewDirection.DotProduct(p.Normal));

			logMsgDbLn2("rotation acceptable?", dp.ToString());

			if (dp < 0.05) { return false; }

			return true;
		}

	}

	internal struct PointMeasurements
	{
		internal XYZ P1 { get; }
		internal  XYZ P2 { get; }

		internal XYZ delta { get; }
		private XYZ sqDelta;

		internal double distanceXY { get; }
		internal double distanceXZ { get; }
		internal double distanceYZ { get; }
		internal double distanceXYZ { get; }

		internal PointMeasurements(XYZ p1, XYZ p2, XYZ origin)
		{
			P1 = p1 - origin;
			P2 = p2 - origin;

			delta = p2 - p1;
			sqDelta = delta.Multiply(delta);

			distanceXY = Math.Sqrt(sqDelta.X + sqDelta.Y);
			distanceXZ = Math.Sqrt(sqDelta.X + sqDelta.Z);
			distanceYZ = Math.Sqrt(sqDelta.Y + sqDelta.Z);

			distanceXYZ = Math.Sqrt(sqDelta.X + sqDelta.Y + sqDelta.Z);
		}
	}
}
