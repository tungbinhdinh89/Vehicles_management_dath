param(
    [Parameter(Mandatory=$true)]
    [ValidateSet('restore','build','test','run','migrate','migration','check','provision','smoke','integration')]
    [string]$Action,
    [string]$Name = 'InitialCreate'
)
$ErrorActionPreference = 'Stop'
$backendRoot = Split-Path $PSScriptRoot -Parent
$originalLocation = Get-Location
$originalEnvironment = @{}
$allowed = @('ConnectionStrings__VehicleCare','ASPNETCORE_ENVIRONMENT','ASPNETCORE_URLS')
try {
    Set-Location $backendRoot
    if (Test-Path -LiteralPath '.env') {
        foreach ($line in Get-Content -LiteralPath '.env') {
            if ($line -match '^\s*([A-Za-z_][A-Za-z0-9_]*)\s*=(.*)$') {
                $key = $matches[1]
                $value = $matches[2].Trim()
                if ($allowed -notcontains $key) { throw "Unsupported environment key: $key" }
                if ($value.Length -ge 2 -and $value.StartsWith('"') -and $value.EndsWith('"')) { $value = $value.Substring(1, $value.Length - 2) }
                if ($null -eq [Environment]::GetEnvironmentVariable($key, 'Process')) {
                    $originalEnvironment[$key] = $null
                    [Environment]::SetEnvironmentVariable($key, $value, 'Process')
                }
            } elseif ($line.Trim() -and -not $line.Trim().StartsWith('#')) { throw 'Invalid .env line.' }
        }
    }
    $dotnetExecutable = if (Test-Path '.tools/dotnet/dotnet.exe') { Join-Path $backendRoot '.tools/dotnet/dotnet.exe' } else { 'dotnet' }
    if ($dotnetExecutable -ne 'dotnet') {
        $originalEnvironment['DOTNET_ROOT'] = $env:DOTNET_ROOT
        $originalEnvironment['PATH'] = $env:PATH
        $env:DOTNET_ROOT = Split-Path $dotnetExecutable -Parent
        $env:PATH = "$env:DOTNET_ROOT;$env:PATH"
    }
    switch ($Action) {
        'restore' { & $dotnetExecutable restore VehicleCare.slnx; if ($LASTEXITCODE -eq 0) { & $dotnetExecutable tool restore } }
        'build' { & $dotnetExecutable build VehicleCare.slnx }
        'test' {
            & $dotnetExecutable test tests/VehicleCare.UnitTests
            if ($LASTEXITCODE -eq 0) { & $dotnetExecutable test tests/VehicleCare.IntegrationTests --filter 'Category=Host' }
        }
        'integration' { & $dotnetExecutable test tests/VehicleCare.IntegrationTests }
        'run' { & $dotnetExecutable run --project src/VehicleCare.Api }
        'migration' { & $dotnetExecutable tool run dotnet-ef migrations add $Name --project src/VehicleCare.Infrastructure --startup-project src/VehicleCare.Api --output-dir Migrations }
        'migrate' { & $dotnetExecutable tool run dotnet-ef database update --project src/VehicleCare.Infrastructure --startup-project src/VehicleCare.Api }
        default { & $dotnetExecutable run --project tools/VehicleCare.DatabaseTool -- $Action }
    }
    if ($LASTEXITCODE -ne 0) { throw "Backend command failed (exit $LASTEXITCODE)." }
} finally {
    foreach ($key in $originalEnvironment.Keys) { [Environment]::SetEnvironmentVariable($key, $originalEnvironment[$key], 'Process') }
    Set-Location $originalLocation
}
