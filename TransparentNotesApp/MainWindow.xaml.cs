using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Core;
using Forms = System.Windows.Forms;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using DragEventArgs = System.Windows.DragEventArgs;
using Rectangle = System.Windows.Shapes.Rectangle;
using DataFormats = System.Windows.DataFormats;
using MessageBox = System.Windows.MessageBox;

namespace TransparentNotesApp
{
    /// <summary>
    /// Provides Windows API interoperability for advanced window management.
    /// Handles screen capture exclusion, mouse transparency, and hotkey registration.
    /// </summary>
    public static class WindowHelper
    {
        private const int WDA_EXCLUDEFROMCAPTURE = 17;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WM_HOTKEY = 0x0312;

        [DllImport("user32.dll")]
        private static extern int SetWindowDisplayAffinity(IntPtr hwnd, int dwAffinity);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const uint MOD_ALT = 1;
        private const uint VK_P = 0x50;
        private const uint VK_S = 0x53;
        private const uint VK_H = 0x48;

        /// <summary>
        /// Hides the specified window from screen capture and screen recording software.
        /// Uses SetWindowDisplayAffinity with WDA_EXCLUDEFROMCAPTURE flag.
        /// </summary>
        /// <param name="window">The window to hide from screen capture.</param>
        public static void HideFromScreenCapture(Window window)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            SetWindowDisplayAffinity(hwnd, WDA_EXCLUDEFROMCAPTURE);
        }

        /// <summary>
        /// Sets the mouse transparency of a window for click-through functionality.
        /// When enabled, mouse clicks and interactions pass through to underlying windows.
        /// </summary>
        /// <param name="window">The window to modify.</param>
        /// <param name="transparent">True to enable click-through, false to disable.</param>
        public static void SetMouseTransparent(Window window, bool transparent)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            if (transparent)
                SetWindowLong(hwnd, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT);
            else
                SetWindowLong(hwnd, GWL_EXSTYLE, exStyle & ~WS_EX_TRANSPARENT);
        }

        /// <summary>
        /// Registers global hotkeys for the specified window.
        /// Alt+P: Toggle pass-through mode
        /// Alt+S: Bring window to front (show)
        /// Alt+H: Hide window
        /// </summary>
        /// <param name="window">The window that will receive the hotkey message.</param>
        public static void RegisterGlobalHotkeys(Window window)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            RegisterHotKey(hwnd, 1, MOD_ALT, VK_P);  // Alt+P for pass-through
            RegisterHotKey(hwnd, 2, MOD_ALT, VK_S);  // Alt+S for show/front
            RegisterHotKey(hwnd, 3, MOD_ALT, VK_H);  // Alt+H for hide
        }

        /// <summary>
        /// Unregisters all global hotkeys for cleanup purposes.
        /// Should be called when the application is shutting down.
        /// </summary>
        /// <param name="window">The window to unregister hotkeys from.</param>
        public static void UnregisterGlobalHotkeys(Window window)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            UnregisterHotKey(hwnd, 1);  // Alt+P
            UnregisterHotKey(hwnd, 2);  // Alt+S
            UnregisterHotKey(hwnd, 3);  // Alt+H
        }
    }
    /// <summary>
    /// Main window for the Transparent Notes Application.
    /// Manages transparency, resizing, opacity control, and pass-through mouse mode.
    /// This window is hidden from screen capture software on Windows systems.
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isResizing = false;
        private string resizeEdge = "";
        private System.Windows.Point lastMousePosition;
        private System.Windows.Point resizeStartPosition;
        private bool isMouseTransparent = false;
        private IntPtr hwnd;
        private readonly string notesFilePath;
        private Forms.NotifyIcon? trayIcon;
        private TextSelection? savedSelection;
        private System.Windows.Threading.DispatcherTimer? topmostTimer;
        private double zoomLevel = 1.0;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// Sets up event handlers for window loading and closing.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Initialize notes file path in AppData
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string appFolder = System.IO.Path.Combine(appDataPath, "TransparentNotesApp");
            Directory.CreateDirectory(appFolder);
            notesFilePath = System.IO.Path.Combine(appFolder, "notes.rtf");

            this.Loaded += (s, e) =>
            {
                hwnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;
                WindowHelper.HideFromScreenCapture(this);
                WindowHelper.RegisterGlobalHotkeys(this);

                // Initialize UI elements with current values
                OpacityTextBox.Text = this.Opacity.ToString("F2");
                FontSizeTextBox.Text = NotesRichTextBox.FontSize.ToString("F0");

                // Register message hook for global hotkey detection
                var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
                if (source != null)
                    source.AddHook(WndProc);

                // Initialize system tray icon
                InitializeTrayIcon();

                // Initialize WebView2 in InPrivate mode for privacy
                InitializeWebViewPrivate();

                // Load saved notes
                LoadNotes();
            };
            this.Closing += (s, e) =>
            {
                SaveNotes();
                WindowHelper.UnregisterGlobalHotkeys(this);
                topmostTimer?.Stop();
                trayIcon?.Dispose();

                // Clean up WebView2 temp data for privacy
                try
                {
                    Browser?.Dispose();
                    var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "TransparentNotesWebView");
                    if (Directory.Exists(tempPath))
                    {
                        Directory.Delete(tempPath, true);
                    }
                }
                catch { /* Ignore cleanup errors */ }
            };
        }

        /// <summary>
        /// Initializes WebView2 in InPrivate mode for privacy.
        /// No browsing history, cookies, or cache will be saved.
        /// </summary>
        private async void InitializeWebViewPrivate()
        {
            try
            {
                // Create InPrivate environment (no persistent data)
                var options = new CoreWebView2EnvironmentOptions();
                var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "TransparentNotesWebView");
                var env = await CoreWebView2Environment.CreateAsync(null, tempPath, options);

                // Initialize with InPrivate profile
                await Browser.EnsureCoreWebView2Async(env);

                // Additional privacy settings
                if (Browser.CoreWebView2 != null)
                {
                    // Disable developer tools
                    Browser.CoreWebView2.Settings.AreDevToolsEnabled = false;
                    // Disable context menu for cleaner experience
                    Browser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                    // Disable status bar
                    Browser.CoreWebView2.Settings.IsStatusBarEnabled = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 init failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Initializes the system tray icon with context menu for Show/Exit actions.
        /// The app is hidden from taskbar, so this provides the only way to interact.
        /// </summary>
        private void InitializeTrayIcon()
        {
            trayIcon = new Forms.NotifyIcon
            {
                Icon = System.Drawing.Icon.ExtractAssociatedIcon(Environment.ProcessPath ?? ""),
                Visible = true,
                Text = "Transparent Notes"
            };

            var contextMenu = new Forms.ContextMenuStrip();
            contextMenu.Items.Add("Show/Hide", null, (s, e) =>
            {
                if (this.Visibility == Visibility.Visible)
                    this.Hide();
                else
                    this.Show();
            });
            contextMenu.Items.Add("-"); // Separator
            contextMenu.Items.Add("Exit", null, (s, e) =>
            {
                this.Close();
            });

            trayIcon.ContextMenuStrip = contextMenu;
            trayIcon.DoubleClick += (s, e) =>
            {
                if (this.Visibility == Visibility.Visible)
                    this.Hide();
                else
                    this.Show();
            };
        }

        /// <summary>
        /// Message hook for processing Windows messages.
        /// Handles WM_HOTKEY messages for global hotkey detection.
        /// Alt+P (id=1): Toggle pass-through mode
        /// Alt+S (id=2): Bring window to front
        /// Alt+H (id=3): Hide window
        /// </summary>
        /// <returns>IntPtr.Zero to continue message processing.</returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                int hotkeyId = wParam.ToInt32();
                if (hotkeyId == 1)  // Alt+P: Toggle pass-through
                {
                    isMouseTransparent = !isMouseTransparent;
                    WindowHelper.SetMouseTransparent(this, isMouseTransparent);
                    AlwaysOnTopCheckBox.IsChecked = isMouseTransparent;
                    handled = true;
                }
                else if (hotkeyId == 2)  // Alt+S: Bring to front
                {
                    this.Show();
                    this.Activate();
                    this.Topmost = true;  // Temporarily force topmost
                    this.Topmost = AlwaysOnTopCheckBox.IsChecked == true;  // Restore setting
                    handled = true;
                }
                else if (hotkeyId == 3)  // Alt+H: Hide window
                {
                    this.Hide();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles mouse down event on the window.
        /// Enables window dragging when left mouse button is pressed.
        /// Also brings window to front when clicked.
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Bring window to front when clicked
            Activate();
        }

        /// <summary>
        /// Handles mouse down on drag areas (header, toolbar, edges).
        /// Enables window dragging when left mouse button is pressed.
        /// </summary>
        private void DragArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Activate();
                DragMove();
            }
        }

        /// <summary>
        /// Handles mouse down event on the resize area (bottom-right corner).
        /// Initiates window resize operation and captures mouse.
        /// Temporarily disables transparency to improve visual feedback during resize.
        /// </summary>
        private void ResizeArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isResizing = true;
            // Temporarily disable transparency during resize operation
            if (isMouseTransparent)
            {
                WindowHelper.SetMouseTransparent(this, false);
            }
            // Store current mouse position for delta calculation
            var pos = e.GetPosition(null);
            lastMousePosition = new System.Windows.Point(pos.X, pos.Y);
            ((Rectangle)sender).CaptureMouse();
            e.Handled = true;
        }

        /// <summary>
        /// Handles mouse move event during window resize operation.
        /// Calculates position deltas and updates window dimensions accordingly.
        /// Enforces minimum window size constraints.
        /// </summary>
        private void ResizeArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing && e.LeftButton == MouseButtonState.Pressed)
            {
                var currentPos = e.GetPosition(null);
                double deltaX = currentPos.X - lastMousePosition.X;
                double deltaY = currentPos.Y - lastMousePosition.Y;

                double newWidth = this.Width + deltaX;
                double newHeight = this.Height + deltaY;

                if (newWidth >= 475)
                    this.Width = newWidth;
                if (newHeight >= 50)
                    this.Height = newHeight;

                lastMousePosition = new System.Windows.Point(currentPos.X, currentPos.Y);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles mouse up event to complete the resize operation.
        /// Releases mouse capture and restores transparency if it was enabled.
        /// </summary>
        private void ResizeArea_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isResizing)
            {
                isResizing = false;
                ((Rectangle)sender).ReleaseMouseCapture();
                // Restore transparency after resize operation completes
                if (isMouseTransparent)
                {
                    WindowHelper.SetMouseTransparent(this, true);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles mouse down on any resize edge/corner.
        /// </summary>
        private void ResizeEdge_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isResizing = true;
            resizeEdge = ((Rectangle)sender).Tag?.ToString() ?? "";
            if (isMouseTransparent)
            {
                WindowHelper.SetMouseTransparent(this, false);
            }
            resizeStartPosition = PointToScreen(e.GetPosition(this));
            ((Rectangle)sender).CaptureMouse();
            e.Handled = true;
        }

        /// <summary>
        /// Handles mouse move during edge/corner resize.
        /// </summary>
        private void ResizeEdge_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isResizing || e.LeftButton != MouseButtonState.Pressed) return;

            var currentPos = PointToScreen(e.GetPosition(this));
            double deltaX = currentPos.X - resizeStartPosition.X;
            double deltaY = currentPos.Y - resizeStartPosition.Y;

            double newLeft = this.Left;
            double newTop = this.Top;
            double newWidth = this.Width;
            double newHeight = this.Height;

            // Handle horizontal resizing
            if (resizeEdge.Contains("Left"))
            {
                newWidth = Math.Max(this.MinWidth, this.Width - deltaX);
                if (newWidth > this.MinWidth)
                    newLeft = this.Left + deltaX;
            }
            else if (resizeEdge.Contains("Right"))
            {
                newWidth = Math.Max(this.MinWidth, this.Width + deltaX);
            }

            // Handle vertical resizing
            if (resizeEdge.Contains("Top"))
            {
                newHeight = Math.Max(this.MinHeight, this.Height - deltaY);
                if (newHeight > this.MinHeight)
                    newTop = this.Top + deltaY;
            }
            else if (resizeEdge.Contains("Bottom"))
            {
                newHeight = Math.Max(this.MinHeight, this.Height + deltaY);
            }

            this.Left = newLeft;
            this.Top = newTop;
            this.Width = newWidth;
            this.Height = newHeight;

            resizeStartPosition = currentPos;
            e.Handled = true;
        }

        /// <summary>
        /// Handles mouse up to complete edge/corner resize.
        /// </summary>
        private void ResizeEdge_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isResizing)
            {
                isResizing = false;
                resizeEdge = "";
                ((Rectangle)sender).ReleaseMouseCapture();
                if (isMouseTransparent)
                {
                    WindowHelper.SetMouseTransparent(this, true);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles Ctrl+MouseWheel for zoom in/out on text and web tabs.
        /// </summary>
        private void TabControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control) return;

            double zoomDelta = e.Delta > 0 ? 0.1 : -0.1;
            zoomLevel = Math.Max(0.5, Math.Min(3.0, zoomLevel + zoomDelta));

            // Apply zoom to RichTextBox (text tab)
            if (NotesRichTextBox != null)
            {
                var transform = new System.Windows.Media.ScaleTransform(zoomLevel, zoomLevel);
                NotesRichTextBox.LayoutTransform = transform;
            }

            // Apply zoom to WebView2 (web tab)
            if (Browser?.CoreWebView2 != null)
            {
                Browser.ZoomFactor = zoomLevel;
            }

            e.Handled = true;
        }

        /// <summary>
        /// Handles key down event in the opacity input field.
        /// Applies opacity change when Enter key is pressed.
        /// Validates input range (0.1 to 1.0).
        /// </summary>
        private void OpacityTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (double.TryParse(OpacityTextBox.Text, out double opacity))
                {
                    opacity = Math.Max(0.1, Math.Min(1.0, opacity));
                    this.Opacity = opacity;
                    OpacityTextBox.Text = opacity.ToString("F2");
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Saves the RichTextBox selection when FontSizeTextBox receives focus.
        /// This preserves the selection so font size can be applied to it.
        /// </summary>
        private void FontSizeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            savedSelection = NotesRichTextBox.Selection;
        }

        /// <summary>
        /// Handles key down event in the font size input field.
        /// Applies font size change when Enter key is pressed.
        /// Validates input range (8 to 72).
        /// </summary>
        private void FontSizeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (NotesRichTextBox != null && double.TryParse(FontSizeTextBox.Text, out double fontSize))
                {
                    fontSize = Math.Max(8, Math.Min(72, fontSize));

                    // Apply to selection if there is one, otherwise set default
                    if (savedSelection != null && !savedSelection.IsEmpty)
                    {
                        savedSelection.ApplyPropertyValue(TextElement.FontSizeProperty, fontSize);
                    }
                    else
                    {
                        NotesRichTextBox.FontSize = fontSize;
                    }

                    FontSizeTextBox.Text = fontSize.ToString("F0");
                    NotesRichTextBox.Focus();
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the checked state change of the Always on Top checkbox.
        /// Enables topmost window behavior and mouse pass-through mode.
        /// Starts a timer to keep window on top against fullscreen apps like PowerPoint.
        /// </summary>
        private void AlwaysOnTopCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            isMouseTransparent = true;
            WindowHelper.SetMouseTransparent(this, true);

            // Start timer to keep on top against fullscreen apps (PowerPoint, etc.)
            topmostTimer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            topmostTimer.Tick += (s, args) =>
            {
                if (this.Topmost)
                {
                    this.Topmost = false;
                    this.Topmost = true;
                }
            };
            topmostTimer.Start();
        }

        /// <summary>
        /// Handles the unchecked state change of the Always on Top checkbox.
        /// Disables topmost window behavior and mouse pass-through mode.
        /// Stops the topmost timer.
        /// </summary>
        private void AlwaysOnTopCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            isMouseTransparent = false;
            WindowHelper.SetMouseTransparent(this, false);

            // Stop the topmost timer
            topmostTimer?.Stop();
            topmostTimer = null;
        }

        /// <summary>
        /// Handles the Bold button click - toggles bold formatting on selection.
        /// </summary>
        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            var selection = NotesRichTextBox.Selection;
            object currentWeight = selection.GetPropertyValue(TextElement.FontWeightProperty);
            FontWeight newWeight = (currentWeight is FontWeight weight && weight == FontWeights.Bold)
                ? FontWeights.Normal
                : FontWeights.Bold;
            selection.ApplyPropertyValue(TextElement.FontWeightProperty, newWeight);
            NotesRichTextBox.Focus();
        }

        /// <summary>
        /// Handles the Bullet List button click - creates a bullet list.
        /// </summary>
        private void BulletListButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyListStyle(TextMarkerStyle.Disc);
            NotesRichTextBox.Focus();
        }

        /// <summary>
        /// Handles the Numbered List button click - creates a numbered list.
        /// </summary>
        private void NumberedListButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyListStyle(TextMarkerStyle.Decimal);
            NotesRichTextBox.Focus();
        }

        /// <summary>
        /// Applies the specified list style to the current selection or paragraph.
        /// </summary>
        private void ApplyListStyle(TextMarkerStyle markerStyle)
        {
            var selection = NotesRichTextBox.Selection;
            var start = selection.Start.Paragraph;
            var end = selection.End.Paragraph;

            if (start == null) return;

            // Check if already in a list
            if (start.Parent is ListItem listItem && listItem.Parent is List existingList)
            {
                if (existingList.MarkerStyle == markerStyle)
                {
                    // Remove from list
                    RemoveFromList(listItem);
                    return;
                }
                else
                {
                    // Change list type
                    existingList.MarkerStyle = markerStyle;
                    return;
                }
            }

            // Create new list
            var list = new List { MarkerStyle = markerStyle };
            var document = NotesRichTextBox.Document;

            // Collect paragraphs to convert
            var paragraphsToConvert = new System.Collections.Generic.List<Paragraph>();
            Block? currentBlock = start;

            while (currentBlock != null)
            {
                if (currentBlock is Paragraph para)
                {
                    paragraphsToConvert.Add(para);
                }
                if (currentBlock == end) break;
                currentBlock = currentBlock.NextBlock;
            }

            if (paragraphsToConvert.Count == 0) return;

            // Insert list before first paragraph
            document.Blocks.InsertBefore(paragraphsToConvert[0], list);

            // Move paragraphs into list items
            foreach (var para in paragraphsToConvert)
            {
                document.Blocks.Remove(para);
                var item = new ListItem(para);
                list.ListItems.Add(item);
            }
        }

        /// <summary>
        /// Removes a list item from its parent list, converting back to a paragraph.
        /// </summary>
        private void RemoveFromList(ListItem listItem)
        {
            if (listItem.Parent is not List parentList) return;

            var document = NotesRichTextBox.Document;
            var para = new Paragraph();

            foreach (var block in listItem.Blocks.ToList())
            {
                if (block is Paragraph p)
                {
                    foreach (var inline in p.Inlines.ToList())
                    {
                        para.Inlines.Add(inline);
                    }
                }
            }

            document.Blocks.InsertAfter(parentList, para);
            parentList.ListItems.Remove(listItem);

            if (parentList.ListItems.Count == 0)
            {
                document.Blocks.Remove(parentList);
            }
        }

        /// <summary>
        /// Updates the Bold toggle button state based on current selection.
        /// </summary>
        private void NotesRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object fontWeight = NotesRichTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty);
            BoldButton.IsChecked = (fontWeight is FontWeight weight && weight == FontWeights.Bold);
        }

        /// <summary>
        /// Saves the RichTextBox content to an RTF file for persistence.
        /// </summary>
        private void SaveNotes()
        {
            try
            {
                using var fs = new FileStream(notesFilePath, FileMode.Create);
                var range = new TextRange(
                    NotesRichTextBox.Document.ContentStart,
                    NotesRichTextBox.Document.ContentEnd);
                range.Save(fs, DataFormats.Rtf);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving notes: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads previously saved RTF content into the RichTextBox.
        /// </summary>
        private void LoadNotes()
        {
            try
            {
                if (File.Exists(notesFilePath))
                {
                    using var fs = new FileStream(notesFilePath, FileMode.Open);
                    var range = new TextRange(
                        NotesRichTextBox.Document.ContentStart,
                        NotesRichTextBox.Document.ContentEnd);
                    range.Load(fs, DataFormats.Rtf);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading notes: {ex.Message}");
            }
        }

        /// <summary>
        /// Handles the Drop event for the Web tab.
        /// Extracts URL from dropped text and navigates the browser to it.
        /// </summary>
        private void WebTab_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string url = (string)e.Data.GetData(DataFormats.Text);
                
                // Simple check if it looks like a URL, if not prepend https://
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                     // Basic check if it's likely a domain
                     if (url.Contains(".") && !url.Contains(" "))
                     {
                         url = "https://" + url;
                     }
                }

                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    try
                    {
                        Browser.Source = new Uri(url);
                        WebHintText.Visibility = Visibility.Collapsed;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Invalid URL: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Called when the WebView2 has finished loading a page.
        /// Injects JavaScript to make the page background transparent.
        /// </summary>
        private async void Browser_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                // Update URL bar with current URL
                if (Browser.Source != null)
                {
                    UrlTextBox.Text = Browser.Source.ToString();
                }

                // Inject JavaScript to set the background of the body and html to transparent
                // This allows the WPF window's transparency to show through
                try
                {
                    string script = "document.documentElement.style.background = 'transparent'; document.body.style.background = 'transparent';";
                    await Browser.ExecuteScriptAsync(script);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Script injection failed: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Handles Enter key press in URL bar to navigate.
        /// </summary>
        private void UrlTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                NavigateToUrl(UrlTextBox.Text);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles Go button click to navigate.
        /// </summary>
        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToUrl(UrlTextBox.Text);
        }

        /// <summary>
        /// Navigates the browser to the specified URL.
        /// </summary>
        private void NavigateToUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return;

            // Add https:// if no protocol specified
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                try
                {
                    Browser.Source = new Uri(url);
                    WebHintText.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Invalid URL: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid URL");
            }
        }
    }
}