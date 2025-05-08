using Fusion;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


//게임 시작 시 씬을 이동하고 모든 유저의 씬 동기화를 맞추는 매니저
public class ClientSceneManager : MonoBehaviour
{
    //싱글톤 | 로딩 패널 | 진행 바
    public static ClientSceneManager instance;
    public GameObject loadingScreen;
    public Slider progressBar;
    public TextMeshProUGUI loadingCount;

    //씬 이름 변수 지정
    const string RedTeamSceneName = "RedTeam";
    const string BlueTeamSceneName = "BlueTeam";
    const string StageSceneName = "StageScene";

    private void Awake()
    {
        if (instance == null) { instance = this; }

        DontDestroyOnLoad(gameObject);
    }

    private string SeparateScene(UserData player)
    {
        if(player.WhereTeam == UserData.Team.Red)
        {
            return RedTeamSceneName;
        }
        else
        {
            return BlueTeamSceneName;
        }
    }

    #region 선택 씬으로 가기
    public void GoSelectScene(UserData player)
    {
        string name = SeparateScene(player);
        SceneManager.sceneLoaded += OnSelectSceneLoaded;
        StartCoroutine(LoadNextScene(name));
    }

    IEnumerator InitSelectManager()
    {
        yield return new WaitForSeconds(2f);
        FusionConnection connection = FusionConnection.instance;

        connection.CreateSelectManager();
        if (connection._sessionManager.HasStateAuthority)
        {
            connection.runner.Despawn(connection._sessionManager.GetComponent<NetworkObject>());
        }
    }

    void OnSelectSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InitSelectManager());
        SceneManager.sceneLoaded -= OnSelectSceneLoaded;
    }


    #endregion

    #region 게임 맵 씬으로 이동
    public void GoGameMap(string _sceneName)
    {
        StartCoroutine(SettingGameMap(_sceneName));
    }

    IEnumerator SettingGameMap(string _sceneName)
    {
        yield return StartCoroutine(LoadNextScene(_sceneName));

        FusionConnection connection = FusionConnection.instance;

        connection.CreateRoundtManager();

        if (connection._selectManager.HasStateAuthority)
        {
            connection.runner.Despawn(connection._selectManager.GetComponent<NetworkObject>());
        }
    }

    #endregion

    //씬 이동
    IEnumerator LoadNextScene(string _sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(_sceneName);
        op.allowSceneActivation = false;

        loadingScreen.SetActive(true);

        progressBar.value = 0f;
        while (!op.isDone)
        {
            yield return null;

            loadingCount.text = ((int)(progressBar.value * 100)).ToString() + "%";

            if (progressBar.value < 0.9f)
            {
                progressBar.value = Mathf.MoveTowards(progressBar.value, 0.9f, Time.deltaTime);
            }
            else if (op.progress >= 0.9f)
            {
                progressBar.value = Mathf.MoveTowards(progressBar.value, 1f, Time.deltaTime);
            }
            if (progressBar.value >= 1.0f)
            {               
                op.allowSceneActivation = true;                
            }
        }
        yield return new WaitForSeconds(3f);
        loadingScreen.SetActive(false);
    }
}
