using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Lyra.Core.Accounts;
using Lyra.Core.API;
using Lyra.Core.Blocks;
using Lyra.Data.Crypto;

namespace SimpleWallet
{
    public class SimpleWallet
    {
        public Wallet LyraWallet { get; private set; }
        public string AccountId => LyraWallet?.AccountId;

        public SimpleWallet(string networkId, string name, string password)
        {
            var path = Wallet.GetFullFolderName(networkId, "wallets");
            var storage = new SecuredWalletStore(path);
            LyraWallet = Wallet.Open(storage, name, password);
        }

        public SimpleWallet(string networkId, string privateKey)
        {
            var store = new AccountInMemoryStorage();
            var name = Guid.NewGuid().ToString();
            Wallet.Create(store, name, "", networkId, privateKey);
            LyraWallet = Wallet.Open(store, name, "");
        }

        public static (string privateKey, string accountId) GenerateWalletKey()
        {
            return Signatures.GenerateWallet();
        }

        public static void CreateNewWallet(string networkId, string name, string password)
        {
            var path = Wallet.GetFullFolderName(networkId, "wallets");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var storage = new SecuredWalletStore(path);
            Wallet.Create(storage, name, password, networkId);
        }

        public void RestoreWallet(string networkId, string name, string password, string publicKey)
        {
            var path = Wallet.GetFullFolderName(networkId, "wallets");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            var storage = new SecuredWalletStore(path);
            Wallet.Create(storage, name, password, networkId, publicKey);
        }


        public async Task<APIResultCodes> RefreshBalanceAsync()
        {
            // implicty receive
            var rpcClient = LyraRestClient.Create(LyraWallet.NetworkId, Environment.OSVersion.Platform.ToString(), $"{LyraGlobal.PRODUCTNAME} Client Cli", "1.0a");
            return await LyraWallet.Sync(rpcClient);
        }

        public decimal GetBalance(string tokenName = null)
        {
            // user should do RefreshBalanceAsync first
            if (tokenName == null)
                return LyraWallet.MainBalance;

            var lasttx = LyraWallet.GetLatestBlock();
            if (lasttx != null && lasttx.Balances.ContainsKey(tokenName))
                return lasttx.Balances[tokenName].ToBalanceDecimal();

            return 0m;
        }

        public async Task<APIResultCodes> SendToken(string destAddress, string tokenName, decimal amount)
        {
            var syncResult = await RefreshBalanceAsync();
            if (syncResult != APIResultCodes.Success)
            {
                return syncResult;
            }

            var result = await LyraWallet.Send(amount, destAddress, tokenName);
            return result.ResultCode;
        }
    }
}
