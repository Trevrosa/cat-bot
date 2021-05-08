#!/bin/bash

cd '~/cat-bot/'
sudo git pull
git reset --hard

dotnet publish -r linux-x64
chmod +wxr "~/cat-bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
sudo "~/cat-bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
