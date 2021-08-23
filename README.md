
# Preparation / Configuration

## Credentials with Windows Credential Manager
To open the Windows credential manager, click on the start menu and just type "Credential Manager". All credentials have to be added to the "Windows Credential > Generic Credentials subcategory.

For Vertec, create an entry called `Vertec Login` with
* Username: Your Vertec username
* Password: Your Vertec password

For Toggl, create an entry called `Toggl Token` with
* Username: Your email (actually, you can put anything in there)
* Password: Your Toggl API token. You can get that from you Toggl profile under https://track.toggl.com/profile (scroll to the bottom to "API Token").

# Commands

* `check [--verbose]` - Checks if the credentials are configured correctly and if both Toggl and Vertec can be accessed.
* `list [date] [--verbose]` - Grabs the data from Toggl for the given date (today if nothing is specified) and shows the data that would be written to Vertec without actually changing anything in Vertec.
* `update [date] [--verbose]` - Works just like `list` except that after showing the data it will try to update Vertec with that data.

# Run From Source
Make sure you have at least the .NET 5 SDK installed.
```
dotnet --version
```
should return something like "5.0.302"

Clone this repository
```
# _with_ SSH key (which as a developer you really should have)
git clone ssh://git@bitbucket.svc.elca.ch:7999/~lan/toggl2vertec.git
# _without_ an SSH key (shame on you)
git clone https://bitbucket.svc.elca.ch/scm/~lan/toggl2vertec.git
```

Go to the source directory and build to make sure everything is OK
```
cd toggl2vertec/src/Toggl2Vertec
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

## Convenient PowerShell Alias
To make the setup a bit more convenient, you can easily create an alias in your PowerShell Core to run the tool without having to navigate to the correct path first.

Open your PowerShell profile in your favorite editor
```
# Using VisualStudio Code editor here
code $PROFILE
```

Add the following code to the end of your profile, replacing the `[PathToYourToggleSource]` with your actual path of course
```
function Toggle2Vertec() {
    Param(
        [parameter(ValueFromRemainingArguments = $true)]
        [string[]]$Passthrough
    )
    & "[PathToYourToggleSource]\src\Toggl2Vertec\bin\Debug\net5.0\Toggl2Vertec.exe" @Passthrough
}
Set-Alias -Name "t2v" -Value Toggle2Vertec
```

With that done, you can run the tool by simply opening a new PowerShell and doing something like
```
t2v list --verbose
```
