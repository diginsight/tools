$dll = "$env:USERPROFILE\.nuget\packages\diginsight.smartcache\3.7.1.14\lib\net10.0\Diginsight.SmartCache.dll"
$bytes = [System.IO.File]::ReadAllBytes($dll)
$sb = New-Object System.Text.StringBuilder
$strings = New-Object System.Collections.Generic.List[string]
foreach ($b in $bytes) {
    if ($b -ge 32 -and $b -lt 127) { [void]$sb.Append([char]$b) }
    else {
        if ($sb.Length -ge 4) { $strings.Add($sb.ToString()) }
        [void]$sb.Clear()
    }
}
if ($sb.Length -ge 4) { $strings.Add($sb.ToString()) }

'--- lifetime / expiration related identifiers ---'
$strings |
Where-Object { $_ -match '^(get_|set_)?(MaxAge|AbsoluteExpiration|SlidingExpiration|LocalEntry|Expiration|TimeToLive|Ttl|Retention|Duration|Tolerance|Enabled|MaxSize|LowPriority)' } |
ForEach-Object { $_ -replace '^(get_|set_)', '' } |
Sort-Object -Unique
