# Transparent Notes App

A sophisticated Windows WPF application that provides a transparent, always-visible notes window that remains hidden from screen capture software (Zoom, Microsoft Teams, Google Meet, etc.).

## Developer

**Created by: Jose Mondragon (MDragon Solutions)**

This application was developed with precision and care to provide a secure notes-taking experience while screen sharing.

## Features

### Core Features
- **Screen Capture Invisible**: The window is completely hidden from screen sharing software (Zoom, Microsoft Teams, Google Meet, OBS, etc.) using Windows API `SetWindowDisplayAffinity`
- **Transparent Window**: Semi-transparent notes window (adjustable opacity: 0.1 - 1.0) that doesn't interfere with content visibility
- **Resizable Window**: Drag the bottom-right corner to resize the window to fit your needs
- **Always on Top**: Toggle window to stay above all other applications
- **Pass-Through Mode**: Press **Alt+P** to toggle mouse pass-through mode - when enabled, mouse clicks pass through the window to applications underneath
- **Adjustable Opacity**: Use numeric input (0.1 - 1.0) with Enter key to set transparency level
- **Adjustable Font Size**: Use numeric input (8 - 32) with Enter key to change note text size
- **Tabbed Interface**: Switch between Text notes and Image content (expandable feature)
- **Keyboard Shortcut**: Global Alt+P hotkey works even when window is in pass-through mode

## Quick Start

### System Requirements
- Windows 10 or later
- .NET 10.0 or compatible runtime
- No external dependencies required

### Installation

1. Download or clone the repository
2. Navigate to the `TransparentNotesApp` folder
3. Run the application:
   ```bash
   dotnet run
   ```

Or build and run:
```bash
dotnet build
cd bin/Debug/net10.0-windows
TransparentNotesApp.exe
```

## Usage Guide

### Window Controls

| Control | Action | Purpose |
|---------|--------|---------|
| **Always on Top Checkbox** | Check/Uncheck | Toggle window to stay above all other windows and enable pass-through mode |
| **Opacity TextBox** | Type value (0.1-1.0) + Enter | Adjust window transparency (0.1 = very transparent, 1.0 = opaque) |
| **Font Size TextBox** | Type value (8-32) + Enter | Change the size of text in the notes area |
| **Alt+P Hotkey** | Press anywhere | Toggle pass-through mode (allows clicks to pass through window) |
| **Bottom-Right Corner** | Drag | Resize the window to your desired dimensions |

### Keyboard Shortcuts

- **Alt+P**: Toggle mouse pass-through mode globally
- **Enter** (in Opacity/Font Size fields): Apply the numeric input value

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
MainWindow (Transparent, No Title Bar)
├── Grid (2 rows: Auto for controls, * for content)
│   ├── StackPanel (Row 0 - Controls)
│   │   ├── AlwaysOnTopCheckBox + Label
│   │   ├── OpacityTextBox (0.1-1.0)
│   │   └── FontSizeTextBox (8-32)
│   ├── TabControl (Row 1 - Content)
│   │   ├── Text Tab (TextBox for notes)
│   │   └── Image Tab (Placeholder)
│   └── ResizeArea (Rectangle overlay - transparent, 40x40)
```

---

## Security & Privacy Features

1. Screen Capture Protection: Uses Windows native API to exclude window from screen sharing
   - Tested with: Zoom, Microsoft Teams, Google Meet, OBS
   - Note: This only works on Windows; effectiveness may vary by platform/software

2. No Data Transmission: Application stores data locally only
3. No External Connections: Works completely offline
4. No Tracking: Zero telemetry or analytics

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
2. Persistence: Notes content is not saved between sessions (can be added via serialization)
3. Windows Only: Application requires Windows OS and relies on Windows-specific APIs
4. Screen Capture Evasion: Effectiveness depends on the specific screen sharing software and its implementation

## Version History

### v1.0 (Current)
- Initial release
- Core features implemented: screen capture hiding, transparency, resizing, pass-through mode
- Numeric controls for opacity and font size (instead of sliders)
- Global Alt+P hotkey for pass-through toggle

## Contributing

This is a personal portfolio project by José Mondragon (MDragon Solutions). 

Suggestions for improvements:
- Add note persistence (JSON/SQLite storage)
- Implement color themes
- Add more keyboard shortcuts
- Extend Image tab functionality
- Add system tray integration

## License

This project is created by José Mondragon (MDragon Solutions) for portfolio purposes.

**Created by: Jose Mondragon - MDragon Solutions**

## Future Enhancements

- Save/Load notes functionality
- Custom themes (dark mode, colors)
- Note templates
- Auto-backup feature
- Search functionality
- Markdown support
- System tray minimization
- Cloud sync (optional)
- Note encryption
- Export to PDF/TXT

## Project Structure

```
TransparentNotesApp/
├── MainWindow.xaml          # UI Layout definition
├── MainWindow.xaml.cs       # Main window logic & event handlers
├── App.xaml                 # Application resources
├── App.xaml.cs              # Application startup logic
├── AssemblyInfo.cs          # Assembly metadata
├── TransparentNotesApp.csproj # Project configuration
└── bin/                     # Build output
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

**Last Updated**: December 25, 2025
**Created by**: Jose Mondragon - MDragon Solutions
**Project Type**: WPF Desktop Application (.NET 10.0)

