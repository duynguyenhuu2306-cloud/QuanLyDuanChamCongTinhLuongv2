param(
    [switch]$SkipBackend,
    [switch]$OpenBrowser
)

$ErrorActionPreference = 'Stop'
$projectRoot = $PSScriptRoot
$frontendPath = Join-Path $projectRoot 'frontend'
$frontendScript = Join-Path $frontendPath 'server.js'
$certificateDirectory = Join-Path $PSScriptRoot '.local-certs'
$certificatePath = Join-Path $certificateDirectory 'localhost.pfx'
$certificatePassword = 'test123'

function Test-ListeningPort([int]$Port) {
    return [bool](Get-NetTCPConnection -State Listen -LocalPort $Port -ErrorAction SilentlyContinue | Select-Object -First 1)
}

function Test-FrontendResponse {
    try {
        $response = Invoke-WebRequest -Uri 'http://127.0.0.1:5500/' -UseBasicParsing -TimeoutSec 2
        return $response.StatusCode -eq 200
    } catch {
        return $false
    }
}

function Test-HttpsFrontendResponse {
    try {
        $response = Invoke-WebRequest -Uri 'https://localhost:5501/' -UseBasicParsing -TimeoutSec 2
        return $response.StatusCode -eq 200
    } catch {
        return $false
    }
}

if (-not (Get-Command node -ErrorAction SilentlyContinue)) {
    throw 'Node.js was not found. Install Node.js and run this script again.'
}

if (-not (Test-Path -LiteralPath $frontendScript)) {
    throw "Frontend server script was not found: $frontendScript"
}

New-Item -ItemType Directory -Path $certificateDirectory -Force | Out-Null
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw 'The .NET SDK is required to create and trust the local HTTPS development certificate.'
}

& dotnet dev-certs https --check --trust | Out-Null
if ($LASTEXITCODE -ne 0) {
    & dotnet dev-certs https --trust
    if ($LASTEXITCODE -ne 0) {
        throw 'Could not create/trust the HTTPS development certificate. Accept the Windows trust prompt and retry.'
    }
}

& dotnet dev-certs https --export-path $certificatePath --password $certificatePassword
if ($LASTEXITCODE -ne 0) {
    throw 'Could not export the HTTPS development certificate for the frontend.'
}

$httpAvailable = (Test-ListeningPort 5500) -and (Test-FrontendResponse)
$httpsAvailable = (Test-ListeningPort 5501) -and (Test-HttpsFrontendResponse)

if ($httpAvailable -and $httpsAvailable) {
    Write-Host 'Frontend is already running at http://localhost:5500/ and https://localhost:5501/' -ForegroundColor Green
} else {
    if ((Test-ListeningPort 5500) -or (Test-ListeningPort 5501)) {
        throw 'Port 5500 or 5501 is occupied by a process that is not serving both Workly URLs. Close that process and retry.'
    }

    $env:WORKLY_HTTPS_PFX = $certificatePath
    $env:WORKLY_HTTPS_PFX_PASSWORD = $certificatePassword
    Start-Process -FilePath 'node' -ArgumentList @('server.js') -WorkingDirectory $frontendPath -WindowStyle Hidden

    $deadline = (Get-Date).AddSeconds(10)
    while ((-not (Test-FrontendResponse) -or -not (Test-HttpsFrontendResponse)) -and (Get-Date) -lt $deadline) {
        Start-Sleep -Milliseconds 250
    }

    if (-not (Test-FrontendResponse) -or -not (Test-HttpsFrontendResponse)) {
        throw 'Could not start both frontend endpoints. Check Node.js and ports 5500/5501.'
    }

    Write-Host 'Started frontend at http://localhost:5500/ and https://localhost:5501/' -ForegroundColor Green
}

if (-not $SkipBackend) {
    $solution = Join-Path $projectRoot 'QuanLyDuAnChamCong.sln'
    $vs = Get-Command devenv.com -ErrorAction SilentlyContinue

    if (-not $vs) {
        $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
        if (Test-Path -LiteralPath $vswhere) {
            $vsPath = & $vswhere -latest -products '*' -find 'Common7\IDE\devenv.exe' | Select-Object -First 1
            if ($vsPath) {
                $vs = [pscustomobject]@{ Source = $vsPath }
            }
        }
    }

    if ($vs) {
        Start-Process -FilePath $vs.Source -ArgumentList @('"' + $solution + '"')
        Write-Host 'Opened the solution in Visual Studio. Select the "New Profile" multi-start configuration and press F5 to run the APIs.' -ForegroundColor Cyan
    } else {
        Write-Host 'Visual Studio was not found. The frontend is running; open QuanLyDuAnChamCong.sln and start the "New Profile" multi-start configuration to run the APIs.' -ForegroundColor Yellow
    }
}

if ($OpenBrowser) {
    Start-Process 'https://localhost:5501/'
}

Write-Host 'The frontend runs in the background for this Windows session. Run this script again after restarting the computer.' -ForegroundColor Gray
