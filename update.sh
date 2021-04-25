git clone --no-checkout https://github.com/Trevrosa/cat-bot.git "/home/trev/existing-dir-tmp/"
rm -rf "/home/trev/cat-bot/.git/"
mv "/home/trev/existing-dir-tmp/.git/" "/home/trev/cat-bot/"
rm -rf "/home/trev/existing-dir-tmp/"
cd "/home/trev/cat-bot/"
git reset --hard HEAD

dotnet publish -r linux-x64
chmod +wxr "/home/trev/cat-bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
"/home/trev/cat-bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
