
$sourceUri = "https://github.com/bblanchon/pdfium-binaries/releases/latest/download/pdfium-win-x64.tgz"

$tmpFolderPath = Join-Path $PSScriptRoot ".tmp"
$tmpFilePath = Join-Path $tmpFolderPath "pdfium-win-x64.tgz"
$targetFolderPath = Join-Path $PSScriptRoot "Libraries"
$targetFolderPath = Join-Path $targetFolderPath "Pdfium"
$latestFolderPath = Join-Path $targetFolderPath "latest"
$versionFilePath = Join-Path $latestFolderPath "VERSION"

If (-not (Test-Path -Path $tmpFolderPath)) {
    New-Item $tmpFolderPath -ItemType Directory
}

If (Test-Path -Path $tmpFilePath) {
    Remove-Item $tmpFilePath
}

Invoke-WebRequest $sourceUri -OutFile $tmpFilePath

If (-not (Test-Path -Path $latestFolderPath)) {
    New-Item $latestFolderPath -ItemType Directory
}

Set-Location $latestFolderPath

tar -x -v -z -f "$tmpFilePath" 

If (Test-Path $versionFilePath) {
    $versionInfo = Get-Content $versionFilePath | ConvertFrom-StringData
    $version = [System.String]::Join(".", $versionInfo.MAJOR, $versionInfo.MINOR, $versionInfo.BUILD, $versionInfo.PATCH)

    $archiveFilePath = Join-Path $targetFolderPath "pdfium-win-x64-$version.tgz"

    If (-not (Test-Path $archiveFilePath)) {
        Copy-Item -Path $tmpFilePath $archiveFilePath
    }
}
