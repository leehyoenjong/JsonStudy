using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void onClickReferPosts()
    {
        HttpInfo info = new HttpInfo();
        info.Set(RequestType.GET, "photos", (downloadhandler, http) =>
        {
            //통신이 성공하면 호출되는 람다식
            print(downloadhandler.text);
            string jsonData = "{\"data\":" + downloadhandler.text + "}";
            var allUser = JsonUtility.FromJson<ArrayData<PhotoInfo>>(jsonData);

            for (int i = 0; i < 10; i++)
            {
                DownLoadPhoto(allUser.data[i].thumbnailUrl, allUser.data[i].id);
            }
        });


        HttpManager.GetInstance.SendRequest(info);
    }

    public void onClickReferUsers()
    {
        HttpInfo info = new HttpInfo();
        info.Set(RequestType.GET, "users", (downloadhandler, http) =>
        {
            //통신이 성공하면 호출되는 람다식
            print(downloadhandler.text);

            //만약에 JsonUtility 를 쓴다면 앞에 키값을 부여해줘야함
            ParseUseJsonUtility(downloadhandler.text);
            //Newtonsoft를 쓴다면
            ParseuseNewtonJson(downloadhandler.text);
        });


        HttpManager.GetInstance.SendRequest(info);
    }

    void ParseUseJsonUtility(string receiveData)
    {
        string jsonData = "{\"data\":" + receiveData + "}";
        var allUser = JsonUtility.FromJson<ArrayData<UserData>>(jsonData);
    }


    List<UserInfo> userinfos = new List<UserInfo>();
    void ParseuseNewtonJson(string receiveData)
    {
        JArray jArray = JArray.Parse(receiveData);

        for (int i = 0; i < jArray.Count; i++)
        {
            UserInfo userinfo = new UserInfo();
            userinfo.name = jArray[i]["name"].ToString();
            userinfo.email = jArray[i]["email"].ToString();
            userinfos.Add(userinfo);
        }
    }

    public GameObject imageFactory;
    public Transform trContent;

    void DownLoadPhoto(string photoUrl, int idx)
    {
        HttpInfo info = new HttpInfo();

        info.Set(RequestType.TEXTURE, photoUrl, (downloadhandler, httpinfo) =>
        {
            //다운로드 데이터를 sprite 형식으로 바꾸자
            Texture2D text = ((DownloadHandlerTexture)downloadhandler).texture;
            Sprite sprite = Sprite.Create(text, new Rect(0, 0, text.width, text.height), Vector2.zero);
            var go = Instantiate(imageFactory, trContent);
            go.GetComponent<Image>().sprite = sprite;

            FileStream file = new FileStream(Application.dataPath + "/" + info.costom + ".png", FileMode.Create);
            file.Write(downloadhandler.data, 0, downloadhandler.data.Length);
            file.Close();

        }, false);
        info.costom = idx.ToString();
        HttpManager.GetInstance.SendRequest(info);
    }

    [Serializable]
    public struct PhotoInfo
    {
        public int albumId;
        public int id;
        public string title;
        public string url;
        public string thumbnailUrl;
    }

    [Serializable]
    public struct Company
    {
        public string name;
        public string catchPhrase;
        public string bs;
    }
    [Serializable]
    public struct GeoData
    {
        public string lat;
        public string ing;
    }
    [Serializable]
    public struct AddressData
    {
        public string street;
        public string suite;
        public string city;
        public string zipcode;
        public GeoData geo;
    }
    [Serializable]
    public struct UserData
    {
        public int id;
        public string name;
        public string username;
        public string email;
        public AddressData address;
        public string phone;
        public string website;
        public Company company;
    }
}