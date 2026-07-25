@echo off
rem Test runner for the RoiWizard core.
rem
rem No dependencies: builds with the compiler shipped in the .NET Framework, so no test
rem framework is pulled into the BecqMoni solution. SetExporter.cs is built against the
rem minimal host-type stubs in HostStubs.cs (never part of the BecqMoni build). Only
rem RoiWizardForm*.cs stays out: it needs XPTable and the managers, i.e. the application
rem tree itself.
rem
rem Usage:  tests\run_tests.cmd
rem Exit code: 0 - all tests passed, 1 - failures.
rem
rem (ASCII only on purpose: cmd.exe reads this file in the OEM codepage.)

setlocal
set ROOT=%~dp0..
set CORE=%ROOT%\BecquerelMonitor\RoiWizard
set CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC%" set CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe

"%CSC%" -nologo -target:exe -out:"%~dp0RoiWizardTests.exe" -langversion:5 ^
  "%CORE%\RoiWizardStrings.Designer.cs" ^
  "%CORE%\NuclideCatalog.cs" ^
  "%CORE%\SpectralLine.cs" ^
  "%CORE%\LineSetBuilder.cs" ^
  "%CORE%\LineMerger.cs" ^
  "%CORE%\SecondaryPeaks.cs" ^
  "%CORE%\AnchorPicker.cs" ^
  "%CORE%\SetChecker.cs" ^
  "%CORE%\ZoneCalculator.cs" ^
  "%CORE%\SetExporter.cs" ^
  "%~dp0HostStubs.cs" ^
  "%~dp0RoiWizardTests.cs"
if errorlevel 1 (
  echo BUILD FAILED
  exit /b 1
)

"%~dp0RoiWizardTests.exe" "%CORE%\nuclides.xml"
exit /b %errorlevel%
