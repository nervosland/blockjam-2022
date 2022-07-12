using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Dignitas : MonoBehaviour
{
    public class Attribute
    {
        public int value;
        public string trait_type;

    }
    public class MetaDataResponse
    {
        public string image;
        public string name;
        public string description;
        public List<Attribute> attributes;

    }

    public static GameObject selectedObject;

    async public void LoadURI(string tokenId)
    {
        if (selectedObject == null)
            return;
        string contract = "0xEd1dD0a4ddF6823e4f9Aef6CBA92627b71707B10";

        // fetch uri from chain
        string uri = await ERC721.URI(Godwoken.chain, Godwoken.network, contract, tokenId, Godwoken.RPC);
        print("URI: " + uri);

        // fetch json from uri
        UnityWebRequest webRequest = UnityWebRequest.Get(uri);
        await webRequest.SendWebRequest();
        MetaDataResponse data = JsonUtility.FromJson<MetaDataResponse>(System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data));

        // parse json to get image uri
        string imageUri = "https://ipfs.io/ipfs/" + data.image;
        Debug.Log("imageUri: " + imageUri);

        // fetch image and display in game
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(imageUri);
        await textureRequest.SendWebRequest();
        selectedObject.GetComponentsInChildren<Renderer>()[1].material.mainTexture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;
    }
}
