#!/bin/bash


dotnet dev-certs https --trust
dotnet add src/Tests/Tests.csproj package xunit
dotnet add src/Tests/Tests.csproj package Microsoft.NET.Test.Sdk
dotnet add src/Tests/Tests.csproj package xunit.runner.visualstudio


cd /workspaces/AutoDo/srcFrontend/AutoDoUI
npm install"