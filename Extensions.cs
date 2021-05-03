using Aspose.Imaging;
using Aspose.Imaging.Brushes;
using Aspose.Imaging.ImageOptions;
using Aspose.Imaging.Sources;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace cat_bot
{
    public static class Extensions
    {
        private static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo;

        public static string ToTitleCase(this string str)
        {
            return TextInfo.ToTitleCase(str);
        }

        public static Task<Image> Grey(string dir)
        {
            if (!File.Exists(dir))
            {
                throw new FileNotFoundException($"File not found.", dir);
            }

            using (Image image = Image.Load(dir))
            {
                RasterCachedImage rasterCachedImage = (RasterCachedImage)image;

                if (!rasterCachedImage.IsCached)
                {
                    rasterCachedImage.CacheData();
                }

                rasterCachedImage.Grayscale();
                rasterCachedImage.Save(dir);
            }

            Image image1 = Image.Load(dir);
            return Task.FromResult(image1);
        }

        public static Task<Image> Grey(Image image)
        {
            RasterCachedImage rasterCachedImage = (RasterCachedImage)image;

            if (!rasterCachedImage.IsCached)
            {
                rasterCachedImage.CacheData();
            }

            rasterCachedImage.Grayscale();

            return Task.FromResult((Image)rasterCachedImage);
        }

        public static Task<Image> Grey(FileStream file)
        {
            if (!File.Exists(file.Name))
            {
                throw new FileNotFoundException($"File not found.", file.Name);
            }

            using (Image image = Image.Load(file))
            {
                RasterCachedImage rasterCachedImage = (RasterCachedImage)image;

                if (!rasterCachedImage.IsCached)
                {
                    rasterCachedImage.CacheData();
                }

                rasterCachedImage.Grayscale();
                rasterCachedImage.Save(file);
            }

            Image image1 = Image.Load(file);
            return Task.FromResult(image1);
        }

        public static bool IsPrivileged(this Command cmd)
        {
            return Privileged.Contains(cmd.QualifiedName);
        }

        private readonly static List<string> Privileged = new()
        {
            "eval",
            "sudo",
            "unholy",
            "bash"
        };

        public static async Task RunCommandAsync(this Command cmd, CommandContext ctx, DiscordClient client)
        {
            List<DiscordUser> owners = client.CurrentApplication.Owners.ToList();

            if (cmd.IsPrivileged())
            {
                if (owners.Contains(ctx.User))
                {
                    await client.GetCommandsNext().ExecuteCommandAsync(ctx);
                }
                else if (ctx.User.IsWhitelisted(cmd.QualifiedName))
                {
                    await client.GetCommandsNext().ExecuteCommandAsync(ctx);
                }
                else
                {
                    DiscordEmoji emoji = DiscordEmoji.FromName(ctx.Client, ":no_entry:");

                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                        .WithDescription($"{emoji} You don't have the permissions needed to run this command. {emoji}")
                        .WithColor(DiscordColor.Red)
                        .WithTimestamp(DateTimeOffset.UtcNow.GetHongKongTime())
                        .WithFooter($"Requested by: {ctx.Member.GetFullUsername()}",
                            null);

                    await ctx.RespondAsync(null, embed);
                }
            }
            else
            {
                if (owners.Contains(ctx.User))
                {
                    await client.GetCommandsNext().ExecuteCommandAsync(ctx);
                }
                else if (!ctx.User.IsBlacklisted(cmd.QualifiedName))
                {
                    await client.GetCommandsNext().ExecuteCommandAsync(ctx);
                }
                else
                {
                    DiscordEmoji emoji = DiscordEmoji.FromName(ctx.Client, ":no_entry:");

                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
                        .WithDescription($"{emoji} You don't have the permissions needed to run this command. {emoji}")
                        .WithColor(DiscordColor.Red)
                        .WithTimestamp(DateTimeOffset.UtcNow.GetHongKongTime())
                        .WithFooter($"Requested by: {ctx.Member.GetFullUsername()}",
                            null);

                    await ctx.RespondAsync(null, embed);
                }
            }
        }

        public static async Task Throw(this string value)
        {
            if (value == null)
            {
                throw new InvalidOperationException("message");
            }

            await Task.Yield();
        }

        public static Stream ConvertAudioToPcm(string path)
        {
            Process ffmpeg = Process.Start(new ProcessStartInfo
            {
                FileName = "/bin/ffmpeg",
                Arguments = $@"-i ""{path}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            Stream pcm = ffmpeg.StandardOutput.BaseStream;

            return pcm;
        }

        public static IEnumerable<string> SplitInParts(this string s, int partLength)
        {
            if (String.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", nameof(partLength));
            }

            for (int i = 0; i < s.Length; i += partLength)
            {
                yield return s.Substring(i, partLength);
            }
        }

        public static async Task<List<DiscordMessage>> SendLongMessageAsync(this DiscordChannel channel, string content)
        {
            List<string> strings = content.SplitInParts(DateTime.UtcNow.Year).ToList();
            List<DiscordMessage> result = new();

            for (int i = 0; i < strings.Count; i++)
            {
                string s = strings[i];
                DiscordMessage msg = await channel.SendMessageAsync(s);
                result.Add(msg);

                await Task.Delay(12).ConfigureAwait(false);
            }

            return await Task.FromResult(result).ConfigureAwait(false);
        }

        public static string Remove(this string str, string replace)
        {
            return str.Replace(replace, String.Empty);
        }

        public static DateTime GetHongKongTime(this DateTimeOffset offset)
        {
            return offset.UtcDateTime.AddHours(8);
        }

        public static Task<DiscordGuild> GetGuild(this DiscordUser user, DiscordClient client)
        {
            return Task.FromResult(client.Guilds.Values.First(x => x.Members.ContainsKey(user.Id)));
        }

        #region IsBlack/Whitelisted

        public static bool IsBlacklisted(this DiscordUser user, string command)
        {
            bool result = false;

            if (Program.Blacklisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(user.Id))))
            {
                result = true;

                return result;
            }
            else if (Program.Whitelisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(user.Id))))
            {
                result = false;

                return result;
            }
            else
            {
                return result;
            }
        }

        public static bool IsBlacklisted(this DiscordMember member, string command)
        {
            bool result = false;

            if (Program.Blacklisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))))
            {
                result = true;

                return result;
            }
            else if (Program.Whitelisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))))
            {
                result = false;

                return result;
            }
            else
            {
                return result;
            }
        }

        public static bool IsWhitelisted(this DiscordUser user, string command)
        {
            bool result = false;

            if (Program.Blacklisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(user.Id))))
            {
                result = false;

                return result;
            }
            else if (Program.Whitelisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(user.Id))))
            {
                result = true;

                return result;
            }
            else
            {
                return result;
            }
        }

        public static bool IsWhitelisted(this DiscordMember member, string command)
        {
            bool result = false;

            if (Program.Blacklisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))))
            {
                result = false;

                return result;
            }
            else if (Program.Whitelisted.Any(x => x.Key == command && x.Value.Any(x => x.Equals(member.Id))))
            {
                result = true;

                return result;
            }
            else
            {
                return result;
            }
        }

        #endregion IsBlack/Whitelisted

        public static void AddRange(this Dictionary<string, string> dict, Dictionary<string, string> dict2)
        {
            foreach (KeyValuePair<string, string> E in dict2)
            {
                dict.Add(E.Key, E.Value);
            }
        }

        public static void AddRange(this WebHeaderCollection dict, Dictionary<string, string> dict2)
        {
            foreach (KeyValuePair<string, string> E in dict2)
            {
                dict.Add(E.Key, E.Value);
            }
        }

        public static async Task<string> RunBashAsync(this string cmd)
        {
            string escapedArgs = cmd.Replace("\"", "\\\"");

            Process process = new()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string result = await process.StandardOutput.ReadToEndAsync();
            process.WaitForExit();

            if (String.IsNullOrWhiteSpace(result))
            {
                result = "No result returned";
            }

            return await Task.FromResult(result).ConfigureAwait(false);
        }

        #region ListThings

        public static List<string> ListMethods(this Type type)
        {
            List<string> result = new();

            MethodInfo[] array = type.GetMethods();
            for (int i = 0; i < array.Length; i++)
            {
                MethodInfo method = array[i];
                ParameterInfo[] parameters = method.GetParameters();
                string parameterDescriptions = String.Concat("(", String.Join(", ", parameters.Select(x => x.ParameterType + " " + x.Name).ToArray()), ")");

                string methodResult = $"{method.ReturnType} {method.Name}{parameterDescriptions}";

                result.Add(Regex.Replace(methodResult, @"`\d+", ""));
            }

            return result;
        }

        public static List<string> ListProperties(this Type type)
        {
            List<string> result = new();

            PropertyInfo[] array = type.GetProperties();
            for (int i = 0; i < array.Length; i++)
            {
                PropertyInfo property = array[i];
                string propertyResult = $"{property.PropertyType} {property.Name}";

                result.Add(Regex.Replace(propertyResult, @"`\d+", ""));
            }

            return result;
        }

        #endregion ListThings

        #region TrySendMessageAsync

        public static async Task TrySendMessageAsync(this DiscordMember member, string content)
        {
            try
            {
                await member.SendMessageAsync(content);
            }
            catch
            {
                // do nothing
            }
        }

        public static async Task TrySendMessageAsync(this DiscordMember member, string content, DiscordChannel channel)
        {
            try
            {
                await member.SendMessageAsync(content);
            }
            catch
            {
                await channel.SendMessageAsync($"{member.Mention}, Please turn on your DMs!");
            }
        }

        public static async Task TrySendMessageAsync(this DiscordMember member, DiscordEmbed embed)
        {
            try
            {
                await member.SendMessageAsync(embed);
            }
            catch
            {
                // do nothing
            }
        }

        public static async Task TrySendMessageAsync(this DiscordMember member, DiscordEmbed embed, DiscordChannel channel)
        {
            try
            {
                await member.SendMessageAsync(embed);
            }
            catch
            {
                await channel.SendMessageAsync($"{member.Mention}, Please turn on your DMs!");
            }
        }

        public static async Task TrySendMessageAsync(this DiscordMember member, DiscordMessageBuilder message)
        {
            try
            {
                await member.SendMessageAsync(message);
            }
            catch
            {
                // do nothing
            }
        }

        public static async Task TrySendMessageAsync(this DiscordMember member, DiscordMessageBuilder message, DiscordChannel channel)
        {
            try
            {
                await member.SendMessageAsync(message);
            }
            catch
            {
                await channel.SendMessageAsync($"{member.Mention}, Please turn on your DMs!");
            }
        }

        #endregion TrySendMessageAsync

        public static async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Method = "GET";

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new(stream))
            {
                return await Task.FromResult(await reader.ReadToEndAsync().ConfigureAwait(false));
            }
        }

        public static async Task<string> GetAsync(string uri, Dictionary<string, string> headers)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.Headers.AddRange(headers);

            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new(stream))
            {
                return await Task.FromResult(await reader.ReadToEndAsync().ConfigureAwait(false));
            }
        }

        public static string GetFullUsername(this DiscordMember member)
        {
            return String.Concat(member.Username, "#", member.Discriminator);
        }

        public static string GetFullUsername(this DiscordUser user)
        {
            return String.Concat(user.Username, "#", user.Discriminator);
        }

        public static bool IsDivisible(this int x, int n)
        {
            return (x % n) == 0;
        }

        #region BanAsync

        public static async Task BanAsync(this DiscordGuild guild, ulong id, string reason)
        {
            await guild.BanMemberAsync(id, 3, reason);
        }

        public static async Task BanAsync(this DiscordGuild guild, ulong id)
        {
            await guild.BanMemberAsync(id, 3, null);
        }

        public static async Task BanAsync(this DiscordGuild guild, DiscordMember member, string reason)
        {
            await guild.BanMemberAsync(member.Id, 3, reason);
        }

        public static async Task BanAsync(this DiscordGuild guild, DiscordMember member)
        {
            await guild.BanMemberAsync(member.Id, 3, null);
        }

        public static async Task BanAsync(this DiscordGuild guild, DiscordUser user, string reason)
        {
            await guild.BanMemberAsync(user.Id, 3, reason);
        }

        public static async Task BanAsync(this DiscordGuild guild, DiscordUser user)
        {
            await guild.BanMemberAsync(user.Id, 3, null);
        }

        #endregion BanAsync

        public static async Task RestartAsync(this DiscordClient client)
        {
            await client.DisconnectAsync();
            await client.ConnectAsync();
        }

        public static string BreakMentions(this string input)
        {
            input = input.Replace("@", "@\u200B").Replace("#", "#\u200B").Replace("&", "&\u200B");
            return input;
        }

        public static string BreakMentions(this string input, MentionType mentionType)
        {
            switch (mentionType)
            {
                case MentionType.User:
                    {
                        input = input.Replace("@", "@\u200B");

                        return input;
                    }
                case MentionType.Channel:
                    {
                        input = input.Replace("#", "#\u200B");

                        return input;
                    }
                case MentionType.Role:
                    {
                        input = input.Replace("&", "&\u200B");

                        return input;
                    }
                case MentionType.All:
                    {
                        input = BreakMentions(input);

                        return input;
                    }
                default:
                    {
                        input = BreakMentions(input);

                        return input;
                    }
            }
        }

        #region GetUniqueKey

        public static string GetUniqueKey(int size)
        {
            byte[] data = new byte[4 * size];
            char[] chars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVXYZW23456789".ToCharArray();

            using (RNGCryptoServiceProvider crypto = new())
            {
                crypto.GetBytes(data);
            }

            StringBuilder result = new(size);
            for (int i = 0; i < size; i++)
            {
                uint rnd = BitConverter.ToUInt32(data, i * 4);
                long idx = rnd % chars.Length;
                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        public static string GetUniqueKey(int size, string customchars)
        {
            byte[] data = new byte[4 * size];
            char[] chars = customchars.ToCharArray();

            using (RNGCryptoServiceProvider crypto = new())
            {
                crypto.GetBytes(data);
            }

            StringBuilder result = new(size);
            for (int i = 0; i < size; i++)
            {
                uint rnd = BitConverter.ToUInt32(data, i * 4);
                long idx = rnd % chars.Length;
                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        public static string GetUniqueKey(string chars)
        {
            int size = chars.ToCharArray().Length;

            byte[] data = new byte[4 * size];
            char[] uniquechars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVXYZW23456789".ToCharArray();

            using (RNGCryptoServiceProvider crypto = new())
            {
                crypto.GetBytes(data);
            }

            StringBuilder result = new(size);
            for (int i = 0; i < size; i++)
            {
                uint rnd = BitConverter.ToUInt32(data, i * 4);
                long idx = rnd % uniquechars.Length;
                result.Append(uniquechars[idx]);
            }

            return result.ToString();
        }

        public static string GetUniqueKey(string chars, string customchars)
        {
            int size = chars.ToCharArray().Length;

            byte[] data = new byte[4 * size];
            char[] uniquechars = customchars.ToCharArray();

            using (RNGCryptoServiceProvider crypto = new())
            {
                crypto.GetBytes(data);
            }

            StringBuilder result = new(size);
            for (int i = 0; i < size; i++)
            {
                uint rnd = BitConverter.ToUInt32(data, i * 4);
                long idx = rnd % uniquechars.Length;
                result.Append(uniquechars[idx]);
            }

            return result.ToString();
        }

        #endregion GetUniqueKey

        public static async Task NukeAsync(this DiscordChannel channel)
        {
            int pos = channel.Position;
            await channel.SendMessageAsync($"Nuking {channel.Name}...");

            DiscordChannel qq = await channel.CloneAsync();
            await qq.SendMessageAsync($"Nuked this channel. \nhttps://imgur.com/LIyGeCR");

            await channel.DeleteAsync();

            await qq.ModifyAsync(x => x.Position = pos);
        }

        public static async Task<double> GetCpuLoadAsync(TimeSpan MeasurementWindow)
        {
            Process CurrentProcess = Process.GetCurrentProcess();

            TimeSpan StartCpuTime = CurrentProcess.TotalProcessorTime;
            Stopwatch Timer = Stopwatch.StartNew();

            await Task.Delay(MeasurementWindow).ConfigureAwait(false);

            TimeSpan EndCpuTime = CurrentProcess.TotalProcessorTime;
            Timer.Stop();

            return await Task.FromResult((EndCpuTime - StartCpuTime).TotalMilliseconds / (Environment.ProcessorCount * Timer.ElapsedMilliseconds)).ConfigureAwait(false);
        }
    }

    public enum MentionType
    {
        Channel,
        User,
        Role,
        All
    }

    public class MakeTrans
    {
        private readonly Dictionary<char, char> d;

        public MakeTrans(string intab, string outab)
        {
            d = Enumerable.Range(0, intab.Length).ToDictionary(i => intab[i], i => outab[i]);
        }

        public string Translate(string source)
        {
            StringBuilder sb = new(source.Length);

            for (int i = 0; i < source.Length; i++)
            {
                char newsource = source[i];
                sb.Append(d.ContainsKey(newsource) ? d[newsource] : newsource);
            }

            return sb.ToString();
        }
    }
}