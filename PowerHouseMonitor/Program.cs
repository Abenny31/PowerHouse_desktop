using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace PowerHouseMonitor
{
    /// <summary>
    /// Console application that monitors a SQL Server database for unread form submissions
    /// and automatically launches the PowerHouse desktop application when new data is detected.
    /// </summary>
    internal static class Program
    {
        private static readonly string[] DefaultDesktopAppPaths =
        {
            Path.Combine(AppContext.BaseDirectory, "..", "PowerHouse_desktop", "bin", "Release", "net6.0-windows8.0", "PowerHouse_desktop.exe"),
            Path.Combine(AppContext.BaseDirectory, "..", "PowerHouse_desktop", "bin", "Debug", "net6.0-windows8.0", "PowerHouse_desktop.exe"),
            Path.Combine(AppContext.BaseDirectory, "PowerHouse_desktop.exe")
        };

        private const string JsonSettingsFileName = "MonitorSettings.json";
        private const string DesktopAppPathKey = "DesktopAppPath";
        private const string ConnectionStringKey = "ConnStringDB";
        private const string PreventMultipleInstancesKey = "PreventMultipleInstances";
        private const string LogDirectoryKey = "LogDirectory";
        private static string _logDirectory = AppContext.BaseDirectory;

        /// <summary>
        /// Main entry point for the PowerHouseMonitor application.
        /// Checks for unread submissions and launches the desktop app if needed.
        /// </summary>
        /// <param name="args">Command line arguments (not used).</param>
        /// <returns>
        /// 0 if no unread submissions or app already running,
        /// 1 if desktop app launched successfully,
        /// 2 if an error occurred.
        /// </returns>
        private static int Main(string[] args)
        {
            try
            {
                var monitorSettings = LoadMonitorSettings();
                _logDirectory = ResolveLogDirectory(monitorSettings);
                var connectionString = GetConnectionString();
                var unreadCount = GetUnreadCount(connectionString);

                if (unreadCount <= 0)
                {
                    LogInfo("No unread submissions. Exiting.");
                    return 0;
                }

                LogInfo($"Detected {unreadCount} unread submission(s).");

                var desktopAppPath = ResolveDesktopAppPath(monitorSettings);
                var preventMultipleInstances = ShouldPreventMultipleInstances(monitorSettings);

                if (preventMultipleInstances && IsDesktopAppRunning(desktopAppPath))
                {
                    LogInfo("Desktop app already running. Skipping launch.");
                    return 0;
                }

                LaunchDesktopApp(desktopAppPath);
                LogInfo($"Launch command issued for '{desktopAppPath}'.");

                return 1;
            }
            catch (Exception ex)
            {
                LogError($"ERROR: {ex.Message}", ex);
                return 2;
            }
        }

        /// <summary>
        /// Retrieves the database connection string from App.config.
        /// </summary>
        /// <returns>The SQL Server connection string.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the connection string is missing from configuration.</exception>
        private static string GetConnectionString()
        {
            var connectionString = ConfigurationManager.AppSettings[ConnectionStringKey];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException($"App setting '{ConnectionStringKey}' is missing. Update App.config with the database connection string.");
            }

            return connectionString;
        }

        /// <summary>
        /// Queries the database to count unread form submissions.
        /// </summary>
        /// <param name="connectionString">The SQL Server connection string.</param>
        /// <returns>The number of unread submissions (IsRead = 0).</returns>
        private static int GetUnreadCount(string connectionString)
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            using var command = new SqlCommand("SELECT COUNT(*) FROM formDataDbSet WHERE IsRead = 0", connection);
            var scalar = command.ExecuteScalar();
            if (scalar == null || scalar == DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(scalar);
        }

        /// <summary>
        /// Resolves the path to the PowerHouse desktop application executable.
        /// Searches configured path first, then default locations.
        /// </summary>
        /// <returns>The full path to PowerHouse_desktop.exe.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the desktop application cannot be located.</exception>
        private static string ResolveDesktopAppPath(MonitorSettings monitorSettings)
        {
            if (!string.IsNullOrWhiteSpace(monitorSettings.DesktopAppPath))
            {
                var configuredFromJson = ResolvePath(monitorSettings.DesktopAppPath);
                if (File.Exists(configuredFromJson))
                {
                    return configuredFromJson;
                }

                LogInfo($"DesktopAppPath from {JsonSettingsFileName} not found: {configuredFromJson}");
            }

            var configuredPath = ConfigurationManager.AppSettings[DesktopAppPathKey];
            if (!string.IsNullOrWhiteSpace(configuredPath))
            {
                var resolved = ResolvePath(configuredPath);
                if (File.Exists(resolved))
                {
                    return resolved;
                }

                LogInfo($"Configured desktop app path not found: {resolved}");
            }

            foreach (var candidate in DefaultDesktopAppPaths.Select(ResolvePath))
            {
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            throw new FileNotFoundException("Unable to locate PowerHouse_desktop.exe. Set the DesktopAppPath appSetting to the full path of the desktop application.");
        }

        /// <summary>
        /// Resolves a path by expanding environment variables and converting to absolute path.
        /// </summary>
        /// <param name="path">The path to resolve (can be relative or contain environment variables).</param>
        /// <returns>The fully resolved absolute path.</returns>
        private static string ResolvePath(string path)
        {
            var expanded = Environment.ExpandEnvironmentVariables(path.Trim());
            if (Path.IsPathRooted(expanded))
            {
                return Path.GetFullPath(expanded);
            }

            var combined = Path.Combine(AppContext.BaseDirectory, expanded);
            return Path.GetFullPath(combined);
        }

        /// <summary>
        /// Determines whether multiple instances of the desktop app should be prevented.
        /// </summary>
        /// <returns>True if multiple instances should be prevented (default), false otherwise.</returns>
        private static bool ShouldPreventMultipleInstances(MonitorSettings monitorSettings)
        {
            if (monitorSettings.PreventMultipleInstances.HasValue)
            {
                return monitorSettings.PreventMultipleInstances.Value;
            }

            var setting = ConfigurationManager.AppSettings[PreventMultipleInstancesKey];
            return string.IsNullOrWhiteSpace(setting) || bool.TryParse(setting, out var result) && result;
        }

        /// <summary>
        /// Checks if the desktop application is currently running.
        /// </summary>
        /// <param name="desktopAppPath">The full path to the desktop application executable.</param>
        /// <returns>True if the application is running, false otherwise.</returns>
        private static bool IsDesktopAppRunning(string desktopAppPath)
        {
            try
            {
                var processName = Path.GetFileNameWithoutExtension(desktopAppPath);
                return Process.GetProcessesByName(processName).Any();
            }
            catch (Exception ex)
            {
                LogError($"Unable to check running processes: {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// Launches the PowerHouse desktop application.
        /// </summary>
        /// <param name="desktopAppPath">The full path to the desktop application executable.</param>
        private static void LaunchDesktopApp(string desktopAppPath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = desktopAppPath,
                WorkingDirectory = Path.GetDirectoryName(desktopAppPath) ?? AppContext.BaseDirectory,
                UseShellExecute = true
            };

            Process.Start(startInfo);
        }

        /// <summary>
        /// Loads optional monitor settings from MonitorSettings.json located alongside the executable.
        /// </summary>
        private static MonitorSettings LoadMonitorSettings()
        {
            var settingsPath = Path.Combine(AppContext.BaseDirectory, JsonSettingsFileName);
            if (!File.Exists(settingsPath))
            {
                return MonitorSettings.Empty;
            }

            try
            {
                var json = File.ReadAllText(settingsPath);
                var settings = JsonSerializer.Deserialize<MonitorSettings>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                });

                return settings ?? MonitorSettings.Empty;
            }
            catch (Exception ex)
            {
                LogError($"Unable to read {JsonSettingsFileName}: {ex.Message}", ex);
                return MonitorSettings.Empty;
            }
        }

        private static string ResolveLogDirectory(MonitorSettings monitorSettings)
        {
            var configured = monitorSettings.LogDirectory;
            if (!string.IsNullOrWhiteSpace(configured))
            {
                return ResolvePath(configured);
            }

            var fromConfig = ConfigurationManager.AppSettings[LogDirectoryKey];
            if (!string.IsNullOrWhiteSpace(fromConfig))
            {
                return ResolvePath(fromConfig);
            }

            return AppContext.BaseDirectory;
        }

        private static void LogInfo(string message) => LogInternal(message, isError: false);

        private static void LogError(string message, Exception? ex = null)
        {
            var combined = ex is null ? message : $"{message}{Environment.NewLine}{ex}";
            LogInternal(combined, isError: true);
        }

        private static void LogInternal(string message, bool isError)
        {
            var timestamp = DateTime.UtcNow;
            var formatted = $"{timestamp:u}, {message}";

            try
            {
                Directory.CreateDirectory(_logDirectory);
                var fileName = $"log{timestamp:yyyyMMdd}.txt";
                var fullPath = Path.Combine(_logDirectory, fileName);
                File.AppendAllText(fullPath, formatted + Environment.NewLine);
            }
            catch
            {
                // Avoid throwing from logging; fall back silently.
            }

            if (isError)
            {
                Console.Error.WriteLine(formatted);
            }
            else
            {
                Console.WriteLine(formatted);
            }
        }

        private sealed record MonitorSettings(string? DesktopAppPath, bool? PreventMultipleInstances, string? LogDirectory)
        {
            public static MonitorSettings Empty { get; } = new(null, null, null);
        }
    }
}
