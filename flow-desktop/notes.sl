+	`git branch -m master main`
	rename default branch from `master` to `main`.

	github.com treats any branch named `main` as the main branch.
	`git init` on Visual Studio defaults to `master` as the first branch.

	the change just helps you follow convention.
	
+   setting up local storage on desktop, what ROOM does on Android.

    EF's migration tool can't load a WinUI project,
    the app's startup code throws when the tool loads it outside the running app.

    so the database lives in its own class library,
    one the tool can load. the app references it.

+   find where your `.slnx` lives.

    `dir *.slnx` in the app folder, in this case, it's `...\flow-desktop`
    
    set it as current working directory.

+   this creates the library beside the app folder, not inside it.
    anything inside the app folder gets compiled into the app.

    `dotnet new classlib -n flow-desktop.Data -f net8.0 -o ..\flow-desktop.Data`

+   add the library to the solution, so Rider shows it:
    `dotnet sln flow-desktop.slnx add ..\flow-desktop.Data\flow-desktop.Data.csproj`

+   install into the library via NuGet, both at `8.0.34`:
    `Microsoft.EntityFrameworkCore.Sqlite`
    `Microsoft.EntityFrameworkCore.Design`

    picked `8.` because the project targets .NET 8.

    NuGet lists the newest version by default, which needs a newer .NET,
    so pick the version from the dropdown.

    both packages must be on the same version.

+   install the migration tool, on that same version:
    `dotnet tool install --global dotnet-ef --version 8.0.34`

    confirm it with `dotnet ef --version`

+   create the db in the library, see `FlowDb.cs`, and its tables, see `LocalCatalog.cs`.

+   generate the first migration:
    `...`

    `InitialCreate` is the migration name,
    it generates the code that creates the tables.
    