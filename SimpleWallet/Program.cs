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

            // send from wallet1 to wallet2
            var result = await wallet1.SendToken(wallet2.AccountId, LyraGlobal.OFFICIALTICKERCODE, 10);
            if(result == Lyra.Core.Blocks.APIResultCodes.Success)
            {
                Console.WriteLine($"Success send {LyraGlobal.OFFICIALTICKERCODE} {10} to {wallet2.AccountId}");

                Console.WriteLine("Rrfreshing balance for wallet2...");
                await wallet2.RefreshBalanceAsync();
                DumpWallet(wallet2);
            }
            else
            {
                Console.WriteLine($"Failed to send token. error: {result}");
            }
        }

        private static void DumpWallet(SimpleWallet simpleWallet)
        {
            Console.WriteLine($"{simpleWallet.LyraWallet.AccountName} Account ID: {simpleWallet.AccountId}");
            Console.WriteLine($"{simpleWallet.LyraWallet.AccountName} Balance: {simpleWallet.GetBalance(LyraGlobal.OFFICIALTICKERCODE)}");
        }
    }
}
