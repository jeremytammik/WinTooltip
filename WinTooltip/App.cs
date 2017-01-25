#region Namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;
#endregion // Namespaces

namespace WinTooltip
{
  // C:\a\lib\revit\2013\SDK\Samples\ModelessDialog\ModelessForm_IdlingEvent\CS\Application.cs

  /// <summary>
  /// External application handling a modeless 
  /// form and the Idling event, based on the 
  /// ModelessForm_IdlingEvent SDK sample.
  /// </summary>
  class App : IExternalApplication
  {
    /// <summary>
    /// Singleton external application class instance.
    /// </summary>
    internal static App _app = null;

    /// <summary>
    /// Provide access to singleton class instance.
    /// </summary>
    public static App Instance
    {
      get { return _app; }
    }

    /// <summary>
    /// The tooltip form to display.
    /// </summary>
    internal static JtTooltipForm2 _form = null;

    /// <summary>
    /// Dispose and null out form.
    /// Return true if it was previously not disposed.
    /// </summary>
    static bool CloseForm()
    {
      bool rc = _form != null;

      if( rc )
      {
        if( !_form.IsDisposed )
        {
          _form.Dispose();
        }
        _form = null;
      }
      return rc;
    }

    /// <summary>
    /// Create and show the form, 
    /// unless it already exists.
    /// </summary>
    /// <remarks>
    /// The external command invokes 
    /// this on end-user request.
    /// </remarks>
    public void ShowForm( UIApplication uiapp )
    {
      // If we do not have a form yet, create and show it

      if( _form == null || _form.IsDisposed )
      {
        // Instantiate JtTooltipForm to use 
        // the designer generated form.

        _form = new JtTooltipForm2();

        _form.Show();

        // If we have a form, we need Idling too

        uiapp.Idling += IdlingHandler;
      }
    }

    /// <summary>
    /// Hide the form.
    /// </summary>
    /// <remarks>
    /// The external command invokes 
    /// this on end-user request.
    /// </remarks>
    public void HideForm( UIApplication uiapp )
    {
      if( CloseForm() )
      {
        // If the form was showing, we had subscribed

        uiapp.Idling -= IdlingHandler;
      }
    }

    public Result OnStartup( UIControlledApplication a )
    {
      _app = this;
      _form = null;

      return Result.Succeeded;
    }

    public Result OnShutdown( UIControlledApplication a )
    {
      if( CloseForm() )
      {
        a.Idling -= IdlingHandler;
      }
      return Result.Succeeded;
    }

    /// <summary>
    /// Return currently active UIView or null.
    /// </summary>
    static UIView GetActiveUiView( 
      UIDocument uidoc )
    {
      Document doc = uidoc.Document;
      View view = doc.ActiveView;
      IList<UIView> uiviews = uidoc.GetOpenUIViews();
      UIView uiview = null;

      foreach( UIView uv in uiviews )
      {
        if( uv.ViewId.Equals( view.Id ) )
        {
          uiview = uv;
          break;
        }
      }
      return uiview;
    }

    /// <summary>
    /// Return the 3D view named "{3D}".
    /// </summary>
    View3D GetView3d( Document doc )
    {
      return new FilteredElementCollector( doc )
        .OfClass( typeof( View3D ) )
        .Cast<View3D>()
        .FirstOrDefault<View3D>( 
          v => v.Name.Equals( "{3D}" ) );
    }

    /// <summary>
    /// Return a string describing the given element:
    /// .NET type name,
    /// category name,
    /// family and symbol name for a family instance,
    /// element id and element name.
    /// </summary>
    static string ElementDescription(
      Element e )
    {
      if( null == e )
      {
        return "<null>";
      }

      // For a wall, the element name equals the
      // wall type name, which is equivalent to the
      // family name ...

      FamilyInstance fi = e as FamilyInstance;

      string typeName = e.GetType().Name;

      string categoryName = ( null == e.Category )
        ? string.Empty
        : e.Category.Name + " ";

      string familyName = ( null == fi )
        ? string.Empty
        : fi.Symbol.Family.Name + " ";

      string symbolName = ( null == fi
        || e.Name.Equals( fi.Symbol.Name ) )
          ? string.Empty
          : fi.Symbol.Name + " ";

      return string.Format( "{0} {1}{2}{3}<{4} {5}>",
        typeName, categoryName, familyName,
        symbolName, e.Id.IntegerValue, e.Name );
    }

    /// <summary>
    /// Idling event handler.
    /// </summary>
    /// <remarks>
    /// We keep the handler very simple. First check
    /// if we still have the form. If not, unsubscribe 
    /// from Idling, for we no longer need it and it 
    /// makes Revit speedier. If the form is around, 
    /// check if it has a request ready and process 
    /// it accordingly.
    /// </remarks>
    public void IdlingHandler( 
      object sender, 
      IdlingEventArgs args )
    {
      UIApplication uiapp = sender as UIApplication;
      UIDocument uidoc = uiapp.ActiveUIDocument;

      // UI document is null if the project is closed.

      if( null == uidoc || _form.IsDisposed )
      {
        uiapp.Idling -= IdlingHandler;
      }
      else // form still exists
      {
        Document doc = uidoc.Document;
        View view = doc.ActiveView;

        UIView uiview = GetActiveUiView( uidoc );

        Rectangle rect = uiview.GetWindowRectangle();

        Point p = System.Windows.Forms.Cursor.Position;

        double dx = (double) ( p.X - rect.Left )
          / ( rect.Right - rect.Left );

        double dy = (double) ( p.Y - rect.Bottom )
          / ( rect.Top - rect.Bottom );

        IList<XYZ> corners = uiview.GetZoomCorners();
        XYZ a = corners[0];
        XYZ b = corners[1];
        XYZ v = b - a;

        XYZ q = a
          + dx * v.X * XYZ.BasisX
          + dy * v.Y * XYZ.BasisY;

        // If the current view happens to be a 3D view, 
        // we could simply use it right away. In 
        // general we have to find a different one to 
        // run the ReferenceIntersector in.

        View3D view3d = GetView3d( doc );

        XYZ viewdir = view.ViewDirection;

        XYZ origin = q + 1000 * viewdir;

        // Find all elements:

        //ReferenceIntersector ri
        //  = new ReferenceIntersector( view3d );

        // Find all elements except roofs:

        ElementFilter f = new ElementCategoryFilter( 
          BuiltInCategory.OST_Roofs, true );

        ReferenceIntersector ri 
          = new ReferenceIntersector( f, 
            FindReferenceTarget.Element, view3d );

        ReferenceWithContext rc
          = ri.FindNearest( origin, -viewdir );

        string s = "Element not found";

        if( null != rc )
        {
          Reference r = rc.GetReference();

          Element e = doc.GetElement( r );

          s = ElementDescription( e );
        }

        // Move tooltip to current cursor 
        // location and set tooltip text.

        _form.Location = p + new Size( _form.Offset );
        _form.SetText( s );
      }
    }
  }
}
