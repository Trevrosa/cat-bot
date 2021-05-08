#!/bin/bash

cd '~/cat-bot/'
git reset --hard --oneline HEAD --
sudo git clone --no-checkout 'https://github.com/Trevrosa/cat-bot.git' '~/existing-dir-tmp/'
if [ -d "~/cat-bot/.git/" ]; then rm -rf "~/cat-bot/.git/" fi
mv "~/existing-dir-tmp/.git/" "~/cat-bot/"
rm -rf "~/existing-dir-tmp/"
git reset --hard --oneline HEAD --

dotnet publish -r linux-x64
chmod +wxr "~/cat-bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
sudo "~/cat-bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
