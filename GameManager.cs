using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GoogleMobileAds.Api;



public class GameManager : MonoBehaviour
{
    // public variables for in yhe inspectoer
    public TextMeshProUGUI coinText, costSpawnTimerUpgrade, CurentExp, coinPerSecText, levelText,
        currentEntetiesOnScreen, actualMaxEnteties, maxEntetiesUpgradeCostText, costOfflineTimeUpgrade,
        actualSpawnTime, actualOfflineTime;
    public CanvasGroup Upgrades_Screen, UI, Loading_Screen;
    [Space(10)]
    public List<GameObject> Enteties;
    public List<TextMeshProUGUI> bugList;
    public GameObject coinHolder;
    public bool touched = false;
    //private varriables to use in the code
    private TextMeshProUGUI rewardText;
    private SaveManager save;
    public Transform selected;
    private TimeSpan maxOffline;
    private string lastDateTime;
    private List<string> spawnedEnteties;
    private float CoinsPerSecond, current,  timerReward,  spawnDelay,  rewardDelay, reward, TimerEnt, TimerAds, AdsDelay;
    private string loadGamePath;
    private int level, maxOfflineTimeH, maxOfflineTimeM, maxEnteties, curEntetiesOnScreen, maxUpgradeCost, Spawned, exp, expNeeded;
    private bool exceeded;
    //contant variables to use in the code
    private const float  coinDelay = 1f,saveDelay = 60f, SpawnUpgradeCostIncrement = 100f;
    private const int defaultMaxOfflineTimeH = 1, defaultMaxOfflineTimeM = 0, 
        defaultMaxUpgradeCost = 100, defaultExpNeeded = 10;
    public const string Threek = "Threek", Cube = "Cube", Thrat = "Thrat", Cube2 = "Cube2", vijf = "5", zes = "6", zeven = "7";
    private const string FileName = "/PlayerData.json";
    Action<string> code;
    CommerFunctionsClassUnity helper;
    Progressbar Entetie_Bar;
    Progressbar Exp_Bar;


    private void Awake()
    {
        helper = GetComponent<CommerFunctionsClassUnity>();
        StartCoroutine(LoadingScreen());
        AdsDelay = 60f;
    }
    private void Start()
    {
        /*PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                // Continue with Play Games Services
                PlayGamesPlatform.Instance.RequestServerSideAccess(
                /* forceRefreshToken=  false,
                (string code) => {
                    //code to server
                });
            }
            else
            {
                // Disable your integration with Play Games Services or show a login button
                // to ask users to sign-in. Clicking it should call
                // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            }
        }*/

        
    }
    private void Update()
    {
        //checking if there are more enteties than allowed
        checkEnteties();
        if (!exceeded)
        {
            //timer that checks if it is time to spawn a entetie
            CheckForSpawn(spawnDelay);
        }
        //set the text that displays the exp
        CheckForExpUpdate();
        //checks if it is time for a on screen reward
        checkForReward();
        //checks if it is time for a ad
        CheckForAd();
    }

    //functions to inisiate the progressbars
    void InitEntetieBar()
    {
        GameObject obj = GameObject.Find("Progressbar_Enteties");
        Entetie_Bar = obj.GetComponent<Progressbar>();
        Entetie_Bar.minimum = 0;
        Entetie_Bar.maximum = maxEnteties;
    }
    void InitExpBar()
    {
        GameObject obj = GameObject.Find("RadialProgressBarExp");
        Exp_Bar = obj.GetComponent<Progressbar>();
    }

    //functions for loading and saving the game
    public IEnumerator LoadGame()
    {
        spawnedEnteties = new();
        loadGamePath = Application.persistentDataPath + FileName;
        if (!File.Exists(loadGamePath))
        {
            bugSquasher("No savegame");
            //no save found so starting with default values
            defaultValues();
        }
        else if (File.Exists(loadGamePath))
        {
            load(loadGamePath);
        }
        inits();
        yield return null;
    }
    void inits()
    {
        rewardDelay = 30f;
        StartCoroutine(helper.initAds());
        helper.CreateBannerAd();
        helper.ListenToAdEvents();
        helper.LoadAd();
        helper.LoadInterstitialAd();
        helper.RegisterEventHandlers(helper.interstitial);
        helper.RegisterReloadHandler(helper.interstitial);
        StartCoroutine(CoinsTimer(coinDelay));
        StartCoroutine(SaveTimer(saveDelay));
        InitEntetieBar();
        InitExpBar();
        bugSquasher("initialized ads");
        CheckDifDateTime();
        setOfflineTimeText();
        setSpawnTimeText();
        curEntetiesOnScreen = spawnedEnteties.Count;
    }
    void load(string path)
    {
        string loadPlayerData = File.ReadAllText(path);
        save = JsonUtility.FromJson<SaveManager>(loadPlayerData);
        current = save.coins;
        CoinsPerSecond = save.coinsPerSecond;
        spawnedEnteties = save.list;
        spawnDelay = save.timerspawn;
        exp = save.curExp;
        levelText.text = save.curLvl.ToString();
        expNeeded = save.nextLvl;
        lastDateTime = save.curDateTime;
        maxOfflineTimeH = save.maxOfflineTimeH;
        maxOfflineTimeM = save.maxOfflineTimeM;
        string maxTimeSpan = "00:" + maxOfflineTimeH.ToString("00") + ':' + maxOfflineTimeM.ToString("00");
        maxOffline = TimeSpan.Parse(maxTimeSpan);
        costOfflineTimeUpgrade.text = save.offlineCost.ToString();
        costSpawnTimerUpgrade.text = save.spawnCost.ToString();
        maxEnteties = save.maxEnt;
        maxUpgradeCost = save.maxCost;
        maxEntetiesUpgradeCostText.text = save.maxCost.ToString();
        actualMaxEnteties.text = save.maxEnt.ToString();

        int countIndex = spawnedEnteties.Count;
        Transform pos;
        for (int i = 0; i < countIndex; i++)
        {

            if (spawnedEnteties[i] == Threek)
            {
                pos = getSpawnPos(Enteties[0]);
                Instantiate(Enteties[0], pos);
            }
            else if (spawnedEnteties[i] == Cube)
            {
                pos = getSpawnPos(Enteties[1]);
                Instantiate(Enteties[1], pos);
            }
            else if (spawnedEnteties[i] == Thrat)
            {
                pos = getSpawnPos(Enteties[2]);
                Instantiate(Enteties[2], pos);
            }
            else if (spawnedEnteties[i] == Cube2)
            {
                pos = getSpawnPos(Enteties[3]);
                Instantiate(Enteties[3], pos);
            }
        }
        
    }
    public IEnumerator LoadingScreen()
    {
        if (Loading_Screen.alpha == 0)
        {
            Loading_Screen.alpha = 1;
        }
        
        yield return StartCoroutine(LoadGame());

        Loading_Screen.alpha = 0f;
        Loading_Screen.interactable = false;
        Loading_Screen.blocksRaycasts = false;

        yield return null;
    }
    public void bugSquasher(string linetoadd)
    {
        bool emptySpace = false;
        for (int i = 0; i < bugList.Count; i++)
        {
            if (bugList[i].text == "")
            {
                emptySpace = true;
                bugList[i].text = linetoadd;
                break;
            }
        }
        if (emptySpace == false)
        {
            bugList[4].text = bugList[3].text;
            bugList[3].text = bugList[2].text;
            bugList[2].text = bugList[1].text;
            bugList[1].text = bugList[0].text;
            bugList[0].text = linetoadd;
        }
    }
    public void SaveGame()
    {
        string currentDateTime = DateTime.Now.ToString();
        save = new()
        {
            list = spawnedEnteties,
            coins = current,
            coinsPerSecond = CoinsPerSecond,
            timerspawn = spawnDelay,
            totalSpawned = Spawned,
            curExp = exp,
            curLvl = level,
            nextLvl = expNeeded,
            curDateTime = currentDateTime,
            maxOfflineTimeH = maxOfflineTimeH,
            maxOfflineTimeM = maxOfflineTimeM,
            spawnCost = float.Parse(costSpawnTimerUpgrade.text),
            offlineCost = float.Parse(costOfflineTimeUpgrade.text),
            maxEnt = maxEnteties,
            maxCost = maxUpgradeCost
        };
        string saveFilePath = Application.persistentDataPath + FileName;
        string saveGameData = JsonUtility.ToJson(save);

        File.WriteAllText(saveFilePath, saveGameData);
        Debug.Log("saved game");
    }

    //misc start up functions
    void defaultValues()
    {
        Debug.Log("No Save");
        freeSpawnCreature();
        bugSquasher("free spawned a entetie");
        setDefaultSpawnDelay();
        bugSquasher("setted spawn delay");
        expNeeded = defaultExpNeeded;
        bugSquasher("set defauld exp");
        maxOfflineTimeH = defaultMaxOfflineTimeH;
        maxOfflineTimeM = defaultMaxOfflineTimeM;
        maxOffline = TimeSpan.Parse("00:01:00");
        bugSquasher("setted default max online time");
        setOfflineTimeText();
        setSpawnTimeText();
        bugSquasher("setted default spawn time");
        maxEnteties = 10;
        maxUpgradeCost = defaultMaxUpgradeCost;
        maxEntetiesUpgradeCostText.text = maxUpgradeCost.ToString();
        bugSquasher("end of setting defaults");
    }
    void CheckDifDateTime()
    {
        DateTime CurDateTime = DateTime.Now;
        DateTime last = DateTime.Parse(lastDateTime);
        DateTime.Compare(CurDateTime, last);

        TimeSpan diff = CurDateTime - last;
        
        string awayH = diff.ToString("hh");
        string awayM = diff.ToString("mm");
        int Hours = int.Parse(awayH);
        int Minutes = int.Parse(awayM);
        if  (diff < maxOffline)
        {
            float min = Hours * 60;
            min += Minutes;
            float sec = min * 60;
            current = CoinsPerSecond * sec;
            setCoins();
        }
        else if (diff >= maxOffline)
        {
            Hours = maxOfflineTimeH;
            Minutes = maxOfflineTimeM;
            float min = Hours * 60;
            min += Minutes;
            float sec = min * 60;
            current = CoinsPerSecond * sec;
            setCoins();
        }
        
    }
    IEnumerator CoinsTimer(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            setCoins();
        }
    }
    IEnumerator SaveTimer(float Delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(Delay);
            SaveGame();
        }
    }
    public void setDefaultSpawnDelay()
    {
        spawnDelay = 5f;
    }
    void setSpawnTimeText()
    {
        actualSpawnTime.text = spawnDelay.ToString("0.00") + "sec";
    }
    void setOfflineTimeText()
    {
        actualOfflineTime.text = maxOfflineTimeH.ToString("00") + 'H' + maxOfflineTimeM.ToString("00") + 'M';
    }

    //functions for in update()
    void CheckForExpUpdate()
    {
        Exp_Bar.current = exp;
        Exp_Bar.maximum = expNeeded;
    }
    void checkForReward()
    {
        timerReward += Time.deltaTime;
        if (timerReward >= rewardDelay)
        {
            
            rewardText = coinHolder.GetComponentInChildren<TextMeshProUGUI>();
            reward = getPercentCoins();
            rewardText.text = '+' + reward.ToString("0");
            Transform pos = getSpawnPos(coinHolder);
            coinHolder.transform.position = pos.position;
            coinHolder.SetActive(true);
            timerReward -= rewardDelay;
        }
    }
    void CheckForSpawn(float delay)
    {
        TimerEnt += Time.deltaTime;
        if (TimerEnt >= delay)
        {
            freeSpawnCreature();
            TimerEnt -= delay;
        }
    }
    void checkEnteties()
    {
        curEntetiesOnScreen = spawnedEnteties.Count;
        Entetie_Bar.current = curEntetiesOnScreen;
        currentEntetiesOnScreen.text = spawnedEnteties.Count.ToString() + '/' + maxEnteties;
        if (curEntetiesOnScreen >= maxEnteties)
        {
            exceeded = true;
        }
        else if(curEntetiesOnScreen < maxEnteties)
        {
            exceeded = false;
        }
    }
    public void freeSpawnCreature()
    {
        Transform pose = getSpawnPos(Enteties[0]);
        GameObject entetie = Enteties[0];
        getSpawned(Enteties[0].name);
        CoinsPerSecond += 0.05f;
        GameObject newentetie = Instantiate(entetie, pose);
    }
    void CheckForAd()
    {
        TimerAds += Time.deltaTime;

        if(TimerAds >= AdsDelay)
        {
            helper.ShowInterstitialAd();
            TimerAds -= AdsDelay;
        }
    }

    //functions to make other functions work
    float getPercentCoins()
    {
        int amount = UnityEngine.Random.Range(1, 100);
        float perc = current / 100 * amount;
        return perc;
    }
    public Transform getSpawnPos(GameObject ent)
    {
        int height = Screen.height;
        int width = Screen.width;

        int y = UnityEngine.Random.Range(0, height);
        int x = UnityEngine.Random.Range(0, width);

        ent.transform.position = new Vector3(x, y, 0);

        Transform newPos = ent.transform;

        

        return newPos;
    }
    public void getSpawned(string spawned)
    {
        spawnedEnteties.Add(spawned);
        Spawned++;
        if (spawned == Threek)
        {
            CoinsPerSecond += 0.05f;
        }

        if (spawned == Cube)
        {
            spawnedEnteties.Remove(Threek);
            spawnedEnteties.Remove(Threek);
            CoinsPerSecond -= 0.1f;
            CoinsPerSecond += 0.15f;
            
        }

        if (spawned == Thrat)
        {
            spawnedEnteties.Remove(Cube);
            spawnedEnteties.Remove(Cube);
            CoinsPerSecond -= 0.3f;
            CoinsPerSecond += 0.4f;
             
        }

        if (spawned == Cube2)
        {
            spawnedEnteties.Remove(Thrat);
            spawnedEnteties.Remove(Thrat);
            CoinsPerSecond -= 0.8f;
            CoinsPerSecond += 1f;
        }
        
        if (spawned == vijf)
        {
            spawnedEnteties.Remove(Cube2);
            spawnedEnteties.Remove(Cube2);
            
            CoinsPerSecond -= 2f;
            CoinsPerSecond += 2.3f;
        }
        
        if (spawned == zes)
        {
            spawnedEnteties.Remove(vijf);
            spawnedEnteties.Remove(vijf);
            CoinsPerSecond -= 4.6f;
            CoinsPerSecond += 5f;
        }
        
        if (spawned == zeven)
        {
            spawnedEnteties.Remove(zes);
            spawnedEnteties.Remove(zes);
            CoinsPerSecond -= 9.2f;
            CoinsPerSecond += 10;
        }
    }
    public void setCoins()
    {
        
        current += CoinsPerSecond;
        if (current > 999)
        {
            float temp = current / 1000;
            coinText.text = temp.ToString("0.00") + "k ";
            
        }
        else if(current < 1000)
        {
            coinText.text = current.ToString("0.00");
        }
        coinPerSecText.text = CoinsPerSecond.ToString("0.0") + "ps"; 

    }
    void levelUp()
    {
        if (expNeeded <= exp)
        {
            level = int.Parse(levelText.text);
            level++;
            current += level * 50;
            levelText.text = level.ToString();
            Exp_Bar.minimum = exp;
            expNeeded += expNeeded * 2;
        }
        else
        {
            //nolevelup yet
        }
    }
    public void addExp(int amount)
    {
        exp += amount;
        levelUp();
    }

    //functions for on buttons
    public void openStore()
    {
        Time.timeScale = 0;
        Upgrades_Screen.alpha = 1f;
        Upgrades_Screen.interactable = true;
        Upgrades_Screen.blocksRaycasts = true;
    }
    public void closeStore()
    {
        Upgrades_Screen.alpha = 0f;
        Upgrades_Screen.interactable = false;
        Upgrades_Screen.blocksRaycasts = false;
        Time.timeScale = 1;
    }
    public void SpawnUpgrade()
    {
        float cost = float.Parse(costSpawnTimerUpgrade.text);
        if (cost <= current)
        {
            //buy
            spawnDelay -= 0.1f;
            current -= cost;
            cost += SpawnUpgradeCostIncrement;
            costSpawnTimerUpgrade.text = cost.ToString();
            setSpawnTimeText();
            setCoins();
            SaveGame();
        }
        else
        {
            //nobuy
        }
        
    }
    public void OfflineUpgrade()
    {
        float cost = float.Parse(costOfflineTimeUpgrade.text);
        if (cost <= current)
        {
            //buy
            if (maxOfflineTimeM < 59)
            {
                maxOfflineTimeM ++;
            }
            else if(maxOfflineTimeM >= 59)
            {
                maxOfflineTimeM = 0;
                maxOfflineTimeH++;
            }
            current -= cost;
            cost += 120f;
            costOfflineTimeUpgrade.text = cost.ToString();
            setOfflineTimeText();
            SaveGame();
            setCoins();
        }
        else
        {
            //nobuy
        }
        
    }
    public void maxEntetieUpgrade()
    {
        int cost = maxUpgradeCost;
        if (current >= cost)
        {
            maxEnteties++;
            Entetie_Bar.maximum = maxEnteties;
            current -= cost;
            setCoins();
            actualMaxEnteties.text = maxEnteties.ToString();
            maxUpgradeCost += 140;
            maxEntetiesUpgradeCostText.text = maxUpgradeCost.ToString();
            SaveGame();
        }
        
    }
    public void rewardOnScreen()
    {
        current += reward;
        rewardDelay = UnityEngine.Random.Range(60f, 300f);
        coinHolder.SetActive(false);
    }
}
