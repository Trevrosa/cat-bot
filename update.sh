#!/bin/bash

cd '~/cat-bot/'
sudo git pull
git reset --hard

dotnet publish -r linux-x64
chmod +wxr "./bin/Debug/net5.0/linux-x64/publish/cat bot"
sudo "./bin/Debug/net5.0/linux-x64/publish/cat bot"
