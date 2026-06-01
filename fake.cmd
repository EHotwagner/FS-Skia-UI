@echo off
setlocal
pushd "%~dp0"
rem Feature 045: dedicated compiled front-end (build/Build.fsproj) replaces the FSX runner.
dotnet run --project build/Build.fsproj -- %*
set EXITCODE=%ERRORLEVEL%
popd
exit /b %EXITCODE%
