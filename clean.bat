:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
::Clean The TerrariaDepotDownloader Project Via Batch            ::
::Gihub https://github.com/RussDev7/TerrariaDepotDownloader      ::
::Devoloped, Maintained, And Sponsored By D.RUSS#2430            ::
:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
@ECHO OFF

Rem | Check If Clean Is Necessary
if not exist *.zip (
    echo ERROR: You need to run build.bat first!
	echo(
	pause
	goto :EOF
)

Rem | Clean Up Files
FOR /d %%a IN ("%~dp0\*") DO IF /i NOT "%%~nxa"=="release" RD /S /Q "%%a"
FOR %%a IN ("%~dp0\*") DO IF /i NOT "%%~nxa"=="clean.bat" IF /i NOT "%%~xa"==".zip" DEL "%%a"

Rem | Move Files Out Of Release
FOR /F "delims=|" %%a IN ('dir /b /s /ad %~dp0') do set "folder=%%a"
robocopy "%folder%" "%~dp0\" /move /NJH /NJS /NDL

Rem | Remove Temp Files
rmdir "release"

Rem | Operation Complete
echo(
pause

Rem | Self Terminate
del "%~f0"