# https://medium.com/better-programming/how-to-automatically-add-the-ticket-number-in-git-commit-message-bda5426ded05
# https://sqlnotesfromtheunderground.wordpress.com/2018/01/01/git-pre-commit-hook-with-powershell/
# https://gist.github.com/bartoszmajsak/1396344
# https://githooks.com/
[CmdletBinding()]
Param(
  [Parameter(Mandatory = $True, Position = 0)]
  [string]
  $MessagePath
)

$message = Get-Content -Path $MessagePath -Raw
$tickets = [string]::Empty

$branch = & git rev-parse --abbrev-ref HEAD | Out-String
$branch = $branch.Trim()
$ticketNumberMatches = [Regex]::Matches($branch, "(?:\w+\/)*([a-zA-Z]{2,}[-_]\d+)");
foreach ($match in $ticketNumberMatches) {
  if ($match.Success -and -not ($message.Contains($match.Groups[1].Value.ToUpper()))) {
    $tickets += "[$($match.Groups[1].Value.ToUpper())] "
  }
}

if ([string]::IsNullOrWhiteSpace($tickets))
{
    "$message" | Out-File -NoNewLine -Encoding Utf8 $MessagePath
}
else {
    "${tickets}${message}" | Out-File -NoNewline -Encoding Utf8 $MessagePath
}
