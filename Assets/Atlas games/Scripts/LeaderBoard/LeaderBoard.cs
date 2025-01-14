using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mopsicus.InfiniteScroll;
using Newtonsoft.Json;
using Spine;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class LeaderBoard : MonoBehaviour
{
    public GameObject errorText;
    public GameObject loadingText;
    public int items_height = 150;
    public LeaderboardListItem self_object;
    public LeaderboardList listOfUsers;
    public  InfiniteScroll scroll;
    private LeaderBoardResponseModel _leaderboardResponse;
    public bool in_game_mode = false;
    private void OnEnable() {
        if (in_game_mode) {
            GetData();
            OnSubmitData(User.UserProfile);
        }
    }
    private void OnSubmitData(UserResponse user_data) {
        if (!self_object) return;
        LeaderboardData data = new LeaderboardData(){
            user_name = user_data.display_name,
            rank = user_data.rank,
            points = user_data.points,
        };
        self_object.SetData(data,false);
    }


    public void GetData()
    {
        scroll.RefreshViews();
        scroll.gameObject.SetActive(false);
        errorText.SetActive(false);
        loadingText.SetActive(true);
        Get_user();
    }

     int OnHeightItem(int index)
    {
        return items_height;
    }

     void OnFillItem(int index, GameObject item)
    {
        // This cach system dosn't actually work but due to short time left to release we ignore this for now
       LeaderboardData data = listOfUsers.data[index]; 
        item.GetComponent<LeaderboardListItem>().SetData(data);
    }

    public void ClearList()
    {
        scroll.RefreshViews();
    }
    public async void Get_user()
    {
        if (listOfUsers.data.Length > 0)ShowData();
        _leaderboardResponse = await APIManager.instance.Get_leader_board();
        listOfUsers.data = _leaderboardResponse.results;
        ShowData();
    }

    void ShowData()
    {
        loadingText.SetActive(false);
        scroll.gameObject.SetActive(true);
        scroll.OnFill += OnFillItem;
        scroll.OnHeight += OnHeightItem;
        scroll.InitData(listOfUsers.data.Length);
    }
}
