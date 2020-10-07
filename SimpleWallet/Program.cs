using Lyra.Core.Accounts;
using Lyra.Core.API;
using Lyra.Core.Blocks;
using Lyra.Data.Crypto;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleWallet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        private void CreateNewWallet(string networkId, string name, string password)
        {
            var path = Wallet.GetFullFolderName(networkId, "wallets");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var storage = new SecuredWalletStore(path);
            Wallet.Create(storage, name, password, networkId);
        }

        private void RestoreWallet(string networkId, string name, string password, string privateKey)
        {
            var path = Wallet.GetFullFolderName(networkId, "wallets");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var storage = new SecuredWalletStore(path);
            Wallet.Create(storage, name, password, networkId, privateKey);
        }

        private Wallet OpenWallet(string networkId, string name, string password)
        {
            var path = Wallet.GetFullFolderName(networkId, "wallets");
            var storage = new SecuredWalletStore(path);
            return Wallet.Open(storage, name, password);
        }

        private Wallet GetWalletInMemory(string networkId, string privateKey = null)
        {
            var store = new AccountInMemoryStorage();
            var name = Guid.NewGuid().ToString();
            if (privateKey == null)
                Wallet.Create(store, name, "", networkId, Signatures.GenerateWallet().privateKey);
            else
                Wallet.Create(store, name, "", networkId, privateKey);
            var wallet = Wallet.Open(store, name, "");
            return wallet;
        }

        private async Task<APIResultCodes> RefreshBalanceAsync(Wallet wallet)
        {
            // implicty receive
            var rpcClient = LyraRestClient.Create(wallet.NetworkId, Environment.OSVersion.Platform.ToString(), $"{LyraGlobal.PRODUCTNAME} Client Cli", "1.0a");
            return await wallet.Sync(rpcClient);
        }

        private decimal GetBalance(Wallet wallet, string tokenName = null)
        {
            // user should do RefreshBalanceAsync first
            if (tokenName == null)
                return wallet.MainBalance;

            var lasttx = wallet.GetLatestBlock();
            if (lasttx != null && lasttx.Balances.ContainsKey(tokenName))
                return lasttx.Balances[tokenName].ToBalanceDecimal();

            return 0m;
        }

        private async Task<APIResultCodes> SendToken(Wallet wallet, string destAddress, string tokenName, decimal amount)
        {
            var syncResult = await RefreshBalanceAsync(wallet);
            if(syncResult != APIResultCodes.Success)
            {
                return syncResult;
            }

            var result = await wallet.Send(amount, destAddress, tokenName);
            return result.ResultCode;
        }
    }
}
