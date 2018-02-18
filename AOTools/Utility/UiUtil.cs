#region Using directives

using Autodesk.Revit.UI;
using UtilityLibrary;

#endregion

// itemname:	UiUtil
// username:	jeffs
// created:		2/18/2018 10:08:49 AM


namespace AOTools.Utility
{
	public static class UiUtil
	{
		private const string NAMESPACE_PREFIX = "AOTools.Resources";

		// method to add a pushbutton to the ribbon
		public static bool AddPushButton(RibbonPanel Panel, string ButtonName,
			string ButtonText, string Image16, string Image32,
			string dllPath, string dllClass, string ToolTip)
		{
			try
			{
				PushButtonData m_pdData = new PushButtonData(ButtonName,
					ButtonText, dllPath, dllClass);
				// if we have a path for a small image, try to load the image
				if (Image16.Length != 0)
				{
					try
					{
						// load the image
						m_pdData.Image = CsUtilities.GetBitmapImage(Image16, NAMESPACE_PREFIX);
					}
					catch
					{
						// could not locate the image
					}
				}

				// if have a path for a large image, try to load the image
				if (Image32.Length != 0)
				{
					try
					{
						// load the image
						m_pdData.LargeImage = CsUtilities.GetBitmapImage(Image32, NAMESPACE_PREFIX);
					}
					catch
					{
						// could not locate the image
					}
				}
				// set the tooltip
				m_pdData.ToolTip = ToolTip;

				// add it to the panel
				PushButton m_pb = Panel.AddItem(m_pdData) as PushButton;

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
