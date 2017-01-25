#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace WinTooltip
{
  [Transaction( TransactionMode.ReadOnly )]
  public class CmdTooltipOn : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      App.Instance.ShowForm( 
        commandData.Application );

      return Result.Succeeded;
    }
  }

  [Transaction( TransactionMode.ReadOnly )]
  public class CmdTooltipOff : IExternalCommand
  {
    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements )
    {
      App.Instance.HideForm(
        commandData.Application );

      return Result.Succeeded;
    }
  }
}
