#!/bin/bash

if [[ -z "$TRAVIS_PULL_REQUEST" ]] || [[ $TRAVIS_PULL_REQUEST == “false” ]]; then
    dotnet build ../src/DnDTracker.Migrations/DnDTracker.Migrations.csproj -c Release
    dotnet ../src/DnDTracker.Migrations/bin/Release/netcoreapp2.2/DnDTracker.Migrations.dll "$1"
fi