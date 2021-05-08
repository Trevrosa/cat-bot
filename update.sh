#!/bin/bash

cd '~/cat-bot/'
sudo git pull
if [ -d "~/cat-bot/.git/" ]; then rm -rf "~/cat-bot/.git/" fi
mv "~/existing-dir-tmp/.git/" "~/cat-bot/"
rm -rf "~/existing-dir-tmp/"
git reset --hard HEAD --

dotnet publish -r linux-x64
chmod +wxr "~/cat-bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
sudo "~/cat-bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
