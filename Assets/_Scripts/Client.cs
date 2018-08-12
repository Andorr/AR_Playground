using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Globalization;
using System.Text;

using UnityEngine;

using System.IO;

public class Client {
	
	public static string url = "https://tihlde-api.herokuapp.com/v1/items/";
	
	public delegate void Callback(Texture2D[] textures);

	public static IEnumerator FetchImages(Callback callback = null)
    {
		// Start fetching from url
        using (WWW www = new WWW(url))
        {
			// Wait for response
            yield return www;

			if(www.error != null) {
				Debug.Log("ERROR: " + www.error);
			}

			// Deserialize response-json-string to Document-class
			string jsonData = JsonHelper.fixJson(www.text);
			Document[] Documents = JsonHelper.FromJson<Document>(jsonData);

			foreach(Document d in Documents) {
				Debug.Log(d);
			}

			// Convert Documents to Texture2D
			Texture2D[] textures = ConvertDocumentsToTextures(Documents);
			
			// If callback is provided, call callback
			if(callback != null) {
				callback(textures);
			}
        }
    }

	// Converts Documents to Texture2Ds
	private static Texture2D[] ConvertDocumentsToTextures(Document[] Documents) {
		List<Texture2D> textures = new List<Texture2D>();
		foreach(Document d in Documents) {
			foreach(Page p in d.pages) {
				string base64 = p.data;
				byte[] bytes = System.Convert.FromBase64String(base64);
				Texture2D texture = new Texture2D(1,1);
				bool status = texture.LoadImage(bytes);
				if(status) {
					textures.Add(texture);
				}
			}
			
		}
		return textures.ToArray();
	}
}

[System.Serializable]
class Document {
	public string name = null;
	public Page[] pages = null;

	public override string ToString() {
		string output = name + ", ";
		foreach(Page p in pages) {
			output += "\n" + p;
		}
		return output;
	}
}

[System.Serializable]
class Page {
	public int number = 0;
	public string data = null;

	public override string ToString() {
		return "Number: " + number;
	}
}

// Helper class for serializing/deseralizing from/to json
// Makes it possible to deserialize json-arrays with Wrapper-class.
static class JsonHelper
{
	// Deserialize
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

	// Serialize object
    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

	// Serialize object
    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

	// Serialize
	public static string fixJson(string value)
	{
		value = "{\"Items\":" + value + "}";
		return value;
	}

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
