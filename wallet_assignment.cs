using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using QRCoder;
using QBitNinja.Client;
using QBitNinja.Client.Models;

namespace Blockchain
{
    class Program
    {
        static void Main(string[] args)
        {
                if(args.Length == 0) 
                {
                Console.WriteLine("Please enter a valid question number as commande line argument");    
                }
                else if(args.First() == "Question-1")
                {
                    Key privateKey = new Key(); // generate a random private key
                    PubKey publicKey = privateKey.PubKey; // Get the public key
                    //Console.WriteLine(publicKey);
                    Console.WriteLine(privateKey.ToString(Network.TestNet));
                    var mainnetAddress = publicKey.GetAddress(Network.Main); //Mainnet address
                    var testnetAddress = publicKey.GetAddress(Network.TestNet); // Testnet address

                    string walletInfo = privateKey.ToString(Network.TestNet)+"@"+mainnetAddress.ToString()+"@"+testnetAddress.ToString();

                    walletInfo = walletInfo.Replace("@", System.Environment.NewLine);

                    System.IO.File.WriteAllText("C:\\Users\\Mathieu\\Documents\\Blockchain\\wallet2.txt", walletInfo);

                    // Generate QR-code
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(testnetAddress.ToString(), QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(11);

                    // Print the bitmap
                    PrintDocument pd = new PrintDocument();
                    pd.PrintPage += (thesender, ev) => {

                        float x = 230.0F;
                        float y = 320.0F;

                        RectangleF srcRect = new RectangleF(40.0F, 40.0F, 600.0F, 600.0F);
                        GraphicsUnit units = GraphicsUnit.Pixel;
                        ev.Graphics.DrawImage(qrCodeImage, x, y, srcRect, units);
                
                    };
                    pd.Print();
                }
                else if(args.First() == "Question-2")
                {
                    Key privateKey = new Key(); // generate a random private key
                    PubKey publicKey = privateKey.PubKey; // Get the public key
                    var vanityAddress = publicKey.GetAddress(Network.TestNet); //TestNet address
                    string Text = vanityAddress.ToString();
                    
                    while(Text.Substring(1, 4).Equals("math", StringComparison.CurrentCultureIgnoreCase) == false)
                    {
                    privateKey = new Key(); // generate a random private key
                    publicKey = privateKey.PubKey; // Get the public key
                    vanityAddress = publicKey.GetAddress(Network.TestNet); //TestNet address
                    Text = vanityAddress.ToString();
                    }

                    Console.WriteLine(vanityAddress.ToString());

                    string walletInfo = privateKey.ToString(Network.TestNet)+"@"+vanityAddress.ToString();
                    walletInfo = walletInfo.Replace("@", System.Environment.NewLine);
                    System.IO.File.WriteAllText("C:\\Users\\Mathieu\\Documents\\Blockchain\\vanity2.txt", walletInfo);

                }
                else if(args.First() == "Question-3")
                {

                
                    // Create a client
                    QBitNinjaClient client = new QBitNinjaClient(Network.Main);
                    string TXID = "12bec2fe21387b60c33f2e766557dfeb8b9f1b75d505745f11100d6b2425cf9f";
                    // Parse transaction id to NBitcoin.uint256 so the client can eat it
                    var transactionId = uint256.Parse(TXID);
                    // Query the transaction
                    GetTransactionResponse transactionResponse = client.GetTransaction(transactionId).Result;

                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine("--------------------------------------------------------------------------");
                    Console.WriteLine("--------------------------------------------------------------------------");
                    // Outputs of the transaction = Received Coins
                    Console.WriteLine("Output associated to TX = "+TXID);
                    Console.WriteLine();
                    List<ICoin> receivedCoins = transactionResponse.ReceivedCoins;
                    Money receivedAmount = Money.Zero;
                    foreach (var coinR in receivedCoins)
                    {
                        Money amount = (Money) coinR.Amount;
                        Console.WriteLine("Amount = "+amount.ToDecimal(MoneyUnit.BTC)+" BTC");
                        var paymentScript = coinR.TxOut.ScriptPubKey;
                        Console.WriteLine("ScriptPubKey = \""+paymentScript+"\"");  // It's the ScriptPubKey
                        var address = paymentScript.GetDestinationAddress(Network.Main);
                        Console.WriteLine("Going to address : "+address);
                        receivedAmount = (Money)coinR.Amount.Add(receivedAmount); 
                        Console.WriteLine();
                    }
                    Console.WriteLine("Total output amount = "+receivedAmount.ToDecimal(MoneyUnit.BTC)+" BTC");
                    Console.WriteLine("--------------------------------------------------------------------------");
                    // Inputs of the transaction
                    Console.WriteLine("Inputs associated to TX = "+TXID);
                    Console.WriteLine(); 
                    List<ICoin> spentCoins = transactionResponse.SpentCoins;
                    Money spentAmount = Money.Zero;
                    foreach (var coinS in spentCoins)
                    {
                        Money amount = (Money) coinS.Amount;
                        Console.WriteLine("Amount = "+amount.ToDecimal(MoneyUnit.BTC)+" BTC");
                        var paymentScript = coinS.TxOut.ScriptPubKey;
                        Console.WriteLine("ScriptPubKey = \""+paymentScript+"\"");  // It's the ScriptPubKey
                        var address = paymentScript.GetDestinationAddress(Network.Main);
                        Console.WriteLine("Coming from address : "+address);
                        spentAmount = (Money)coinS.Amount.Add(spentAmount);
                        Console.WriteLine();
                    }
                    Console.WriteLine("Total input amount = "+spentAmount.ToDecimal(MoneyUnit.BTC)+" BTC");
                    Console.WriteLine("--------------------------------------------------------------------------");

                    // Now looking at the fees
                    NBitcoin.Transaction transaction = transactionResponse.Transaction;
                    var fee = transaction.GetFee(spentCoins.ToArray());
                    Console.WriteLine("Transaction fees = "+fee.ToString()+" BTC");

                    Console.WriteLine("--------------------------------------------------------------------------");
                    

                    var inputs = transaction.Inputs;
                    foreach (TxIn input in inputs)
                    {
                        OutPoint previousOutpoint = input.PrevOut;
                        Console.WriteLine("Previous TX hash : " + previousOutpoint.Hash); // hash of prev tx
                        Console.WriteLine("Previous TX index : " + previousOutpoint.N); // idx of out from prev tx, that has been spent in the current tx
                        Console.WriteLine();
                    }
                    Console.WriteLine("--------------------------------------------------------------------------");
                    Console.WriteLine("--------------------------------------------------------------------------");
                    Console.WriteLine();




                    // Getting the balance from the address
                    string copayAddress = "mz2oHHuMqnJg2grqCrKRDGs2vwG5dwNDRu";
                    BitcoinAddress WalletAddress = new BitcoinPubKeyAddress(copayAddress, Network.TestNet);
                    QBitNinjaClient testnetClient = new QBitNinjaClient(Network.TestNet);
                    var balanceModel = testnetClient.GetBalance(WalletAddress).Result;
                    
                    var unspentCoins = new List<Coin>();
                    foreach (var operation in balanceModel.Operations)
                    {
                    unspentCoins.AddRange(operation.ReceivedCoins.Select(coin => coin as Coin)); 
                    }
                    
                    var balance = unspentCoins.Sum(x => x.Amount.ToDecimal(MoneyUnit.BTC));
                    Console.WriteLine();
                    Console.WriteLine("Copay Address = "+copayAddress);
                    Console.WriteLine("Total balance = "+balance.ToString());

                    // Get all operations for that address
                    if (balanceModel.Operations.Count > 0)
                    {
                        foreach (var operation in balanceModel.Operations)
                        {
                            // We take the transaction ID
                            var TXIDb = operation.TransactionId;
                            GetTransactionResponse transactionResponseb = testnetClient.GetTransaction(TXIDb).Result;
                            NBitcoin.Transaction transactionb = transactionResponse.Transaction;

                            Console.WriteLine();
                            Console.WriteLine("Transaction list for the address : ");
                            Console.WriteLine();
                            Console.WriteLine("--------------------------------------------------------------------------");
                            Console.WriteLine("--------------------------------------------------------------------------");
                            // Outputs of the transaction = Received Coins
                            Console.WriteLine("Transaction ID : "+TXIDb);
                            Console.WriteLine();
                            Console.WriteLine("OUTPUTS");
                            Console.WriteLine();
                            List<ICoin> receivedCoinsb = transactionResponseb.ReceivedCoins;
                            foreach (var coinR in receivedCoinsb)
                            {
                                Money amount = (Money) coinR.Amount;
                                Console.WriteLine("Amount = "+amount.ToDecimal(MoneyUnit.BTC)+" BTC");
                                var paymentScript = coinR.TxOut.ScriptPubKey;

                                var address = paymentScript.GetDestinationAddress(Network.TestNet);
                                Console.WriteLine("Going to address : "+address);
                                Console.WriteLine();
                            }
                            Console.WriteLine("--------------------------------------------------------------------------");
                            // Inputs of the transaction
                            Console.WriteLine("INPUTS");
                            Console.WriteLine(); 
                            List<ICoin> spentCoinsb = transactionResponseb.SpentCoins;
                            foreach (var coinS in spentCoinsb)
                            {
                                Money amount = (Money) coinS.Amount;
                                Console.WriteLine("Amount = "+amount.ToDecimal(MoneyUnit.BTC)+" BTC");
                                var paymentScript = coinS.TxOut.ScriptPubKey;
                                var address = paymentScript.GetDestinationAddress(Network.TestNet);
                                Console.WriteLine("Coming from address : "+address);
                                Console.WriteLine();
                            }
                            Console.WriteLine("--------------------------------------------------------------------------");

                            // Now looking at the fees
                            var feeb = transactionb.GetFee(spentCoinsb.ToArray());
                            Console.WriteLine("Transaction fees = "+feeb.ToString()+" BTC");

                            Console.WriteLine("--------------------------------------------------------------------------");
                                    
                            var inputsb = transactionb.Inputs;
                            foreach (TxIn input in inputsb)
                            {
                                OutPoint previousOutpoint = input.PrevOut;
                                Console.WriteLine("Previous TX hash : " + previousOutpoint.Hash); // hash of prev tx
                                Console.WriteLine("Previous TX index : " + previousOutpoint.N); // idx of out from prev tx, that has been spent in the current tx
                                Console.WriteLine();
                            }
                            Console.WriteLine("--------------------------------------------------------------------------");
                            Console.WriteLine("--------------------------------------------------------------------------");
                            Console.WriteLine();

                        }
                    }
                    // List of UTXO
                    Console.WriteLine();
                    Console.WriteLine("List of UTXO");
                    foreach(var c in unspentCoins)
                    {
                    Console.WriteLine("--------------------------------------------------------------------------");
                    Console.WriteLine(c.Outpoint.ToString());
                    Console.WriteLine();
                    }
                }    

                 else if(args.First() == "Question-4")
                {


                    // Sending a transaction

                    var bitcoinPrivateKey = new BitcoinSecret("cP8UmUrZgiRfQZF9C7VYvJQuPK2Ao9ZTnR1BcZ79hADqn9yNTFXg");
                    var network = bitcoinPrivateKey.Network;
                    var address = bitcoinPrivateKey.GetAddress();

                    var client = new QBitNinjaClient(Network.TestNet);
                    var transactionId = uint256.Parse("9e698153d261be07b939ee3e4638b0c09eb466b485f2e09eded5a5eb6fff0519");
                    var transactionResponse = client.GetTransaction(transactionId).Result;

                    // From where ?
                    var receivedCoins = transactionResponse.ReceivedCoins;
                    OutPoint outPointToSpend = null;
                    foreach (var coin in receivedCoins)
                    {
                        if (coin.TxOut.ScriptPubKey == bitcoinPrivateKey.ScriptPubKey)
                        {
                            outPointToSpend = coin.Outpoint;
                        }
                    }
                    if(outPointToSpend == null)
                        throw new Exception("TxOut doesn't contain our ScriptPubKey");
                    Console.WriteLine("We want to spend {0}. outpoint:", outPointToSpend.N + 1);

                    var transaction = Transaction.Create(network);
                    transaction.Inputs.Add(new TxIn()
                    {
                    PrevOut = outPointToSpend
                    });


                    // To where ?
                    var CopayAddress = new BitcoinPubKeyAddress("mokLh8W4J4DzZpj2XyEhE1ReZf5BnsSZcs");

                    // How much we want to spend
                    var CopayAmount = new Money(0.08m, MoneyUnit.BTC);

                    // How much miner fee we want to pay
                    var minerFee = new Money(0.0001m, MoneyUnit.BTC);

                    // How much we want to get back as change
                    var txInAmount = (Money)receivedCoins[(int) outPointToSpend.N].Amount;
                    var changeAmount = txInAmount - CopayAmount - minerFee;

                    TxOut CopayTxOut = new TxOut()
                    {
                        Value = CopayAmount,
                        ScriptPubKey = CopayAddress.ScriptPubKey
                    };
                    TxOut changeTxOut = new TxOut()
                    {
                        Value = changeAmount,
                        ScriptPubKey = bitcoinPrivateKey.ScriptPubKey
                    };

                    transaction.Outputs.Add(CopayTxOut);
                    transaction.Outputs.Add(changeTxOut);

                    // We can use the private key to sign
                    transaction.Inputs[0].ScriptSig =  bitcoinPrivateKey.ScriptPubKey;
                    transaction.Sign(bitcoinPrivateKey, receivedCoins.ToArray());

                    BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;

                    if (!broadcastResponse.Success)
                    {
                        Console.Error.WriteLine("ErrorCode: " + broadcastResponse.Error.ErrorCode);
                        Console.Error.WriteLine("Error message: " + broadcastResponse.Error.Reason);
                    }
                    else
                    {
                        Console.WriteLine("Success! Hash of the transaction :");
                        Console.WriteLine(transaction.GetHash());
                    } 


                    // Confirmations of the transaction
                    var newTXID = uint256.Parse("4777e829eabd4dd5821153d016e5f3256998e962d0e10af1935bfd7ee95e45d0");
                    int n = 0;
                    while (n < 10)
                    {
                        GetTransactionResponse newTXResponse = client.GetTransaction(newTXID).Result;
                        Console.WriteLine();
                        Console.WriteLine("--------------------------------------------------------------------------");
                        Console.WriteLine("Current number of confirmations : " + newTXResponse.Block.Confirmations);
                        Console.WriteLine("--------------------------------------------------------------------------");
                        Console.WriteLine(); 

                        System.Threading.Thread.Sleep(10000);
                        n++;
                    }
                }

                
                else if(args.First() == "Question-5")
                {
                // We identify the blocks in question
                // And we create a list of address that have received these coinbase transactions
        	    var client = new QBitNinjaClient(Network.Main);
                var blockHeight = new BlockFeature(2);
                var blockResponse = client.GetBlock(blockHeight).Result;
                var TX = blockResponse.Block.Transactions.First();
                var reward = TX.Outputs.First();
                var nextAddr = reward.ScriptPubKey.GetDestinationAddress(Network.Main);
                Console.WriteLine(nextAddr);
                }

        }
    }
}