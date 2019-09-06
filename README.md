DnDTracker
=====

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
> [andrewschlein](https://github.com/AndrewSchlein)

## Development
### Requirements

1. bash (packaged with git and cmder)
2. [Node.js](https://nodejs.org/en/download/)
3. [aws cli](https://aws.amazon.com/cli/)
4. [dynamodb-local](https://s3-us-west-2.amazonaws.com/dynamodb-local/dynamodb_local_latest.zip)
5. [NUnit3 Adapter](https://marketplace.visualstudio.com/items?itemName=NUnitDevelopers.NUnit3TestAdapter)
6. [.NET Core 2.2 Runtime and SDK](https://dotnet.microsoft.com/download)

## Setup

#### DynamoDb

From within `/scripts`, run this within bash to install the admin panel:
> `./install-dynamodb-admin.sh`

Extract `dynamodb-local` to a `./scripts/dynamodb`.
Then, run this to start your local DynamoDb instance:
> `./start-dynamodb.sh`

Finally, run this to start your admin panel:
> `./start-dynamodb-admin.sh`

## Testing

🤯 Woops. Still need to write this.

## Building

🤯 Woops. Still need to write this.

## Deploying

🤯 Woops. Still need to write this.