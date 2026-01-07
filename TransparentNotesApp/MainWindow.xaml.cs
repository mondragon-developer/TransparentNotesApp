using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Web.WebView2.Core;

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
        /// Registers a global Alt+P hotkey for the specified window.
        /// The hotkey can be detected even when the window is not in focus.
        /// </summary>
        /// <param name="window">The window that will receive the hotkey message.</param>
        public static void RegisterGlobalHotkey(Window window)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            RegisterHotKey(hwnd, 1, MOD_ALT, VK_P);
        }

        /// <summary>
        /// Unregisters the global Alt+P hotkey for cleanup purposes.
        /// Should be called when the application is shutting down.
        /// </summary>
        /// <param name="window">The window to unregister the hotkey from.</param>
        public static void UnregisterGlobalHotkey(Window window)
        {
            var hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            UnregisterHotKey(hwnd, 1);
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
        private Point lastMousePosition;
        private bool isMouseTransparent = false;
        private IntPtr hwnd;
        private readonly string notesFilePath;

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
                WindowHelper.RegisterGlobalHotkey(this);

                // Initialize UI elements with current values
                OpacityTextBox.Text = this.Opacity.ToString("F2");
                FontSizeTextBox.Text = NotesRichTextBox.FontSize.ToString("F0");

                // Register message hook for global hotkey detection
                var source = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
                if (source != null)
                    source.AddHook(WndProc);

                // Load saved notes
                LoadNotes();
            };
            this.Closing += (s, e) =>
            {
                SaveNotes();
                WindowHelper.UnregisterGlobalHotkey(this);
            };
        }

        /// <summary>
        /// Message hook for processing Windows messages.
        /// Specifically handles WM_HOTKEY messages for global Alt+P hotkey detection.
        /// </summary>
        /// <returns>IntPtr.Zero to continue message processing.</returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312; // Hotkey message constant
            if (msg == WM_HOTKEY && wParam.ToInt32() == 1)
            {
                isMouseTransparent = !isMouseTransparent;
                WindowHelper.SetMouseTransparent(this, isMouseTransparent);
                AlwaysOnTopCheckBox.IsChecked = isMouseTransparent;
                handled = true;
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles mouse down event on the window.
        /// Enables window dragging when left mouse button is pressed.
        /// </summary>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
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
            lastMousePosition = new Point(pos.X, pos.Y);
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

                lastMousePosition = new Point(currentPos.X, currentPos.Y);
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
        /// Handles key down event in the font size input field.
        /// Applies font size change when Enter key is pressed.
        /// Validates input range (8 to 32).
        /// </summary>
        private void FontSizeTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (NotesRichTextBox != null && double.TryParse(FontSizeTextBox.Text, out double fontSize))
                {
                    fontSize = Math.Max(8, Math.Min(32, fontSize));
                    NotesRichTextBox.FontSize = fontSize;
                    FontSizeTextBox.Text = fontSize.ToString("F0");
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handles the checked state change of the Always on Top checkbox.
        /// Enables topmost window behavior and mouse pass-through mode.
        /// </summary>
        private void AlwaysOnTopCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
            isMouseTransparent = true;
            WindowHelper.SetMouseTransparent(this, true);
        }

        /// <summary>
        /// Handles the unchecked state change of the Always on Top checkbox.
        /// Disables topmost window behavior and mouse pass-through mode.
        /// </summary>
        private void AlwaysOnTopCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
            isMouseTransparent = false;
            WindowHelper.SetMouseTransparent(this, false);
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
    }
}