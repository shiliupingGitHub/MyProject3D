::echo off
set realProjectPath=%cd%
set virtualProjectPath=%realProjectPath%\link
md  %virtualProjectPath%
mklink /D %virtualProjectPath%\Assets %realProjectPath%\Assets 
mklink /D %virtualProjectPath%\ProjectSettings %realProjectPath%\ProjectSettings 
mklink /D %virtualProjectPath%\Packages %realProjectPath%\Packages 