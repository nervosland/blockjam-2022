# blockjam-2022

## Smart contracts: 

Main game contract:   `0x29862367c3Ba29e47519DAf105fA0a93F23C0C05`

Character NFT Colonist (COL) contract:  `0x840909e2149C1A359b4C1129aBE10dCD1BEab979`

Nervos Land Token (NLT):  `0x59B663f43ad26Ed16A6232131beAcd88D908f02e`

Faucet NLT: `0xdf153acC0C827bD43940a1A024b8232cE30E1289`


# BOUNTIES

## -- ChainSafe SDK Bounty --

Chainsafe SDK is the main transport service Unity -> EVM. All blockchain-related operations in a game use this SDK. 
For example, starting the game each user must create their own village by calling Game contract address with the `createVillage` method:

```
// creating new village
async public static Task<string> CreateVillage()
    {
        // smart contract method to call
        string method = "createVillage";
        // value in wei
        string value = "0";
        // gas limit OPTIONAL
        string gasLimit = "1000000";
        // gas price OPTIONAL
        string gasPrice = "";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {
            string response = await Web3GL.SendContract(method, gameContractABI, gameContractAddress, "[]", value, gasLimit, gasPrice);
            print(response);

            return response;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            print(e);

            return e.Message;
        }
    }
```

or getting data token URI of NFT contract calling `tokenURI`. But in this case, Chainsafe SDK has special ERC721 class:

```
    async public static Task<string> GetTokenURI(string contract, string tokenID)
    {
        return await ERC721.URI(chain, network, contract, tokenID);

    }
```

Unfortunately, not all methods of SDK support Nervos Godwoken yet. For example, `AllErc721` that could be very useful.
Also, I didn't use `Minter - Beta EVM Testnet` and all-new related methods because of beta status. Anyway, all minting process works pretty well for me with `SendContract()`:

```
        string method = "mintCharacter";
        args = "[\"" + addressOwner + "\",\"" + tokenURI + "\"]";
        try
        {
            Debug.Log("Sending mint token...");
            Debug.Log("args:" + args);
            string response = await Web3GL.SendContract(method, NFT.characterNFTcontractABI, NFT.characterNFTcontractAddress, args, value, gasLimit, gasPrice);
            print("mintCharacter tx:" + response);

            string result = await CheckTx(response);


            while (result == "pending")
            {
                result = await CheckTx(response);
                await new WaitForSeconds(1f);
            }

            Debug.Log("Status tx: " + result);

            if (result == "fail")
                return false;
            return true;


        }
        catch (Exception e)
        {
            Debug.LogException(e);
            print(e);

            return false;
        }
```


## -- Filecoin Bounty --

In the game, players can mint their own NFT characters with attributes used in gameplay. So, to store metadata I used `NFT.storage` to upload all related information and get it back.

For example, in game NFT Manager we can see token URI of each NFT character:

![image](https://user-images.githubusercontent.com/107640719/178401122-23563e0e-e669-44c4-a206-f78568179e99.png)

To upload this metadata I used `https://github.com/ericvanderwal/NFTStorageUnitySDK`. All related code see in `NftStorage` folder.

## -- Dignitas Bounty --

Provided by judges smart-contract address `0xed1dd0a4ddf6823e4f9aef6cba92627b71707b10` has 5 items with attributes - `health`, `speed`, `power`, `defend`, `attack`. The idea of using each is to allow users to customize flags by texturing with images of attributes and then increasing this attribute of the nearest game character (the second part wasn't implemented all). All related code see in `Dignitas` folder.

![image](https://user-images.githubusercontent.com/107640719/178403303-21481628-da5b-423f-bf84-a13b9972259e.png)

