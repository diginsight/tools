$ErrorActionPreference = 'Continue'
$base = Join-Path $env:USERPROFILE ".nuget\packages"
$pkgs = Get-ChildItem $base -Directory -Filter "diginsight.components*"
foreach ($p in $pkgs) {
  $ver = (Get-ChildItem $p.FullName -Directory | Sort-Object { [version]($_.Name) } -Descending | Select-Object -First 1)
  if (-not $ver) { continue }
  $dlls = Get-ChildItem $ver.FullName -Recurse -Filter "*.dll" | Where-Object { $_.FullName -match '\\lib\\net8' }
  foreach ($dll in $dlls) {
    $txt = [System.Text.Encoding]::UTF8.GetString([System.IO.File]::ReadAllBytes($dll.FullName))
    if ($txt -notmatch 'CredentialProvider') { continue }
    "=== $($p.Name) $($ver.Name) : $($dll.Name) ==="
    $runs = @([regex]::Matches($txt, '[\x20-\x7E]{3,}') | ForEach-Object { $_.Value })
    for ($i=0; $i -lt $runs.Count; $i++) {
      if ($runs[$i] -like '*CredentialProvider*') {
        $lo=[Math]::Max(0,$i-8); $hi=[Math]::Min($runs.Count-1,$i+8)
        "  [$i] " + (($runs[$lo..$hi]) -join ' ~ ')
      }
    }
  }
}
