# Release Process

For testing a release, publish the project to a (not quite) single file executable

```
$version = "1.0.0"
dotnet publish .\src\Toggl2Vertec.sln -c Release -r win-x64 -p:Version="$version"
```
