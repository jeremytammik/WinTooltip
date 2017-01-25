using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace WinTooltip
{
  // http://stackoverflow.com/questions/2461448/how-to-make-a-floating-tooltip-control-in-windows-forms

  /// <summary>
  /// A tooltip class to display 
  /// information from a control.
  /// </summary>
  class ToolTipWindow : Form
  {
    /// <summary>
    /// The offset from the mouse pointer 
    /// to show the window at.
    /// </summary>
    public Point Offset { get; set; }

    internal ToolTipWindow( 
      Control controlToDisplay )
    {
      FormBorderStyle = FormBorderStyle.None;
      TopMost = true;
      ShowInTaskbar = false;
      Opacity = 0.9;
      Width = controlToDisplay.Width;
      Height = controlToDisplay.Height;
      Controls.Add( controlToDisplay );
      controlToDisplay.Show();
    }

    /// <summary>
    /// Move the window to an offset of mouse pointer.
    /// </summary>
    protected override void OnShown( EventArgs e )
    {
      base.OnShown( e );
      
      Location = new Point( 
        MousePosition.X + Offset.X, 
        MousePosition.Y + Offset.Y );
    }

    /// <summary>
    /// Move the window to an offset of mouse pointer.
    /// </summary>
    protected override void OnVisibleChanged( 
      EventArgs e )
    {
      base.OnVisibleChanged( e );
      
      if( Visible )
      {
        Location = new Point( 
          MousePosition.X + Offset.X, 
          MousePosition.Y + Offset.Y );
      }
    }
  }
}
