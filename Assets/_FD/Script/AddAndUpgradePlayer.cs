using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddAndUpgradePlayer : MonoBehaviour, IGetTouchEvent, IKeyboardCall
{
    public void KeyDown(KeyCode keyCode)
    {
        TouchEvent();
    }
    public KeyCode[] KeyType { get { return new KeyCode[] { Key }; } }
    public int KeyObjectID { get { return gameObject.GetInstanceID(); } }
    public KeyCode Key;
    public int beginPlayer = 0;
    public GameObject addIcon, upgradeIcon;
    public GameObject upgradeFX;
    List<int> prices = new List<int>();
    //public int[] prices;
    public Player_Archer[] Players;

    int currentPlayer = -1;

    public Player_Archer GetcurrentPlayer
    {
        get { return currentPlayer >= 0 ? Players[currentPlayer] : Players[0]; }
    }
    // Start is called before the first frame update
    void Start()
    {
        //for(int i = 0; i < Players.Length; i++)
        //{
        //    prices.Add(Players[i].upgradedCharacterID.price);
        //}

        if (Players.Length <= 0)
        {
            Debug.LogError("No player in " + gameObject.name);
            enabled = false;
            return;
        }


        if (beginPlayer > 0)
        {
            currentPlayer = beginPlayer;
            SetPlayer();
        }
        InvokeRepeating("CheckStatus", 0, 0.2f);
    }

    private void CheckStatus()
    {
        addIcon.SetActive(Players[0].upgradedCharacterID.price <= GameManager.Instance.currentExp && currentPlayer == -1);
        upgradeIcon.SetActive((currentPlayer + 1 < Players.Length)
            && (Players[currentPlayer + 1].upgradedCharacterID.price <= GameManager.Instance.currentExp)
            && currentPlayer > -1);
    }

    void SetPlayer()
    {
        foreach (var player in Players)
        {
            player.gameObject.SetActive(false);
        }

        Players[currentPlayer].gameObject.SetActive(true);
        if (upgradeFX)
            SpawnSystemHelper.GetNextObject(upgradeFX, true).transform.position = transform.position;
    }

    public void TouchEvent()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing)
            return;

        if (currentPlayer + 1 < Players.Length && (addIcon.activeInHierarchy || upgradeIcon.activeInHierarchy))
        {
            currentPlayer++;
            GameManager.Instance.currentExp -= Players[currentPlayer].upgradedCharacterID.price;
            SetPlayer();
            SoundManager.PlaySfx(currentPlayer == 0 ? SoundManager.Instance.soundAddArcher : SoundManager.Instance.soundUpgradeArcher);
        }
        if (GetComponent<TutorialFinder>())
        {
            GetComponent<TutorialFinder>().InitiateTutorialClick();
        }
    }
}
