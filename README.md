
# Preparation / Configuration

## Credentials with Windows Credential Manager
To open the Windows credential manager, click on the start menu and just type "Credential Manager". All credentials have to be added to the "Windows Credential > Generic Credentials subcategory.

For Vertec, create an entry called `Vertec Login` with
* Username: Your Vertec username
* Password: Your Vertec password

For Toggl, create an entry called `Toggl Token` with
* Username: Your email (actually, you can put anything in there)
* Password: Your Toggl API token. You can get that from you Toggl profile under https://track.toggl.com/profile (scroll to the bottome to "API Token").

# Run From Source
Make sure you have at least the .NET 5 SDK installed.
```
dotnet --version
```
should return something like "5.0.302"

Clone this repository
```
git clone ssh://git@bitbucket.svc.elca.ch:7999/~lan/toggl2vertec.git
```

Go to the source directory and build to make sure everything is OK
```
cd toggle2vertec/src/Toggl2Vertec
dotnet build
```

Check your configuration with
```
dotnet run check
```

Show your daily summary from Toggl in "Vertec format" without actually changing anything in Vertec
```
dotnet run list [DATE-IN-YYYY-MM-DD-FORM]
```

Update Vertec for the day - this will override any existing entries for which there is corresponding data in Toggl
```
dotnet run update
```
