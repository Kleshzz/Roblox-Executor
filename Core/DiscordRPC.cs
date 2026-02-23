using System;
using DiscordRPC;
using RobloxExecutor.Core;

namespace RobloxExecutor.Core
{
    public static class DiscordManager
    {
        private static DiscordRpcClient _client;
        private const string APP_ID = "1475440533043417158"; // Replace with your actual App ID

        public static void Initialize()
        {
            if (_client != null) return;

            _client = new DiscordRpcClient(APP_ID);

            _client.OnReady += (sender, e) =>
            {
                Logger.Log("Discord RPC Ready for user: " + e.User.Username);
            };

            _client.OnError += (sender, e) =>
            {
                Logger.Log("Discord RPC Error: " + e.Message);
            };

            _client.Initialize();
            SetPresence();
        }

        public static void SetPresence(string state = "Idle")
        {
            if (_client == null || !_client.IsInitialized) return;

            _client.SetPresence(new RichPresence()
            {
                Details = "Kleshzz`s Executor",
                State = state,
                Assets = new Assets()
                {
                    LargeImageKey = "logo",
                    LargeImageText = "Kleshzz`s Executor"
                },
                Timestamps = Timestamps.Now
            });
        }

        public static void SetInjected()
        {
            SetPresence("Injected into Roblox");
        }

        public static void Shutdown()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }
    }
}
