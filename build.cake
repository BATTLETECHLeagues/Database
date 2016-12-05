#tool "Microsoft.SQLServer.SMO"
#r System.Data
#r Microsoft.SqlServer.ConnectionInfo
#r Microsoft.SqlServer.Smo
#r Microsoft.SqlServer.Management.Sdk.Sfc

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var devDatabaseConnectionString = Argument("devConnectionString",@"Server=(localdb)\MSSQLLocalDB;Database=BCAPIDatabase_Dev;Trusted_Connection=True;");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/Example/bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("./src/Example.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
   
});

Task("Create-Dev-Database")
    .Does(() =>
{
    using (System.Data.SqlClient.SqlConnection sqlConnection = new   System.Data.SqlClient.SqlConnection(devDatabaseConnectionString))
    {
        Microsoft.SqlServer.Management.Common.ServerConnection svrConnection = new  Microsoft.SqlServer.Management.Common.ServerConnection(sqlConnection);
        Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(svrConnection);

        string script = System.IO.File.ReadAllText("./src/database/create/CreateDevDatabase.sql");
        string[] singleCommand = System.Text.RegularExpressions.Regex.Split(script, "^GO", System.Text.RegularExpressions.RegexOptions.Multiline);
         System.Collections.Specialized.StringCollection scl = new  System.Collections.Specialized.StringCollection();
        foreach(string t in singleCommand)
        {
            if(t.Trim().Length > 0) scl.Add(t.Trim());
        }
        try
        {
            int[] result = server.ConnectionContext.ExecuteNonQuery(scl, Microsoft.SqlServer.Management.Common.ExecutionTypes.ContinueOnError);
            // Now check the result array to find any possible errors??
        }
        finally
        {

        }
    }      
});

Task("Migrate-Database")
    .Does(() =>
{
    var files = GetFiles("./src/database/migration/*.sql");
    foreach(var file in files)
    {
        using (System.Data.SqlClient.SqlConnection sqlConnection = new   System.Data.SqlClient.SqlConnection(devDatabaseConnectionString))
        {
            Microsoft.SqlServer.Management.Common.ServerConnection svrConnection = new  Microsoft.SqlServer.Management.Common.ServerConnection(sqlConnection);
            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(svrConnection);

            string script = System.IO.File.ReadAllText(file.FullPath);
            Information(file.FullPath);
            string[] singleCommand = System.Text.RegularExpressions.Regex.Split(script, "^GO", System.Text.RegularExpressions.RegexOptions.Multiline);
            System.Collections.Specialized.StringCollection scl = new  System.Collections.Specialized.StringCollection();
            foreach(string t in singleCommand)
            {
                if(t.Trim().Length > 0) scl.Add(t.Trim());
            }
            try
            {
                int[] result = server.ConnectionContext.ExecuteNonQuery(scl, Microsoft.SqlServer.Management.Common.ExecutionTypes.ContinueOnError);
                // Now check the result array to find any possible errors??
            }
            finally
            {

            }
        }  
    }
});


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Create-Dev-Database")
    .IsDependentOn("Migrate-Database");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
