# PowerHouse Desktop Solution

A Windows desktop application suite for managing and monitoring form submissions from a SQL Server database.

## Solution Structure

This solution contains three projects:

### 1. **PowerHouse_desktop**
- **Type**: Windows Forms Application (.NET 6.0)
- **Purpose**: Main desktop application for viewing and managing form submissions
- **Technology**: WinForms, Entity Framework Core
- **Target**: net6.0-windows8.0

### 2. **PowerHouseMonitor**
- **Type**: Console Application (.NET 6.0)
- **Purpose**: Background monitor that automatically launches the desktop app when new unread submissions are detected
- **Technology**: ADO.NET, System.Configuration
- **Target**: net6.0
- **Documentation**: See [PowerHouseMonitor/README.md](PowerHouseMonitor/README.md)

### 3. **Setup1_advinst**
- **Type**: Advanced Installer Project
- **Purpose**: Installation package for deploying the application

## Quick Start

### Prerequisites

- Windows 8.0 or later
- .NET 6.0 Runtime
- SQL Server (any edition)
- Visual Studio 2022 (for development)

### Building the Solution

1. Open `PowerHouse_desktop.sln` in Visual Studio
2. Restore NuGet packages
3. Build the solution (Ctrl+Shift+B)

```bash
# Or using .NET CLI
dotnet restore
dotnet build
```

### Configuration

Both applications require configuration:

1. **PowerHouse_desktop**: Configure database connection in application settings
2. **PowerHouseMonitor**:
   - Copy `App.config.example` to `App.config` and update the `ConnStringDB` entry
   - Set the desktop application path inside `PowerHouseMonitor/MonitorSettings.json`. This file ships with the monitor executable, so clients can edit it after installation to point at their local `PowerHouse_desktop.exe`.
   - Configure `LogDirectory` (either in App.config or MonitorSettings.json) if you want monitor logs written to a specific folder; otherwise logs stay beside the executable.

See individual project READMEs for detailed configuration instructions.

## Architecture

```
┌─────────────────────────────────────────┐
│         SQL Server Database             │
│         (formDataDbSet table)           │
└────────────┬────────────────────────────┘
             │
             │ Monitors for IsRead=0
             │
┌────────────▼────────────────────────────┐
│      PowerHouseMonitor.exe              │
│   (Scheduled Task / Background)         │
└────────────┬────────────────────────────┘
             │
             │ Launches when unread items exist
             │
┌────────────▼────────────────────────────┐
│     PowerHouse_desktop.exe              │
│   (Windows Forms Application)           │
└─────────────────────────────────────────┘
```

## Deployment

### Option 1: Manual Deployment

1. Build the solution in Release mode
2. Copy binaries from `bin/Release` folders
3. Configure `App.config` files
4. Set up Windows Task Scheduler for PowerHouseMonitor

### Option 2: Installer

1. Build the `Setup1_advinst` project
2. Run the generated installer
3. Configure applications post-installation

## Database Schema

The application expects a SQL Server database with at least:

**Table: `formDataDbSet`**
- `IsRead` (bit) - Flag indicating if submission has been read
- Additional columns as needed by your application

## Development Guidelines

### Git Workflow

- Main branch: `main`
- Feature branches: `feature/description`
- Bug fixes: `bugfix/description`

### Code Style

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and concise

### Testing

- Test database connectivity before deployment
- Verify path resolution in different environments
- Test scheduled task execution
- Validate multi-instance prevention

## Security Best Practices

1. **Never commit configuration files** with real credentials
   - `App.config` is gitignored
   - Use `App.config.example` as template

2. **Use Windows Integrated Authentication** when possible
   - Avoids storing passwords in configuration
   - Leverages Windows security

3. **Principle of Least Privilege**
   - Database user needs only SELECT on `formDataDbSet`
   - Scheduled task runs with minimal permissions

4. **Secure Configuration Storage**
   - Consider Azure Key Vault for production
   - Use encrypted configuration sections
   - Protect configuration files with NTFS permissions

## Troubleshooting

### Common Issues

1. **Monitor doesn't launch desktop app**
   - Check Task Scheduler logs
   - Verify App.config exists and is valid
   - Ensure database is accessible

2. **Desktop app not found**
   - Set `DesktopAppPath` in PowerHouseMonitor's App.config
   - Verify file permissions

3. **Database connection errors**
   - Test connection string with SSMS
   - Check firewall settings
   - Verify SQL Server is running

### Logs

- PowerHouseMonitor outputs to console (capture in Task Scheduler)
- Check Windows Event Viewer for application errors
- SQL Server logs for database connectivity issues

## Project Structure

```
PowerHouse_desktop/
├── .gitignore                          # Git ignore rules
├── PowerHouse_desktop.sln              # Visual Studio solution
├── README.md                           # This file
│
├── PowerHouse_desktop/                 # Main desktop application
│   ├── PowerHouse_desktop.csproj
│   ├── Program.cs
│   └── [Forms and other files]
│
├── PowerHouseMonitor/                  # Background monitor
│   ├── README.md                       # Detailed monitor docs
│   ├── PowerHouseMonitor.csproj
│   ├── Program.cs
│   ├── App.config.example              # Configuration template
│   └── App.config                      # (gitignored)
│
└── Setup1_advinst/                     # Installer project
    └── Setup1_advinst.aiproj
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for development guidelines.

## License

[Add your license information here]

## Support

[Add support contact information here]

## Version History

- **v1.0.0** - Initial release
  - Desktop application for form management
  - Background monitor for automatic launching
  - Advanced Installer package
