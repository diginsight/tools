# Validate Metadata Files
# This script validates all .metadata.yml files in the repository against the schema

param(
    [string]$Path = ".",
    [switch]$Fix,
    [switch]$Verbose
)

# Import PowerShell YAML module (install if needed)
if (-not (Get-Module -ListAvailable -Name powershell-yaml)) {
    Write-Host "Installing powershell-yaml module..." -ForegroundColor Yellow
    Install-Module -Name powershell-yaml -Force -Scope CurrentUser
}

Import-Module powershell-yaml

# Required fields in metadata
$requiredFields = @{
    article = @('title', 'author', 'created', 'last_updated', 'status', 'version', 'tags')
    validations = @('grammar', 'readability', 'logic', 'facts', 'structure', 'gaps', 'understandability')
    cross_references = @('related_articles', 'correlated_topics')
    analytics = @('word_count', 'reading_time_minutes', 'code_blocks', 'images', 'external_links')
}

$validStatuses = @('draft', 'in-review', 'published', 'archived')
$validOutcomes = @('passed', 'minor_issues', 'needs_revision', 'failed')

function Test-MetadataFile {
    param($FilePath)
    
    $errors = @()
    $warnings = @()
    
    try {
        $content = Get-Content $FilePath -Raw
        $metadata = ConvertFrom-Yaml $content
        
        # Check article section
        if (-not $metadata.article) {
            $errors += "Missing 'article' section"
        } else {
            foreach ($field in $requiredFields.article) {
                if (-not $metadata.article[$field]) {
                    $errors += "Missing required field: article.$field"
                }
            }
            
            # Validate status
            if ($metadata.article.status -and $metadata.article.status -notin $validStatuses) {
                $errors += "Invalid status: '$($metadata.article.status)'. Must be one of: $($validStatuses -join ', ')"
            }
            
            # Validate dates
            foreach ($dateField in @('created', 'last_updated', 'published_date')) {
                if ($metadata.article[$dateField] -and $metadata.article[$dateField] -ne $null) {
                    try {
                        [datetime]::Parse($metadata.article[$dateField]) | Out-Null
                    } catch {
                        $errors += "Invalid date format in article.$dateField"
                    }
                }
            }
            
            # Validate tags
            if ($metadata.article.tags -and $metadata.article.tags.Count -eq 0) {
                $warnings += "No tags specified"
            }
        }
        
        # Check validations section
        if (-not $metadata.validations) {
            $errors += "Missing 'validations' section"
        } else {
            foreach ($validation in $requiredFields.validations) {
                if (-not $metadata.validations[$validation]) {
                    $warnings += "Missing validation section: validations.$validation"
                }
                
                # Check outcomes
                if ($metadata.validations[$validation].outcome) {
                    $outcome = $metadata.validations[$validation].outcome
                    if ($outcome -notin $validOutcomes -and $outcome -ne 'comprehensive' -and $outcome -ne 'major_gaps' -and $outcome -ne 'minor_gaps') {
                        $warnings += "Invalid outcome in $validation: '$outcome'"
                    }
                }
            }
        }
        
        # Check cross_references
        if (-not $metadata.cross_references) {
            $warnings += "Missing 'cross_references' section"
        }
        
        # Check analytics
        if (-not $metadata.analytics) {
            $warnings += "Missing 'analytics' section"
        }
        
        return @{
            Valid = ($errors.Count -eq 0)
            Errors = $errors
            Warnings = $warnings
        }
        
    } catch {
        return @{
            Valid = $false
            Errors = @("Failed to parse YAML: $($_.Exception.Message)")
            Warnings = @()
        }
    }
}

# Find all metadata files
Write-Host "Scanning for metadata files in: $Path" -ForegroundColor Cyan
$metadataFiles = Get-ChildItem -Path $Path -Filter "*.metadata.yml" -Recurse

Write-Host "Found $($metadataFiles.Count) metadata files`n" -ForegroundColor Cyan

$totalFiles = 0
$validFiles = 0
$invalidFiles = 0
$allResults = @()

foreach ($file in $metadataFiles) {
    $totalFiles++
    $result = Test-MetadataFile -FilePath $file.FullName
    
    $allResults += [PSCustomObject]@{
        File = $file.FullName.Replace((Get-Location).Path, '.')
        Valid = $result.Valid
        Errors = $result.Errors
        Warnings = $result.Warnings
    }
    
    if ($result.Valid) {
        $validFiles++
        if ($Verbose -or $result.Warnings.Count -gt 0) {
            Write-Host "✓ $($file.Name)" -ForegroundColor Green
            if ($result.Warnings.Count -gt 0) {
                foreach ($warning in $result.Warnings) {
                    Write-Host "  ⚠ $warning" -ForegroundColor Yellow
                }
            }
        }
    } else {
        $invalidFiles++
        Write-Host "✗ $($file.Name)" -ForegroundColor Red
        foreach ($error in $result.Errors) {
            Write-Host "  ✗ $error" -ForegroundColor Red
        }
        if ($result.Warnings.Count -gt 0) {
            foreach ($warning in $result.Warnings) {
                Write-Host "  ⚠ $warning" -ForegroundColor Yellow
            }
        }
    }
}

# Summary
Write-Host "`n" + ("=" * 60) -ForegroundColor Cyan
Write-Host "Validation Summary" -ForegroundColor Cyan
Write-Host ("=" * 60) -ForegroundColor Cyan
Write-Host "Total files:   $totalFiles"
Write-Host "Valid:         $validFiles" -ForegroundColor Green
Write-Host "Invalid:       $invalidFiles" -ForegroundColor $(if ($invalidFiles -gt 0) { "Red" } else { "White" })
Write-Host ("=" * 60) -ForegroundColor Cyan

# Export results if requested
if ($Verbose) {
    $reportPath = Join-Path $Path "validation-report.json"
    $allResults | ConvertTo-Json -Depth 10 | Out-File $reportPath
    Write-Host "`nDetailed report saved to: $reportPath" -ForegroundColor Cyan
}

# Exit code
exit $invalidFiles
