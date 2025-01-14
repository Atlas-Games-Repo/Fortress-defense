using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LeaderboardListItem : MonoBehaviour
{
    public TMP_Text userNameText;
    public TMP_Text rxpText;
    public TMP_Text rank_text;
    public GameObject Number_one_rank;
    public Image image;
    // this function sets the data recived from leaderboard.cs to the list item component 
    public async void SetData(LeaderboardData data, bool show_avatar = true)
    {
        if (data.rank == 1 && Number_one_rank) Number_one_rank.SetActive(true);
        rxpText.text = data.points.ToString();
        rank_text.text = data.rank.ToString();
        userNameText.text = data.user_name;
        if (!show_avatar && !image) {
            return;
        };
        image.sprite = await APIManager.instance.Get_rofile_picture(data.user_avatar);
    }
}
