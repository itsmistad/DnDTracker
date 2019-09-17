#!/bin/bash

# Environment identifier is pre-defined as "local". This will affect the tables the app connects to (dndtracker-{env}-*) and the target AWS endpoint.
# If you change env to anything else, be sure to replace INSERT_ACCESS_KEY and INSERT_SECRET_KEY.
ENV_FILE="{\"env\":\"local\",\"aws\":{\"access_key\":\"INSERT_ACCESS_KEY\",\"secret_key\":\"INSERT_SECRET_KEY\"}}"
DIR=$(pwd)

dotnet publish ../src/DnDTracker.Web/DnDTracker.Web.csproj -c Release -o $DIR/deploy/output
cd ./deploy/output
echo $ENV_FILE >> env.json
mkdir ../deployables
zip ../deployables/dndtracker-app.zip *
cd ../deployables
cp ../../../aws-windows-deployment-manifest.json ./aws-windows-deployment-manifest.json
zip deploy.zip ./dndtracker-app.zip ./aws-windows-deployment-manifest.json
rm ./aws-windows-deployment-manifest.json