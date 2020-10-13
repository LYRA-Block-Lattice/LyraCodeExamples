#Simple wallet example for Lyra

Notes:

1) Send/receive or any transactional operation must be kept in serial per account (wallet). Parallel transaction operations for the same account will fail randomly.
2) Multiple accounts/wallets can operate in parallel. Transactions on different accounts (wallets) don't affect each other.
3) Please don't open wallets which have the same private key twice. It's not prohibited by may affect a proper wallet operation.
4) Wallet file stores only private key and DPoS vote info. A balance refreshing ('sync' command in CLI) is recommended before new transaction.
5) Wallet.SendOnce will return immediately if failed. Wallet.Send will retry in 20 seconds if first attempt failed. 
This will cover for special situations when network is in the View Change state (when new primary nodes are added or old nodes removed).
