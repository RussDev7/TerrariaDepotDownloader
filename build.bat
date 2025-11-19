:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
::Install TerrariaDepotDownloader Via MSBuild Tools              ::
::Gihub https://github.com/RussDev7/TerrariaDepotDownloader      ::
::Devoloped, Maintained, And Sponsored By Discord:dannyruss      ::
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
@ECHO OFF

Rem | Set parameters.
Set "VersionPrefix=1.8.5.8"
Set "filename=TerrariaDepotDownloader-%VersionPrefix%"

Rem | Put the expected location of vswhere into a variable.
set "VSWHERE=%ProgramFiles(x86)%\Microsoft Visual Studio\Installer\vswhere.exe"

Rem | Ask for the newest VS install that includes Microsoft.Component.MSBuild
Rem | and let vswhere do the glob‑expansion that finds MSBuild.exe.
for /f "usebackq tokens=*" %%I in (`
  "%VSWHERE%" -latest ^
              -products * ^
              -requires Microsoft.Component.MSBuild ^
              -find MSBuild\**\Bin\MSBuild.exe
`) do (
    set "MSBUILD=%%I"
)

Rem | Install SLN under x64 profile.
"%MSBUILD%" ".\src\TerrariaDepotDownloader.sln" /p:Configuration=Release /p:Platform=x64"

Rem | Delete paths & create paths.
rmdir /s /q ".\release"
mkdir ".\release"

Rem | Copy over items.
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
copy ".\src\TerrariaDepotDownloader\DepotDownloader\QRCoder.dll" ".\release\%filename%\"

Rem | Clean up files.
del /f /q /s ".\release\*.xml"
del /f /q /s ".\release\*.pdb"
del /f /q /s ".\release\*.config"

Rem | Delete & create ZIP release.
if exist ".\%filename%.zip" (del /f ".\%filename%.zip")
powershell.exe -nologo -noprofile -command "Compress-Archive -Path ".\release\*" -DestinationPath ".\%filename%.zip""

Rem | Operation complete.
echo(
pause

