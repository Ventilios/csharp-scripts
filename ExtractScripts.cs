using Microsoft.SqlServer.Management.Smo;  
using Microsoft.SqlServer.Management.Common;
using Microsoft.Data.SqlClient;
using System.Collections.Specialized;
using Microsoft.SqlServer.Management.Sdk.Sfc;

internal class Program
{
    // Project will need to reference Microsoft SQL Server Management Objects
    // Website: https://www.nuget.org/packages/Microsoft.SqlServer.SqlManagementObjects
    // dotnet add package Microsoft.SqlServer.SqlManagementObjects --version 170.23.0
    private static void Main(string[] args)
    {
        // Script export location
        string fileExportLocation = @"C:\temp\SqlExport.sql";

        // Azure SQL Server connection properties
        string serverName = "yoursqlserver.database.windows.net";
        string databaseName = "yourdbsqltest";
        bool encrypt = true;
        int connectionTimeout = 30;
        string authentication = "Active Directory Interactive";

        string connectionString = $"Server={serverName};Database={databaseName};Encrypt={encrypt};Connection Timeout={connectionTimeout};Authentication={authentication}";
        
        // Create a new connection to the Azure SQL Server
        SqlConnection connection = new(connectionString);
        ServerConnection serverConnection = new(connection);

        //  Create a new instance of the Server object
        Server server = new(serverConnection);
        Database database = server.Databases["dbsqltest"]; 

        // Display the server and database information
        string consoleMessage = "{{{{ Server: " + server.Name + " - Database: " + database.Name + " - SKU: " + database.AzureEdition + " }}}}";
        Console.WriteLine(consoleMessage);

        Transfer transferDatabase = new Transfer(database);  
        ScriptingOptions scriptTransferOptions = new();

        // Set the transfer options for the script
        // https://learn.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.management.smo.transfer?view=sql-smo-160
        scriptTransferOptions.DriAll = true;
        scriptTransferOptions.AllowSystemObjects = false;
        scriptTransferOptions.IncludeIfNotExists = true;
        scriptTransferOptions.ScriptSchema = true;
        scriptTransferOptions.WithDependencies = true;
        scriptTransferOptions.Indexes = true;
        //scriptTransferOptions.ScriptData = true;
        //scriptTransferOptions.ScriptDrops = true;
        scriptTransferOptions.FileName = fileExportLocation;	

        transferDatabase.DiscoveryProgress += (sender, e) =>
        {
            Console.WriteLine("Scripting: " + e.Current.ToString() + " of " + e.Total.ToString());
        };

        // Generate the script
        transferDatabase.Options = scriptTransferOptions;
        IEnumerable<string> transferScriptOutput = transferDatabase.EnumScriptTransfer();

        // Output the script to the console
        //foreach (string script in transferScriptOutput)
        //    Console.WriteLine(script);

    }
}
