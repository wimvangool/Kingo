@echo off

set ProjectFile=%~1
set ProjectDir=%~dp1

for %%f in (%ProjectDir%\*.nuspec) do (
  nuget pack "%projectfile%" -prop configuration=release -includereferencedprojects -outputdirectory "d:\development\nuget packages"	
)