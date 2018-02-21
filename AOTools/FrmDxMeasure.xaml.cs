using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using AOTools.Utility;
using Autodesk.Revit.DB;
using Point = System.Drawing.Point;

using static AOTools.Utility.Util;

using static AOTools.AppSettings.ConfigSettings.SettingsUsr;

using static UtilityLibrary.MessageUtilities;
using Visibility = System.Windows.Visibility;

namespace AOTools
{
	/// <summary>
	/// Interaction logic for FrmDxMeasure.xaml
	/// </summary>
	public partial class FrmDxMeasure : Window
	{
		private static bool _showWorkplane;

		public FrmDxMeasure()
		{
			InitializeComponent();
			LoadSettings();
		}

		#region Routines

		private void LoadSettings()
		{
			lblMessage.Content = "";

			if (SmUsrSetg.FormMeasurePointsLocation.Equals(new Point(0, 0)))
			{
				this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
			}
			else
			{
				this.Top = SmUsrSetg.FormMeasurePointsLocation.Y;
				this.Left = SmUsrSetg.FormMeasurePointsLocation.X;
			}

			ShowWorkplane = SmUsrSetg.MeasurePointsShowWorkplane;
		}

		// flip setting for show or no show the work plane
		internal bool ShowWorkplane
		{
			get { return _showWorkplane; }
			private set
			{
				_showWorkplane = value;
				cbxWpOnOff.IsChecked = value;
			}
		}

		internal void UpdatePoints(PointMeasurements? pm, Util.VType vtype,
			XYZ normal, XYZ origin, string planeName, Units units)
		{
			if (pm == null)
			{
				ClearText();

				lblMessage.Content = "Please Select Two Points to Measure";

				return;
			}

			lblMessage.Content = "View is a " + vtype.VTName;

			if (planeName != null)
			{
				lblMessage.Content += "  plane name: " + planeName;
			}

			lblP1X.Content = FormatLengthNumber(pm.Value.P1.X, units);
			lblP1Y.Content = FormatLengthNumber(pm.Value.P1.Y, units);
			lblP1Z.Content = FormatLengthNumber(pm.Value.P1.Z, units);

			lblP2X.Content = FormatLengthNumber(pm.Value.P2.X, units);
			lblP2Y.Content = FormatLengthNumber(pm.Value.P2.Y, units);
			lblP2Z.Content = FormatLengthNumber(pm.Value.P2.Z, units);

			lblDistX.Content = FormatLengthNumber(pm.Value.delta.X, units);
			lblDistY.Content = FormatLengthNumber(pm.Value.delta.Y, units);
			lblDistZ.Content = FormatLengthNumber(pm.Value.delta.Z, units);

			lblDistXY.Content = FormatLengthNumber(pm.Value.distanceXY, units);
			lblDistXZ.Content = FormatLengthNumber(pm.Value.distanceXZ, units);
			lblDistYZ.Content = FormatLengthNumber(pm.Value.distanceYZ, units);

			lblDistXYZ.Content = FormatLengthNumber(pm.Value.distanceXYZ, units);

			lblWpOriginX.Content = FormatLengthNumber(origin.X, units);
			lblWpOriginY.Content = FormatLengthNumber(origin.Y, units);
			lblWpOriginZ.Content = FormatLengthNumber(origin.Z, units);

			lblWpNormalX.Content = $"{normal.X:F4}";
			lblWpNormalY.Content = $"{normal.Y:F4}";
			lblWpNormalZ.Content = $"{normal.Z:F4}";
		}

		internal void ClearText()
		{
			lblMessage.Content = "";

			lblP1X.Content = "";
			lblP1Y.Content = "";
			lblP1Z.Content = "";

			lblP2X.Content = "";
			lblP2Y.Content = "";
			lblP2Z.Content = "";

			lblDistX.Content = "";
			lblDistY.Content = "";
			lblDistZ.Content = "";

			lblDistXY.Content = "";
			lblDistXZ.Content = "";
			lblDistYZ.Content = "";

			lblDistXYZ.Content = "";

			lblWpOriginX.Content = "";
			lblWpOriginY.Content = "";
			lblWpOriginZ.Content = "";

			lblWpNormalX.Content = "";
			lblWpNormalY.Content = "";
			lblWpNormalZ.Content = "";
		}


		#endregion

		#region Events

		private void btnDone_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
//			Hide();
		}

		private void btnSelectPoints_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
//			Hide();
		}

		private void cbxWpOnOff_Click(object sender, RoutedEventArgs e)
		{
			ShowWorkplane = cbxWpOnOff.IsChecked ?? false;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			LoadSettings();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
//			e.Cancel = true;
//			this.Visibility = Visibility.Hidden;

			logMsgDbLn2("window", "closing");
			logMsgDbLn2("dialog result", DialogResult.ToString());

			SmUsrSetg.FormMeasurePointsLocation = new Point((int) this.Left, (int) this.Top);
			SmUsrSetg.MeasurePointsShowWorkplane = this.ShowWorkplane;
			SmUsr.Save();
		}


		#endregion

	}
}
