DnDTracker
=====
**CSCE 3444 Team Name:** Silicon Valley

`master`    
[![Build Status](https://travis-ci.com/itsmistad/DnDTracker.svg?branch=master)](https://travis-ci.com/itsmistad/DnDTracker) 

`develop`   
[![Build Status](https://travis-ci.com/itsmistad/DnDTracker.svg?branch=develop)](https://travis-ci.com/itsmistad/DnDTracker)

DnDTracker is a campaign utility and data tracker for Dungeons and Dragons.

You can find `master` live [here](https://dnd.mistad.net/) and `develop` [here](https://dev.dnd.mistad.net/).

Here's what it does:

1. Persists information about your current and past campaigns in a database.
2. Provides the Dungeon Master with an interface of the campaign's current state and event log.
3. Provides the players with an interface of their character's and campaign's current state.
4. Allows the Dungeon Master to modify the characters' states.
5. Authenticates users through Google's Auth API.

This code is open-source and under the [MIT License](https://opensource.org/licenses/MIT).

#### Contributors
> [itsmistad](https://github.com/itsmistad/) - Derek Williamson
> 
> [tylercarey98](https://github.com/TylerCarey98) - Tyler Carey
>
> [nathanielduncan](https://github.com/nathanielduncan)
>
> [andrewschlein](https://github.com/AndrewSchlein) - Andrew Schlein

## Development
### Requirements

1. bash (packaged with git and cmder)
2. [Node.js](https://nodejs.org/en/download/)
3. [aws cli](https://aws.amazon.com/cli/)
4. [dynamodb-local](https://s3-us-west-2.amazonaws.com/dynamodb-local/dynamodb_local_latest.zip)
5. [NUnit3 Adapter](https://marketplace.visualstudio.com/items?itemName=NUnitDevelopers.NUnit3TestAdapter)
6. [.NET Core 2.2 Runtime and SDK](https://dotnet.microsoft.com/download)
7. [Java](https://www.java.com/en/download/)

## Setup

#### DynamoDb

From within `/scripts`, run this within bash to install the admin panel:
> `./install-dynamodb-admin.sh`

Extract `dynamodb-local` to `./scripts/dynamodb`.
Then, run this to start your local DynamoDb instance:
> `./start-dynamodb.sh`

Finally, run this to start your admin panel:
> `./start-dynamodb-admin.sh`

## Testing

From the base directory of the repo, run this to execute all unit tests:
> `dotnet test`

Alternatively, you can run the tests from within Visual Studio:

<img width="300px" src="https://cdn.mistad.net/8677548.png" alt="screenshot of right click on test project hovering over run tests option"/>

## Building and Running

Run this to download the packages and build the app:
> `dotnet build -c Release`

Then, run this to start it:
> `dotnet ./src/DnDTracker.Web/bin/Release/netcoreapp2.2/DnDTracker.Web.dll`

Alternatively, you could just use Visual Studio 😉

## Publishing

This script requires the bash `zip` command. This is natively available in unix environments, but not on Windows. You can download it [here](https://sourceforge.net/projects/infozip/).

You can create an AWS ElasticBeanstalk-ready zip file by running this command in `./scripts` AFTER adding credentials to the script file:
> `./create-deployable.sh`

This will create a `deploy` directory with a `deployables` subdirectory.

`dndtracker-app.zip` is the compiled application. `deploy.zip` is the ElasticBeanstalk-ready deployable.

Please read the comments in the script before running.