# blockjam-2022

## Video presentation

[![photo_2022-07-12_04-55-23](https://user-images.githubusercontent.com/107640719/178449815-c6bbc18d-3b7b-4b27-b5db-04db6dc1f9cf.jpg)](https://www.youtube.com/embed/PGWBTjnhCDA "Nervos.land BlockJam 2022")


[Play demo here](http://demo.nervos.land/)


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

To upload this metadata I used `https://github.com/ericvanderwal/NFTStorageUnitySDK` (very useful, thx Eric). 

Some code snippets that I used in game:

```
// load skin asset 
UnityWebRequest request = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + fileDirectory + fileAssetName);
await request.SendWebRequest();

if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
{
    // handle failure
    Debug.Log("request failure: " + request.result.ToString());
}
else
{
    try
    {
        StartCoroutine(NFTstorage.NetworkManager.UploadObject(CallBackOnUpload, request.downloadHandler.data));
    }
    catch (Exception x)
    {
        // handle failure
        Debug.LogError(x.Message);
    }
}
        
// Callback 
private void CallBackOnUpload(NFTstorage.DataResponse obj)
{
    if (!obj.Success)
    {
        Debug.Log("Error upload file data");
        return;
    }

    Debug.Log("CallBackOnUpload: " + obj.Values[0].cid);

    if (obj.Values != null && obj.Values.Count > 0)
    {
        if (obj.Values != null)
        {
            // set IPFS to NFTMetadata object
            nftMetaData.SetIPFS(obj.Values[0].cid);
        }
    }
}
```

Then continue to set-up your `NFTMetadata` object and upload:

```
public NFTstorage.ERC721.NftMetaData nftMetaData;

// here process of setting nftMetaData

var bytes = NFTstorage.Helper.ERC721MetaDataToBytes(nftMetaData);
// Upload metadata to NFT.storage
StartCoroutine(NFTstorage.NetworkManager.UploadObject(CallBackOnUploadMetadata, bytes));

// Callbacks
// Upload Metadata
async private void CallBackOnUploadMetadata(NFTstorage.DataResponse obj)
{
    if (obj.Success)
    {
        if (obj.Values != null && obj.Values.Count > 0)
        {
            var path = Helper.GenerateGatewayPath(obj.Values[0].cid, Constants.GatewaysSubdomain[0], true);
            Debug.Log("Metadata set to: " + path);
        }
    }
    else
    {
        Debug.Log("Error uploading metadata to NFT.storage " + obj.Error.message);
    }
}
```

I added additional own fields to NFTMetadata class of NFTStorageUnitySDK. 
Completed real working in game token URI: `bafkreid3mdgtmgmqnhbkzatkcggsr7evnfjl7pldryz4j2ayxl6q3sa444.ipfs.nftstorage.link`

## -- Dignitas Bounty --

Provided by judges smart-contract address `0xed1dd0a4ddf6823e4f9aef6cba92627b71707b10` has 5 items with attributes - `health`, `speed`, `power`, `defend`, `attack`. The idea of using each is to allow users to customize flags by texturing with images of attributes and then increasing this attribute of the nearest game character (the second part wasn't implemented all). All related code see in `Dignitas.cs` file.

![image](https://user-images.githubusercontent.com/107640719/178403303-21481628-da5b-423f-bf84-a13b9972259e.png)

