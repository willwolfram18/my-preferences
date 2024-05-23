$userName = "TODO_FIXME"

#region Modules
if (Get-Module -ListAvailable -Name "oh-my-posh") {
    oh-my-posh init pwsh --config "C:\Users\${env:UserName}\AppData\Local\Programs\oh-my-posh\themes\agnoster.minimal.omp.json" | Invoke-Expression
}
else {
    Write-Host -ForegroundColor Red "Missing oh-my-posh module: https://ohmyposh.dev/docs/installation/windows"
}

if (Get-Module -ListAvailable -Name "posh-git") {
    Import-Module posh-git
}
else {
    Install-Module posh-git -Scope CurrentUser -Force
    Import-Module posh-git
}

if ($PSVersionTable.PSVersion.Major -ge 7) {
    Set-PSReadLineKeyHandler -Chord "Tab" -Function AcceptSuggestion
    Set-PSReadLineKeyHandler -Chord "Shift+Tab" -Function MenuComplete
}
#endregion

# Stops the GitHub CLI from constantly pestering for authentication details for GitHub Enterprise accounts
$env:GCM_GITHUB_ACCOUNTFILTERING = $false

# Add all executables in my ~/.bin folder to the path
ls ~/.bin | Where-Object { $_.PSIsContainer } | ForEach-Object { $env:Path += ";$($_.FullName)" }

#region Kuberenetes helpers
function kube() { kubectl $args }
Set-Alias k kube
function kgrep($name, $type = "pod") { kube get $type $args | grep $name }
function kbash($pod) { kube exec -it $args $pod -- bash }
function kls($pod, $path = ".") { kube exec $args $pod -- ls $path }
function kcat($pod, $file) { kube exec $args $pod -- cat $file }
# Short hand for getting the first item that matches the grep pattern
function kfirst($name, $type = "pod") { kube get $type -o=name $args | grep $name | Select-Object -first 1 }
function kfbash($pod) { kbash $(kfirst $pod) }
function kfcat($pod, $file) { kcat $(kfirst $pod) $file }
#endregion

#region Aliases
Set-Alias grep Select-String
Set-Alias clip Set-Clipboard
Set-Alias jq jq-windows-amd64
Set-Alias vs 'C:\Program Files\Microsoft Visual Studio\2022\Professional\Common7\IDE\devenv.exe'
#endregion

#region Other functions
function base64($text) {
    $textBytes = [System.Text.Encoding]::UTF8.GetBytes($text)
    return [Convert]::ToBase64String($textBytes);
}

function decode64($text) {
    $textBytes = [Convert]::FromBase64String($text)
    return [System.Text.Encoding]::UTF8.GetString($textBytes);
}
#endregion
