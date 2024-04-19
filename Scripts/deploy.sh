#!/bin/bash

source .env

DESTINATION="$RIMWORLD_MOD_PATH/Improved Need Indicator"

mkdir "$DESTINATION"

mkdir "$DESTINATION/1.2"
cp -a ./1.2 "$DESTINATION"

mkdir "$DESTINATION/1.3"
cp -a ./1.3 "$DESTINATION"

mkdir "$DESTINATION/1.4"
cp -a ./1.4 "$DESTINATION"

mkdir "$DESTINATION/1.5"
cp -a ./1.5 "$DESTINATION"

mkdir "$DESTINATION/About"
cp -a ./About "$DESTINATION"

mkdir "$DESTINATION/Languages"
cp -a ./Languages "$DESTINATION"

mkdir "$DESTINATION/Scripts"
cp -a ./Scripts "$DESTINATION"

cp ./Dockerfile "$DESTINATION"
cp ./LICENSE "$DESTINATION"
cp ./readme.md "$DESTINATION"
