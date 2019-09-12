#!/bin/bash

# To avoid having to deal with docker, running migrations for higher environments is a manual process.
# Setup your env.json and run this script immediately following a merge.

dotnet build ../src/DnDTracker.Migrations/DnDTracker.Migrations.csproj -c Release
dotnet ../src/DnDTracker.Migrations/bin/Release/netcoreapp2.2/DnDTracker.Migrations.dll "$1"