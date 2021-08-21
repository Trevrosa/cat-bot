#!/bin/bash

cd '/home/trev/cat-bot/'
sudo git stash
pull=$(sudo git pull -f)

if [[ $pull =~ 'Already up to date.' ]];
then
   chmod +wxr './bin/Debug/net*/linux-x64/publish/cat bot'
   sudo './bin/Debug/net*/linux-x64/publish/cat bot'
else
   git reset --hard
   sudo dotnet restore
   sudo dotnet publish -r linux-x64 --no-restore
   chmod +wxr './bin/Debug/net*/linux-x64/publish/cat bot'
   sudo './bin/Debug/net*/linux-x64/publish/cat bot'
fi
