@echo off

set ProjectFile=%~1
set ProjectDir=%~dp1
set OutputDir=%~2 

if not exist "%OutputDir%" (
  mkdir "%OutputDir%"
)
for %%f in (%ProjectDir%\Properties\*.nuspec) do (
  nuget pack "%ProjectFile%" -prop configuration=release -includereferencedprojects -outputdirectory "%OutputDir% "	
)