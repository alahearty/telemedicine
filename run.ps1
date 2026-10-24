#requires -Version 5.1
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

function Write-Step {
    param([string]$Message)
    Write-Host ""
    Write-Host "==> $Message" -ForegroundColor Cyan
}

function Stop-WithError {
    param([string]$Message)
    Write-Host ""
    Write-Host "ERROR: $Message" -ForegroundColor Red
    exit 1
}

function Require-Command {
    param([string]$Name, [string]$Hint)
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        Stop-WithError "$Name was not found. $Hint"
    }
}

function Start-AppWindow {
    param([string]$Title, [string]$WorkingDirectory, [string]$Command)

    $safeTitle = $Title.Replace("'", "''")
    $safeDirectory = $WorkingDirectory.Replace("'", "''")
    $script = @'
$Host.UI.RawUI.WindowTitle = '__TITLE__'
Set-Location -LiteralPath '__DIRECTORY__'
__COMMAND__
'@
    $script = $script.Replace('__TITLE__', $safeTitle).Replace('__DIRECTORY__', $safeDirectory).Replace('__COMMAND__', $Command)
    $encoded = [Convert]::ToBase64String([Text.Encoding]::Unicode.GetBytes($script))
    Start-Process -FilePath 'powershell.exe' -ArgumentList @(
        '-NoExit',
        '-NoProfile',
        '-ExecutionPolicy', 'Bypass',
        '-EncodedCommand', $encoded
    ) | Out-Null
}

Write-Host "Telemedicine launcher" -ForegroundColor Green
Write-Host "Repository: $repoRoot"

Require-Command 'dotnet' 'Install the .NET 10 SDK and reopen this window.'
Require-Command 'node' 'Install Node.js 20 or newer and reopen this window.'
Require-Command 'npm' 'Install npm and reopen this window.'

$spaDirectory = Join-Path $repoRoot 'telemedicine-spa'
if (-not (Test-Path (Join-Path $spaDirectory 'node_modules'))) {
    Write-Step 'Installing frontend dependencies'
    Push-Location $spaDirectory
    try {
        & npm install
        if ($LASTEXITCODE -ne 0) { Stop-WithError 'npm install failed.' }
    }
    finally {
        Pop-Location
    }
}

Write-Host ""
Write-Host "PostgreSQL must be running and the telemedicine database must be available." -ForegroundColor Yellow
Write-Host "Demo login: administrator@localhost / Administrator1!"

Write-Step 'Starting the ASP.NET Core API on http://127.0.0.1:5231'
Start-AppWindow -Title 'Telemedicine API (http://127.0.0.1:5231)' -WorkingDirectory $repoRoot -Command @'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
dotnet run --project 'telemedicine-webapi\src\telemedicine.api' --no-launch-profile --urls 'http://127.0.0.1:5231'
'@

Write-Step 'Starting the Vue SPA on http://127.0.0.1:8080'
Start-AppWindow -Title 'Telemedicine SPA (http://127.0.0.1:8080)' -WorkingDirectory $spaDirectory -Command @'
npm run serve
'@

Write-Host ""
Write-Host "Frontend: http://127.0.0.1:8080" -ForegroundColor Yellow
Write-Host "API:      http://127.0.0.1:5231"
Write-Host "Swagger:  http://127.0.0.1:5231/swagger"

Start-Sleep -Seconds 8
Start-Process 'http://127.0.0.1:8080'
