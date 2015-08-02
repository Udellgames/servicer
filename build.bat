@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)
 
set version=1.0.0
if not "%PackageVersion%" == "" (
   set version=%PackageVersion%
)

set nuget=
if "%nuget%" == "" (
	set nuget=nuget
)

REM Package restore
call %nuget% restore Servicer\packages.config -OutputDirectory %cd%\packages -NonInteractive
call %nuget% restore ServicerTests\packages.config -OutputDirectory %cd%\packages -NonInteractive
call %nuget% restore Servicer.Net4\packages.config -OutputDirectory %cd%\packages -NonInteractive
call %nuget% restore Servicer.Net4Tests\packages.config -OutputDirectory %cd%\packages -NonInteractive
call %nuget% restore Servicer.Net45\packages.config -OutputDirectory %cd%\packages -NonInteractive
call %nuget% restore Servicer.Net45Tests\packages.config -OutputDirectory %cd%\packages -NonInteractive

REM Build
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild Servicer.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=diag /nr:false

REM Package
mkdir Build
mkdir Build\lib
mkdir Build\lib\net35
mkdir Build\lib\net4
mkdir Build\lib\net45

%nuget% pack "Servicer.nuspec" -NoPackageAnalysis -verbosity detailed -o Build -Version %version% -p Configuration="%config%"