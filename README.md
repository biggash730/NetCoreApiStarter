# ASP.Net Core 3 WebApi Starter
This solution has 3 .Net Core Class libraries and 1 ASP.Net Core Web Application with MVC and WebAPI

## Projects:
+ NetCoreStarter.Services - Class Library
+ NetCoreStarter.Shared -Class Library
+ NetCoreStarter.Utils -Class Library
+ NetCoreStarter.Web - Web Application

## Getting Started:
+ Clone the project `git clone https://github.com/biggash730/NetCoreApiStarter.git`
+ Refactor project namespace `NetCoreStarter` with your prefered project name.
+ Run update packages `Update-Package`
+ Ensure migrations are correctly configured
+ Run `Update-Database`
+ Enjoy!!


# Projects
## NetCoreStarter.Services Structure
+ Dependencies
+ MessageProcessor.cs
+ ServiceScheduler.cs

## NetCoreStarter.Shared
+ Dependencies
+ Classes
    * BaseEntities.cs
    * ErrorViewModel.cs
    * Role.cs
    * RoleClaim.cs
    * User.cs
    * UserClaim.cs
    * UserLogin.cs
    * UserRole.cs
    * UserToken.cs
+ Filters
    * AppSettingsFilter.cs
    * BaseFilter.cs
    * UserFilter.cs

## NetCoreStarter.Utils
+ Dependencies
+ Generators
    * StringGenerators.cs
+ Helpers
    * DateHelpers.cs
    * ImageHelpers.cs
    * InfobipHelpers.cs
    * MessageHelpers.cs
    * WebHelpers.cs
+ Validators
    * Validators.cs
+ Keys.cs

## NetCoreStarter.Web
+ Connected Services
+ Dependencies
+ wwwroot
+ ApiControllers
    * AccountController.cs
    * AppSettingsController.cs
    * BaseController.cs
+ Controllers
    * HomeController.cs
+ Migrations
+ Models
    * ApplicationDbContext.cs
    * DbSetExtension.cs
    * IdentityExtensions.cs
    * SeedData.cs
+ Repositories
    * BaseRepository.cs
    * RoleRepository.cs
    * UserRepository.cs
+ Views
    * Home
    * Shared
    * _ViewImports.cshtml
    * _ViewStart.cshtml
+ appsettings
+ Program.cs
+ Startup.cs