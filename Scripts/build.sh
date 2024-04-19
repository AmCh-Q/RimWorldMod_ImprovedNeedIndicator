#!/bin/bash

# Build the assemblies using a containerized dotnet
podman run --rm -v $PWD:/mod dotnet build ./Source/"Improved Need Indicator".sln --framework net472 --configuration Release  "$@"
