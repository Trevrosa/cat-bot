using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus.Exceptions;
using Sentry;
using DSharpPlus.VoiceNext;
using Serilog;
using Serilog.Exceptions;
using Serilog.Events;
using System.Diagnostics;
using static cat_bot.Extensions;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace cat_bot
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            using (SentrySdk.Init(new SentryOptions() { Dsn = "https://218dee17e1a340e19ca5382bbadeda35@o545219.ingest.sentry.io/5666766" }))
            {
                MainAsync().GetAwaiter().GetResult();
            }
        }

        public static async Task MainAsync()
        {
            #region config

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(outputTemplate: String.Concat(@"{Timestamp:yyyy-MM-dd HH:mm:ss} ", @"{Level:u4} {Message:lj}{NewLine}{Exception}"))
                .WriteTo.File($"{RootDir}/cat.log", outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u4}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            ILoggerFactory logFactory = new LoggerFactory().AddSerilog();

            DiscordClient discord = new(new DiscordConfiguration()
            {
                Token = JsonDocument.Parse(File.OpenRead($"{RootDir}/token.json")).RootElement.GetProperty("token").ToString(),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.Debug,
                LoggerFactory = logFactory
            });

            CommandsNextExtension cnext = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { Prefix },
                CaseSensitive = false,
                DmHelp = false,
                EnableDms = true,
                EnableMentionPrefix = true
            });

            cnext.RegisterCommands<Commands>();
            cnext.CommandErrored += CommandErrored;

            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromHours(3)
            });

            AppDomain.CurrentDomain.ProcessExit += async (s, ev) =>
            {
                try
                {
                    await discord.DisconnectAsync();
                }
                catch
                {
                    // do nothing
                }
            };

            Console.CancelKeyPress += async (s, ev) =>
            {
                try
                {
                    await discord.DisconnectAsync();
                }
                catch
                {
                    // do nothing
                }
            };

            #endregion config

            discord.Ready += Ready;
            discord.GuildAvailable += Magic;
            discord.ClientErrored += ClientError;
            discord.GuildMemberRemoved += Reinvite;

            discord.MessageCreated += CommandHandler;
            discord.MessageDeleted += Snipe;
            discord.MessageUpdated += EditSnipe;
            discord.GuildMemberAdded += Role;

            await discord.ConnectAsync(new() { ActivityType = ActivityType.Playing, Name = "coded by trev !!" });
            await Task.Delay(-1).ConfigureAwait(false);
        }

        private static Task Ready(DiscordClient sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

        private static Task Magic(DiscordClient sender, GuildCreateEventArgs e)
        {
            EditSnipeMessage.Add(e.Guild, new());
            DeletedSnipeMessage.Add(e.Guild, new());
            DeletedSnipeDeleter.Add(e.Guild, new());

            return Task.CompletedTask;
        }

        #region PublicVars

        private static Dictionary<string, List<ulong>> whitelisted = new();

        public static Dictionary<string, List<ulong>> Whitelisted
        {
            get { return whitelisted; }
            set { whitelisted = value; }
        }

        private static Dictionary<string, List<ulong>> blacklisted = new();

        public static Dictionary<string, List<ulong>> Blacklisted
        {
            get { return blacklisted; }
            set { blacklisted = value; }
        }

        private static Dictionary<DiscordGuild, List<DiscordMessage>> deletedSnipeMessage = new();

        public static Dictionary<DiscordGuild, List<DiscordMessage>> DeletedSnipeMessage
        {
            get { return deletedSnipeMessage; }
            set { deletedSnipeMessage = value; }
        }

        private static Dictionary<DiscordGuild, Dictionary<ulong, string>> deletedSnipeDeleter = new();

        public static Dictionary<DiscordGuild, Dictionary<ulong, string>> DeletedSnipeDeleter
        {
            get { return deletedSnipeDeleter; }
            set { deletedSnipeDeleter = value; }
        }

        private static Dictionary<DiscordGuild, List<MessageUpdateEventArgs>> editSnipeMessage = new();

        public static Dictionary<DiscordGuild, List<MessageUpdateEventArgs>> EditSnipeMessage
        {
            get { return editSnipeMessage; }
            set { editSnipeMessage = value; }
        }

        private static string prefix = GetUniqueKey(30);

        public static string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        private static Dictionary<Timer, DiscordMember> currentmuted = new();

        public static Dictionary<Timer, DiscordMember> CurrentMuted
        {
            get { return currentmuted; }
            set { currentmuted = value; }
        }

        #endregion PublicVars

        public static readonly string RootDir = $"/home/trev/cat-bot";

        private static Task CommandHandler(DiscordClient sender, MessageCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Author != sender.CurrentUser)
                {
                    if (e.Message.Channel.Id == 860547879956775002)
                    {
                        if (e.Message.Content.ToLower() != "ur mom lol")
                        {
                            await e.Message.DeleteAsync();
                        }
                    }

                    if (e.Message.Content.StartsWith("!"))
                    {
                        Command cmd = sender.GetCommandsNext().FindCommand(e.Message.Content[1..], out string args);

                        CommandContext ctx = sender.GetCommandsNext().CreateContext(e.Message, Prefix, cmd, args);

                        await cmd.RunCommandAsync(ctx, sender);
                    }

                    if ((e.Message.Content.ToLower().StartsWith("cat") || e.Message.Content.ToLower().StartsWith("cate")) && e.Message.Content.Trim().Count(x => x == ' ') <= 1)
                    {
                        Command cmd = sender.GetCommandsNext().FindCommand(e.Message.Content, out string args);

                        CommandContext ctx = sender.GetCommandsNext().CreateContext(e.Message, Prefix, cmd, args);

                        await cmd.RunCommandAsync(ctx, sender);
                    }

                    if ((e.Message.Content.ToLower().StartsWith("dog") || e.Message.Content.ToLower().StartsWith("doge")) && e.Message.Content.Trim().Count(x => x == ' ') <= 1)
                    {
                        Command cmd = sender.GetCommandsNext().FindCommand("dog", out string args);

                        CommandContext ctx = sender.GetCommandsNext().CreateContext(e.Message, Prefix, cmd, args);

                        await cmd.RunCommandAsync(ctx, sender);
                    }

                    if (e.Message.Content.ToLower().StartsWith("!timer") && e.Message.Content.Trim().Count(x => x == ' ') == 1 && (e.Author != sender.CurrentUser) &&
                        ((e.Author as DiscordMember).Roles.FirstOrDefault(x => x.Name == "ahaha") is null))
                    {
                        var iflee = new StreamReader("./gays.txt");
                        var aeee = await iflee.ReadLineAsync();
                        var aee = int.Parse(aeee);
                        iflee.Close();

                        if (aee < 5)
                        {
                            aee += 1;
                            File.WriteAllText("./gays.txt", aee.ToString());
                            var time = int.Parse(e.Message.Content.Split(" ").Last());
                            var timee = time;
                            var msg = await e.Channel.SendMessageAsync("timer");
                            if (time <= 1500 || e.Author.Id == 758926553454870529)
                            {
                                while (time > 0)
                                {
                                    time -= 1;
                                    await msg.ModifyAsync(time.ToString() + " seconds left");
                                    await Task.Delay(1000);
                                }
                                aee -= 1;
                                File.WriteAllText("./gays.txt", aee.ToString());
                                await msg.ModifyAsync($"timer for {timee} seconds done!");
                                await e.Channel.SendMessageAsync(e.Author.Mention);
                            }
                        }
                    }
                }
            });

            return Task.CompletedTask;
        }

        private static Task Reinvite(DiscordClient sender, GuildMemberRemoveEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Guild.Name.ToLower().Contains("homa") && sender.CurrentApplication.Owners.Select(x => x.Id).Any(x => x == e.Member.Id))
                {
                    DiscordChannel channel = await sender.GetChannelAsync(812339716216455188);
                    DiscordInvite invite = await e.Guild.Channels.Values.First(x => x.Type is ChannelType.Text).CreateInviteAsync(604800);

                    await channel.SendMessageAsync($"https://discord.gg/{invite.Code}");
                }
            });

            return Task.CompletedTask;
        }

        public static readonly Dictionary<string, string> ApiKey = new() { { "x-api-key", "f1b5f4e7-f4dd-4014-b9be-e33fc0b94da1" } };

        private static Task CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                DiscordChannel botchannel = await e.Context.Client.GetChannelAsync(812620259714138112);
                DiscordMember member = await botchannel.Guild.GetMemberAsync(758926553454870529);

                switch (e.Exception)
                {
                    case ChecksFailedException:
                        {
                            try
                            {
                                DiscordEmoji emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                                DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                                    .WithDescription($"{emoji} You don't have the permissions needed to run this command. {emoji}")
                                    .WithColor(DiscordColor.Red)
                                    .WithTimestamp(DateTimeOffset.Now.GetHongKongTime())
                                    .WithFooter($"Requested by: {e.Context.Member.GetFullUsername()}",
                                        null);

                                await e.Context.RespondAsync(null, embed);
                            }
                            catch
                            {
                                // do nothing
                            }

                            throw new Exception(e.Exception.Message, e.Exception);
                        }
                    case FileNotFoundException:
                        {
                            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                                .WithDescription($"The specified file was not found.")
                                .WithColor(DiscordColor.Red)
                                .WithTimestamp(DateTimeOffset.Now.GetHongKongTime());

                            await e.Context.RespondAsync(null, embed);

                            throw new Exception(e.Exception.Message, e.Exception);
                        }
                    case NullReferenceException:
                        {
                            throw new Exception(e.Exception.Message, e.Exception);
                        }
                    case ArgumentException:
                        {
                            throw new Exception(e.Exception.Message, e.Exception);
                        }
                    case CommandNotFoundException:
                        {
                            throw new Exception(e.Exception.Message, e.Exception);
                        }
                    default:
                        {
                            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                                .WithTitle($"Exception occurred while running `{e.Command.QualifiedName}` (Executed by {e.Context.Member.GetFullUsername()}):")
                                .AddField("Type", $"{e.Exception.GetType()}", true)
                                .AddField("Message", $"{e.Exception.Message}", true)
                                .AddField("Inner Exception", !String.IsNullOrEmpty(e.Exception.InnerException.Demystify().ToString()) ?
                                    Formatter.BlockCode(e.Exception.InnerException.Demystify().ToString(), "csharp") : "N/A")
                                .AddField("Stack Trace", !String.IsNullOrEmpty(e.Exception.Demystify().StackTrace) ?
                                    Formatter.BlockCode(e.Exception.Demystify().StackTrace.Replace("Jess", "trev"), "csharp") : "N/A")
                                .AddField("Jump Link", e.Context.Message.JumpLink.ToString())
                                .WithColor(DiscordColor.Red)
                                .WithTimestamp(DateTimeOffset.Now);

                            DiscordMessageBuilder ER = new()
                            {
                                Content = member.Mention,
                                Embed = embed
                            };

                            await botchannel.SendMessageAsync(ER);
                            throw new Exception(e.Exception.Message, e.Exception);
                        }
                }
            });

            return Task.CompletedTask;
        }

        private static Task ClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    DiscordChannel botchannel = await sender.GetChannelAsync(812620259714138112);
                    DiscordMember member = await botchannel.Guild.GetMemberAsync(758926553454870529);

                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                                .WithTitle($"Exception occurred:")
                                .AddField("Type", $"{e.Exception.GetType()}", true)
                                .AddField("Message", $"{e.Exception.Message}", true)
                                .AddField("Inner Exception", !String.IsNullOrEmpty(e.Exception.InnerException.Demystify().ToString()) ?
                                    Formatter.BlockCode(e.Exception.InnerException.Demystify().ToString(), "csharp") : "N/A")
                                .AddField("Stack Trace", !String.IsNullOrEmpty(e.Exception.Demystify().StackTrace) ?
                                    Formatter.BlockCode(e.Exception.Demystify().StackTrace.Replace("Jess", "trev"), "csharp") : "N/A")
                                .WithColor(DiscordColor.Red)
                                .WithTimestamp(DateTimeOffset.Now);

                    DiscordMessageBuilder msg = new()
                    {
                        Content = member.Mention,
                        Embed = embed
                    };

                    await botchannel.SendMessageAsync(msg);
                }
                catch
                {
                    // do nothing
                }

                throw new Exception(e.Exception.Message, e.Exception?.InnerException);
            });

            return Task.CompletedTask;
        }

        private static Task Snipe(DiscordClient sender, MessageDeleteEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                DiscordMember member = e.Message.Author as DiscordMember;
                KeyValuePair<DiscordGuild, List<DiscordMessage>> currentMessage = DeletedSnipeMessage.First(x => x.Key == e.Guild);
                KeyValuePair<DiscordGuild, Dictionary<ulong, string>> currentDeleter = DeletedSnipeDeleter.First(x => x.Key == e.Guild);

                if (!member.IsBot)
                {
                    try
                    {
                        currentMessage.Value.Add(e.Message);
                    }
                    catch
                    {
                        currentMessage.Value.Clear();
                        currentMessage.Value.Add(e.Message);
                    }
                }
            });

            return Task.CompletedTask;
        }

        private static Task EditSnipe(DiscordClient sender, MessageUpdateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Channel.Type is not ChannelType.Private)
                {
                    DiscordMember member = await e.Guild.GetMemberAsync(e.Message.Author.Id);
                    KeyValuePair<DiscordGuild, List<MessageUpdateEventArgs>> current = EditSnipeMessage.First(x => x.Key == e.Guild);

                    if (!member.IsBot)
                    {
                        if (current.Value.Count < 5)
                        {
                            current.Value.Add(e);
                        }
                        else
                        {
                            current.Value.Clear();
                            current.Value.Add(e);
                        }
                    }
                }
            });

            return Task.CompletedTask;
        }

        private static Task Role(DiscordClient sender, Guild​Member​Add​Event​Args e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Member.IsBot)
                {
                    await e.Member.GrantRoleAsync(e.Guild.Roles[849940342614654994]);
                }
                else if (CurrentMuted.ContainsValue(e.Member))
                {
                    await e.Member.GrantRoleAsync(e.Guild.Roles[875582549592797195]);
                    await e.Member.GrantRoleAsync(e.Guild.Roles[849939933841457202]);
                }
                else
                {
                    await e.Member.GrantRoleAsync(e.Guild.Roles[849939933841457202]);
                }
            });

            return Task.CompletedTask;
        }
    }
}