using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ReaLTaiizor.Colors;
using ReaLTaiizor.Controls;
using ReaLTaiizor.Manager;

namespace PnpUtilGui.Controls
{
    /// <summary>
    /// MaterialButton that renders its "Contained" fill with a fixed color
    /// (#0067C0 by default) instead of the global skin manager primary color.
    /// </summary>
    /// <remarks>
    /// ReaLTaiizor paints MaterialButton from the shared MaterialSkinManager
    /// color scheme and offers no per-button color option. The fixed color is
    /// applied by temporarily swapping the scheme's primary colors (readonly
    /// public fields, written via reflection) for the duration of this
    /// button's own paint. Painting is single-threaded (UI thread) and the
    /// swap is restored in a finally block, so the rest of the UI (header,
    /// other buttons) keeps the original theme colors. Hover and ripple
    /// states keep working because MaterialButton derives them from the same
    /// primary color it reads during the paint (BlendColor/Lighten). Raw
    /// field writes are used instead of replacing the ColorScheme object so
    /// that no ColorSchemeChanged event is raised (which would invalidate the
    /// whole form on every button repaint).
    /// </remarks>
    public class FixedColorMaterialButton : MaterialButton
    {
        private static readonly FieldInfo PrimaryColorField = typeof(MaterialColorScheme).GetField("PrimaryColor");
        private static readonly FieldInfo PrimaryBrushField = typeof(MaterialColorScheme).GetField("PrimaryBrush");
        private static readonly FieldInfo LightPrimaryColorField = typeof(MaterialColorScheme).GetField("LightPrimaryColor");

        private bool _painting;
        private SolidBrush _fixedBrush;
        private Color _brushColor;

        /// <summary>
        /// Fill color used when <see cref="MaterialButton.Type"/> is Contained.
        /// Defaults to #0067C0 (Windows accent blue).
        /// </summary>
        public Color FixedColor = Color.FromArgb(0x00, 0x67, 0xC0);

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_painting || Type != MaterialButtonType.Contained ||
                PrimaryColorField == null || PrimaryBrushField == null || LightPrimaryColorField == null)
            {
                base.OnPaint(e);
                return;
            }

            _painting = true;
            try
            {
                var scheme = MaterialSkinManager.Instance.ColorScheme;
                var primaryColor = (Color)PrimaryColorField.GetValue(scheme);
                var primaryBrush = (SolidBrush)PrimaryBrushField.GetValue(scheme);
                var lightPrimaryColor = (Color)LightPrimaryColorField.GetValue(scheme);

                if (_fixedBrush == null || _brushColor != FixedColor)
                {
                    if (_fixedBrush != null)
                    {
                        _fixedBrush.Dispose();
                    }
                    _fixedBrush = new SolidBrush(FixedColor);
                    _brushColor = FixedColor;
                }

                try
                {
                    PrimaryColorField.SetValue(scheme, FixedColor);
                    PrimaryBrushField.SetValue(scheme, _fixedBrush);
                    LightPrimaryColorField.SetValue(scheme, ControlPaint.Light(FixedColor, 0.6f));
                    base.OnPaint(e);
                }
                finally
                {
                    PrimaryColorField.SetValue(scheme, primaryColor);
                    PrimaryBrushField.SetValue(scheme, primaryBrush);
                    LightPrimaryColorField.SetValue(scheme, lightPrimaryColor);
                }
            }
            finally
            {
                _painting = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _fixedBrush != null)
            {
                _fixedBrush.Dispose();
                _fixedBrush = null;
            }
            base.Dispose(disposing);
        }
    }
}