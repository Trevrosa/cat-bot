git clone --no-checkout https://github.com/Trevrosa/cat-bot.git "/root/existing-dir-tmp/" 
rm "/root/cat bot/.git/" -rf
mv "/root/existing-dir-tmp/.git/" "/root/cat bot/"
rm "/root/existing-dir-tmp/" -rf
cd "/root/cat bot/"
git reset --hard HEAD

dotnet publish -r linux-x64
chmod +x "/root/cat bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
"/root/cat bot/bin/Debug/net5.0/linux-x64/publish/cat bot"
