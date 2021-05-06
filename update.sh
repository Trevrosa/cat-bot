#!/bin/bash

cd "~/cat-bot/"
git clone --no-checkout https://github.com/Trevrosa/cat-bot.git "~/existing-dir-tmp/"
rm -rf "~/cat-bot/.git/"
mv "~/existing-dir-tmp/.git/" "~/cat-bot/"
rm -rf "~/existing-dir-tmp/"
git reset --hard HEAD

dotnet publish -r linux-x64
chmod +wxr "~/cat-bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
"~/cat-bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
