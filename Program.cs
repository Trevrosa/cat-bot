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
using static cat_bot.Extensions;
using DSharpPlus.Exceptions;
using Sentry;

namespace cat_bot
{
    public class Program
    {
        private static void Main(string[] args)
        {
            using (SentrySdk.Init(new SentryOptions() { Dsn = "https://218dee17e1a340e19ca5382bbadeda35@o545219.ingest.sentry.io/5666766", TracesSampleRate = 1 }))
            {
                MainAsync().GetAwaiter().GetResult();
            }
        }

        public static async Task MainAsync()
        {
            #region config

            discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = JsonDocument.Parse(File.OpenRead("/root/cat bot/token.json")).RootElement.GetProperty("token").ToString(),
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.Debug
            });

            CommandsNextExtension commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" },
                CaseSensitive = false,
                DmHelp = false,
                EnableDms = true,
                EnableMentionPrefix = true
            });

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

            commands.RegisterCommands<Commands>();

            #endregion config

            commands.CommandErrored += CommandErrored;
            discord.ClientErrored += ClientError;
            discord.MessageCreated += Cat;
            discord.Ready += Ready;
            discord.GuildMemberRemoved += Reinvite;

            discord.MessageDeleted += Snipe;
            discord.MessageUpdated += EditSnipe;

            DiscordActivity av = new()
            {
                ActivityType = ActivityType.Playing,
                Name = "coded by trev !!"
            };

            await discord.ConnectAsync(av);
            await Task.Delay(-1);
        }

        public static DiscordClient discord;

        public static Dictionary<string, List<ulong>> Whitelisted = new();
        public static Dictionary<string, List<ulong>> Blacklisted = new();

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

        private static Task Ready(DiscordClient sender, ReadyEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                //if (File.Exists($"/root/cat bot/whitelisted.txt"))
                //{
                //    string[] text = await File.ReadAllLinesAsync($"/root/cat bot/whitelisted.txt");
                //    List<string> e = text.ToList();
                //    for (int i = 0; i < e.Count; i++) { string ae = e[i]; Whitelisted.Add(ae.Split(": ").First(), ae.Split(": ").Last().Split(", ").Select(x => ulong.Parse(x)).ToList()); }
                //}

                //if (File.Exists($"/root/cat bot/blacklisted.txt"))
                //{
                //    string[] text = await File.ReadAllLinesAsync($"/root/cat bot/blacklisted.txt");
                //    List<string> e = text.ToList();
                //    for (int i = 0; i < e.Count; i++) { string ae = e[i]; Blacklisted.Add(ae.Split(": ").First(), ae.Split(": ").Last().Split(", ").Select(x => ulong.Parse(x)).ToList()); }
                //}
            });

            return Task.CompletedTask;
        }

        public static readonly Dictionary<string, string> ApiKey = new() { { "x-api-key", "f1b5f4e7-f4dd-4014-b9be-e33fc0b94da1" } };

        private static Task Cat(DiscordClient sender, MessageCreateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Author != sender.CurrentUser)
                {
                    if (e.Message.Content.ToLower().StartsWith("cat"))
                    {
                        Command cmd = sender.GetCommandsNext().FindCommand("cat", out string args);

                        CommandContext fctx = sender.GetCommandsNext().CreateFakeContext(e.Author, e.Channel, e.Message.Content, "!", cmd, args);
                        await sender.GetCommandsNext().ExecuteCommandAsync(fctx);
                    }
                }
            });

            return Task.CompletedTask;
        }

        private static Task CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                DiscordGuild guild = e.Context.Client.Guilds.Values.First(x => x.Name == "minecrumbs");

                ulong s = guild.Channels.Values.FirstOrDefault(x => x.Name.ToLower() == "log") != null ? guild.Channels.Values.First(x => x.Name.ToLower() == "log").Id : 12312312;
                DiscordChannel botchannel = guild.GetChannel(s);
                DiscordMember member = await guild.GetMemberAsync(758926553454870529);

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
                                .AddField("Inner Exception", e.Exception.InnerException != null ? e.Exception.InnerException.ToString() : "N/A")
                                .AddField("Stack Trace", e.Exception.StackTrace != null ? Regex.Replace(e.Exception.StackTrace.Replace("Jess", "trev"), @"`\d+", "") : "N/A")
                                .AddField("Jump Link", e.Context.Message.JumpLink.ToString())
                                .WithColor(DiscordColor.Red)
                                .WithTimestamp(DateTimeOffset.Now.GetHongKongTime());

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
                    DiscordGuild guild = sender.Guilds.Values.First();
                    ulong s = guild.Channels.Values.FirstOrDefault(x => x.Name.ToLower().Contains("log")) != null ? guild.Channels.Values.First(x => x.Name.ToLower().Contains("log")).Id : 12312312;
                    DiscordChannel botchannel = guild.GetChannel(s);

                    DiscordMember member = await guild.GetMemberAsync(758926553454870529);

                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                                .WithTitle($"Exception occurred:")
                                .AddField("Type", $"{e.Exception.GetType()}", true)
                                .AddField("Message", $"{e.Exception.Message}", true)
                                .AddField("Inner Exception", e.Exception.InnerException != null ? e.Exception.InnerException.ToString() : "N/A")
                                .AddField("Stack Trace", e.Exception.StackTrace != null ? e.Exception.StackTrace.Replace("Jess", "trev") : "N/A")
                                .WithColor(DiscordColor.Red)
                                .WithTimestamp(DateTimeOffset.Now.GetHongKongTime());

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

                throw new Exception(e.Exception.Message, e.Exception);
            });

            return Task.CompletedTask;
        }

        public static Dictionary<DiscordGuild, MessageUpdateEventArgs> EditSnipeMessage = new();

        private static Task EditSnipe(DiscordClient sender, MessageUpdateEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                if (e.Channel.Type is not ChannelType.Private)
                {
                    DiscordMember member = await e.Guild.GetMemberAsync(e.Message.Author.Id);

                    if (!member.IsBot)
                    {
                        try
                        {
                            EditSnipeMessage.Add(e.Guild, e);
                        }
                        catch
                        {
                            EditSnipeMessage.Remove(e.Guild);
                            EditSnipeMessage.Add(e.Guild, e);
                        }
                    }
                }
            });

            return Task.CompletedTask;
        }

        public static Dictionary<DiscordGuild, DiscordMessage> DeletedSnipeMessage = new();
        public static Dictionary<DiscordGuild, string> DeletedSnipeDeleter = new();

        private static Task Snipe(DiscordClient sender, MessageDeleteEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                DiscordMember member = await e.Guild.GetMemberAsync(e.Message.Author.Id);

                if (!member.IsBot)
                {
                    try
                    {
                        DeletedSnipeMessage.Add(e.Guild, e.Message);
                    }
                    catch
                    {
                        DeletedSnipeMessage.Remove(e.Guild);
                        DeletedSnipeMessage.Add(e.Guild, e.Message);
                    }
                }

                if (e.Guild.CurrentMember.PermissionsIn(e.Channel).HasPermission(Permissions.ViewAuditLog))
                {
                    IReadOnlyList<DiscordAuditLogEntry> audits = await e.Guild.GetAuditLogsAsync(1);
                    DiscordAuditLogEntry log = audits.FirstOrDefault(x => x.ActionType is AuditLogActionType.MessageDelete);

                    try
                    {
                        DeletedSnipeDeleter.Add(e.Guild, log.UserResponsible.Mention);
                    }
                    catch
                    {
                        try
                        {
                            DeletedSnipeDeleter.Remove(e.Guild);
                            DeletedSnipeDeleter.Add(e.Guild, log.UserResponsible.Mention);
                        }
                        catch
                        {
                            try
                            {
                                DeletedSnipeDeleter.Add(e.Guild, member.Mention);
                            }
                            catch
                            {
                                DeletedSnipeDeleter.Remove(e.Guild);
                                DeletedSnipeDeleter.Add(e.Guild, member.Mention);
                            }
                        }
                    }
                }
                else
                {
                    IReadOnlyList<DiscordAuditLogEntry> audits = await e.Guild.GetAuditLogsAsync(1);
                    DiscordAuditLogEntry log = audits.First(x => x.ActionType is AuditLogActionType.MessageDelete);

                    try
                    {
                        DeletedSnipeDeleter.Add(e.Guild, log.UserResponsible.Mention);
                    }
                    catch
                    {
                        try
                        {
                            DeletedSnipeDeleter.Remove(e.Guild);
                            DeletedSnipeDeleter.Add(e.Guild, log.UserResponsible.Mention);
                        }
                        catch (UnauthorizedException)
                        {
                            if (e.Guild.Owner.Username == "Homa")
                            {
                                try
                                {
                                    DeletedSnipeDeleter.Add(e.Guild, "homa give cat bot audit logs permission to see who deleted this message you gay");
                                }
                                catch
                                {
                                    DeletedSnipeDeleter.Remove(e.Guild);
                                    DeletedSnipeDeleter.Add(e.Guild, "homa give cat bot audit logs permission to see who deleted this message you gay");
                                }
                            }
                            else
                            {
                                try
                                {
                                    DeletedSnipeDeleter.Add(e.Guild, "give cat bot audit logs permision to see who deleted this message");
                                }
                                catch
                                {
                                    DeletedSnipeDeleter.Remove(e.Guild);
                                    DeletedSnipeDeleter.Add(e.Guild, "give cat bot audit logs permision to see who deleted this message");
                                }
                            }
                        }
                    }
                }
            });

            return Task.CompletedTask;
        }
    }
}