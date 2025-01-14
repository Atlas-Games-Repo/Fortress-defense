using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public enum DialogAction
{
    Next,Previous,Close
}
public class Dialog : MonoBehaviour
{
    Animator _animator;
    
    [Header("UI Elements")]
    public VideoPlayer videoPlayer;
    public Image dialogImage;
    public TMP_Text dialogText;
    public TMP_Text dialogTitle;
    public Image dialogImageNext;
    public TMP_Text dialogTextNext;
    public VideoPlayer videoPlayerNext;
    public Button previousButton;
    public Animator buttonsAnimator;
    public HorizontalLayoutGroup layoutGroup;
    public HorizontalLayoutGroup nextLayoutGroup;
    public GameObject pictureContentHolder;
    public GameObject nextPictureContentHolder;
    public int textDialogPadding = 25;
    
    private int _initialPaddingRight;
    private int _initialPaddingLeft;
    [HideInInspector]public bool isOpen;
    private Tip _currentTip;
    private int _dialogStep;
    private TutorialNew _tutorial;
    private Vector2 _nextImageOriginalSize;
    private Vector2 _imageOriginalSize;
    private Vector2 _nextVideoOriginalSize;
    private Vector2 _videoOriginalSize;
    private Vector2 _originalTextSize;
    private Vector2 _originalNextTextSize;
    private bool _isInit;
    public void OnAnimationFinish()
    {
        dialogText.text = _currentTip.tipText;
        dialogTitle.text = "Tip #" + (_dialogStep).ToString();
        videoPlayer.gameObject.SetActive(false);
        dialogImage.gameObject.SetActive(false);
        layoutGroup.padding.left = _initialPaddingLeft;
        layoutGroup.padding.right = _initialPaddingRight;
        dialogText.alignment = TextAlignmentOptions.Left;
        pictureContentHolder.SetActive(true);
        layoutGroup.childControlWidth = false;

        switch (_currentTip.dialogContentType)
        {
            case DialogContent.Text: 
                layoutGroup.padding.left = textDialogPadding;
                layoutGroup.padding.right = textDialogPadding;
                layoutGroup.childControlWidth = true;
                pictureContentHolder.SetActive(false);
                dialogText.alignment = TextAlignmentOptions.Center;
                break;
            case DialogContent.Image:
                dialogImage.gameObject.SetActive(true);
                dialogImage.sprite = _currentTip.dialogImage;
                ResizeImage(dialogImage.rectTransform,_imageOriginalSize);
                break;
            case DialogContent.Video:
                videoPlayer.gameObject.SetActive(true);
                videoPlayer.clip = _currentTip.dialogVideo;
                ResizeImage(videoPlayer.GetComponent<RawImage>().rectTransform, _videoOriginalSize);
                break;
        }
    }

    public void DialogChange(Tip tip,DialogAction action,Tip currentTip,TutorialNew tutorial)
    {
        if (!_isInit)
        {
            _isInit = true;
            _animator = GetComponent<Animator>();
            _originalTextSize = dialogText.rectTransform.sizeDelta;
            _originalNextTextSize = dialogTextNext.rectTransform.sizeDelta;
            _initialPaddingLeft = layoutGroup.padding.left;
            _initialPaddingRight = layoutGroup.padding.right;
            _imageOriginalSize = dialogImage.rectTransform.sizeDelta;
            _nextImageOriginalSize = dialogImageNext.rectTransform.sizeDelta;
        }
        _tutorial = tutorial;
        _currentTip = currentTip;
        if (action != DialogAction.Close)
        {
            videoPlayerNext.gameObject.SetActive(false);
            dialogImageNext.gameObject.SetActive(false);
            nextLayoutGroup.childControlWidth = false;
            nextLayoutGroup.padding.left = _initialPaddingLeft;
            nextLayoutGroup.padding.right = _initialPaddingRight;
            dialogText.alignment = TextAlignmentOptions.Left;
            nextPictureContentHolder.SetActive(true);
            dialogTextNext.text = currentTip.tipText;

            switch (tip.dialogContentType)
            {
                case DialogContent.Text:
                    nextLayoutGroup.padding.left = textDialogPadding;
                    nextLayoutGroup.padding.right = textDialogPadding;
                    nextLayoutGroup.childControlWidth = true;
                    dialogTextNext.alignment = TextAlignmentOptions.Center;
                    nextPictureContentHolder.SetActive(false);
                    break;
                case DialogContent.Image:
                    dialogImageNext.gameObject.SetActive(true);
                    dialogImageNext.sprite = tip.dialogImage;
                    ResizeImage(dialogImageNext.rectTransform,_nextImageOriginalSize);
                    break;
                case DialogContent.Video:
                    videoPlayerNext.gameObject.SetActive(true);
                    videoPlayerNext.clip = tip.dialogVideo;
                    videoPlayerNext.isLooping = true;
                    videoPlayerNext.Play();
                    ResizeImage(videoPlayerNext.GetComponent<RawImage>().rectTransform,_nextImageOriginalSize);
                    break;
            }
        }
        else
        {
            _animator.SetTrigger("Close");
            previousButton.interactable = false;
            isOpen = false;
            _dialogStep = 0;
        }

        switch (action)
        {
            case DialogAction.Next:
                _dialogStep++;
                dialogTitle.text = "Tip #" + (_dialogStep).ToString();
                if (!isOpen)
                {
                    dialogText.text = _currentTip.tipText;
                    if (_currentTip.dialogContentType == DialogContent.Image)
                    {
                        dialogImage.gameObject.SetActive(true);
                        dialogImage.sprite = _currentTip.dialogImage;
                        videoPlayer.gameObject.SetActive(false);
                        
                    }else if (_currentTip.dialogContentType == DialogContent.Video)
                    {
                        dialogImage.gameObject.SetActive(false);
                        videoPlayer.gameObject.SetActive(true);
                        videoPlayer.clip = _currentTip.dialogVideo;
                        videoPlayer.isLooping = true;
                        videoPlayer.Play();
                    }
                    else
                    {
                        videoPlayer.gameObject.SetActive(false);
                        dialogImage.gameObject.SetActive(false);
                        layoutGroup.padding.left = textDialogPadding;
                        layoutGroup.padding.right = textDialogPadding;
                        layoutGroup.childControlWidth = true;
                        dialogText.alignment = TextAlignmentOptions.Center;
                        dialogText.text = currentTip.tipText;
                   pictureContentHolder.SetActive(false);
                    }
               previousButton.interactable = false;
                 _animator.SetTrigger("Open");
                 isOpen = true;
                }
                else if(_dialogStep>0)
                {
                    _animator.SetTrigger("Next");
                    previousButton.interactable = true;
                }
                break;
            case DialogAction.Previous:
                _dialogStep--;
                if (_dialogStep <2 )
                {
                    buttonsAnimator.SetTrigger("OneButton");
                    previousButton.interactable = false;
                }
                dialogTitle.text = "Tip #" + (_dialogStep + 1).ToString();
                _animator.SetTrigger("Previous");
               
                break;
            case DialogAction.Close:
                break;
        }
  
    }

    private bool isLast = false;
    public void OnFinishCloseAnimation()
    {
        if (isLast)
        {
            _tutorial.NextStep();
        }
    }
    public void NextStep()
    {
        if ( _tutorial.tutorialSteps.Count -1 > _tutorial.tipOrder )
        {
            if (_tutorial.tutorialSteps[_tutorial.tipOrder].tipType == TipType.Dialog )
            {
                buttonsAnimator.SetTrigger("TwoButton");
                _tutorial.NextStep();
            }
        }
        else
        {
            isLast = true;
            _animator.SetTrigger("Close");
            Time.timeScale = 1;
        }
        previousButton.interactable = true;
    }

    public void PreviousStep()
    {
       
        _tutorial.PreviousStep();
    }

    void ResizeImage(RectTransform itemImage,Vector2 originalSize)
    {
        if (itemImage.GetComponent<Image>())
        {
            itemImage.GetComponent<Image>().SetNativeSize();
        }else if (itemImage.GetComponent<RawImage>())
        {
            itemImage.GetComponent<RawImage>().SetNativeSize();
        }
    
        if (itemImage.sizeDelta.x > itemImage.sizeDelta.y)
        {
            float aspectRatio = (float)itemImage.sizeDelta.x / itemImage.sizeDelta.y;
            itemImage.sizeDelta = new Vector2(originalSize.x, originalSize.y / aspectRatio);
        }
        else
        {
            float aspectRatio = (float)itemImage.sizeDelta.y / itemImage.sizeDelta.x;
            itemImage.sizeDelta = new Vector2(originalSize.x / aspectRatio, originalSize.y);
        }
    }
}

