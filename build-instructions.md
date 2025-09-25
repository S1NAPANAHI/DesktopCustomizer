# Build Instructions for Desktop Customizer v1.0

## Prerequisites
1. Visual Studio 2022 or Visual Studio Code with C# extension
2. .NET 6.0 SDK or later
3. Windows 10/11 (development machine)

## Quick Build Steps

### Option 1: Visual Studio 2022
1. Open Visual Studio 2022
2. Select "Create a new project"
3. Choose "WPF Application" (.NET 6 or later)
4. Name the project "DesktopCustomizer"
5. Replace all generated files with the provided source files
6. Add NuGet packages:
   - Right-click project → Manage NuGet Packages
   - Install: `System.Management` version 8.0.0
   - Install: `Newtonsoft.Json` version 13.0.3
   - Install: `System.Windows.Forms` (for NotifyIcon)
7. Build the solution (Ctrl+Shift+B)

### Option 2: Command Line (.NET CLI)
```bash
# Clone the repository
git clone https://github.com/S1NAPANAHI/DesktopCustomizer.git
cd DesktopCustomizer

# Restore dependencies
dotnet restore

# Build the application
dotnet build

# Run the application
dotnet run
```

### Option 3: Manual Setup
1. Create a new folder: `DesktopCustomizer`
2. Copy all `.cs`, `.xaml`, and `.csproj` files into the folder
3. Open command prompt in the folder
4. Run: `dotnet restore`
5. Run: `dotnet build`
6. Find the executable in `bin\Debug\net6.0-windows\`

## File Structure
```
DesktopCustomizer/
├── DesktopCustomizer.csproj    # Project file
├── App.xaml                    # Application definition
├── App.xaml.cs                 # Application logic
├── SystemInfoWidget.xaml       # System monitor widget UI
├── SystemInfoWidget.xaml.cs    # System monitor logic
├── ClockWidget.xaml            # Clock widget UI
├── ClockWidget.xaml.cs         # Clock widget logic
├── SettingsWindow.xaml         # Settings window UI
├── SettingsWindow.xaml.cs      # Settings window logic
├── AssemblyInfo.cs             # Assembly information
├── README.md                   # Documentation
└── build-instructions.md       # This file
```

## Troubleshooting

### Common Issues:

**"System.Management not found"**
- Solution: Add the NuGet package `System.Management`

**"NotifyIcon not available"**
- Solution: Add reference to `System.Windows.Forms`

**"Performance counters not working"**
- Solution: Run as Administrator for full system access

**"Widgets not showing"**
- Solution: Check if widgets are enabled in settings
- Make sure windows are set to Topmost=True

### Performance Counter Issues:
If system monitoring doesn't work:
1. Run the application as Administrator
2. Enable Performance Counters in Windows:
   ```cmd
   lodctr /r
   winmgmt /resyncperf
   ```

## Building for Distribution

### Create Release Build:
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

This creates a single executable file that includes all dependencies.

### Manual Distribution:
1. Build in Release mode
2. Copy the entire `bin\Release\net6.0-windows\` folder
3. Include Visual C++ Redistributables if targeting older systems
4. Create installer using tools like Inno Setup or NSIS

## Development Environment Setup

### Required Visual Studio Workloads:
- .NET Desktop Development
- Windows application development

### Required NuGet Packages:
- `System.Management` (8.0.0) - For system performance monitoring
- `Newtonsoft.Json` (13.0.3) - For settings serialization
- `System.Windows.Forms` - For system tray integration

## Testing

### Run Tests:
```bash
# Unit tests (when added)
dotnet test

# Manual testing checklist:
# 1. Widgets appear on startup
# 2. System tray icon works
# 3. Settings can be changed and saved
# 4. Widgets are draggable
# 5. Performance data updates correctly
# 6. Application exits cleanly
```

## Performance Optimization

### For Better Performance:
1. Run as Administrator for full Performance Counter access
2. Close unnecessary applications
3. Ensure .NET 6 runtime is installed
4. Use Release build for deployment

## Next Development Steps
1. Test on different Windows 11 versions
2. Add more widget types (weather, notes, etc.)
3. Implement taskbar customization
4. Add wallpaper management
5. Create plugin architecture
6. Add auto-updater functionality

## Known Limitations
- Requires .NET 6 runtime on target systems
- Performance counters may need Administrator privileges
- Some Windows 11 features might be restricted by UAC
- Widget positions are saved on application exit

## Support
If you encounter issues:
1. Check Windows Event Viewer for errors
2. Verify .NET 6 is installed
3. Try running as Administrator
4. Check antivirus software isn't blocking the app
5. Create an issue on the GitHub repository

## Contributing
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

---

**Happy coding! 🚀**