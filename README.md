# Summary

Sample project to reproduce https://github.com/Azure/azure-sdk-for-net/issues/36883

# How to reproduce

```
dotnet build MSBuildTaskSample.sln

dotnet publish MSBuildTaskSample.sln

dotnet msbuild msbuild.proj /t:Example /v:n

```
