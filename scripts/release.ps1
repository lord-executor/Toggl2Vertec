
param(
    [string] $version
)

if (!$version) {
    Write-Error "Version argument is required"
    exit -1
}

Write-Output "Building package for release version $version"

$repoRoot = Resolve-Path "$PSScriptRoot/.."
$solutionFile = "$repoRoot\src\Toggl2Vertec.sln"
$releaseFile = "$repoRoot\t2v-win-x64.zip"
$manifestFile = "$repoRoot\t2v.json"
$gitTag = "v$version"

Push-Location $repoRoot
dotnet publish $solutionFile -c Release -r win-x64 --no-self-contained -p:Version="$version"


Compress-Archive -Path "$repoRoot\src\Toggl2Vertec\bin\Release\net6.0\win-x64\publish\*" -DestinationPath $releaseFile
$sha256 = (Get-FileHash -Algorithm SHA256 $releaseFile).Hash

$manifest = Get-Content $manifestFile | ConvertFrom-Json
$manifest.version = $version
$manifest.hash = $sha256
$manifest.url = $manifest.url -replace "/v\d\.\d\.\d/","/$gitTag/"
$manifest | ConvertTo-Json > $manifestFile

git add .
git commit -m "Release $gitTag"
git tag $gitTag

Write-Output ""
Write-Output "Remaining manual steps:"
Write-Output "* check and push the release commit"
Write-Output "  > git show HEAD"
Write-Output "  > git push"
Write-Output "* push the release tag with"
Write-Output "  > git push origin $gitTag"
Write-Output "* create a new github release attach the generated $releaseFile to the github release"

Pop-Location
