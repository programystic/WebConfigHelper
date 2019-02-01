nuget pack ..\WebConfigHelper\WebConfigHelper.csproj -Prop Configuration=Release

xcopy *.nupkg c:\repos\nuget\ /D /Y
del *.nupkg