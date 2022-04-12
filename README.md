
# Prerequisites
* Toggl2Vertec requires .NET 6. You can download the ".NET Runtime 6.x" from [here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) if you don't already have it.
* [scoop.sh](https://scoop.sh/) for a convenient installation and update experience.

## Recommended
* [PowerShell Core](https://github.com/PowerShell/PowerShell)
* [Windows Terminal](https://github.com/microsoft/terminal)
  * [Windows Terminal Setup Guide](https://hackmd.io/@larcanum/BJe--EJl8)

# Installation

## Using Scoop
You can get scoop from [scoop.sh](https://scoop.sh/).

Install Toggl2Vertec 
```
scoop install https://raw.githubusercontent.com/lord-executor/Toggl2Vertec/main/t2v.json
```

# Preparation / Configuration

## Credentials with Windows Credential Manager
To open the Windows credential manager, click on the start menu and just type "Credential Manager". All credentials have to be added to the "Windows Credential > Generic Credentials subcategory.

For Vertec, create an entry called `Vertec Login` with
* Username: Your Vertec username
* Password: Your Vertec password

For Toggl, create an entry called `Toggl Token` with
* Username: Your email (actually, you can put anything in there)
* Password: 

# Getting Started
The instructions here assume that you have installed Toggl2Vertec using _Scoop_, so you might have to use your brain for some smart replacement when you chose a different installation method.

Before you can do anything useful, you will need a proper configuration which consists of two parts: the configuration file that includes the URL of your Vertec server and your Toggl and Vertec credentials.

To find your Toggl API token, you can go to your Toggl profile under https://track.toggl.com/profile and scroll to the bottom to "API Token". There you can "click to reveal" the token and then use it together with your Vertec login credentials when prompted while running
```
t2v credentials
```

TODO: config file

Now that everything is configured, you can _check_ your configuration by running
```
t2v check
```

Show your daily summary from Toggl in "Vertec format" without actually changing anything in Vertec
```
t2v list [DATE-IN-YYYY-MM-DD-FORM]
# for example
t2v list 2022-05-25
```

Update Vertec for the day - this will override any existing entries for which there is corresponding data in Toggl
```
t2v update [DATE-IN-YYYY-MM-DD-FORM]
# for example
t2v update 2022-05-25
```

# Commands

* `t2v --version` - Prints the Toggl2Vertec version number (assembly file version)
* `t2v --help` - Prints a list of all the available commands and also general usage information
```
Description:
  Synchronizes time entries from Toggl (Trac) to Vertec

Usage:
  Toggl2Vertec [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  check          checks configurations and tries to access Toggl and Vertec
  list <date>    lists the aggregated data from Toggl in Vertec form [default: TODAY]
  update <date>  updates Vertec with the data retrieved from Toggl [default: TODAY]
  credentials    configures Toggl & Vertec credentials throught the command line
```
* `t2v --help [command]` - Prints description and all options for any given command

# Features
* All credentials are stored encrypted in your [Windows Credential Manager](https://support.microsoft.com/en-us/windows/accessing-credential-manager-1b5c916a-6a16-889f-8581-fc16e8165ac0) - no custom credential management
* Layered configuration with user configuration stored under `~/t2v.settings.json`
* TODO

# Build From Source
Make sure you have at least the .NET 6 SDK installed.
```
dotnet --version
```
should return something like "6.0.102"

Clone this repository
```
# _with_ SSH key (which as a developer you really should have)
git clone git@github.com:lord-executor/Toggl2Vertec.git
# _without_ an SSH key (shame on you)
git clone https://github.com/lord-executor/Toggl2Vertec.git
```

Go to the source directory and build to make sure everything is OK
```
cd toggl2vertec/src/Toggl2Vertec
dotnet build
```

## Convenient PowerShell Alias
To make the setup a bit more convenient, you can easily create an alias in your PowerShell Core to run the tool without having to navigate to the correct path first.

Open your PowerShell profile in your favorite editor
```
# Using VisualStudio Code editor here
code $PROFILE
```

Add the following code to the end of your profile, replacing the `[PathToYourToggl2VertecSource]` with your actual path of course
```
function Toggl2Vertec() {
    Param(
        [parameter(ValueFromRemainingArguments = $true)]
        [string[]]$Passthrough
    )
    & "[PathToYourToggl2VertecSource]\src\Toggl2Vertec\bin\Debug\net6.0\Toggl2Vertec.exe" @Passthrough
}
Set-Alias -Name "t2v" -Value Toggl2Vertec
```

With that done, you can run the tool by simply opening a new PowerShell and doing something like
```
t2v list --verbose
```