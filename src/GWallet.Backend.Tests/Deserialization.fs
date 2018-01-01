﻿namespace GWallet.Backend.Tests

open System
open System.Numerics

open NUnit.Framework
open Newtonsoft.Json

open GWallet.Backend

module Deserialization =

    [<Test>]
    let ``deserialize cache does not fail``() =

        let deserializedCache = Caching.ImportFromJson
                                    MarshallingData.SofisticatedCachingDataExampleInJson

        Assert.That(deserializedCache, Is.Not.Null)

        Assert.That(deserializedCache.Balances, Is.Not.Null)
        Assert.That(deserializedCache.Balances.ContainsKey("0xFOOBARBAZ"))
        let balance,date = deserializedCache.Balances.Item "0xFOOBARBAZ"
        Assert.That(balance, Is.EqualTo(123456789.12345678m))
        Assert.That(date, Is.EqualTo (MarshallingData.SomeDate))

        Assert.That(deserializedCache.UsdPrice, Is.Not.Null)
        let price,date = deserializedCache.UsdPrice.Item Currency.ETH
        Assert.That(price, Is.EqualTo(161.796))
        Assert.That(date, Is.EqualTo (MarshallingData.SomeDate))

    [<Test>]
    let ``unsigned btc transaction import``() =
        let deserializedUnsignedTrans: UnsignedTransaction<IBlockchainFeeInfo> =
            Account.ImportUnsignedTransactionFromJson
                MarshallingData.UnsignedBtcTransactionExampleInJson

        Assert.That(deserializedUnsignedTrans, Is.Not.Null)
        Assert.That(deserializedUnsignedTrans.Proposal, Is.Not.Null)
        Assert.That(deserializedUnsignedTrans.Cache, Is.Not.Null)
        Assert.That(deserializedUnsignedTrans.Metadata, Is.Not.Null)

        Assert.That(deserializedUnsignedTrans.Proposal.Amount.ValueToSend, Is.EqualTo(10.01m))
        Assert.That(deserializedUnsignedTrans.Proposal.Amount.IdealValueRemainingAfterSending,
                    Is.EqualTo(1.01m))
        Assert.That(deserializedUnsignedTrans.Proposal.Currency, Is.EqualTo(Currency.BTC))
        Assert.That(deserializedUnsignedTrans.Proposal.DestinationAddress,
                    Is.EqualTo("13jxHQDxGto46QhjFiMb78dZdys9ZD8vW5"))
        Assert.That(deserializedUnsignedTrans.Proposal.OriginAddress,
                    Is.EqualTo("16pKBjGGZkUXo1afyBNf5ttFvV9hauS1kR"))

        let btcTxMetadata = deserializedUnsignedTrans.Metadata :?> Bitcoin.TransactionMetadata
        Assert.That(btcTxMetadata.Fee.EstimatedTransactionSizeInBytes, Is.EqualTo(10))
        Assert.That(btcTxMetadata.Fee.AmountPerKiloByteForFastTransaction, Is.EqualTo(0.1m))
        Assert.That(btcTxMetadata.Fee.EstimatedTransactionSizeInBytes, Is.EqualTo(10))
        Assert.That(btcTxMetadata.TransactionDraft.Inputs.Length, Is.EqualTo(1))
        Assert.That(btcTxMetadata.TransactionDraft.Outputs.Length, Is.EqualTo(1))
        Assert.That(deserializedUnsignedTrans.Metadata.FeeEstimationTime,
                    Is.EqualTo(MarshallingData.SomeDate))

        Assert.That(deserializedUnsignedTrans.Cache.Balances.Count, Is.EqualTo(0))
        Assert.That(deserializedUnsignedTrans.Cache.UsdPrice.Count, Is.EqualTo(0))

    [<Test>]
    let ``unsigned ether transaction import``() =
        let deserializedUnsignedTrans: UnsignedTransaction<IBlockchainFeeInfo> =
            Account.ImportUnsignedTransactionFromJson
                MarshallingData.UnsignedEtherTransactionExampleInJson

        Assert.That(deserializedUnsignedTrans, Is.Not.Null)
        Assert.That(deserializedUnsignedTrans.Proposal, Is.Not.Null)
        Assert.That(deserializedUnsignedTrans.Cache, Is.Not.Null)
        Assert.That(deserializedUnsignedTrans.Metadata, Is.Not.Null)

        Assert.That(deserializedUnsignedTrans.Proposal.Amount.ValueToSend, Is.EqualTo(10.01m))
        Assert.That(deserializedUnsignedTrans.Proposal.Amount.IdealValueRemainingAfterSending,
                    Is.EqualTo(1.01m))
        Assert.That(deserializedUnsignedTrans.Proposal.Currency, Is.EqualTo(Currency.ETC))
        Assert.That(deserializedUnsignedTrans.Proposal.DestinationAddress,
                    Is.EqualTo("0xf3j4m0rjxdddud9403j"))
        Assert.That(deserializedUnsignedTrans.Proposal.OriginAddress,
                    Is.EqualTo("0xf3j4m0rjx94sushh03j"))

        let etherTxMetadata = deserializedUnsignedTrans.Metadata :?> Ether.TransactionMetadata
        Assert.That(etherTxMetadata.TransactionCount, Is.EqualTo(69))
        Assert.That(etherTxMetadata.Fee.Currency, Is.EqualTo(Currency.ETC))
        Assert.That(etherTxMetadata.Fee.GasPriceInWei, Is.EqualTo(6969))
        Assert.That(deserializedUnsignedTrans.Metadata.FeeEstimationTime,
                    Is.EqualTo(MarshallingData.SomeDate))

        Assert.That(deserializedUnsignedTrans.Cache.Balances.Count, Is.EqualTo(0))
        Assert.That(deserializedUnsignedTrans.Cache.UsdPrice.Count, Is.EqualTo(0))

    [<Test>]
    let ``signed btc transaction import``() =

        let deserializedSignedTrans: SignedTransaction<IBlockchainFeeInfo> =
            Account.ImportSignedTransactionFromJson
                MarshallingData.SignedBtcTransactionExampleInJson

        Assert.That(deserializedSignedTrans, Is.Not.Null)

        Assert.That(deserializedSignedTrans.RawTransaction,
            Is.EqualTo("ropkrpork4p4rkpo4kprok4rp"))

        Assert.That(deserializedSignedTrans.TransactionInfo, Is.Not.Null)
        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal, Is.Not.Null)
        Assert.That(deserializedSignedTrans.TransactionInfo.Cache, Is.Not.Null)
        Assert.That(deserializedSignedTrans.TransactionInfo.Metadata, Is.Not.Null)

        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal.Amount.ValueToSend,
                    Is.EqualTo(10.01m))
        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal.Amount.IdealValueRemainingAfterSending,
                    Is.EqualTo(1.01m))
        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal.Currency,
                    Is.EqualTo(Currency.BTC))
        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal.DestinationAddress,
                    Is.EqualTo("13jxHQDxGto46QhjFiMb78dZdys9ZD8vW5"))
        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal.OriginAddress,
                    Is.EqualTo("16pKBjGGZkUXo1afyBNf5ttFvV9hauS1kR"))

        let btcTxMetadata = deserializedSignedTrans.TransactionInfo.Metadata :?> Bitcoin.TransactionMetadata
        Assert.That(btcTxMetadata.Fee.EstimatedTransactionSizeInBytes, Is.EqualTo(10))
        Assert.That(btcTxMetadata.Fee.AmountPerKiloByteForFastTransaction, Is.EqualTo(0.1m))
        Assert.That(btcTxMetadata.TransactionDraft.Inputs.Length, Is.EqualTo(1))
        Assert.That(btcTxMetadata.TransactionDraft.Outputs.Length, Is.EqualTo(1))
        Assert.That(deserializedSignedTrans.TransactionInfo.Metadata.FeeEstimationTime,
                    Is.EqualTo(MarshallingData.SomeDate))

        Assert.That(deserializedSignedTrans.TransactionInfo.Cache.Balances.Count,
                    Is.EqualTo(0))
        Assert.That(deserializedSignedTrans.TransactionInfo.Cache.UsdPrice.Count,
                    Is.EqualTo(0))

    [<Test>]
    let ``signed ether transaction import``() =

        let deserializedSignedTrans: SignedTransaction<IBlockchainFeeInfo> =
            Account.ImportSignedTransactionFromJson
                MarshallingData.SignedEtherTransactionExampleInJson

        Assert.That(deserializedSignedTrans, Is.Not.Null)

        Assert.That(deserializedSignedTrans.RawTransaction,
            Is.EqualTo("doijfsoifjdosisdjfomirmjosmi"))

        Assert.That(deserializedSignedTrans.TransactionInfo, Is.Not.Null)
        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal, Is.Not.Null)
        Assert.That(deserializedSignedTrans.TransactionInfo.Cache, Is.Not.Null)
        Assert.That(deserializedSignedTrans.TransactionInfo.Metadata, Is.Not.Null)

        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal.Amount.ValueToSend,
                    Is.EqualTo(10.01m))
        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal.Amount.IdealValueRemainingAfterSending,
                    Is.EqualTo(1.01m))
        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal.Currency,
                    Is.EqualTo(Currency.ETC))
        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal.DestinationAddress,
                    Is.EqualTo("0xf3j4m0rjxdddud9403j"))
        Assert.That(deserializedSignedTrans.TransactionInfo.Proposal.OriginAddress,
                    Is.EqualTo("0xf3j4m0rjx94sushh03j"))

        let etherTxMetadata = deserializedSignedTrans.TransactionInfo.Metadata :?> Ether.TransactionMetadata
        Assert.That(etherTxMetadata.TransactionCount, Is.EqualTo(69))
        Assert.That(etherTxMetadata.Fee.Currency,
                    Is.EqualTo(Currency.ETC))
        Assert.That(etherTxMetadata.Fee.GasPriceInWei,
                    Is.EqualTo(6969))
        Assert.That(deserializedSignedTrans.TransactionInfo.Metadata.FeeEstimationTime,
                    Is.EqualTo(MarshallingData.SomeDate))

        Assert.That(deserializedSignedTrans.TransactionInfo.Cache.Balances.Count,
                    Is.EqualTo(2))
        Assert.That(deserializedSignedTrans.TransactionInfo.Cache.UsdPrice.Count,
                    Is.EqualTo(2))
