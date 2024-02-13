using Microsoft.SqlServer.Dac; 
using Microsoft.Data.SqlClient;
using System.Globalization;
using System.Threading;

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
        string fileExportLocation = @"C:\temp\SqlExportScript.dacpac";
        Version applicationVersion = new Version(1, 0, 0);

        // Azure SQL Server connection properties
        string serverName = "yourserver.database.windows.net";
        string databaseName = "yourdbsqltest";
        bool encrypt = true;
        int connectionTimeout = 30;
        string authentication = "Active Directory Interactive";

        string connectionString = $"Server={serverName};Database={databaseName};Encrypt={encrypt};Connection Timeout={connectionTimeout};Authentication={authentication};";

        DacExtractOptions extractOptions = new DacExtractOptions
        {
            ExtractApplicationScopedObjectsOnly = true,
            ExtractReferencedServerScopedElements = false,
            VerifyExtraction = true
        };

        DacServices dacServices = new DacServices(connectionString);

        dacServices.ProgressChanged += (sender, e) => Console.WriteLine($"{e.Status}: {e.Message}");

        Console.WriteLine("Starting extraction...");
        dacServices.Extract(fileExportLocation, databaseName, "SqlExport", applicationVersion, null, null, extractOptions);
        Console.WriteLine($"Extraction completed. Dacpac saved to: {fileExportLocation}");

    }
}
