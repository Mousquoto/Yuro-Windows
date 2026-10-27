param([string]$Version='1.0.3')
$ErrorActionPreference='Stop'
if ($Version -notmatch '^\d+\.\d+\.\d+$') { throw 'Version invalide' }
$projectRoot=Split-Path $PSScriptRoot -Parent
$outDir=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../YURO-PC'))
$unpacked=Join-Path $outDir 'win-unpacked'
$compiler=Join-Path $env:WINDIR 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
Push-Location $PSScriptRoot
try {
  npm version $Version --no-git-tag-version --allow-same-version
  if ($LASTEXITCODE -ne 0) { throw 'Version impossible' }
  npx electron-builder --win dir --x64 --publish never
  if ($LASTEXITCODE -ne 0) { throw 'Construction de l application impossible' }
  "provider: github
owner: Mousquoto
repo: Yuro-Windows
releaseType: release
" | Set-Content -LiteralPath (Join-Path $unpacked 'resources/app-update.yml') -Encoding utf8
  $installerSource=Join-Path $PSScriptRoot 'Installer.cs'
  $sourceText=[IO.File]::ReadAllText($installerSource) -replace 'key.SetValue\("DisplayVersion","[^"]+"\)', ('key.SetValue("DisplayVersion","'+$Version+'")')
  [IO.File]::WriteAllText($installerSource,$sourceText)
  $uiSource=Join-Path $PSScriptRoot 'InstallerUI.cs'
  $uiText=[IO.File]::ReadAllText($uiSource) -replace 'YURO POUR WINDOWS   /   \d+\.\d+\.\d+', "YURO POUR WINDOWS   /   $Version"
  [IO.File]::WriteAllText($uiSource,$uiText)
  $refs=@('/reference:System.Windows.Forms.dll','/reference:System.Drawing.dll','/reference:System.IO.Compression.dll','/reference:System.IO.Compression.FileSystem.dll','/reference:Microsoft.CSharp.dll')
  & $compiler /nologo /target:winexe /platform:x64 @refs "/win32icon:$PSScriptRoot/assets/yuro.ico" "/resource:$PSScriptRoot/assets/yuro-logo.png,Logo.png" "/out:$unpacked/YURO-Desinstaller.exe" "$PSScriptRoot/Installer.cs" "$PSScriptRoot/InstallerUI.cs"
  if ($LASTEXITCODE -ne 0) { throw 'Construction du désinstallateur impossible' }
  $payload=Join-Path $outDir ('payload-'+[guid]::NewGuid().ToString('N')+'.zip')
  Add-Type -AssemblyName System.IO.Compression.FileSystem
  [IO.Compression.ZipFile]::CreateFromDirectory($unpacked,$payload,[IO.Compression.CompressionLevel]::Optimal,$false)
  $installer=Join-Path $outDir "YURO-Installation-$Version.exe"
  & $compiler /nologo /target:winexe /platform:x64 @refs "/win32icon:$PSScriptRoot/assets/yuro.ico" "/resource:$PSScriptRoot/assets/yuro-logo.png,Logo.png" "/resource:$payload,Payload.zip" "/out:$installer" "$PSScriptRoot/Installer.cs" "$PSScriptRoot/InstallerUI.cs"
  if ($LASTEXITCODE -ne 0) { throw 'Construction de l installateur impossible' }
  $hash=[Convert]::ToBase64String([Security.Cryptography.SHA512]::Create().ComputeHash([IO.File]::ReadAllBytes($installer)))
  $size=(Get-Item -LiteralPath $installer).Length
  $name=[IO.Path]::GetFileName($installer)
  $date=[DateTime]::UtcNow.ToString('o')
  "version: $Version`nfiles:`n  - url: $name`n    sha512: $hash`n    size: $size`npath: $name`nsha512: $hash`nreleaseDate: '$date'`n" | Set-Content -LiteralPath (Join-Path $outDir 'latest.yml') -Encoding utf8
  Write-Output $installer
} finally { Pop-Location }
