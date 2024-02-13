using Microsoft.SqlServer.Dac; 
using System.Globalization;
using System.Reflection;

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
        string serverName = "yoursqlserver.database.windows.net";
        string databaseName = "yourdbimportbactest";
        bool encrypt = true;
        int connectionTimeout = 30;
        string authentication = "Active Directory Interactive";

        string connectionString = $"Server={serverName};Database={databaseName};Encrypt={encrypt};Connection Timeout={connectionTimeout};Authentication={authentication};TrustServerCertificate=true;";

        DacServices dacServices = new DacServices(connectionString);
        BacPackage bacPackage = BacPackage.Load(fileExportLocation);

        dacServices.ProgressChanged += (sender, e) => Console.WriteLine($"{e.Status}: {e.Message}");

        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.dac.dacservices.importbacpac?view=sql-dacfx-162
        // Assuming here the database is existing (prefer to create a new database outside of this)
        Console.WriteLine($"Starting import...");
        dacServices.ImportBacpac(bacPackage, databaseName);
        Console.WriteLine($"Import completed. Bacpac used: {fileExportLocation}");
    }
}
