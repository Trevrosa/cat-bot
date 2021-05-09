#!/bin/bash

cd '/home/trev/cat-bot/'
pull=$(sudo git pull)

if [[ $pull =~ "Already up to date." ]];
then
   chmod +wxr "./bin/Debug/net5.0/linux-x64/publish/cat bot"
   sudo "./bin/Debug/net5.0/linux-x64/publish/cat bot"
else
   git reset --hard
   dotnet publish -r linux-x64
   chmod +wxr "./bin/Debug/net5.0/linux-x64/publish/cat bot"
   sudo "./bin/Debug/net5.0/linux-x64/publish/cat bot"
fi
