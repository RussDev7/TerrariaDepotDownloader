:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
::Install TerrariaDepotDownloader Via MSBuild                    ::
::Gihub https://github.com/RussDev7/TerrariaDepotDownloader      ::
::Devoloped, Maintained, And Sponsored By Discord:dannyruss      ::
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
@ECHO OFF

Rem | Set Params
Set "VersionPrefix=1.8.5.7"
Set "filename=TerrariaDepotDownloader-%VersionPrefix%"

Rem | Install SLN Under x64 Profile
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\MSBuild.exe ".\src\TerrariaDepotDownloader.sln" /p:Configuration=Release /p:Platform=x64"

Rem | Delete Paths & Create Paths
rmdir /s /q ".\release"
mkdir ".\release"

Rem | Copy Over Items
xcopy /E /Y ".\src\TerrariaDepotDownloader\bin\x64\Release" ".\release\%filename%\"
copy ".\src\TerrariaDepotDownloader\ExternalResources\ManifestVersions.cfg" ".\release\%filename%\"
copy ".\src\packages\DotNetZip.1.16.0\lib\net40\DotNetZip.dll" ".\release\%filename%\"
copy ".\src\TerrariaDepotDownloader\DepotDownloader\DepotDownloader.dll" ".\release\%filename%\"
copy ".\src\TerrariaDepotDownloader\DepotDownloader\protobuf-net.Core.dll" ".\release\%filename%\"
copy ".\src\TerrariaDepotDownloader\DepotDownloader\protobuf-net.dll" ".\release\%filename%\"
copy ".\src\TerrariaDepotDownloader\DepotDownloader\SteamKit2.dll" ".\release\%filename%\"
copy ".\src\TerrariaDepotDownloader\DepotDownloader\DepotDownloader.runtimeconfig.json" ".\release\%filename%\"
copy ".\src\TerrariaDepotDownloader\DepotDownloader\DepotDownloader.deps.json" ".\release\%filename%\"
copy ".\src\TerrariaDepotDownloader\DepotDownloader\System.IO.Hashing.dll" ".\release\%filename%\"

Rem | Clean Up Files
del /f /q /s ".\release\*.xml"
del /f /q /s ".\release\*.pdb"
del /f /q /s ".\release\*.config"

Rem | Delete & Create ZIP Release
if exist ".\%filename%.zip" (del /f ".\%filename%.zip")
powershell.exe -nologo -noprofile -command "Compress-Archive -Path ".\release\*" -DestinationPath ".\%filename%.zip""

Rem | Operation Complete
echo(
pause
