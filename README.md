# Transparent Notes App

A sophisticated Windows WPF application that provides a transparent, always-visible notes window that remains hidden from screen capture software (Zoom, Microsoft Teams, Google Meet, etc.).

## Developer

**Created by: Jose Mondragon (MDragon Solutions)**

This application was developed with precision and care to provide a secure notes-taking experience while screen sharing.

## Features

### Core Features
- **Screen Capture Invisible**: The window is completely hidden from screen sharing software (Zoom, Microsoft Teams, Google Meet, OBS, etc.) using Windows API `SetWindowDisplayAffinity`
- **Hidden from Taskbar**: Window doesn't appear in the taskbar - fully stealth mode during screen sharing
- **System Tray Integration**: Access the app via system tray icon (double-click to show/hide, right-click for menu)
- **Transparent Window**: Semi-transparent notes window (adjustable opacity: 0.1 - 1.0) that doesn't interfere with content visibility
- **Rich Text Formatting**: Bold text, bullet lists, and numbered lists with a formatting toolbar
- **Paste from Word/Google Docs**: Supports pasting formatted content including tables from external applications
- **Automatic Note Persistence**: Notes are automatically saved and restored between sessions (stored as RTF)
- **Web Tab with URL Bar**: Type or paste URLs, drag and drop links, browse with transparent background - fully interactive (click links, scroll, fill forms, watch videos)
- **Private Browsing**: WebView2 runs in InPrivate mode - no history, cookies, or cache saved; all data deleted on close
- **Resizable Window**: Drag any of the 4 corner handles to resize the window
- **Draggable Header**: Drag the header bar or toolbar to move the window
- **Always on Top**: Toggle window to stay above all other applications (even PowerPoint presentations)
- **Pass-Through Mode**: Press **Alt+P** to toggle mouse pass-through mode - when enabled, mouse clicks pass through the window to applications underneath
- **Adjustable Opacity**: Use numeric input (0.1 - 1.0) with Enter key to set transparency level
- **Adjustable Font Size**: Use numeric input (8 - 72) with Enter key to change selected text or default size
- **Zoom Support**: Use **Ctrl+Mouse Wheel** to zoom in/out on text and web content
- **Tabbed Interface**: Switch between Text notes, Image content, and Web browser
- **Global Hotkeys**: Alt+P (pass-through), Alt+S (show/front), Alt+H (hide)
- **Code Signed**: Digitally signed with Microsoft Azure Trusted Signing for security and trust

## Quick Start

### System Requirements
- Windows 10 or later
- .NET 10.0 or compatible runtime
- No external dependencies required

### Installation

#### Option 1: Download Release (Recommended)
1. Go to the [Releases](https://github.com/yourusername/TransparentNotesApp/releases) page
2. Download `TransparentNotesApp.exe` (single-file, self-contained)
3. Double-click to run - no installation required!

#### Option 2: Build from Source
1. Clone the repository
2. Navigate to the `TransparentNotesApp` folder
3. Run the application:
   ```bash
   dotnet run
   ```

Or build and publish a release:
```bash
dotnet publish -c Release
# Output: TransparentNotesApp/bin/Release/net10.0-windows/win-x64/publish/TransparentNotesApp.exe
```

## Usage Guide

### Window Controls

| Control | Action | Purpose |
|---------|--------|---------|
| **System Tray Icon** | Double-click | Show/Hide the application window |
| **System Tray Icon** | Right-click | Access menu (Show/Hide, Exit) |
| **Header Bar** | Drag | Move the window |
| **Toolbar** | Drag | Move the window |
| **Corner Handles** | Drag | Resize the window (4 visible corners) |
| **Always on Top Checkbox** | Check/Uncheck | Toggle window to stay above all other windows and enable pass-through mode |
| **Opacity TextBox** | Type value (0.1-1.0) + Enter | Adjust window transparency (0.1 = very transparent, 1.0 = opaque) |
| **Font Size TextBox** | Type value (8-72) + Enter | Change font size of selected text or set default size |
| **Bold Button (B)** | Click | Toggle bold formatting on selected text |
| **Bullet List Button (•)** | Click | Create or toggle bullet list |
| **Numbered List Button (1.)** | Click | Create or toggle numbered list |
| **Web Tab URL Bar** | Type URL + Enter or Go | Navigate to any website |
| **Web Tab** | Drag & drop URL | Load a web page with transparent background |

### Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| **Alt+P** | Toggle mouse pass-through mode (global) |
| **Alt+S** | Show window / Bring to front (global) |
| **Alt+H** | Hide window (global) |
| **Ctrl+Mouse Wheel** | Zoom in/out on text and web content |
| **Enter** | Apply value in Opacity/Font Size/URL fields |

### Minimum Window Size
- **Width**: 475 pixels
- **Height**: 100 pixels

## Technical Architecture

### Technologies
- Framework: WPF (Windows Presentation Foundation)
- .NET Version: .NET 10.0 (Windows-specific)
- Language: C# 12
- Windows APIs: P/Invoke for advanced window management

### Key Components

#### WindowHelper Class
Static utility class that handles Windows API interactions:
- `HideFromScreenCapture()`: Uses `SetWindowDisplayAffinity` to hide window from screen capture
- `SetMouseTransparent()`: Uses `SetWindowLong`/`GetWindowLong` with `WS_EX_TRANSPARENT` flag for click-through functionality
- `RegisterGlobalHotkey()`: Registers Alt+P global hotkey
- `UnregisterGlobalHotkey()`: Cleans up hotkey registration

#### MainWindow Class
Main WPF window with event handlers:
- **ResizeArea**: Transparent 40x40 Rectangle overlay in bottom-right corner
  - `ResizeArea_MouseDown()`: Starts resize operation, temporarily disables transparency
  - `ResizeArea_MouseMove()`: Tracks mouse movement and updates window dimensions
  - `ResizeArea_MouseUp()`: Completes resize, re-enables transparency
- **Window_MouseDown()**: Enables window dragging with `DragMove()`
- **Input Handlers**: 
  - `OpacityTextBox_PreviewKeyDown()`: Updates opacity on Enter key
  - `FontSizeTextBox_PreviewKeyDown()`: Updates font size on Enter key
- **Hotkey Handler**: `WndProc()` message hook for global Alt+P detection
- **Checkbox Handlers**: Toggle "Always on Top" and mouse transparency

### XAML Structure
```
MainWindow (Transparent, No Title Bar, Hidden from Taskbar)
├── Grid (2 rows: Auto for controls, * for content)
│   ├── StackPanel (Row 0 - Controls)
│   │   ├── AlwaysOnTopCheckBox + Label + Alt+P Tip
│   │   ├── OpacityTextBox (0.1-1.0)
│   │   └── FontSizeTextBox (8-72)
│   ├── TabControl (Row 1 - Content)
│   │   ├── Text Tab
│   │   │   ├── ToolBar (Bold, Bullet List, Numbered List)
│   │   │   └── RichTextBox (FlowDocument)
│   │   ├── Image Tab (Placeholder)
│   │   └── Web Tab
│   │       └── WebView2 (Drag & drop URL, transparent background)
│   └── ResizeArea (Rectangle overlay - transparent, 40x40)
└── System Tray Icon (NotifyIcon with context menu)
```

### Notes Storage
Notes are automatically saved to:
```
%LOCALAPPDATA%\TransparentNotesApp\notes.rtf
```
(e.g., `C:\Users\YourName\AppData\Local\TransparentNotesApp\notes.rtf`)

---

## Security & Privacy Features

1. **Screen Capture Protection**: Uses Windows native API to exclude window from screen sharing
   - Tested with: Zoom, Microsoft Teams, Google Meet, OBS
   - Note: This only works on Windows; effectiveness may vary by platform/software

2. **Private Web Browsing**: WebView2 runs in InPrivate mode
   - No browsing history saved
   - No cookies persisted after session
   - No cache stored
   - Temp data folder deleted on app close

3. **No Data Transmission**: Application stores data locally only
4. **No External Connections**: Works completely offline (except Web tab)
5. **No Tracking**: Zero telemetry or analytics

## Advanced Features

### Pass-Through Mode
When "Always on Top" is checked:
- Window remains visible above all other applications
- Mouse clicks and interactions pass through to underlying windows
- Perfect for having notes while interacting with applications underneath
- Toggle with Alt+P (works globally)

### Resize Behavior
- Resize handle automatically appears in bottom-right corner
- Transparency temporarily disabled during resize for better visual feedback
- Transparency re-enabled after resize completes
- Minimum dimensions enforced: 475px wide × 100px tall

### Opacity Control
- Enter values between 0.1 and 1.0
- Press Enter to apply changes
- Real-time preview as you type (before pressing Enter)
- Values are automatically clamped to valid range

## Code Quality

### Code Review Results
- Removed: 2 unused slider event handlers (OpacitySlider_ValueChanged, FontSizeSlider_ValueChanged)
- Removed: 2 unused edge detection methods (GetEdgeAtPosition, UpdateCursor)
- Removed: 2 empty event handlers (Window_MouseMove, Window_MouseUp)
- Fixed: 1 duplicate WM_HOTKEY constant definition
- Cleaned: Removed 7 unused using statements (System.Text, System.Windows.Data, System.Windows.Documents, System.Windows.Media, System.Windows.Media.Imaging, System.Windows.Navigation)
- Result: Clean, maintainable codebase with no dead code

## Known Limitations

1. Image Tab: Currently a placeholder - can be extended to load and display images
2. Windows Only: Application requires Windows OS and relies on Windows-specific APIs
3. Screen Capture Evasion: Effectiveness depends on the specific screen sharing software and its implementation

## Version History

### v1.3 (Current)
- **Web Tab Enhancements**:
  - Added URL bar for typing/pasting URLs
  - Private browsing mode (InPrivate WebView2) - no history, cookies, or cache saved
  - Fully interactive web pages (click links, scroll, fill forms, watch videos)
  - Auto-cleanup of temp data on app close
- **Window Improvements**:
  - Corner resize handles (4 visible corners)
  - Drag header or toolbar to move window
  - Stays on top even against fullscreen apps (PowerPoint presentations)
  - More transparent control backgrounds
- **Global Hotkeys**:
  - Alt+S: Show window / Bring to front
  - Alt+H: Hide window
- **Zoom Support**: Ctrl+Mouse Wheel to zoom text and web content
- Fixed font size to apply to selected text

### v1.2
- Added Web tab with drag-and-drop URL loading
- WebView2 integration with transparent background for web pages
- Hidden from taskbar for complete stealth during screen sharing
- System tray icon with show/hide and exit options
- Fixed font size to apply to selected text (not just new text)
- Increased max font size from 32 to 72

### v1.1
- Added rich text formatting (bold, bullet lists, numbered lists)
- Added automatic note persistence (saves/loads RTF format)
- Added paste support for formatted content from Word/Google Docs
- Added formatting toolbar
- Configured single-file self-contained executable
- Added Azure Trusted Signing for code signing
- Added GitHub Actions workflow for automated signed releases
- Added application icon

### v1.0
- Initial release
- Core features implemented: screen capture hiding, transparency, resizing, pass-through mode
- Numeric controls for opacity and font size (instead of sliders)
- Global Alt+P hotkey for pass-through toggle

## Contributing

This is a personal portfolio project by José Mondragon (MDragon Solutions). 

Suggestions for improvements:
- Implement color themes
- Add more keyboard shortcuts (Ctrl+B for bold, etc.)
- Extend Image tab functionality
- Add italic and underline formatting

## License

This project is created by José Mondragon (MDragon Solutions) for portfolio purposes.

**Created by: José Mondragon - MDragon Solutions**

## Future Enhancements

- Custom themes (dark mode, colors)
- Note templates
- Auto-backup feature
- Search functionality
- Markdown support
- Cloud sync (optional)
- Note encryption
- Export to PDF/TXT
- Italic and underline formatting
- Keyboard shortcuts for formatting (Ctrl+B, Ctrl+I, etc.)

## Project Structure

```
TransparentNotesApp/
├── .github/
│   └── workflows/
│       └── release.yml      # GitHub Actions for signed releases
├── TransparentNotesApp/
│   ├── MainWindow.xaml      # UI Layout definition
│   ├── MainWindow.xaml.cs   # Main window logic & event handlers
│   ├── App.xaml             # Application resources
│   ├── App.xaml.cs          # Application startup logic
│   ├── AssemblyInfo.cs      # Assembly metadata
│   ├── app.ico              # Application icon
│   └── TransparentNotesApp.csproj # Project configuration
└── TransparentNotesApp.Tests/
    └── MainWindowTests.cs   # Unit tests
```

## Windows APIs Used

- SetWindowDisplayAffinity: Hide window from screen capture
- GetWindowLong/SetWindowLong: Manage extended window styles
- WS_EX_TRANSPARENT: Extended window style for click-through
- RegisterHotKey/UnregisterHotKey: Global hotkey registration
- HwndSource.AddHook: Message interception for hotkey handling

## Architecture Diagrams

### System Architecture

```mermaid
graph TB
    subgraph App["Transparent Notes Application"]
        subgraph Window["MainWindow - Transparent WPF Window"]
            Config["Window Configuration<br/>- Hidden from Screen Capture<br/>- Always on Top Support<br/>- Mouse Pass-Through"]
        end
        
        subgraph UI["User Interface Components"]
            Controls["Control Panel<br/>- Opacity Input<br/>- Font Size Input<br/>- Always on Top Checkbox"]
            TabCtrl["TabControl<br/>- Text Tab TextBox<br/>- Image Tab Placeholder"]
            ResizeRect["Resize Area Rectangle<br/>40x40 Bottom-Right Corner"]
        end
        
        subgraph Helper["WindowHelper Class<br/>Static Utility"]
            Hide["HideFromScreenCapture<br/>SetWindowDisplayAffinity"]
            Mouse["SetMouseTransparent<br/>SetWindowLong/GetWindowLong"]
            Hotkey["RegisterGlobalHotkey<br/>Alt+P Detection"]
        end
        
        subgraph WinAPI["Windows API Layer"]
            API1["SetWindowDisplayAffinity"]
            API2["RegisterHotKey"]
            API3["GetWindowLong/SetWindowLong"]
            API4["HwndSource.AddHook"]
        end
    end
    
    Window --> UI
    Window --> Helper
    Helper --> WinAPI
    
    style Window fill:#e1f5ff
    style UI fill:#f3e5f5
    style Helper fill:#e8f5e9
    style WinAPI fill:#fff3e0
```

### Event Flow Diagram

```mermaid
sequenceDiagram
    participant User
    participant MainWindow
    participant WindowHelper
    participant WinAPI as Windows API

    User->>MainWindow: Window Load
    MainWindow->>WindowHelper: HideFromScreenCapture
    WindowHelper->>WinAPI: SetWindowDisplayAffinity
    MainWindow->>WindowHelper: RegisterGlobalHotkey
    WindowHelper->>WinAPI: RegisterHotKey Alt+P

    User->>MainWindow: Drag Window Edge
    MainWindow->>MainWindow: Window_MouseDown
    MainWindow->>MainWindow: DragMove()

    User->>MainWindow: Mouse on Resize Area
    MainWindow->>MainWindow: ResizeArea_MouseDown
    MainWindow->>MainWindow: isResizing = true
    MainWindow->>WindowHelper: Disable Transparency
    MainWindow->>MainWindow: Capture Mouse

    User->>MainWindow: Drag Resize Corner
    MainWindow->>MainWindow: ResizeArea_MouseMove
    MainWindow->>MainWindow: Calculate Delta X/Y
    MainWindow->>MainWindow: Update Width/Height

    User->>MainWindow: Release Mouse
    MainWindow->>MainWindow: ResizeArea_MouseUp
    MainWindow->>WindowHelper: Restore Transparency

    User->>MainWindow: Type Opacity + Enter
    MainWindow->>MainWindow: OpacityTextBox_PreviewKeyDown
    MainWindow->>MainWindow: Parse & Validate 0.1-1.0
    MainWindow->>MainWindow: Set Window.Opacity

    User->>MainWindow: Check Always On Top
    MainWindow->>MainWindow: AlwaysOnTopCheckBox_Checked
    MainWindow->>MainWindow: Set Topmost = true
    MainWindow->>WindowHelper: Enable Click-Through

    User->>WinAPI: Press Alt+P (Global)
    WinAPI->>MainWindow: WndProc - WM_HOTKEY
    MainWindow->>MainWindow: Toggle isMouseTransparent
    MainWindow->>WindowHelper: SetMouseTransparent()
```

### Windows API Integration Diagram

```mermaid
flowchart LR
    subgraph Application["Application Layer"]
        MainWin["MainWindow"]
        Helper["WindowHelper"]
    end
    
    subgraph PInvoke["P/Invoke Layer"]
        SetDispAff["SetWindowDisplayAffinity"]
        RegHotKey["RegisterHotKey"]
        GetWL["GetWindowLong"]
        SetWL["SetWindowLong"]
        HwndSrc["HwndSource.AddHook"]
    end
    
    subgraph Windows["Windows API Layer"]
        user32["user32.dll"]
    end
    
    MainWin -->|Hide from Capture| Helper
    MainWin -->|Register Hotkey| Helper
    MainWin -->|Set Transparency| Helper
    MainWin -->|Message Hook| HwndSrc
    
    Helper -->|WDA_EXCLUDEFROMCAPTURE| SetDispAff
    Helper -->|MOD_ALT + VK_P| RegHotKey
    Helper -->|GWL_EXSTYLE| GetWL
    Helper -->|WS_EX_TRANSPARENT| SetWL
    
    SetDispAff --> user32
    RegHotKey --> user32
    GetWL --> user32
    SetWL --> user32
    HwndSrc --> user32
    
    style Application fill:#e3f2fd
    style PInvoke fill:#f3e5f5
    style Windows fill:#fff3e0
```

### Data Flow - Opacity Control

```mermaid
flowchart TD
    A["User Types Value<br/>e.g., '0.5'"] --> B["TextBox Displays Input"]
    B --> C["User Presses ENTER"]
    C --> D["PreviewKeyDown Event Fires"]
    D --> E{"Parse Double<br/>Successful?"}
    E -->|No| F["Ignore Invalid Input"]
    E -->|Yes| G["Clamp to Range<br/>0.1 - 1.0"]
    G --> H["Set window.Opacity"]
    H --> I["Format Display<br/>0.50"]
    I --> J["Update UI"]
    J --> K["Window Transparency Changed"]
    
    style A fill:#c8e6c9
    style B fill:#c8e6c9
    style C fill:#ffccbc
    style D fill:#b3e5fc
    style E fill:#ffe0b2
    style G fill:#e1bee7
    style H fill:#f0f4c3
    style K fill:#c8e6c9
```

### Pass-Through Mode State Machine

```mermaid
stateDiagram-v2
    [*] --> Normal
    
    Normal: Normal Mode
    Normal: Window captures mouse
    Normal: User interacts with window
    
    PassThrough: Pass-Through Mode
    PassThrough: Mouse clicks pass through
    PassThrough: Interact with app underneath
    PassThrough: Window still visible on top
    
    Normal -->|Alt+P OR<br/>Check Always on Top| PassThrough
    PassThrough -->|Alt+P OR<br/>Uncheck Always on Top| Normal
    
    note right of Normal
        isMouseTransparent = false
        Topmost may be true/false
    end note
    
    note right of PassThrough
        isMouseTransparent = true
        Topmost = true
    end note
```

### Component Interaction Diagram

```mermaid
graph TB
    subgraph Events["User Events"]
        E1["Mouse Down"]
        E2["Mouse Move"]
        E3["Mouse Up"]
        E4["Key Press"]
        E5["Checkbox Toggle"]
        E6["Global Hotkey Alt+P"]
    end
    
    subgraph Handlers["Event Handlers"]
        H1["Window_MouseDown<br/>DragMove"]
        H2["ResizeArea_MouseDown<br/>Start Resize"]
        H3["ResizeArea_MouseMove<br/>Update Size"]
        H4["ResizeArea_MouseUp<br/>End Resize"]
        H5["TextBox_PreviewKeyDown<br/>Validate Input"]
        H6["Checkbox_Checked/Unchecked<br/>Toggle Mode"]
        H7["WndProc Message Hook<br/>Global Hotkey"]
    end
    
    subgraph State["State Changes"]
        S1["isResizing"]
        S2["window.Width/Height"]
        S3["window.Opacity"]
        S4["window.Topmost"]
        S5["isMouseTransparent"]
    end
    
    E1 --> H1
    E1 --> H2
    E2 --> H3
    E3 --> H4
    E4 --> H5
    E5 --> H6
    E6 --> H7
    
    H1 -.-> S5
    H2 -.-> S1
    H3 -.-> S2
    H4 -.-> S1
    H5 -.-> S3
    H6 -.-> S4
    H6 -.-> S5
    H7 -.-> S5
    
    style Events fill:#b3e5fc
    style Handlers fill:#c8e6c9
    style State fill:#f0f4c3
```

**Last Updated**: January 8, 2026
**Created by**: José Mondragon - MDragon Solutions
**Project Type**: WPF Desktop Application (.NET 10.0)

