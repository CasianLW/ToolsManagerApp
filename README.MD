dotnet installations:

dotnet add package CommunityToolkit.Mvvm
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.Maui.Resizetizer --version 8.0.40
dotnet add package MongoDB.Driver
dotnet add package Microsoft.Extensions.Logging.Console

after all installed, do:
dotnet clean
dotnet restore
dotnet build
