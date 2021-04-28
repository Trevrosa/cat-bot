using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DSharpPlus.Exceptions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net;
using System.Net.Http;
using DSharpPlus.EventArgs;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using static cat_bot.MakeTrans;
using static cat_bot.Extensions;
using static cat_bot.Program;
using Aspose.Drawing;
using Aspose.Imaging;
using Aspose.Zip;
using Aspose.Html;

namespace cat_bot
{
    public class Commands : BaseCommandModule
    {
        private static readonly Random Random = new();
        private static readonly WebClient WebClient = new();

        [Command("unholy"), Hidden]
        public async Task Unholy(CommandContext ctx, int count = 1)
        {
            if (ctx.Channel.Type is ChannelType.Text)
            {
                while (count != 0)
                {
                    int choice = Random.Next(1, 3);

                    switch (choice)
                    {
                        case 1:
                            {
                                string ew = await GetAsync("https://hmtai.herokuapp.com/nsfw/hentai");
                                JsonElement result = JsonDocument.Parse(ew).RootElement;

                                await ctx.Member.TrySendMessageAsync($"{result.GetProperty("url")}", ctx.Channel);
                                count--;

                                break;
                            }
                        case 2:
                            {
                                try
                                {
                                    string ew = await GetAsync("https://api.computerfreaker.cf/v1/hentai");
                                    JsonElement result = JsonDocument.Parse(ew).RootElement;

                                    await ctx.Member.TrySendMessageAsync($"{result.GetProperty("url")}", ctx.Channel);
                                    count--;
                                }
                                catch
                                {
                                    string ew = await GetAsync("https://nekobot.xyz/api/image?type=hentai");
                                    JsonElement result = JsonDocument.Parse(ew).RootElement;

                                    await ctx.Member.TrySendMessageAsync($"{result.GetProperty("message")}", ctx.Channel);
                                    count--;
                                }

                                break;
                            }
                        case 3:
                            {
                                string ew = await GetAsync("https://nekobot.xyz/api/image?type=hentai");
                                JsonElement result = JsonDocument.Parse(ew).RootElement;

                                await ctx.Member.TrySendMessageAsync($"{result.GetProperty("message")}", ctx.Channel);
                                count--;

                                break;
                            }
                    }

                    await Task.Delay(200);
                }
            }
            else if (ctx.Channel.Type is ChannelType.Private)
            {
                DiscordGuild guild = await ctx.User.GetGuild(ctx.Client);
                DiscordMember member = await guild.GetMemberAsync(ctx.User.Id);

                while (count != 0)
                {
                    int choice = Random.Next(1, 3);

                    switch (choice)
                    {
                        case 1:
                            {
                                string ew = await GetAsync("https://hmtai.herokuapp.com/nsfw/hentai");
                                JsonElement result = JsonDocument.Parse(ew).RootElement;

                                await member.SendMessageAsync($"{result.GetProperty("url")}");
                                count--;

                                break;
                            }
                        case 2:
                            {
                                try
                                {
                                    string ew = await GetAsync("https://api.computerfreaker.cf/v1/hentai");
                                    JsonElement result = JsonDocument.Parse(ew).RootElement;

                                    await member.SendMessageAsync($"{result.GetProperty("url")}");
                                    count--;
                                }
                                catch
                                {
                                    string ew = await GetAsync("https://nekobot.xyz/api/image?type=hentai");
                                    JsonElement result = JsonDocument.Parse(ew).RootElement;

                                    await member.SendMessageAsync($"{result.GetProperty("messsage")}");
                                    count--;
                                }

                                break;
                            }
                        case 3:
                            {
                                string ew = await GetAsync("https://nekobot.xyz/api/image?type=hentai");
                                JsonElement result = JsonDocument.Parse(ew).RootElement;

                                await member.SendMessageAsync($"{result.GetProperty("messsage")}");
                                count--;

                                break;
                            }
                    }
                }
            }
        }

        [Command("cat"), Description("Cat!!!!!"), Cooldown(4, 2, CooldownBucketType.Channel)]
        public async Task Cat(CommandContext ctx, string breedoption = "")
        {
            if (breedoption == "")
            {
                string stin = await GetAsync($"https://api.thecatapi.com/v1/images/search?format=json", ApiKey);

                JsonElement result = JsonDocument.Parse(stin).RootElement[0];

                try
                {
                    string breed = result.GetProperty("breeds")[0].GetProperty("name").ToString();

                    DiscordEmbedBuilder msg = new DiscordEmbedBuilder().WithTitle($"Here's a cat!! ({breed})").WithImageUrl(Convert.ToString(result.GetProperty("url"))).WithColor(DiscordColor.Green);
                    await ctx.RespondAsync(msg);
                }
                catch
                {
                    DiscordEmbedBuilder msg = new DiscordEmbedBuilder().WithTitle("Here's a cat!!").WithImageUrl(Convert.ToString(result.GetProperty("url"))).WithColor(DiscordColor.Green);
                    await ctx.RespondAsync(msg);
                }
            }
            else if (breedoption != "")
            {
                try
                {
                    string stin = await GetAsync($"https://api.thecatapi.com/v1/images/search?format=json&breed_ids={breedoption}", ApiKey);

                    JsonElement result = JsonDocument.Parse(stin).RootElement[0];

                    string breed = result.GetProperty("breeds")[0].GetProperty("name").ToString();

                    DiscordEmbedBuilder msg = new DiscordEmbedBuilder().WithTitle($"Here's a cat!! ({breed})").WithImageUrl(Convert.ToString(result.GetProperty("url"))).WithColor(DiscordColor.Green);
                    await ctx.RespondAsync(msg);
                }
                catch
                {
                    if (ctx.Message.Content.ToList().Count(x => x == ' ') > 1)
                    {
                        return;
                    }
                    else
                    {
                        string stin = await GetAsync($"https://api.thecatapi.com/v1/images/search?format=json", ApiKey);

                        JsonElement result = JsonDocument.Parse(stin).RootElement[0];

                        try
                        {
                            string breed = result.GetProperty("breeds")[0].GetProperty("name").ToString();

                            DiscordEmbedBuilder msg = new DiscordEmbedBuilder().WithTitle($"Here's a cat!! ({breed})").WithImageUrl(Convert.ToString(result.GetProperty("url")))
                                .WithColor(DiscordColor.Green);
                            await ctx.RespondAsync(msg);
                        }
                        catch
                        {
                            DiscordEmbedBuilder msg = new DiscordEmbedBuilder().WithTitle("Here's a cat!!").WithImageUrl(Convert.ToString(result.GetProperty("url"))).WithColor(DiscordColor.Green);
                            await ctx.RespondAsync(msg);
                        }
                    }
                }
            }
        }

        [Command("test"), Description("e"), RequireOwner, Hidden]
        public async Task Test(CommandContext ctx)
        {
            Extensions.Throw(null).Wait();
        }

        [Command("whitelist"), Description("Whitelists a person on a command."), RequireOwner, Hidden]
        public async Task Whitelist(CommandContext ctx, DiscordMember member, string command)
        {
            CommandsNextExtension cmdsnext = ctx.Client.GetCommandsNext();

            if (cmdsnext.RegisteredCommands.Any(x => x.Value.QualifiedName == command))
            {
                if (Blacklisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))) && Whitelisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))))
                {
                    List<ulong> whitelistedValue = Whitelisted.First(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))).Value;
                    List<ulong> blacklistedValue = Blacklisted.First(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))).Value;

                    blacklistedValue.Remove(member.Id);

                    if (File.Exists($"{RootDir}/blacklisted.txt"))
                    {
                        string[] lines = await File.ReadAllLinesAsync($"{RootDir}/blacklisted.txt");

                        if (lines.Any(x => x.StartsWith($"{command}:")))
                        {
                            List<string> final = lines.ToList().Where(x => x.StartsWith($"{command}:")).Select(x => x.Remove($", {member.Id}").Remove($"{member.Id}, ")).ToList();

                            File.Delete($"{RootDir}/blacklisted.txt");
                            await File.WriteAllLinesAsync($"{RootDir}/blacklisted.txt", lines);
                        }
                    }

                    whitelistedValue.Add(member.Id);

                    if (File.Exists($"{RootDir}/whitelisted.txt"))
                    {
                        string[] lines = await File.ReadAllLinesAsync($"{RootDir}/whitelisted.txt");

                        if (lines.Any(x => x.StartsWith($"{command}:")))
                        {
                            string oldvalue = lines.First(x => x.Trim() == $"{command}: {String.Join(", ", whitelistedValue)}");

                            List<string> final = lines.Where(x => x.Trim() != $"{command}: {String.Join(", ", whitelistedValue)}").Append($"{oldvalue}, {member.Id}").ToList();

                            File.Delete($"{RootDir}/whitelisted.txt");
                            await File.WriteAllLinesAsync($"{RootDir}/whitelisted.txt", lines);
                        }
                        else
                        {
                            List<string> final = lines.Append($"{command}: {member.Id}").ToList();

                            File.Delete($"{RootDir}/whitelisted.txt");
                            await File.WriteAllLinesAsync($"{RootDir}/whitelisted.txt", lines);
                        }
                    }
                    else
                    {
                        StreamWriter sw = new($"{RootDir}/whitelisted.txt", true, Encoding.UTF8);
                        await sw.WriteLineAsync($"{command}: {member.Id}");
                        sw.Close();
                    }
                }
                else if (Whitelisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))))
                {
                    await ctx.RespondAsync($"That person is already whitelisted!");

                    return;
                }
                else
                {
                    try
                    {
                        List<ulong> whitelistedValue = Whitelisted.First(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))).Value;

                        whitelistedValue.Add(member.Id);

                        if (File.Exists($"{RootDir}/whitelisted.txt"))
                        {
                            string[] lines = await File.ReadAllLinesAsync($"{RootDir}/whitelisted.txt");

                            if (lines.Any(x => x.StartsWith($"{command}:")))
                            {
                                string oldvalue = lines.First(x => x.Trim() == $"{command}: {String.Join(", ", whitelistedValue)}");

                                List<string> final = lines.Where(x => x.Trim() != $"{command}: {String.Join(", ", whitelistedValue)}").Append($"{oldvalue}, {member.Id}").ToList();

                                File.Delete($"{RootDir}/whitelisted.txt");
                                await File.WriteAllLinesAsync($"{RootDir}/whitelisted.txt", lines);
                            }
                            else
                            {
                                List<string> final = lines.Append($"{command}: {member.Id}").ToList();

                                File.Delete($"{RootDir}/whitelisted.txt");
                                await File.WriteAllLinesAsync($"{RootDir}/whitelisted.txt", lines);
                            }
                        }
                        else
                        {
                            StreamWriter sw = new($"{RootDir}/whitelisted.txt", true, Encoding.UTF8);
                            await sw.WriteLineAsync($"{command}: {member.Id}");
                            sw.Close();
                        }
                    }
                    catch
                    {
                        Whitelisted.Add(command, new() { member.Id });

                        List<ulong> whitelistedValue = Whitelisted.First(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))).Value;

                        if (File.Exists($"{RootDir}/whitelisted.txt"))
                        {
                            string[] lines = await File.ReadAllLinesAsync($"{RootDir}/whitelisted.txt");

                            if (lines.Any(x => x.StartsWith($"{command}:")))
                            {
                                string oldvalue = lines.First(x => x.Trim() == $"{command}: {String.Join(", ", whitelistedValue)}");

                                List<string> final = lines.Where(x => x.Trim() != $"{command}: {String.Join(", ", whitelistedValue)}").Append($"{oldvalue}, {member.Id}").ToList();

                                File.Delete($"{RootDir}/whitelisted.txt");
                                await File.WriteAllLinesAsync($"{RootDir}/whitelisted.txt", lines);
                            }
                            else
                            {
                                List<string> final = lines.Append($"{command}: {member.Id}").ToList();

                                File.Delete($"{RootDir}/whitelisted.txt");
                                await File.WriteAllLinesAsync($"{RootDir}/whitelisted.txt", lines);
                            }
                        }
                        else
                        {
                            StreamWriter sw = new($"{RootDir}/whitelisted.txt", true, Encoding.UTF8);
                            await sw.WriteLineAsync($"{command}: {member.Id}");
                            sw.Close();
                        }
                    }
                }

                await ctx.RespondAsync($"{member.Mention} has been whitelisted on the command `{command}`!");
            }
            else
            {
                await ctx.RespondAsync($"That command doesn't exist!");
            }
        }

        [Command("blacklist"), Description("Blacklists a person from using a command."), RequireOwner, Hidden]
        public async Task Blacklist(CommandContext ctx, DiscordMember member, string command)
        {
            CommandsNextExtension cmdsnext = ctx.Client.GetCommandsNext();

            if (cmdsnext.RegisteredCommands.Any(x => x.Value.QualifiedName == command))
            {
                if (Whitelisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))) && Blacklisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))))
                {
                    List<ulong> whitelistedValue = Whitelisted.First(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))).Value;
                    List<ulong> blacklistedValue = Blacklisted.First(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))).Value;

                    whitelistedValue.Remove(member.Id);

                    if (File.Exists($"{RootDir}/whitelisted.txt"))
                    {
                        string[] lines = await File.ReadAllLinesAsync($"{RootDir}/whitelisted.txt");

                        if (lines.Any(x => x.StartsWith($"{command}:")))
                        {
                            List<string> final = lines.ToList().Where(x => x.StartsWith($"{command}:")).Select(x => x.Remove($", {member.Id}").Remove($"{member.Id}, ")).ToList();

                            File.Delete($"{RootDir}/whitelisted.txt");
                            await File.WriteAllLinesAsync($"{RootDir}/whitelisted.txt", lines);
                        }
                    }

                    blacklistedValue.Add(member.Id);

                    if (File.Exists($"{RootDir}/blacklisted.txt"))
                    {
                        string[] lines = await File.ReadAllLinesAsync($"{RootDir}/blacklisted.txt");

                        if (lines.Any(x => x.StartsWith($"{command}:")))
                        {
                            string oldvalue = lines.First(x => x.Trim() == $"{command}: {String.Join(", ", blacklistedValue)}");

                            List<string> final = lines.Where(x => x.Trim() != $"{command}: {String.Join(", ", blacklistedValue)}").Append($"{oldvalue}, {member.Id}").ToList();

                            File.Delete($"{RootDir}/blacklisted.txt");
                            File.WriteAllLines($"{RootDir}/blacklisted.txt", lines);
                        }
                        else
                        {
                            List<string> final = lines.Append($"{command}: {member.Id}").ToList();

                            File.Delete($"{RootDir}/blacklisted.txt");
                            File.WriteAllLines($"{RootDir}/blacklisted.txt", lines);
                        }
                    }
                    else
                    {
                        StreamWriter sw = new($"{RootDir}/blacklisted.txt", true, Encoding.UTF8);
                        await sw.WriteLineAsync($"{command}: {member.Id}");
                        sw.Close();
                    }
                }
                else if (Blacklisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))))
                {
                    await ctx.RespondAsync($"That person is already blacklisted!");

                    return;
                }
                else
                {
                    try
                    {
                        List<ulong> blacklistedValue = Blacklisted.First(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))).Value;

                        blacklistedValue.Add(member.Id);

                        if (File.Exists($"{RootDir}/blacklisted.txt"))
                        {
                            string[] lines = await File.ReadAllLinesAsync($"{RootDir}/blacklisted.txt");

                            if (lines.Any(x => x.StartsWith($"{command}:")))
                            {
                                string oldvalue = lines.First(x => x.Trim() == $"{command}: {String.Join(", ", blacklistedValue)}");

                                List<string> final = lines.Where(x => x.Trim() != $"{command}: {String.Join(", ", blacklistedValue)}").Append($"{oldvalue}, {member.Id}").ToList();

                                File.Delete($"{RootDir}/blacklisted.txt");
                                File.WriteAllLines($"{RootDir}/blacklisted.txt", lines);
                            }
                            else
                            {
                                List<string> final = lines.Append($"{command}: {member.Id}").ToList();

                                File.Delete($"{RootDir}/blacklisted.txt");
                                File.WriteAllLines($"{RootDir}/blacklisted.txt", lines);
                            }
                        }
                        else
                        {
                            StreamWriter sw = new($"{RootDir}/blacklisted.txt", true, Encoding.UTF8);
                            await sw.WriteLineAsync($"{command}: {member.Id}");
                            sw.Close();
                        }
                    }
                    catch
                    {
                        Blacklisted.Add(command, new() { member.Id });

                        List<ulong> blacklistedValue = Blacklisted.First(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))).Value;

                        if (File.Exists($"{RootDir}/blacklisted.txt"))
                        {
                            string[] lines = await File.ReadAllLinesAsync($"{RootDir}/blacklisted.txt");

                            if (lines.Any(x => x.StartsWith($"{command}:")))
                            {
                                string oldvalue = lines.First(x => x.Trim() == $"{command}: {String.Join(", ", blacklistedValue)}");

                                List<string> final = lines.Where(x => x.Trim() != $"{command}: {String.Join(", ", blacklistedValue)}").Append($"{oldvalue}, {member.Id}").ToList();

                                File.Delete($"{RootDir}/blacklisted.txt");
                                File.WriteAllLines($"{RootDir}/blacklisted.txt", lines);
                            }
                            else
                            {
                                List<string> final = lines.Append($"{command}: {member.Id}").ToList();

                                File.Delete($"{RootDir}/blacklisted.txt");
                                File.WriteAllLines($"{RootDir}/blacklisted.txt", lines);
                            }
                        }
                        else
                        {
                            StreamWriter sw = new($"{RootDir}/blacklisted.txt", true, Encoding.UTF8);
                            await sw.WriteLineAsync($"{command}: {member.Id}");
                            sw.Close();
                        }
                    }
                }

                await ctx.RespondAsync($"{member.Mention} has been blacklisted from using the command `{command}`!");
            }
            else
            {
                await ctx.RespondAsync($"That command doesn't exist!");
            }
        }

        [Command("sex"), Aliases("homa", "hona", "sexy"), Description("Sends the sex."), Hidden]
        public async Task Sex(CommandContext ctx, DiscordUser user = null)
        {
            if (user is null)
            {
                await ctx.RespondAsync(new DiscordMessageBuilder().WithFile(File.OpenRead("homa.png")));
            }
            else
            {
                //Image image = Image.Load("homa.png");
            }
        }

        [Command("join"), Description("Joins a voice channel.")]
        public async Task JoinCommand(CommandContext ctx, DiscordChannel channel = null)
        {
            if (channel == null)
            {
                if (ctx.Member.VoiceState is not null)
                {
                    channel = ctx.Member.VoiceState.Channel;

                    await channel.ConnectAsync();
                    await channel.SendMessageAsync($"Joined {channel.Mention}!");
                }
                else
                {
                    await ctx.RespondAsync("You aren't in a voice channel!");
                }
            }
            else if (channel.Type is ChannelType.Voice)
            {
                await channel.ConnectAsync();
                await channel.SendMessageAsync($"Joined {channel.Mention}!");
            }
            else
            {
                await ctx.RespondAsync("I can't join a text channel!");
            }
        }

        [Command("play"), Description("Plays an audio file.")]
        public async Task PlayCommand(CommandContext ctx, string path)
        {
            if (ctx.Guild.CurrentMember.VoiceState is null)
            {
                await ctx.RespondAsync($"I'm not in a voice channel!");
            }
            else
            {
                VoiceNextExtension vnext = ctx.Client.GetVoiceNext();
                VoiceNextConnection connection = vnext.GetConnection(ctx.Guild);

                VoiceTransmitSink transmit = connection.GetTransmitSink();

                Stream pcm = ConvertAudioToPcm(path);

                await pcm.CopyToAsync(transmit);
                await pcm.DisposeAsync();

                string name = String.Empty;

                if (path.Contains("/") && !path.Contains("\\"))
                {
                    name = path.Split("/").Last();
                }
                else if (path.Contains("\\") && !path.Contains("/"))
                {
                    name = path.Split("\\").Last();
                }

                await ctx.RespondAsync($"Playing {Formatter.InlineCode(name)} in {connection.TargetChannel.Mention}!");
            }
        }

        [Command("leave"), Description("Leaves the current channel.")]
        public async Task LeaveCommand(CommandContext ctx)
        {
            VoiceNextExtension vnext = ctx.Client.GetVoiceNext();
            VoiceNextConnection connection = vnext.GetConnection(ctx.Guild);

            connection.Disconnect();
        }

        [Command("steal"), Description("Steals an emoji from another server."), Priority(2)]
        public async Task Steal(CommandContext ctx, DiscordEmoji emoji, string name = null)
        {
            string url = emoji.Url.Remove("?v=1");
            string filename = $"/root/temp-{Random.Next(234235, 325323)}.{url.Split(".").Last()}";

            if (String.IsNullOrEmpty(name))
            {
                name = emoji.Name;
            }

            WebClient.DownloadFile(url, $"{filename}");

            FileStream stream = File.OpenRead(filename);

            try
            {
                DiscordEmoji newemoji = await ctx.Guild.CreateEmojiAsync(name, stream);
                string newname;

                if (newemoji.IsAnimated)
                {
                    newname = $"<a:{newemoji.Name}:{newemoji.Id}>";
                }
                else
                {
                    newname = $"<:{newemoji.Name}:{newemoji.Id}>";
                }

                await ctx.RespondAsync($"{newname} has been added!");
            }
            catch (UnauthorizedException)
            {
                if (ctx.Guild.Owner.Username == "Homa")
                {
                    await ctx.RespondAsync("homa give the bot permissions to create emojis");
                }
                else
                {
                    await ctx.RespondAsync("I don't have the required permissions to create emojis!");
                }
            }

            stream.Close();
            File.Delete(filename);
        }

        [Command("steal"), Description("Steals an emoji from another server."), Priority(1)]
        public async Task Steal(CommandContext ctx, string emoji, string name)
        {
            string url = emoji.Remove("?v=1");
            string filename = $"/root/temp-{Random.Next(234235, 325323)}.{url.Split(".").Last()}";

            if (String.IsNullOrEmpty(name))
            {
                await ctx.RespondAsync("The emoji name cannot be empty.");

                return;
            }

            WebClient.DownloadFile(url, $"{filename}");

            FileStream stream = File.OpenRead(filename);

            try
            {
                DiscordEmoji newemoji = await ctx.Guild.CreateEmojiAsync(name, stream);
                string newname;

                if (newemoji.IsAnimated)
                {
                    newname = $"<a:{newemoji.Name}:{newemoji.Id}>";
                }
                else
                {
                    newname = $"<:{newemoji.Name}:{newemoji.Id}>";
                }

                await ctx.RespondAsync($"{newname} has been added!");
            }
            catch (UnauthorizedException)
            {
                if (ctx.Guild.Owner.Username == "Homa")
                {
                    await ctx.RespondAsync("homa give the bot permissions to create emojis");
                }
                else
                {
                    await ctx.RespondAsync("I don't have the required permissions to create emojis!");
                }
            }

            stream.Close();
            File.Delete(filename);
        }

        [Command("stealall"), Description("Steals all emojis from another server."), RequireOwner, Hidden]
        public async Task StealAll(CommandContext ctx, DiscordGuild guild)
        {
            if (!guild.CurrentMember.PermissionsIn(ctx.Channel).HasPermission(Permissions.ManageEmojis))
            {
                if (ctx.Guild.Owner.Username == "Homa")
                {
                    await ctx.RespondAsync("homa give the bot permissions to create emojis");
                }
                else
                {
                    await ctx.RespondAsync("I don't have the required permissions to create emojis!");
                }
            }
            else if (guild is not null)
            {
                IReadOnlyList<DiscordGuildEmoji> emojis = await guild.GetEmojisAsync();
                List<string> fileNames = new();

                for (int i = 0; i < emojis.Count; i++)
                {
                    DiscordEmoji emoji = emojis[i];

                    string url = emoji.Url.Remove("?v=1");
                    string name = $"/root/temp-{Random.Next(234235, 325323)}.{url.Split(".").Last()}";
                    fileNames.Add(name);

                    WebClient.DownloadFile(url, $"{name}");

                    FileStream stream = File.OpenRead(name);

                    await ctx.Guild.CreateEmojiAsync(emoji.Name, stream);

                    stream.Close();
                }

                for (int i = 0; i < fileNames.Count; i++)
                {
                    string name = fileNames[i];
                    File.Delete(name);
                }

                await ctx.RespondAsync($"All emojis from {Formatter.InlineCode(guild.Name)} have been added.");
            }
            else
            {
                await ctx.RespondAsync($"I'm not in that server!");
            }
        }

        //[Command("linkvertise"), Description("Bypasses a linkvertise link."), Hidden]
        //public async Task Linkvertise(CommandContext ctx, string link = null)
        //{
        //    if (String.IsNullOrEmpty(link))
        //    {
        //        await ctx.RespondAsync("I can't bypass nothing!");
        //    }
        //    else if (!Regex.IsMatch(link, @"(https://linkvertise.com/|https://up-to-down.net/|https://link-to.net/|https://direct-link.net/|https://file-link.net)"))
        //    {
        //        await ctx.RespondAsync("That isn't a linkvertise link!");
        //    }
        //    else
        //    {
        //        link = Regex.Replace(link, @"(https://linkvertise.com/|https://up-to-down.net/|https://link-to.net/|https://direct-link.net/|https://file-link.net)", String.Empty).Remove("?o=sharing");
        //    }
        //}

        [Command("commit"), Description("Returns the commit the bot is on."), Hidden]
        public async Task Commit(CommandContext ctx)
        {
            await Extensions.RunBashAsync($@"cd '{RootDir}/'");
            await Extensions.RunBashAsync("git fetch");

            string commit = await Extensions.RunBashAsync($"git rev-parse HEAD");
            string diff = await Extensions.RunBashAsync($"git status -sb");

            string shorthash = await Extensions.RunBashAsync($"git rev-parse --short HEAD");
            string subject = await Extensions.RunBashAsync($"git log --pretty=format:'%B' -n 1 {commit}");

            string author = await Extensions.RunBashAsync($"git log --pretty=format:'%an' -n 1 {commit}");
            string committer = await Extensions.RunBashAsync($"git log --pretty=format:'%cn' -n 1 {commit}");
            string credits;

            if (author == committer)
            {
                credits = $"{author.Remove("\n")} authored & committed";
            }
            else
            {
                credits = $"{author.Remove("\n")} authored & {committer.Remove("\n")} committed";
            }

            MakeTrans mt = new("[]", "( ");
            string mea;

            if (diff.Trim() == "## master")
            {
                mea = " (even with master)";
            }
            else
            {
                mea = mt.Translate(diff.Split("origin/master").Last());
                mea = mea.Remove("\n");

                int num = int.Parse(mea.Remove("(behind ").Remove("(ahead "));
                string ea = num > 1 ? "s" : "";

                mea += $"commit{ea})";
            }

            string sea = $"Commit: [{Formatter.InlineCode(shorthash)}](https://github.com/Trevrosa/cat-bot/commit/{commit})";

            DiscordEmbedBuilder embed = new DiscordEmbedBuilder().WithTitle(subject).WithDescription($"{credits}\n\n" + sea.Remove("\n") + mea)
                .WithColor(DiscordColor.SpringGreen);

            await ctx.RespondAsync(embed);
        }

        [Command("bash"), Description("Runs a Bash command."), Hidden]
        public async Task Bash(CommandContext ctx, [RemainingText] string args = "")
        {
            if (String.IsNullOrEmpty(args))
            {
                await ctx.RespondAsync("You need to specify a command to run in Bash.");
            }
            else
            {
                string result = await Extensions.RunBashAsync(args);

                if (result.Length > 2000)
                {
                    string newresult = "";
                    int here = 0;

                    char[] array = result.ToCharArray();
                    for (int i = 0; i < array.Length; i++)
                    {
                        char character = array[i];
                        if (here < 2000)
                        {
                            newresult += character;
                            here++;
                        }
                    }

                    await ctx.RespondAsync(newresult);
                    await ctx.RespondAsync($"There is more content in the result. Respond with yes or no for the bot to send a file with the full result.");
                    InteractivityExtension interactivity = ctx.Client.GetInteractivity();
                    InteractivityResult<DiscordMessage> interact = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.User.Id);

                    while (!interact.TimedOut)
                    {
                        if (interact.Result.Content.ToLower() == "yes")
                        {
                            string path = $"/root/temp{Random.Next(23234, 262332)}.txt";
                            StreamWriter sw = new(path, false, Encoding.UTF8);
                            await sw.WriteLineAsync(result);
                            sw.Close();

                            FileStream file = File.OpenRead(path);
                            await ctx.RespondAsync(new DiscordMessageBuilder().WithContent("Here it is:").WithFile(file));
                            file.Close();

                            File.Delete(path);

                            break;
                        }
                        else if (interact.Result.Content.ToLower() == "no")
                        {
                            await ctx.RespondAsync("Okay.");

                            break;
                        }
                    }
                }
                else
                {
                    await ctx.RespondAsync(result);
                }
            }
        }

        [Command("editsnipe"), Description("Gets the last edited message.")]
        public async Task EditSnipe(CommandContext ctx)
        {
            try
            {
                MessageUpdateEventArgs editsnipemessagevalue = EditSnipeMessage.First(x => x.Key.Equals(ctx.Guild)).Value;

                await ctx.RespondAsync(new DiscordEmbedBuilder().WithAuthor(editsnipemessagevalue.Author.GetFullUsername(),
                    null, editsnipemessagevalue.Author.AvatarUrl)
                    .AddField("Original Content", editsnipemessagevalue.MessageBefore.Content).AddField("Edited Content", editsnipemessagevalue.Message.Content)
                    .AddField("Jump Link", editsnipemessagevalue.Message.JumpLink.ToString()).WithFooter($"{editsnipemessagevalue.Message.EditedTimestamp.Value.GetHongKongTime()} GMT+8")
                    .WithColor(DiscordColor.Purple));
            }
            catch
            {
                await ctx.RespondAsync("Nothing to snipe.");
            }
        }

        [Command("snipe"), Description("Gets the last deleted message.")]
        public async Task Snipe(CommandContext ctx)
        {
            try
            {
                DiscordMessage deletedsnipemessagevalue = DeletedSnipeMessage.First(x => x.Key.Equals(ctx.Guild)).Value;
                string deletedsnipedeletervalue = DeletedSnipeDeleter.First(x => x.Key.Equals(ctx.Guild)).Value;

                await ctx.RespondAsync(new DiscordEmbedBuilder().WithAuthor(deletedsnipemessagevalue.Author.GetFullUsername(),
                    null, deletedsnipemessagevalue.Author.AvatarUrl)
                    .AddField("Content", deletedsnipemessagevalue.Content).AddField("Channel", deletedsnipemessagevalue.Channel.Mention).AddField($"Deleted By", deletedsnipedeletervalue)
                    .WithFooter($"{deletedsnipemessagevalue.CreationTimestamp.GetHongKongTime()} GMT+8").WithColor(DiscordColor.Purple));
            }
            catch
            {
                await ctx.RespondAsync("Nothing to snipe.");
            }
        }

        [Command("coinflip"), Description("Flips a coin.")]
        public async Task Coinflip(CommandContext ctx)
        {
            int num = Random.Next(1, 10);

            if (num.IsDivisible(2))
            {
                await ctx.RespondAsync("Heads!");
            }
            else
            {
                await ctx.RespondAsync("Tails!");
            }
        }

        [Command("sudo"), Description("Executes a command as another user."), Hidden]
        public async Task Sudo(CommandContext ctx, [Description("Member to execute the command as.")] DiscordMember member,
            [RemainingText, Description("Command with arguments to execute.")] string command)
        {
            Command cmd = ctx.CommandsNext.FindCommand(command, out string args);
            if (cmd == null)
            {
                await ctx.RespondAsync($"That command doesn't exist!");
            }

            CommandContext fctx = ctx.CommandsNext.CreateFakeContext(member, ctx.Channel, command, ctx.Prefix, cmd, args);
            await cmd.RunCommandAsync(fctx, ctx.Client);
        }

        [Command("eval"), Aliases("evalcs", "cseval", "roslyn"), Description("Evaluates C# code."), Hidden]
        public Task EvalCS(CommandContext ctx, [RemainingText, Description("The code to be evaluated.")] string code)
        {
            _ = Task.Run(async () =>
            {
                DiscordMessage msg = ctx.Message;

                code = code.Remove("```csharp");
                code = code.Remove("```cs");
                code = code.Remove("```");

                DiscordEmbedBuilder embed = new()
                {
                    Title = "Evaluating...",
                    Color = DiscordColor.Purple
                };

                msg = await ctx.RespondAsync("", embed: embed.Build());

                EvaluationEnvironment globals = new(ctx);
                ScriptOptions sopts = ScriptOptions.Default
                    .AddImports("System", "System.Collections.Generic", "System.Diagnostics", "System.Linq", "System.Net.Http", "System.Text", "System.Threading.Tasks", "DSharpPlus",
                        "DSharpPlus.CommandsNext", "DSharpPlus.Entities", "DSharpPlus.EventArgs", "DSharpPlus.Exceptions", "System.IO", "cat_bot", "cat_bot.Extensions",
                        "System.Text.RegularExpressions", "System.Text.Json", "System.Net", "Serilog", "Serilog.Extensions.Logging", "System.Net")
                    .AddReferences(AppDomain.CurrentDomain.GetAssemblies().Where(xa => !xa.IsDynamic && !String.IsNullOrWhiteSpace(xa.Location)));

                Stopwatch sw1 = Stopwatch.StartNew();
                Script<object> cs = CSharpScript.Create(code.Trim(), sopts, typeof(EvaluationEnvironment));
                ImmutableArray<Diagnostic> csc = cs.Compile();
                sw1.Stop();

                if (csc.Any(xd => xd.Severity == DiagnosticSeverity.Error))
                {
                    embed = new DiscordEmbedBuilder
                    {
                        Title = "Compilation failed",
                        Description = String.Concat("Compilation failed after ", sw1.ElapsedMilliseconds.ToString(), "ms with ", csc.Length.ToString("#,##0"), " errors."),
                        Color = DiscordColor.Purple
                    };
                    foreach ((Diagnostic xd, FileLinePositionSpan ls, int line) in from Diagnostic xd in csc.Take(5)
                                                                                   let ls = xd.Location.GetLineSpan()
                                                                                   let line = ls.StartLinePosition.Line
                                                                                   select (xd, ls, line))
                    {
                        embed.AddField(String.Concat("Error at ", line.ToString("#,##0"), ", ", ls.StartLinePosition.Character.ToString("#,##0")), Formatter.InlineCode(xd.GetMessage()), false);
                    }

                    if (csc.Length > 5)
                    {
                        embed.AddField("Some errors ommited", String.Concat((csc.Length - 5).ToString("#,##0"), " more errors not displayed"), false);
                    }
                    await msg.ModifyAsync(embed: embed.Build());
                    return;
                }

                Exception rex = null;
                ScriptState<object> css = null;
                Stopwatch sw2 = Stopwatch.StartNew();
                try
                {
                    css = await cs.RunAsync(globals);
                    rex = css.Exception;
                }
                catch (Exception ex)
                {
                    rex = ex;
                }
                sw2.Stop();

                if (rex != null)
                {
                    DiscordChannel channel = await ctx.Client.GetChannelAsync(812620259714138112);
                    DiscordEmbedBuilder embedBuilder = new();

                    embedBuilder.WithTitle($"Exception occurred while evaluating code:").AddField("Type", $"{rex.GetType()}", true)
                                .AddField("Message", $"{rex.Message}", true).WithColor(DiscordColor.Red).WithTimestamp(DateTimeOffset.Now);

                    if (String.IsNullOrEmpty(rex.InnerException.Demystify().ToString()))
                    {
                        embedBuilder.AddField("Inner Exception", Formatter.BlockCode(rex.InnerException.Demystify().ToString(), "csharp"))
                            .AddField("Stack Trace", Formatter.BlockCode(rex.Demystify().StackTrace.Replace("Jess", "trev"), "csharp"));
                    }
                    else
                    {
                        embedBuilder.AddField("Stack Trace", Formatter.BlockCode(rex.Demystify().StackTrace.Replace("Jess", "trev"), "csharp"));
                    }

                    DiscordMessage message = await channel.SendMessageAsync(embedBuilder.Build());

                    embed = new DiscordEmbedBuilder
                    {
                        Title = "Execution failed",
                        Description = String.Concat("Execution failed after ", sw2.ElapsedMilliseconds.ToString(), "ms with `", rex.GetType(), ": ", rex.Message,
                            $"`\n\nGo to {message.JumpLink} for more info."),
                        Color = DiscordColor.Purple,
                    };
                    await msg.ModifyAsync(embed: embed.Build());
                    return;
                }

                // execution succeeded
                embed = new DiscordEmbedBuilder
                {
                    Title = "Evaluation successful",
                    Color = DiscordColor.Purple,
                };

                embed.AddField("Result", css.ReturnValue != null ? css.ReturnValue.ToString() : "No value returned", false)
                    .AddField("Compilation time", String.Concat(sw1.ElapsedMilliseconds.ToString(), "ms"), true)
                    .AddField("Execution time", String.Concat(sw2.ElapsedMilliseconds.ToString(), "ms"), true);

                if (css.ReturnValue != null)
                    embed.AddField("Return type", css.ReturnValue.GetType().ToString(), true);

                await msg.ModifyAsync(embed: embed.Build());
            });

            return Task.CompletedTask;
        }
    }

    public sealed class EvaluationEnvironment
    {
        public CommandContext Context { get; }
        public DiscordMessage Message => Context.Message;
        public DiscordChannel Channel => Context.Channel;
        public DiscordGuild Guild => Context.Guild;
        public DiscordUser User => Context.User;
        public DiscordMember Member => Context.Member;
        public DiscordClient Client => Context.Client;

        public EvaluationEnvironment(CommandContext ctx)
        {
            this.Context = ctx;
        }
    }
}