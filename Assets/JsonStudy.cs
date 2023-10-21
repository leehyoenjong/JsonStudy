using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

//���� ����
[System.Serializable]
public class UserInfo
{
    public string name;
    public string email;
    public int age;
    public bool gender;//true : ����, false:����
    public string[] favorite;
}

// ������Ʈ ����
[System.Serializable]
public class ObjectInfo
{
    public int type;
    public Transform tr;

}
[System.Serializable]
public class SaveInfo
{
    public int type;
    public Vector3 pos;
    public Quaternion rot;
    public Vector3 scale;

}


[System.Serializable]
public class ArrayData<T>
{
    public List<T> data;
}

public class JsonStudy : MonoBehaviour
{
    public UserInfo info2;

    List<UserInfo> allUserInfo = new List<UserInfo>();

    void Start()
    {
        UserInfo info = new UserInfo();
        info.name = "������";
        info.age = 26;
        info.gender = true;
        info.favorite = new string[] { "���", "����" };

        ////Userinfo�� Json���·� ������
        JObject jObject = new JObject();
        jObject["name"] = "����Ƽ";
        jObject["age"] = 10;
        jObject["gender"] = true;
        JArray jArray = new JArray();
        jArray.Add("ġŲ");
        jArray.Add("����");
        jObject["favorite"] = jArray;

        string jsondata = jObject.ToString(); //JsonUtility.ToJson(info);
        print(jsondata);

        //json ���¸� Userinfo �� ������
        JObject reardjObject = JObject.Parse(jsondata);
        string username = reardjObject["name"].ToObject<string>();
        int age = reardjObject["age"].ToObject<int>();
        bool gender = reardjObject["gender"].ToObject<bool>();
        JArray reardjarray = reardjObject["favorite"].ToObject<JArray>();
        string[] favorite = new string[reardjarray.Count];
        for (int i = 0; i < reardjarray.Count; i++)
        {
            favorite[i] = reardjarray[i].ToObject<string>();
        }


        info2 = JsonUtility.FromJson<UserInfo>(jsondata);

        allUserInfo.Add(info);
        allUserInfo.Add(info2);

        jsondata = JsonUtility.ToJson(jsondata);

    }

    //�����ϰ� ������� ������Ʈ�� ��� ���� ����
    [HideInInspector] public List<ObjectInfo> AllObjectinfo = new List<ObjectInfo>();

    void Update()
    {
        //������ ����� ������Ʈ�� �����. ������ ��ġ, ȸ��, ũ�� ����
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            int randtype = Random.Range(0, 4);
            Vector3 pos = Random.onUnitSphere * Random.Range(1f, 20f);
            Quaternion rot = Random.rotation;
            Vector3 size = Vector3.one * Random.Range(0.5f, 3f);
            //������ ������Ʈ ������ �Է�
            CreatObject(randtype, pos, rot, size); ;
        }

        //AllObjectInfo �����͸� Json ���·� �ٲ���
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            List<SaveInfo> savelist = new List<SaveInfo>();

            //AllObjectInfo �� �̿��ؼ� SaveInfo ������
            for (int i = 0; i < AllObjectinfo.Count; i++)
            {
                SaveInfo info = new SaveInfo();
                info.type = AllObjectinfo[i].type;
                info.pos = AllObjectinfo[i].tr.position;
                info.rot = AllObjectinfo[i].tr.rotation;
                info.scale = AllObjectinfo[i].tr.localScale;
                savelist.Add(info);
            }

            SaveInfoToJson();

            ArrayData<SaveInfo> savedata = new ArrayData<SaveInfo>();
            savedata.data = savelist;

            string jsonData = JsonUtility.ToJson(savedata, true);
            print(jsonData);

            //saveData�� File�� ����
            FileStream file = new FileStream(Application.dataPath + "/object.txt", FileMode.Create);
            byte[] byteData = Encoding.UTF8.GetBytes(jsonData);

            //byteData�� File�� ����
            file.Write(byteData, 0, byteData.Length);
            //File�� �ݾ�����
            file.Close();

            //JsonUtility.ToJson(AllObjectinfo);
        }

        //object.txt�� �̿��ؼ� object���� ��ġ ��Ű��.
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            FileStream file = new FileStream(Application.dataPath + "/object.txt", FileMode.Open);
            byte[] bytDat = new byte[file.Length];
            file.Read(bytDat, 0, bytDat.Length);
            file.Close();

            //byte data�� string���� ��ȯ
            string jsonData = Encoding.UTF8.GetString(bytDat);
            print(jsonData);
            JsonToSaveInfo(jsonData);

            //jsonData�� ArrayData<SaveInfo>�� ��ȯ
            ArrayData<SaveInfo> jsonList = JsonUtility.FromJson<ArrayData<SaveInfo>>(jsonData);

            SaveInfo info;

            for (int i = 0; i < jsonList.data.Count; i++)
            {
                info = jsonList.data[i];
                CreatObject(info.type, info.pos, info.rot, info.scale);
            }
        }
    }

    void CreatObject(int type, Vector3 pos, Quaternion rot, Vector3 size)
    {
        GameObject go = GameObject.CreatePrimitive((PrimitiveType)type);
        go.transform.position = pos;
        go.transform.rotation = rot;
        go.transform.localScale = size;

        ObjectInfo objectInfo = new ObjectInfo();
        objectInfo.type = type;
        objectInfo.tr = go.transform;
        AllObjectinfo.Add(objectInfo);
    }

    void SaveInfoToJson()
    {
        JArray jArray = new JArray();

        for (int i = 0; i < AllObjectinfo.Count; i++)
        {
            JObject jobject = new JObject();
            jobject["type"] = AllObjectinfo[i].type;
            JObject vec3 = new JObject();

            jobject["p"] = AllObjectinfo[i].tr.position.x + "," + AllObjectinfo[i].tr.position.y + "," + AllObjectinfo[i].tr.position.z;
            jobject["r"] = AllObjectinfo[i].tr.rotation.x + "," + AllObjectinfo[i].tr.rotation.y + "," + AllObjectinfo[i].tr.rotation.z + "," + AllObjectinfo[i].tr.rotation.w;
            jobject["s"] = AllObjectinfo[i].tr.localScale.x + "," + AllObjectinfo[i].tr.localScale.y + "," + AllObjectinfo[i].tr.localScale.z;
            jArray.Add(jobject);
        }

        print(jArray.ToString());
    }
    
    void JsonToSaveInfo(string jsondata)
    {
        JObject jobject = JObject.Parse(jsondata);
        JArray jArray = jobject["data"].ToObject<JArray>();

        for (int i = 0; i < jArray.Count; i++)
        {
            print(jArray[i]["type"].ToObject<string>());
            print(jArray[i]["pos"]["x"] + "," + jArray[i]["pos"]["y"] + "," + jArray[i]["pos"]["z"]);
            print(jArray[i]["rot"]["x"] + "," + jArray[i]["rot"]["y"] + "," + jArray[i]["rot"]["z"] + "," + jArray[i]["rot"]["w"]);
            print(jArray[i]["scale"]["x"] + "," + jArray[i]["scale"]["y"] + "," + jArray[i]["scale"]["z"]);
        }
    }

}