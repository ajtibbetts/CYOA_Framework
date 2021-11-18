using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;

    public static SaveManager Instance {get{return _instance;}}

    [SerializeField] private int testInt = 0;

    private void Awake() {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else  _instance = this;
    }

    [ContextMenu("Delete Test Data")]
    public void TestDeleteData()
    {
        var confirmDelete = EditorUtility.DisplayDialog("Confirm File Delete","Are you sure you want to delete the test save file?","Yes","No");
        if(!confirmDelete)
        {
            return;
        }
        ES3.DeleteFile("SaveFile.es3");
        Debug.Log("Test save file deleted.");
    }

    [ContextMenu ("Test Save Data")]
    public void TestSaveData()
    {
        Debug.Log("Saving test data");
        ES3.Save("testInt", testInt);
        ES3.Save("playerCaseRecord", PlayerCaseRecord.Instance);
        ES3.Save("player", Player.Instance);
        
    }

    [ContextMenu("Test Load Data")]
    public void LoadTestData()
    {
        Debug.Log("loading test data");
        testInt = ES3.Load<int>("testInt");

        var loadedCaseRecord = ES3.Load<PlayerCaseRecord>("playerCaseRecord");
        PlayerCaseRecord.Instance.LoadSaveData(loadedCaseRecord);

        var loadedPlayer = ES3.Load<Player>("player");
        Player.Instance.LoadSaveData(loadedPlayer);
    }
}