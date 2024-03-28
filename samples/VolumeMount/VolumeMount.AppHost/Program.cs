﻿var builder = DistributedApplication.CreateBuilder(args);

// Using a persistent volume mount requires a stable password rather than the default generated one.

// To have a persistent volume across container instances, it must be named.
var sqlPassword = builder.AddParameter("sqlpassword", true);
var sqlDatabase = builder.AddSqlServer("sqlserver", sqlPassword)
    .WithDataVolume()
    //.WithVolume($"sqlserver-data", "/var/opt/mssql")
    .AddDatabase("sqldb");

// Postgres must also have a stable password and a named volume
var postgresPassword = builder.AddParameter("postgrespassword", true);
var postgresDatabase = builder.AddPostgres("postgresserver", password: postgresPassword)
    .WithDataVolume()
    .AddDatabase("postgres");

var blobs = builder.AddAzureStorage("Storage")
    // Use the Azurite storage emulator for local development
    .RunAsEmulator(emulator => emulator.WithDataVolume())
    .AddBlobs("BlobConnection");

builder.AddProject<Projects.VolumeMount_BlazorWeb>("blazorweb")
    .WithReference(sqlDatabase)
    .WithReference(postgresDatabase)
    .WithReference(blobs);

builder.Build().Run();
