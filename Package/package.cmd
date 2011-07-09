@echo off
set msbuild=%systemroot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

"%msbuild%" Package.msbuild || pause
