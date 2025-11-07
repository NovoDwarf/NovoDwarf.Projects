#!/usr/bin/env pwsh

param(
    [string]$OutputPath = "_site",
    [switch]$Serve,
    [switch]$FixDeps
)

Write-Host "📚 Generating documentation..." -ForegroundColor Green

# Clean previous build
if (Test-Path $OutputPath) {
    Remove-Item -Recurse -Force $OutputPath
}

Write-Host "Generating API metadata..." -ForegroundColor Yellow
docfx metadata

Write-Host "Building documentation site..." -ForegroundColor Yellow
docfx build

if ($LASTEXITCODE -eq 0) {
    Write-Host "Documentation generated successfully!" -ForegroundColor Green
    
    if ($Serve) {
        Write-Host "Serving documentation on http://localhost:8080" -ForegroundColor Cyan
        docfx serve _site
    }
} else {
    Write-Host "Documentation generation failed!" -ForegroundColor Red
}