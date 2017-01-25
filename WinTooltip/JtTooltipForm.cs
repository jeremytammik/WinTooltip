#region Namespaces
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
#endregion // Namespaces

namespace WinTooltip
{
  /// <summary>
  /// A minimal class to display my own tooltip.
  /// </summary>
  public partial class JtTooltipForm : Form
  {
    /// <summary>
    /// The offset from the mouse pointer 
    /// at which to show the form.
    /// </summary>
    public Point Offset { get; set; }

    public JtTooltipForm()
    {
      InitializeComponent();

      Offset = new Point( 10, 0 );
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

    /// <summary>
    /// Set the tooltip text.
    /// </summary>
    public void SetText( string s )
    {
      label1.Text = s;
    }
  }
}
