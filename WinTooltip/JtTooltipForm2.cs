#region Namespaces
using System;
using System.Drawing;
using System.Windows.Forms;
#endregion // Namespaces

namespace WinTooltip
{
  /// <summary>
  /// A tooltip window designed to move 
  /// around with the cursor position.
  /// </summary>
  class JtTooltipForm2 : Form
  {
    /// <summary>
    /// The offset from the mouse pointer 
    /// at which to show the form.
    /// </summary>
    public Point Offset { get; set; }

    /// <summary>
    /// Tooltip text.
    /// </summary>
    Label _label;

    /// <summary>
    /// Set the tooltip text.
    /// </summary>
    public void SetText( string s )
    {
      _label.Text = s;
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

    public JtTooltipForm2()
    {
      Size = new Size( 200, 20 );

      _label = new Label();

      SuspendLayout();

      _label.AutoSize = false; // the label will not change it's height automatically, only width, so switch off AutoSize to wrap text
      _label.CausesValidation = false;
      _label.Dock = DockStyle.Fill;
      _label.Location = new Point( 0, 0 );
      _label.Size = new Size( 35, 13 );
      _label.Parent = this;

      AutoScaleDimensions = new SizeF( 6F, 13F );
      AutoScaleMode = AutoScaleMode.Font;
      BackColor = SystemColors.Info;
      ClientSize = new Size( 200, 12 );
      Controls.Add( _label );
      FormBorderStyle = FormBorderStyle.None;
      Name = "JtTooltipForm";
      Opacity = 0.8D;
      ShowInTaskbar = false;
      TopMost = true;
      TransparencyKey = Color.White;

      ResumeLayout( false );
      PerformLayout();

      Offset = new Point( 10, 0 );
    }
  }
}
