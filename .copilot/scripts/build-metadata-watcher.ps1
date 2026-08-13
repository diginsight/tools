# MetadataWatcher Setup Script
# Builds the C# LSP server and TypeScript extension

param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [switch]$SkipTests,
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"
$RepositoryRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  MetadataWatcher Build Script" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Verify Prerequisites
Write-Host "[1/5] Checking prerequisites..." -ForegroundColor Yellow

# Check .NET SDK
try {
    $dotnetVersion = dotnet --version
    Write-Host "  ✓ .NET SDK: $dotnetVersion" -ForegroundColor Green
}
catch {
    Write-Error ".NET 8.0 SDK not found. Download from: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
}

# Check Node.js
try {
    $nodeVersion = node --version
    Write-Host "  ✓ Node.js: $nodeVersion" -ForegroundColor Green
}
catch {
    Write-Error "Node.js not found. Download from: https://nodejs.org/"
    exit 1
}

Write-Host ""

# Step 2: Clean Previous Build
Write-Host "[2/5] Cleaning previous builds..." -ForegroundColor Yellow

$MetadataWatcherProject = Join-Path $RepositoryRoot "src\MetadataWatcher\MetadataWatcher.csproj"
$OutputPath = Join-Path $RepositoryRoot ".copilot\bin"
$ExtensionPath = Join-Path $RepositoryRoot ".vscode\extensions\metadata-watcher"

if (Test-Path $OutputPath) {
    Write-Host "  Removing: $OutputPath" -ForegroundColor Gray
    Remove-Item $OutputPath -Recurse -Force
}

dotnet clean $MetadataWatcherProject --configuration $Configuration --verbosity quiet
Write-Host "  ✓ Clean completed" -ForegroundColor Green
Write-Host ""

# Step 3: Restore NuGet Packages
Write-Host "[3/5] Restoring NuGet packages..." -ForegroundColor Yellow

dotnet restore $MetadataWatcherProject
if ($LASTEXITCODE -ne 0) {
    Write-Error "NuGet restore failed"
    exit 1
}

Write-Host "  ✓ Packages restored" -ForegroundColor Green
Write-Host ""

# Step 4: Build and Publish C# Server
Write-Host "[4/5] Building C# LSP Server..." -ForegroundColor Yellow

$PublishArgs = @(
    "publish"
    $MetadataWatcherProject
    "--configuration", $Configuration
    "--runtime", "win-x64"
    "--self-contained"
    "/p:PublishSingleFile=true"
    "/p:DebugType=embedded"
    "--output", $OutputPath
)

if ($Verbose) {
    $PublishArgs += "--verbosity", "detailed"
}
else {
    $PublishArgs += "--verbosity", "minimal"
}

dotnet @PublishArgs

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

$ExecutablePath = Join-Path $OutputPath "MetadataWatcher.exe"
if (Test-Path $ExecutablePath) {
    $FileInfo = Get-Item $ExecutablePath
    $SizeMB = [math]::Round($FileInfo.Length / 1MB, 2)
    Write-Host "  ✓ Executable built: $ExecutablePath ($SizeMB MB)" -ForegroundColor Green
}
else {
    Write-Error "Executable not found after build"
    exit 1
}

Write-Host ""

# Step 5: Build TypeScript Extension
Write-Host "[5/5] Building TypeScript Extension..." -ForegroundColor Yellow

Push-Location $ExtensionPath

# Install npm dependencies if node_modules doesn't exist
if (-not (Test-Path "node_modules")) {
    Write-Host "  Installing npm packages..." -ForegroundColor Gray
    npm install --silent
    if ($LASTEXITCODE -ne 0) {
        Pop-Location
        Write-Error "npm install failed"
        exit 1
    }
}

# Compile TypeScript
Write-Host "  Compiling TypeScript..." -ForegroundColor Gray
npm run compile

if ($LASTEXITCODE -ne 0) {
    Pop-Location
    Write-Error "TypeScript compilation failed"
    exit 1
}

Pop-Location

$ExtensionJs = Join-Path $ExtensionPath "out\extension.js"
if (Test-Path $ExtensionJs) {
    Write-Host "  ✓ Extension compiled: $ExtensionJs" -ForegroundColor Green
}
else {
    Write-Error "Extension JavaScript not found after compilation"
    exit 1
}

Write-Host ""

# Summary
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "  Build Completed Successfully! " -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Reload VS Code: Ctrl+Shift+P → 'Developer: Reload Window'" -ForegroundColor White
Write-Host "  2. Check status bar for: ✓ Metadata Watcher" -ForegroundColor White
Write-Host "  3. Try renaming an article to test synchronization" -ForegroundColor White
Write-Host ""
Write-Host "Server executable: $ExecutablePath" -ForegroundColor Gray
Write-Host "Extension path: $ExtensionPath" -ForegroundColor Gray
Write-Host ""
