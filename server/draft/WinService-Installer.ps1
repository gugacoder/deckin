#$acl = Get-Acl "C:\Program Files (x86)\Processa Sistemas\ProcessaApp"
#$aclRuleArgs = "Processa.com\guga", "Read,Write,ReadAndExecute", "ContainerInherit,ObjectInherit", "None", "Allow"
#$accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule($aclRuleArgs)
#$acl.SetAccessRule($accessRule)
#$acl | Set-Acl "C:\Program Files (x86)\Processa Sistemas\ProcessaApp"

#New-Service -Name ProcessaApp -BinaryPathName "C:\Program Files (x86)\Processa Sistemas\ProcessaApp\Director.exe" -Credential "Processa.com\guga" -Description "Plataforma de aplicativos da Processa." -DisplayName "Processa - App" -StartupType Automatic
New-Service -Name ProcessaApp -BinaryPathName "C:\Program Files (x86)\Processa Sistemas\ProcessaApp\Director.exe" -Description "Plataforma de aplicativos da Processa." -DisplayName "Processa - App" -StartupType Automatic
Start-Service -Name ProcessaApp
