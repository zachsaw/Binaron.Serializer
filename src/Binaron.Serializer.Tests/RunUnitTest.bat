@echo off
if exist "%1" (
    echo Running unit tests with code coverage report...
) else (
    echo Please specify path to dotCover.exe
    exit /b 1
)
del /q %temp%\CoverageReport.html
del /q dotCover.Output.dcvr
dotnet dotcover test -v n --dcFilters="-:type=Binaron.Serializer.IeeeDecimal.*;-:type=Binaron.Serializer.Tests.*"
if %errorlevel% neq 0 exit /b %errorlevel%
%1 r DotCoverReportWin.xml
if %errorlevel% neq 0 exit /b %errorlevel%
del /q dotCover.Output.dcvr
start /NEWWINDOW /WAIT %temp%\CoverageReport.html
if %errorlevel% neq 0 exit /b %errorlevel%
