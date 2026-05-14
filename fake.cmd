@echo off
setlocal
pushd "%~dp0"
dotnet tool restore >nul
set FAKE_ALLOW_NO_DEPENDENCIES=true
dotnet fake %*
set EXITCODE=%ERRORLEVEL%
popd
exit /b %EXITCODE%
