#!/bin/bash

source .env

DESTINATION="$RIMWORLD_MOD_PATH/Improved Need Indicator"

rm -r "$DESTINATION"
mkdir "$DESTINATION"

mkdir "$DESTINATION/1.2"
cp -af ./1.2 "$DESTINATION"

mkdir "$DESTINATION/1.3"
cp -af ./1.3 "$DESTINATION"

mkdir "$DESTINATION/1.4"
cp -af ./1.4 "$DESTINATION"

mkdir "$DESTINATION/1.5"
cp -af ./1.5 "$DESTINATION"

mkdir "$DESTINATION/About"
cp -af ./About "$DESTINATION"

mkdir "$DESTINATION/Defs"
cp -af ./Defs "$DESTINATION"

mkdir "$DESTINATION/Languages"
cp -af ./Languages "$DESTINATION"

mkdir "$DESTINATION/Scripts"
cp -af ./Scripts "$DESTINATION"

mkdir "$DESTINATION/Source"
cp -af ./Source "$DESTINATION"

rm -r "$DESTINATION/Source/.vs"

cp -f ./Dockerfile "$DESTINATION"
cp -f ./LICENSE "$DESTINATION"
cp -f ./readme.md "$DESTINATION"
