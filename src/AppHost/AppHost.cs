var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.NetAspireServer_Api>("api");

builder.Build().Run();
