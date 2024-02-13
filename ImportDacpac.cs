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

        // Script import properties
        string fileExportLocation = @"C:\temp\SqlExportScript.dacpac";

        // Azure SQL Server connection properties
        string serverName = "azuresqlvulntest.database.windows.net";
        string databaseName = "dbtestdacpacimport";
        bool encrypt = true;
        int connectionTimeout = 30;
        string authentication = "Active Directory Interactive";

        string connectionString = $"Server={serverName};Database={databaseName};Encrypt={encrypt};Connection Timeout={connectionTimeout};Authentication={authentication};";

        DacServices dacServices = new DacServices(connectionString);
        DacPackage dacPackage = DacPackage.Load(fileExportLocation);

        // Dacpac deployment options
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.dac.dacdeployoptions?view=sql-dacfx-162
        DacDeployOptions dacDeployOptions = new DacDeployOptions
        {
            BlockOnPossibleDataLoss = true,
            CreateNewDatabase = false,
            DropObjectsNotInSource = false,
            IgnorePermissions = false,
            IgnoreRoleMembership = false,
        };

        dacServices.ProgressChanged += (sender, e) => Console.WriteLine($"{e.Status}: {e.Message}");  

        Console.WriteLine("Starting import...");
        dacServices.Deploy(dacPackage, databaseName, true, dacDeployOptions);
        Console.WriteLine($"Import of {fileExportLocation} completed.");

    }
}
