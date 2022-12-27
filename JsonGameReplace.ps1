[CmdletBinding(PositionalBinding=$false)]param
(
    [Parameter(Mandatory=$true)][string]$JsonFile,
	[Parameter(Mandatory=$true)][string]$Config
)
((Get-Content -path $JsonFile -Raw) -replace '"Subnautica"|"BelowZero"',"`"$Config`"") | Set-Content -Path $JsonFile -NoNewline