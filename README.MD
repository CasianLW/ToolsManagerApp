dotnet installations:

dotnet add package CommunityToolkit.Mvvm
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.Maui.Resizetizer --version 8.0.40

after all installed, do:
dotnet clean
dotnet restore
dotnet build
