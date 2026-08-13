# Check for Stale Validations
# This script finds articles with outdated validation checks

param(
    [string]$Path = ".",
    [int]$GrammarDays = 7,
    [int]$ReadabilityDays = 7,
    [int]$FactsDays = 30,
    [int]$StructureDays = 7,
    [switch]$ExportCsv
)

Import-Module powershell-yaml -ErrorAction SilentlyContinue

function Test-ValidationStale {
    param(
        $Metadata,
        $ValidationName,
        $MaxDays
    )
    
    if (-not $Metadata.validations) { return $null }
    if (-not $Metadata.validations[$ValidationName]) { return $null }
    
    $validation = $Metadata.validations[$ValidationName]
    if (-not $validation.last_run) {
        return @{
            Status = 'Never Run'
            DaysOld = $null
            Stale = $true
        }
    }
    
    try {
        $lastRun = [datetime]::Parse($validation.last_run)
        $daysOld = ((Get-Date) - $lastRun).TotalDays
        
        return @{
            Status = $validation.outcome
            LastRun = $lastRun
            DaysOld = [math]::Round($daysOld, 1)
            Stale = ($daysOld -gt $MaxDays)
        }
    } catch {
        return @{
            Status = 'Invalid Date'
            DaysOld = $null
            Stale = $true
        }
    }
}

Write-Host "Checking for stale validations in: $Path" -ForegroundColor Cyan
Write-Host "Thresholds: Grammar($GrammarDays days), Readability($ReadabilityDays days), Facts($FactsDays days), Structure($StructureDays days)`n" -ForegroundColor Cyan

$metadataFiles = Get-ChildItem -Path $Path -Filter "*.metadata.yml" -Recurse
$results = @()

foreach ($file in $metadataFiles) {
    try {
        $content = Get-Content $file.FullName -Raw
        $metadata = ConvertFrom-Yaml $content
        
        $articleName = $metadata.article.title
        if (-not $articleName) {
            $articleName = $file.BaseName -replace '\.metadata$', ''
        }
        
        # Check each validation type
        $grammarCheck = Test-ValidationStale -Metadata $metadata -ValidationName 'grammar' -MaxDays $GrammarDays
        $readabilityCheck = Test-ValidationStale -Metadata $metadata -ValidationName 'readability' -MaxDays $ReadabilityDays
        $factsCheck = Test-ValidationStale -Metadata $metadata -ValidationName 'facts' -MaxDays $FactsDays
        $structureCheck = Test-ValidationStale -Metadata $metadata -ValidationName 'structure' -MaxDays $StructureDays
        
        $staleValidations = @()
        if ($grammarCheck.Stale) { $staleValidations += "grammar" }
        if ($readabilityCheck.Stale) { $staleValidations += "readability" }
        if ($factsCheck.Stale) { $staleValidations += "facts" }
        if ($structureCheck.Stale) { $staleValidations += "structure" }
        
        if ($staleValidations.Count -gt 0) {
            $articlePath = $file.FullName.Replace('.metadata.yml', '.md')
            $articleExists = Test-Path $articlePath
            $lastModified = if ($articleExists) { (Get-Item $articlePath).LastWriteTime } else { $null }
            
            $result = [PSCustomObject]@{
                Article = $articleName
                File = $file.FullName.Replace((Get-Location).Path, '.')
                Status = $metadata.article.status
                LastModified = $lastModified
                StaleValidations = $staleValidations -join ', '
                GrammarDays = $grammarCheck.DaysOld
                ReadabilityDays = $readabilityCheck.DaysOld
                FactsDays = $factsCheck.DaysOld
                StructureDays = $structureCheck.DaysOld
                ArticleExists = $articleExists
            }
            
            $results += $result
            
            # Display result
            Write-Host "⚠ $articleName" -ForegroundColor Yellow
            Write-Host "  File: $($result.File)" -ForegroundColor Gray
            Write-Host "  Status: $($result.Status)" -ForegroundColor Gray
            if ($lastModified) {
                Write-Host "  Last Modified: $($lastModified.ToString('yyyy-MM-dd'))" -ForegroundColor Gray
            }
            Write-Host "  Stale Validations: $($result.StaleValidations)" -ForegroundColor Yellow
            
            foreach ($validation in $staleValidations) {
                $check = switch ($validation) {
                    'grammar' { $grammarCheck }
                    'readability' { $readabilityCheck }
                    'facts' { $factsCheck }
                    'structure' { $structureCheck }
                }
                
                if ($check.Status -eq 'Never Run') {
                    Write-Host "    - $validation`: Never run" -ForegroundColor Red
                } elseif ($check.DaysOld) {
                    Write-Host "    - $validation`: $($check.DaysOld) days old (status: $($check.Status))" -ForegroundColor Yellow
                } else {
                    Write-Host "    - $validation`: Invalid data" -ForegroundColor Red
                }
            }
            Write-Host ""
        }
        
    } catch {
        Write-Host "✗ Failed to process $($file.Name): $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Summary
Write-Host ("=" * 60) -ForegroundColor Cyan
Write-Host "Summary" -ForegroundColor Cyan
Write-Host ("=" * 60) -ForegroundColor Cyan
Write-Host "Total metadata files checked: $($metadataFiles.Count)"
Write-Host "Articles with stale validations: $($results.Count)" -ForegroundColor $(if ($results.Count -gt 0) { "Yellow" } else { "Green" })

if ($results.Count -gt 0) {
    Write-Host "`nRecommendations:" -ForegroundColor Cyan
    Write-Host "1. Run validation prompts on articles with stale checks"
    Write-Host "2. Use /metadata-update after running validations"
    Write-Host "3. For published articles, prioritize fact-checking validation"
    Write-Host "4. Use /publish-ready to get comprehensive status"
}

# Export to CSV if requested
if ($ExportCsv -and $results.Count -gt 0) {
    $csvPath = Join-Path $Path "stale-validations.csv"
    $results | Export-Csv -Path $csvPath -NoTypeInformation
    Write-Host "`nResults exported to: $csvPath" -ForegroundColor Cyan
}

# Return results for further processing
return $results
