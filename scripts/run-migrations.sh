#!/bin/bash

dotnet build ../src/DnDTracker.Migrations/DnDTracker.Migrations.csproj -c Release
dotnet ../src/DnDTracker.Migrations/bin/Release/netcoreapp2.2/DnDTracker.Migrations.dll "$1"