using Microsoft.SqlServer.Dac; 
using System.Globalization;

internal class Program
{
    // Project will need to reference Microsoft SQL Server Management Objects
    // Website: https://www.nuget.org/packages/Microsoft.SqlServer.SqlManagementObjects
    // dotnet add package Microsoft.SqlServer.SqlManagementObjects --version 170.23.0

    // Website: https://www.nuget.org/packages/Microsoft.SqlServer.DACFx
    // dotnet add package Microsoft.SqlServer.DacFx --version 162.1.172
    private static void Main(string[] args)
    {
        CultureInfo defaultCulture = new CultureInfo("en-US");
        CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
        CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
        
        // Script export properties
        string fileExportLocation = @"C:\temp\SqlExportTest.bacpac";
        Version applicationVersion = new Version(1, 0, 0);

        // Azure SQL Server connection properties
        string serverName = "yourserver.database.windows.net";
        string databaseName = "yourdbsqltest";
        bool encrypt = true;
        int connectionTimeout = 30;
        string authentication = "Active Directory Interactive";

        string connectionString = $"Server={serverName};Database={databaseName};Encrypt={encrypt};Connection Timeout={connectionTimeout};Authentication={authentication};TrustServerCertificate=true;";

        DacServices dacServices = new DacServices(connectionString);

        dacServices.ProgressChanged += (sender, e) => Console.WriteLine($"{e.Status}: {e.Message}");
        
        Console.WriteLine($"Starting extraction to {fileExportLocation}...");
        dacServices.ExportBacpac(fileExportLocation, databaseName, null);
        Console.WriteLine($"Extraction completed. Bacpac saved to: {fileExportLocation}");
    }
}
