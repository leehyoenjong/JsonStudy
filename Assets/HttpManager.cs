using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public enum RequestType
{
    GET, POST, PUT, DELETE, TEXTURE
}

public class HttpInfo
{
    public RequestType RequestType;
    public string Url = "";
    public string body;
    public System.Action<DownloadHandler, HttpInfo> onReceive;
    public string costom;
    public void Set(RequestType type, string u, System.Action<DownloadHandler,HttpInfo> cb, bool useDefaultUrl = true)
    {
        RequestType = type;
        if (useDefaultUrl)
        {
            Url += "https://jsonplaceholder.typicode.com/" + u;
        }
        else
        {
            Url += u;
        }
        onReceive = cb;
    }
}

public class HttpManager : MonoBehaviour
{
    static HttpManager instance;

    public static HttpManager GetInstance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("HttpManager");
                gameObject.AddComponent<HttpManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SendRequest(HttpInfo info)
    {
        switch (info.RequestType)
        {
            case RequestType.GET:
                StartCoroutine(Get(info));
                break;
            case RequestType.TEXTURE:
                StartCoroutine(Texture(info));
                break;
        }
    }


    IEnumerator Texture(HttpInfo info)
    {
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(info.Url))
        {
            //�������� ��û ������
            yield return req.SendWebRequest();
            //�������� ������ ��(����/����)
            //����
            Result(req, info);
        }
    }

    void Result(UnityWebRequest req, HttpInfo info)
    {
        if (req.result == UnityWebRequest.Result.Success)
        {
            //������ JsonData�϶� req.downloadHandler.text
            //������ ���� �϶�, req.downloadHandler.data
            if (info.onReceive != null)
            {
                info.onReceive(req.downloadHandler, info);
            }
        }
        //����
        else
        {
            print("��Ʈ��ũ ��� ���� : " + req.error);
        }
    }



    IEnumerator Get(HttpInfo info)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(info.Url))
        {
            //�������� ��û ������
            yield return req.SendWebRequest();
            //�������� ������ ��(����/����)
            //����
            Result(req, info);
        }
    }
}