# PowerHouseMonitor

A .NET 6.0 console application that monitors a SQL Server database for unread form submissions and automatically launches the PowerHouse desktop application when new data is detected.

## Overview

PowerHouseMonitor acts as a background service or scheduled task that:
- Checks the database for unread form submissions
- Launches the PowerHouse desktop application when unread items exist
- Prevents multiple instances from running simultaneously (configurable)
- Provides intelligent path resolution for the desktop application

## Features

- **Database Monitoring**: Queries SQL Server for unread submissions in the `formDataDbSet` table
- **Automatic Launch**: Starts the desktop application only when needed
- **Instance Management**: Optional prevention of multiple desktop app instances
- **Smart Path Resolution**: Automatically locates the desktop executable using multiple search strategies
- **Environment Variable Support**: Configuration paths can use environment variables (e.g., `%USERPROFILE%`)
- **Detailed Logging**: UTC timestamps for all operations and errors

## Prerequisites

- .NET 6.0 Runtime
- SQL Server database with `formDataDbSet` table
- PowerHouse_desktop.exe application

## Configuration

### App.config Settings

Create an `App.config` file in the application directory with the following structure:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <appSettings>
    <!-- REQUIRED: SQL Server connection string -->
    <add key="ConnStringDB" value="Server=YOUR_SERVER;Database=YOUR_DB;User Id=YOUR_USER;Password=YOUR_PASSWORD;" />
    
    <!-- OPTIONAL: Custom path to PowerHouse_desktop.exe -->
    <add key="DesktopAppPath" value="%USERPROFILE%\Apps\PowerHouse_desktop.exe" />
    
    <!-- OPTIONAL: Prevent multiple instances (default: true) -->
    <add key="PreventMultipleInstances" value="true" />
  </appSettings>
</configuration>
```

#### Configuration Keys

| Key | Required | Default | Description |
|-----|----------|---------|-------------|
| `ConnStringDB` | **Yes** | N/A | SQL Server connection string |
| `DesktopAppPath` | No | Auto-detect | Full path to PowerHouse_desktop.exe. Supports environment variables. |
| `PreventMultipleInstances` | No | `true` | If `true`, won't launch if desktop app is already running |

### Database Requirements

The application expects a table named `formDataDbSet` with at least:
- An `IsRead` column (bit/boolean type)

Query executed:
```sql
SELECT COUNT(*) FROM formDataDbSet WHERE IsRead = 0
```

## Usage

### Manual Execution

```bash
PowerHouseMonitor.exe
```

### Exit Codes

| Code | Meaning |
|------|----------|
| `0` | No unread submissions found, or desktop app already running |
| `1` | Desktop application launched successfully |
| `2` | Error occurred (check stderr output) |

### Scheduled Task Setup (Windows)

1. Open Task Scheduler
2. Create a new task:
   - **Trigger**: Every 5 minutes (or desired interval)
   - **Action**: Start program → `PowerHouseMonitor.exe`
   - **Conditions**: Run whether user is logged on or not
3. Configure appropriate user account with database access

### Example Output

```
2025-10-26 08:15:32Z Detected 3 unread submission(s).
2025-10-26 08:15:32Z Launch command issued for 'C:\Apps\PowerHouse_desktop\PowerHouse_desktop.exe'.
```

## Path Resolution

The application searches for `PowerHouse_desktop.exe` in the following order:

1. **Configured Path**: Value from `DesktopAppPath` in App.config
2. **Relative Release Build**: `../PowerHouse_desktop/bin/Release/net6.0-windows8.0/PowerHouse_desktop.exe`
3. **Relative Debug Build**: `../PowerHouse_desktop/bin/Debug/net6.0-windows8.0/PowerHouse_desktop.exe`
4. **Current Directory**: `PowerHouse_desktop.exe`

All paths support:
- Environment variables (e.g., `%PROGRAMFILES%`, `%USERPROFILE%`)
- Relative paths (resolved from application directory)
- Absolute paths

## Dependencies

- **System.Configuration.ConfigurationManager** (v8.0.0)
- **System.Data.SqlClient** (v4.8.6)

## Troubleshooting

### "App setting 'ConnStringDB' is missing"
- Ensure `App.config` exists in the application directory
- Verify the `ConnStringDB` key is present and has a valid connection string

### "Unable to locate PowerHouse_desktop.exe"
- Set the `DesktopAppPath` in App.config with the full path
- Verify the desktop application exists at the specified location
- Check file permissions

### Desktop app doesn't launch
- Check if `PreventMultipleInstances` is `true` and an instance is already running
- Verify the user account has permissions to launch applications
- Check Windows Event Viewer for additional error details

### Database connection fails
- Verify SQL Server is accessible from the machine
- Test the connection string using SQL Server Management Studio
- Ensure the user has SELECT permissions on `formDataDbSet` table

## Development

### Building

```bash
dotnet build PowerHouseMonitor.csproj
```

### Running

```bash
dotnet run --project PowerHouseMonitor.csproj
```

## Security Considerations

- **Never commit `App.config`** with real credentials to version control
- Use Windows Integrated Authentication when possible
- Store connection strings in secure configuration or Azure Key Vault for production
- Run the scheduled task with least-privilege account

## License

[Add your license information here]

## Support

[Add support contact information here]
