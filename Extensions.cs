using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        public static DateTime GetHongKongTime(this DateTimeOffset offset)
        {
            return offset.UtcDateTime.AddHours(8);
        }

        public static async Task<DiscordGuild> GetGuildAsync(this DiscordUser user, DiscordClient client)
        {
            await Task.Delay(0);

            return client.Guilds.Values.First(x => x.Members.Values.Any(x => x.Id.Equals(user.Id)));
        }

        public static async Task SendMessageAsync(this DiscordUser user, DiscordGuild guild, string content)
        {
            var member = await guild.GetMemberAsync(user.Id);
            await member.TrySendMessageAsync(content);
        }

        public static async Task SendMessageAsync(this DiscordUser user, DiscordGuild guild, DiscordEmbed content)
        {
            var member = await guild.GetMemberAsync(user.Id);
            await member.TrySendMessageAsync(content);
        }

        public static async Task SendMessageAsync(this DiscordUser user, DiscordGuild guild, DiscordMessageBuilder content)
        {
            var member = await guild.GetMemberAsync(user.Id);
            await member.TrySendMessageAsync(content);
        }

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

        public static void AddRange(this Dictionary<string, string> dict, Dictionary<string, string> dict2)
        {
            foreach (var E in dict2)
            {
                dict.Add(E.Key, E.Value);
            }
        }

        public static void AddRange(this WebHeaderCollection dict, Dictionary<string, string> dict2)
        {
            foreach (var E in dict2)
            {
                dict.Add(E.Key, E.Value);
            }
        }

        public static async Task<string> RunBashAsync(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
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

            return result;
        }

        public static List<string> ShowMethods(this Type type)
        {
            List<string> result = new();

            MethodInfo[] array = type.GetMethods();
            for (int i = 0; i < array.Length; i++)
            {
                MethodInfo method = array[i];
                var parameters = method.GetParameters();
                var parameterDescriptions = String.Concat("(", String.Join(", ", method.GetParameters().Select(x => x.ParameterType + " " + x.Name).ToArray()), ")");

                var methodResult = $"{method.ReturnType} {method.Name}{parameterDescriptions}";

                result.Add(Regex.Replace(methodResult, @"`\d+", ""));
            }

            return result;
        }

        public static List<string> ShowProperties(this Type type)
        {
            List<string> result = new();

            PropertyInfo[] array = type.GetProperties();
            for (int i = 0; i < array.Length; i++)
            {
                PropertyInfo property = array[i];
                var propertyResult = $"{property.PropertyType} {property.Name}";

                result.Add(Regex.Replace(propertyResult, @"`\d+", ""));
            }

            return result;
        }

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
                return await reader.ReadToEndAsync();
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
                return await reader.ReadToEndAsync();
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
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;
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
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;
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
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % uniquechars.Length;
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
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % uniquechars.Length;
                result.Append(uniquechars[idx]);
            }

            return result.ToString();
        }

        public static async Task NukeAsync(this DiscordChannel channel)
        {
            var pos = channel.Position;
            await channel.SendMessageAsync($"Nuking {channel.Name}...");

            var qq = await channel.CloneAsync();
            await qq.SendMessageAsync($"Nuked this channel. \nhttps://imgur.com/LIyGeCR");

            await channel.DeleteAsync();

            await qq.ModifyAsync(x => x.Position = pos);
        }

        public static async Task<double> GetCpuLoadAsync(TimeSpan MeasurementWindow)
        {
            Process CurrentProcess = Process.GetCurrentProcess();

            TimeSpan StartCpuTime = CurrentProcess.TotalProcessorTime;
            Stopwatch Timer = Stopwatch.StartNew();

            await Task.Delay(MeasurementWindow);

            TimeSpan EndCpuTime = CurrentProcess.TotalProcessorTime;
            Timer.Stop();

            return (EndCpuTime - StartCpuTime).TotalMilliseconds / (Environment.ProcessorCount * Timer.ElapsedMilliseconds);
        }
    }

    public enum MentionType
    {
        Channel,
        User,
        Role,
        All
    }
}