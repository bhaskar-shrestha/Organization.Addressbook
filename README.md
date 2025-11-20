# Organization.Addressbook
1 day challenge to build Backend for an organization address book using VS Code and Github CoPilot

## Prompt used
create a solution with webapi project and a nunit test project

## Solution layout

- `src/Organization.Addressbook.Api` - ASP.NET Core Web API project
- `tests/Organization.Addressbook.Tests` - NUnit test project

## Build and run (Windows `cmd.exe`)

Restore, build and run the API:

```
dotnet restore
dotnet build
dotnet run --project src\Organization.Addressbook.Api\Organization.Addressbook.Api.csproj
```

Run tests:

```
dotnet test
```

The API exposes a sample endpoint at `GET /api/AddressBook`.
