# Why Toggl2Vertec?
This is a command line tool to perform one-way synchronization from time entered in [Toggl Track](https://track.toggl.com/) into a corporate [Vertec](https://www.vertec.com/ch/) instance. Work time entry isn't exactly a strength of Vertec and it also doesn't provide any good options to manage a personalized _model_ for data entry. On the other side, Toggl is a very popular tool to do just that and with a couple of conventions around the Toggl project configuration and a tool like Toggl2Vertec, it is very easy to separate the work time entry to use Toggl and then just collect and aggregate that data and put it into Vertec.

This is a CLI (command line interface) tool. If you are not familiar with and unwilling to learn about the advantages of CLI tools, then this is not the tool for you. It's _primary_ audience are developers and other technically inclined demographics.


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

When you run `scoop update`, it will also look for Toggl2Vertec updates, but if you want to _only_ update Toggl2Vertec, do this
```
scoop update t2v
```

## Manual
For manual installation, see the [Build From Source](#build-from-source)


# Getting Started
The instructions here assume that you have installed Toggl2Vertec using _Scoop_, so you might have to use your brain for some smart replacement when you chose a different installation method.

## Initial Configuration
Before you can do anything useful, you will need a proper configuration which consists of two parts: the configuration file that includes the URL of your Vertec server and your Toggl and Vertec credentials.

To find your Toggl API token, you can go to your Toggl profile under https://track.toggl.com/profile and scroll to the bottom to "API Token". There you can "click to reveal" the token and then use it together with your Vertec login credentials when prompted while running
```
t2v credentials
```

To get a reasonable starting configuration this repository contains a set of managed configurations under https://github.com/lord-executor/Toggl2Vertec/tree/main/configs any of which you can install by chosing the right configuration file and then executing the following command
```
t2v config https://raw.githubusercontent.com/lord-executor/Toggl2Vertec/main/configs/[YOUR-CHOSEN-CONFIG].json
```

Now that everything is configured, you can _check_ your configuration by running
```
t2v check
```

## Daily Grind
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


# Configuring Toggl
Toggl2Vertec tries to match every Toggl entry that it finds with a Vertec project. In order to do that, you need to actually include that information in your Toggl configuration and the way to do that is to add it to your Toggl project names. Based on the regular expression configured in `$.Toggl.Processors[?(@.Name == 'ProjectFilter')].VertecExpression` it will check every entry for the presence of a Vertec project ID (or "phase"). All the entries that are using the same project are aggregated into one Vertec entry, joining all the individual entry texts with a separating ";".

Since the pattern is configurable, everybody can use their own conventions, as long as it is part of the project name.


# Reference

## Commands

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
  check               checks configurations and tries to access Toggl and Vertec
  list <date>         lists the aggregated data from Toggl in Vertec form [default: 12.04.2022 00:00:00]
  update <date>       updates Vertec with the data retrieved from Toggl [default: 12.04.2022 00:00:00]
  credentials         configures Toggl & Vertec credentials throught the command line
  config <configUrl>  Retrieves a pre-defined configuration file from the given URL and installs it in the user's home directory
```
* `t2v --help [command]` - Prints description and all options for any given command

## Configuration

### Basics
```json
"Toggl": {
  // Toggl Track API endpoint URL
  "BaseUrl": "https://api.track.toggl.com",
  // Target key for the Windows Credential Manager where the Toggl credentials are stored
  "CredentialsKey": "t2v:toggl"
},
```

```json
  "Vertec": {
    // Vertec Server version - currently supported are:
    // * the very legacy "2" web UI
    // * the new "6.5" XML API (should work for 6 and upwards, but 6.5 is the only one I can test with)
    "Version": "2",
    // Vertec web UI or XML API endpoint URL
    "BaseUrl": "https://erp.elcanet.local",
    // Target key for the Windows Credential Manager where the Vertec credentials are stored
    "CredentialsKey": "t2v:vertec"
  },
```

### Processors
Most of the interesting things are implemented using _processors_. The data taken from Toggl is aggregated in a `WorkingDay` structure and then a sequence of processors are applied to that data and each processor can do some transformation on that data before it eventually tries to squish that data into the Vertec structure.

In the end, only the `WorkingDay.Date`, `WorkingDay.Summaries` and `WorkingDay.Attendance` is relevant for updating Vertec. The `WorkingDay.Entries` which initially contains the raw, unmodified Toggl time entries is only used to guesstimate the attendance times.

The configuration for the processors looks something like this:
```json
"Processors": [
  {
    "Name": "...",
    // ...
  },
  {
    "Name": "...",
    // ...
  },
  // ...
]
```

Every processor has a name and some specific configuration to control its behavior in more detail.

**ProjectFilter**
```json
{
  "Name": "ProjectFilter",
  // Regular expression to match Vertec project / phase IDs in Toggl project names
  "VertecExpression": "(\\w+-\\w+-\\w+)",
  // Print a warning if there are multiple Toggl projects that refer to the same Vertec project
  "WarnDuplicate": false,
  // Warn if a Toggl time entry does not have a project associated with it
  "WarnMissingProject": true,
  // Warn if a Toggl project does not have a Vertec project / phase ID
  "WarnMissingVertecNumber": true
}
```
This filter is pretty much mandatory since it is the one that extracts the Vertec project / phase ID from the Toggl project name and it also groups multiple projects together. Depending on your Toggl configuration you may want to change the settings for the warnings to make sure you're not missing anything when updating your Vertec data.

Regardless of whether the warnings are enabled or not, functionally any entry that cannot be associated with a Vertec project will be ignored.

**SummaryRounding**
```json
{
  "Name": "SummaryRounding",
  // Rounding increment in minutes - round to the closest whole multiple of N minutes
  "RoundToMinutes": 5
}
```
This processor applies some simple rounding to the summaries which would otherwise be accurate to the second since Toggl tracks work time to the second. This **will** introduce some discrepancies to the attendance time unless you are using the same rounding accuracy for the _AttendanceProcessor_.

**AttendanceProcessor**
```json
{
  "Name": "AttendanceProcessor",
  // Rounding increment in minutes - round to the closest whole multiple of N minutes
  "RoundToMinutes": 5
}
```
This processor calculates the attendance for Vertec. If this processor is not present, no attendance time will be calculated and sent to Vertec.

Since the attendance data is based on the raw time entries and not on the pre-aggregated reporting data that is used for the summaries, there is a pretty good chance that the total time of the summaries is not the same as the total (rounded) time of the entries. To compensate for that, the _last_ attendance block will be automatically adjusted so that it all adds up in the end. Yes, this is a bit fuzzy, but on average it all works out pretty neatly.

**ForceLunch**
```json
{
  "Name": "ForceLunch",
  // When to start an "imaginary" lunch break
  "StartAt": "12:00",
  // Number of minutes of the "imaginary" lunch break
  "Duration": 30
}
```
This is a feature for people that sometimes work though lunch and want that circumstance to remain _private_. Whenever the overall attendance is greater than 5 hours and consists of a single block of time that _includes_ the `StartAt` time, it will split the attendance into two blocks with a gap from the `StartAt` time that lasts for `Duration` minutes. Effectively this will shift the overall attendance end time by `Duration` minutes.

This is just a convenient bit of "creative time tracking" and the processor can be removed from the configuration without any adverse side effects.

**TextCommentFilter**
```json
{
  "Name": "TextCommentFilter",
  // Marker to start a private comment
  "CommentMarker": "//"
}
```
Normally, all the time entry texts for a given Vertec project are aggregated into a single line. This processor allows you to _exclude_ parts of the text from Vertec. By adding a comment marker into the text like `"public text // private text"`, only the part up to the comment marker will be kept and synchronized to Vertec.


# Features
* The synchronization is limited in scope and does NOT affect any Vertec data outside of that scope
  * It only applies to the _day(s)_ that Toggl2Vertec runs for
  * It only updates Vertec entries with project IDs for which there are matching Toggl entries _on those days_
* All credentials are stored encrypted in your [Windows Credential Manager](https://support.microsoft.com/en-us/windows/accessing-credential-manager-1b5c916a-6a16-889f-8581-fc16e8165ac0) - no custom credential management
* Layered configuration with user configuration stored under `~/t2v.settings.json`
* Flexible and configurable processing pipeline to allow a somewhat personalized treatment of Toggl data import



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