using System;
using Microsoft.Win32;

namespace PnpUtilGui.Utils
{
    /// <summary>
    /// Watches the Windows "app mode" personalization setting (light/dark) and
    /// raises <see cref="DarkModeChanged"/> whenever it changes, so the UI can
    /// follow the operating system theme in real time.
    /// </summary>
    public static class OsThemeWatcher
    {
        private static bool _isDarkMode = ReadWindowsDarkMode();
        private static bool _initialized;

        /// <summary>
        /// Raised when the Windows app mode changes. The parameter indicates
        /// whether dark mode is now active.
        /// </summary>
        public static event Action<bool> DarkModeChanged;

        /// <summary>
        /// Gets a value indicating whether Windows currently uses the dark
        /// theme for applications.
        /// </summary>
        public static bool IsDarkMode => _isDarkMode;

        /// <summary>
        /// Starts listening for OS theme changes. Safe to call multiple times.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;
            SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
        }

        /// <summary>
        /// Stops listening for OS theme changes.
        /// </summary>
        public static void Shutdown()
        {
            if (!_initialized)
            {
                return;
            }

            _initialized = false;
            SystemEvents.UserPreferenceChanged -= OnUserPreferenceChanged;
        }

        private static void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            // "VisualStyle" is the category fired when the Windows theme or the
            // app light/dark mode changes; "General" is kept as a fallback
            // because the exact category differs between Windows versions.
            if (e.Category != UserPreferenceCategory.VisualStyle &&
                e.Category != UserPreferenceCategory.General)
            {
                return;
            }

            var dark = ReadWindowsDarkMode();
            if (dark == _isDarkMode)
            {
                return;
            }

            _isDarkMode = dark;
            DarkModeChanged?.Invoke(dark);
        }

        private static bool ReadWindowsDarkMode()
        {
            try
            {
                using (var key = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    // AppsUseLightTheme: 1 = light mode, 0 = dark mode.
                    if (key?.GetValue("AppsUseLightTheme") is int value)
                    {
                        return value == 0;
                    }
                }
            }
            catch
            {
                // Registry unavailable (restricted system) - fall back to light.
            }

            return false;
        }
    }
}
