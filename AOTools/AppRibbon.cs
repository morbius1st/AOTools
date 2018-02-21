#region Namespaces
using System;
using System.Collections.Generic;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Application = Autodesk.Revit.ApplicationServices.Application;
using static UtilityLibrary.MessageUtilities;

using AOTools.Utility;

#endregion

namespace AOTools
{
	class AppRibbon : IExternalApplication
	{
		internal const string APP_NAME = "AOTools";

		private const string BUTTON_NAME1 = "Delux\nMeasure";
		private const string BUTTON_NAME2 = "Delete\nStyles";
		private const string PANEL_NAME = "AO Tools";
		private const string TAB_NAME = "AO Tools";

		private static bool _eventsRegistered = false;
		private static bool _unitsConfigured = false;

		private static bool _familyDocumentCreated = false;

		private static UIControlledApplication _uiCtrlApp;
		internal static UIApplication UiApp;
		internal static UIDocument Uidoc;
		internal static Application App;
		internal static Document Doc;


		public Result OnStartup(UIControlledApplication app)
		{
			_uiCtrlApp = app;

			clearConsole();

			try
			{
				_uiCtrlApp.Idling += OnIdling;

				// create the ribbon tab first - this is the top level
				// UI item, below this will be the panel that is "on" the tab
				// and below this will be a button that is "on" the panel
				// give the tab a name
				string m_tabName = TAB_NAME;

				// first create the tab
				try
				{
					// try to create the ribbon panel
					app.CreateRibbonTab(m_tabName);
				}
				catch (Exception)
				{
					// might already exist - do nothing
				}

				// got the tab now

				// create the ribbon panel if needed
				// give the panel a name
				string m_panelName = PANEL_NAME;

				RibbonPanel m_RibbonPanel = null;

				// check to see if the panel alrady exists
				// get the Panel within the tab by name
				List<RibbonPanel> m_RP = new List<RibbonPanel>();

				m_RP = app.GetRibbonPanels(m_tabName);

				foreach (RibbonPanel xRP in m_RP)
				{
					if (xRP.Name.ToUpper().Equals(m_panelName.ToUpper()))
					{
						m_RibbonPanel = xRP;
						break;
					}
				}

				// if
				// add the panel if it does not exist
				if (m_RibbonPanel == null)
				{
					// create the ribbon panel on the tab given the tab's name
					// FYI - leave off the ribbon panel's name to put onto the "add-in" tab
					m_RibbonPanel = app.CreateRibbonPanel(m_tabName, m_panelName);
				}
				
				// create a button for the 'delux measure command
				if (!UiUtil.AddPushButton(m_RibbonPanel, "Delux Measure", BUTTON_NAME1,
					"information16.png",
					"information32.png",
					Assembly.GetExecutingAssembly().Location, "AOTools.DxMeasure", 
						"Create and Modify Unit Styles"))

				{
					// creating the pushbutton failed
					TaskDialog td = new TaskDialog("AO Tools");
					td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
					td.MainContent = String.Format(Properties.Resources.ButtonCreateFail,
						Properties.Resources.UnitStyleButtonText);
					td.Show();

					return Result.Failed;
				}

				return Result.Succeeded;
			}
			catch
			{
				return Result.Failed;
			}
		} // end OnStartup



		public Result OnShutdown(UIControlledApplication a)
		{
			try
			{
				// begin code here
				return Result.Succeeded;
			}
			catch
			{
				return Result.Failed;
			}
		} // end OnShutdown


		private void OnIdling(object sender, IdlingEventArgs e)
		{
			_uiCtrlApp.Idling -= OnIdling;
			UiApp = sender as UIApplication;
			Uidoc = UiApp.ActiveUIDocument;
			App = UiApp.Application;

			RegisterDocEvents();
		}

		private void AppClosing(object sender, ApplicationClosingEventArgs args)
		{
			App.DocumentCreated -= DocCreatedEvent;
			App.DocumentCreating -= DocCreatingEvent;
			UiApp.ApplicationClosing -= AppClosing;
		}

		private bool RegisterDocEvents()
		{
			if (_eventsRegistered) return true;
			_eventsRegistered = true;

			try
			{
				App.DocumentCreated += DocCreatedEvent;
				App.DocumentCreating += DocCreatingEvent;
				UiApp.ApplicationClosing += AppClosing;
			}
			catch (Exception)
			{
				return false;
			}

			return true;
		}

		private void DocCreatedEvent(object sender, DocumentCreatedEventArgs args)
		{
			if (_familyDocumentCreated)
			{
				logMsgDbLn2("document", "created");
				_familyDocumentCreated = false;

				SetUnits(args.Document);
			}
		}
		private void DocCreatingEvent(object sender, DocumentCreatingEventArgs args)
		{
			if (args.DocumentType == DocumentType.Family)
			{
				_familyDocumentCreated = true;
				logMsgDbLn2("document", "creating");
			}
		}

		private void SetUnits(Document doc)
		{
			double accuracy = (1.0 / 12.0) / 16.0;

			try
			{
				Units units = new Units(UnitSystem.Imperial);
				FormatOptions fmtOps = 
					new FormatOptions(DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES, 
						UnitSymbolType.UST_NONE, accuracy);

				fmtOps.SuppressSpaces = true;
				fmtOps.SuppressLeadingZeros = true;
				fmtOps.UseDigitGrouping = true;

				units.SetFormatOptions(UnitType.UT_Length, fmtOps);

				using (Transaction t = new Transaction(doc, "Update Units"))
				{
					t.Start();
					doc.SetUnits(units);
					t.Commit();
				}
			}
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
			catch (Exception)
			{
				// ignored
			}
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
		}

	}
}
