# Organization.Addressbook
1 day challenge to build Backend for an organization address book using VS Code and Github CoPilot

## Step 1: Prompt used
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

## Step 2: Prompt used
I want to build a data model to record a list of client organizations with the below specifications:
* The organization name and business number (ABN and ACN)
* The organization addresses as the organization may have different branches in different locations
* Each branch and address can have a different contact details which can be a landline number or mobile number or email address
* Each organization can have multiple people working for them
* Each person can work for more than one organization
* Each person can have a different home, work and/or mailing addresses
* Each person can have different contact details which can be a landline number or mobile number or email address

### Step 2.1
Can you change the model to use GUID as primary keys?