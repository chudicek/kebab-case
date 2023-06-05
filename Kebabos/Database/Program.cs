// lol this is not working

// using System;
// using ;
// using DbUp.Engine;

// class Program
// {
//   static void Main()
//   {
//     // Define the PostgreSQL connection string
//     string connectionString =

//     // Define the directory where your 'schema.sql' file is located
//     string scriptsDirectory = "./schema.sql";

//     // Create an upgrader instance
//     var upgrader = DeployChanges.To
//         .PostgresqlDatabase(connectionString)
//         .WithScriptsFromFileSystem(scriptsDirectory)
//         .LogToConsole()
//         .Build();

//     // Execute the upgrade
//     DatabaseUpgradeResult result = upgrader.PerformUpgrade();

//     // Check if the upgrade was successful
//     if (result.Successful)
//     {
//       Console.WriteLine("Database schema update successful!");
//     }
//     else
//     {
//       Console.WriteLine("Database schema update failed!");
//       Console.WriteLine(result.Error);
//     }
//   }
// }
