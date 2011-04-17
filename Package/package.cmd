@echo off
set msbuild=%systemroot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

set target=%1
if "%target%"=="" set target=Build

set platform=%2
if "%platform%"=="" set platform=x86

"%msbuild%" Package.msbuild /t:%target% /p:Platform=%platform% || pause
