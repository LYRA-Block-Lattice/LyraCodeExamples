using Lyra.Core.API;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleWallet
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Greetings from Lyra Network!");

            var networkId = "testnet";
            var password = "p@ssw0rd";

            // create wallets in file and open it
            //SimpleWallet.CreateNewWallet(networkId, "test1", password);
            var wallet1 = new SimpleWallet(networkId, "test1", password);
            DumpWallet(wallet1);

            // refresh balance
            await wallet1.RefreshBalanceAsync();
            DumpWallet(wallet1);

            // create wallet in memory
            var keyPair = SimpleWallet.GenerateWalletKey();
            var wallet2 = new SimpleWallet(networkId, keyPair.privateKey);
            Console.WriteLine($"Wallet2: {wallet2.AccountId}");

            // 
        }

        private static void DumpWallet(SimpleWallet simpleWallet)
        {
            Console.WriteLine($"Account ID: {simpleWallet.AccountId}");
            Console.WriteLine($"Balance: {simpleWallet.GetBalance(LyraGlobal.OFFICIALTICKERCODE)}");
        }
    }
}
