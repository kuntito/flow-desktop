+	`git branch -m master main`
	rename default branch from `master` to `main`.

	github.com treats any branch named `main` as the main branch.
	`git init` on Visual Studio defaults to `master` as the first branch.

	the change just helps you follow convention.

to add a db, similar to Android's ROOM, the db lives in its own project,
a class library called `flow-desktop.Data`.
EF's migration tool can't load the WinUI app, but it can load a plain library.

the library sat beside the app folder, so git didn't track it.
so i created a parent folder, `flow-desktop-solution`,
moved `flow-desktop` and `flow-desktop.Data` into it,
then moved `.git`, `.gitignore` and `flow-desktop.slnx` up into it too,
and fixed the project paths in the `.slnx`.

i created the library, `flow-desktop.Data` before doing the dir/file moves.
so what i actually did differs from these steps.
these are rewritten in the order i'd do it the first time,
but hasn't been battle tested.

+   create `flow-desktop-solution`, move the app folder into it.
    move `.git`, `.gitignore` and `flow-desktop.slnx` up into it, and fix the paths in the `.slnx`.

+   from the solution folder, create the library:
    `dotnet new classlib -n flow-desktop.Data -f net8.0`

+   add it to the solution:
    `dotnet sln flow-desktop.slnx add flow-desktop.Data/flow-desktop.Data.csproj`

+   install into the library via NuGet, 
    
    both on the same version, `8.` to match `.NET 8` the project uses:
    `Microsoft.EntityFrameworkCore.Sqlite`
    `Microsoft.EntityFrameworkCore.Design`

+   install the migration tool on that version, then check it:
    `dotnet tool install --global dotnet-ef --version 8.0.34`, 
    
    to verify:
    `dotnet ef --version`

+   in the library, create the tables, see `LocalCatalog.cs`, and the db, see `FlowDb.cs`.

+   from the library's folder, generate the first migration:
    `dotnet ef migrations add InitialCreate`

+   from the solution folder, reference the library from the app:
    `dotnet add flow-desktop/flow-desktop.csproj reference flow-desktop.Data/flow-desktop.Data.csproj`
    
+   then `App.xaml.cs` should look like:

`
using flow_desktop.Data;
using Microsoft.EntityFrameworkCore;

namespace flow_desktop
{
    public partial class App : Application
    {
        private Window? _window;


        public App()
        {
            InitializeComponent();
        }


        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            using (var db = new FlowDb())
            {
                db.Database.Migrate();
            }
            
            _window = new MainWindow();
            _window.Activate();
        }
    }
}
`