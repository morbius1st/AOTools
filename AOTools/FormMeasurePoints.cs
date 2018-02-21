using System;
using System.Windows.Forms;
using AOTools.Utility;
using Autodesk.Revit.DB;
using Form = System.Windows.Forms.Form;
using Point = System.Drawing.Point;

using static AOTools.Utility.Util;

using static AOTools.AppSettings.ConfigSettings.SettingsUsr;

namespace AOTools
{
	public partial class FormMeasurePoints : Form
	{
		private static bool showWorkplane;

		public FormMeasurePoints()
		{
			InitializeComponent();
			LoadSettings();
			lblMessage.Text = "";
		}

		private void FormQueryPoints_FormClosing(object sender, FormClosingEventArgs e)
		{
			SmUsrSetg.FormMeasurePointsLocation = this.Location;
			SmUsrSetg.MeasurePointsShowWorkplane = this.ShowWorkplane;
			SmUsr.Save();
			
		}

		private void FormQueryPoints_Load(object sender, EventArgs e)
		{
			LoadSettings();
		}

		private void LoadSettings()
		{
			if (SmUsrSetg.FormMeasurePointsLocation.Equals(new Point(0, 0)))
			{
				CenterToParent();
			}
			else
			{
				this.Location = SmUsrSetg.FormMeasurePointsLocation;
			}

			ShowWorkplane = SmUsrSetg.MeasurePointsShowWorkplane;
		}

		
		private void cbxWpOnOff_CheckedChanged(object sender, EventArgs e)
		{
			showWorkplane = cbxWpOnOff.Checked;
		}


		// custom methods
		// flip setting for show or no show the work plane
		internal bool ShowWorkplane
		{
			get { return showWorkplane; }
			private set
			{
				showWorkplane = value;
				cbxWpOnOff.Checked = value;
			}
		}

		internal void UpdatePoints(PointMeasurements? pm, Util.VType vtype, 
			XYZ normal, XYZ origin, string planeName, Units units)
		{
			if (pm == null)
			{
				ClearText();

				lblMessage.Text = "Please Select Two Points to Measure";
				
				return;
			}

			lblMessage.Text = "View is a " + vtype.VTName;

			if (planeName != null)
			{
				lblMessage.Text += "  plane name: " + planeName;
			}

			lblP1X.Text = FormatLengthNumber(pm.Value.P1.X, units);
			lblP1Y.Text = FormatLengthNumber(pm.Value.P1.Y, units);
			lblP1Z.Text = FormatLengthNumber(pm.Value.P1.Z, units);

			lblP2X.Text = FormatLengthNumber(pm.Value.P2.X, units);
			lblP2Y.Text = FormatLengthNumber(pm.Value.P2.Y, units);
			lblP2Z.Text = FormatLengthNumber(pm.Value.P2.Z, units);

			lblDistX.Text = FormatLengthNumber(pm.Value.delta.X, units);
			lblDistY.Text = FormatLengthNumber(pm.Value.delta.Y, units);
			lblDistZ.Text = FormatLengthNumber(pm.Value.delta.Z, units);

			lblDistXY.Text = FormatLengthNumber(pm.Value.distanceXY, units);
			lblDistXZ.Text = FormatLengthNumber(pm.Value.distanceXZ, units);
			lblDistYZ.Text = FormatLengthNumber(pm.Value.distanceYZ, units);

			lblDistXYZ.Text = FormatLengthNumber(pm.Value.distanceXYZ, units);

			lblWpOriginX.Text = FormatLengthNumber(origin.X, units);
			lblWpOriginY.Text = FormatLengthNumber(origin.Y, units);
			lblWpOriginZ.Text = FormatLengthNumber(origin.Z, units);

			lblWpNormalX.Text = $"{normal.X:F4}";
			lblWpNormalY.Text = $"{normal.Y:F4}";
			lblWpNormalZ.Text = $"{normal.Z:F4}";

		}

		internal void ClearText()
		{
			
			lblMessage.Text = "";

			lblP1X.Text		= "";
			lblP1Y.Text		= "";
			lblP1Z.Text		= "";

			lblP2X.Text		= "";
			lblP2Y.Text		= "";
			lblP2Z.Text		= "";

			lblDistX.Text	= "";
			lblDistY.Text	= "";
			lblDistZ.Text	= "";

			lblDistXY.Text	= "";
			lblDistXZ.Text	= "";
			lblDistYZ.Text	= "";

			lblDistXYZ.Text = "";

			lblWpOriginX.Text = "";
			lblWpOriginY.Text = "";
			lblWpOriginZ.Text = "";

			lblWpNormalX.Text = "";
			lblWpNormalY.Text = "";
			lblWpNormalZ.Text = "";

			lblWpOrigin.Text = "";
		}

	}
}
