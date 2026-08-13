$root = 'c:\dev\darioairoldi\Learn'
$report = Get-Content -Raw "$root\.copilot\temp\externalization-report.json" | ConvertFrom-Json
$inv = Get-Content -Raw "$root\.copilot\temp\externalization-inventory.json" | ConvertFrom-Json

$scanPaths = @("$root\.github\agents","$root\.github\prompts","$root\.copilot\context","$root\.github\templates","$root\01.00-news","$root\02.00-events","$root\03.00-tech","$root\04.00-howto","$root\05.00-issues","$root\06.00-idea")

$residual = New-Object System.Collections.Generic.List[string]
$missingPointer = New-Object System.Collections.Generic.List[string]
$danglingPointer = New-Object System.Collections.Generic.List[string]
$badChangelog = New-Object System.Collections.Generic.List[string]
$migrated = 0

foreach ($sp in $scanPaths) {
    if (-not (Test-Path $sp)) { continue }
    Get-ChildItem -Recurse -File -Path $sp -Include *.md -ErrorAction SilentlyContinue -Force | ForEach-Object {
        $f = $_
        if ($f.FullName -match '\\old\\') { return }
        $raw = [System.IO.File]::ReadAllText($f.FullName)
        if ([string]::IsNullOrWhiteSpace($raw)) { return }
        if ($f.Name -like '*.changelog.md') {
            # changelog file checks
            if ($raw -match '(?m)^\s*\w+_metadata:\s*$') { $badChangelog.Add("$($f.Name): has bottom metadata block") }
            if ($raw -notmatch '(?m)^status:\s*"living"') { $badChangelog.Add("$($f.Name): missing status living") }
            if ($raw -notmatch '(?m)^##\s+v\d+\.\d+\.\d+\s+—') { $badChangelog.Add("$($f.Name): no SemVer heading") }
            return
        }
        $i = $raw.LastIndexOf('<!--')
        if ($i -lt 0) { return }
        $blk = $raw.Substring($i)
        if ($blk -notmatch '_metadata:') { return }
        if ($blk -match '(?m)^\s\schanges:\s*$') { $residual.Add($f.FullName.Replace("$root\","")) }
        if ($blk -match '(?m)^\s\schangelog:\s*"([^"]+)"') {
            $migrated++
            $ptr = $Matches[1]
            $clp = Join-Path $f.DirectoryName $ptr
            if (-not (Test-Path $clp)) { $danglingPointer.Add("$($f.Name) -> $ptr") }
        }
    }
}

Write-Host "Report says migrated: $($report.Count)"
Write-Host "Sources with changelog: pointer: $migrated"
Write-Host "Residual changes: arrays: $($residual.Count)"
$residual | ForEach-Object { Write-Host "   RESIDUAL: $_" }
Write-Host "Dangling pointers: $($danglingPointer.Count)"
$danglingPointer | ForEach-Object { Write-Host "   DANGLING: $_" }
Write-Host "Changelog format issues: $($badChangelog.Count)"
$badChangelog | Select-Object -First 20 | ForEach-Object { Write-Host "   BAD: $_" }
