@echo off
cls
echo 
echo WatchDog Operation
echo -------------------
echo S.  Start
echo E.  Stop
echo I.  Install
echo U.  Uninstall
echo Q.  Quit
echo -------------------

choice /c:seiuq 

if errorlevel 5 goto Quit
if errorlevel 4 goto Uninstall
if errorlevel 3 goto Install
if errorlevel 2 goto Stop
if errorlevel 1 goto Start
 
:Start
Net Start WatchDogService
goto end
 
:Stop
Net Stop WatchDogService
goto end
 
:Restart
Net Start WatchDogService
Net Stop WatchDogService
goto end
 
:Install
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe WatchDogService.exe
Net Start WatchDogService
sc config WatchDogService start= auto
goto end

:Uninstall
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe /u WatchDogService.exe
goto end
 
:Quit
goto end

:end