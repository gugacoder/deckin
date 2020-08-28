#!/bin/bash

find Server -name *.csproj | xargs -I{} dotnet restore {}


