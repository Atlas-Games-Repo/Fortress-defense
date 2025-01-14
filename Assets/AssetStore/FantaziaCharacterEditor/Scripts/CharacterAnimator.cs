using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;
using System;

public class CharacterAnimator : MonoBehaviour
{
    //Script responsible for executing the animations of the character

    //SkeletonAnimation is the class that has all the information of the spine animation
    SkeletonAnimation characterSkeleton;

    [HideInInspector]
    public Jobs MyJob;

    Dictionary<PlayerAnimations, string> WarriorAnimations = new Dictionary<PlayerAnimations, string>();
    Dictionary<PlayerAnimations, string> ArcherAnimations = new Dictionary<PlayerAnimations, string>();
    Dictionary<PlayerAnimations, string> ElementalistAnimations = new Dictionary<PlayerAnimations, string>();
    Dictionary<PlayerAnimations, string> DuelistAnimations = new Dictionary<PlayerAnimations, string>();
    Dictionary<Jobs, Dictionary<PlayerAnimations, string>> JobsAnimations = new Dictionary<Jobs, Dictionary<PlayerAnimations, string>>();

    PlayerAnimations AnimationToPlay;
    GearEquipper AccGE;

    public GameObject ArrowPrefab;
    bool IsFireArrow=true;

    void Awake()
    {
        //Assigns characterAnimator to the first child of this transform. Change it to your animation gameobject if you dont want it to be the first transform
        characterSkeleton = transform.GetChild(0).GetComponent<SkeletonAnimation>();

        //Access the class responsible for equipping gears.
        AccGE = GetComponent<GearEquipper>();

        CreateAnimationsDictionary();
    }

    private void Start()
    {
        //Subscribe the event of the animation to a function called OnEventAnimation
        characterSkeleton.AnimationState.Event += OnEventAnimation;
    }

    void OnEventAnimation(TrackEntry trackEntry, Spine.Event e)
    {
        //This function only contains one event ("OnArrowLeftBow"). You can add whatever event you want the is available in the asset. Read the documentation provided to know the available events
        if (e.Data.Name == "OnArrowLeftBow")
        {
            //Shoots a real arrow when the arrow in the animation leaves the bow

            if (IsFireArrow == false)
                return;

            Vector3 ArrowStartingPosition = Vector3.zero;
            float Angle = 0;
            if (trackEntry.Animation.ToString() == "Shoot1")
            {
                ArrowStartingPosition = transform.Find("ArrowsFirePoints").Find("FirePoint_Shoot1").position;
            }
            else if (trackEntry.Animation.ToString() == "Shoot2")
            {
                ArrowStartingPosition = transform.Find("ArrowsFirePoints").Find("FirePoint_Shoot2").position;
            }
            else if (trackEntry.Animation.ToString() == "Shoot3")
            {
                ArrowStartingPosition = transform.Find("ArrowsFirePoints").Find("FirePoint_Shoot3").position;
                Angle = 50;
            }
            GameObject newArrow = Instantiate(ArrowPrefab, ArrowStartingPosition, Quaternion.Euler(0, 0, 90 + Angle));
            newArrow.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(Mathf.Cos(Angle*Mathf.PI/180), Mathf.Sin(Angle *  Mathf.PI/180)) *3200;
        }

    }

    public void FireArrow_Archer()
    {
        IsFireArrow = !IsFireArrow;
    }
    

    void CreateAnimationsDictionary()
    { 
        //Fills the dicitionaries with all the animations of each job

        WarriorAnimations.Add(PlayerAnimations.Attack1, "Attack1");
        WarriorAnimations.Add(PlayerAnimations.Attack2, "Attack2");
        WarriorAnimations.Add(PlayerAnimations.Idle, "Idle");
        WarriorAnimations.Add(PlayerAnimations.Walk, "Walk");
        WarriorAnimations.Add(PlayerAnimations.Run, "Run");
        WarriorAnimations.Add(PlayerAnimations.FullJump, "Jump");
        WarriorAnimations.Add(PlayerAnimations.Jump1, "Jump1");
        WarriorAnimations.Add(PlayerAnimations.Jump2, "Jump2");
        WarriorAnimations.Add(PlayerAnimations.Jump3, "Jump3");
        WarriorAnimations.Add(PlayerAnimations.Buff, "Buff");
        WarriorAnimations.Add(PlayerAnimations.Hurt, "Hurt");
        WarriorAnimations.Add(PlayerAnimations.Special, "Defence");
        WarriorAnimations.Add(PlayerAnimations.Death, "Death");

        ArcherAnimations.Add(PlayerAnimations.Attack1, "Shoot1");
        ArcherAnimations.Add(PlayerAnimations.Attack2, "Shoot2");
        ArcherAnimations.Add(PlayerAnimations.Idle, "Idle ARCHER");
        ArcherAnimations.Add(PlayerAnimations.Walk, "Walk");
        ArcherAnimations.Add(PlayerAnimations.Run, "Run ARCHER");
        ArcherAnimations.Add(PlayerAnimations.FullJump, "Jump");
        ArcherAnimations.Add(PlayerAnimations.Jump1, "Jump1 ARCHER");
        ArcherAnimations.Add(PlayerAnimations.Jump2, "Jump2");
        ArcherAnimations.Add(PlayerAnimations.Jump3, "Jump3 ARCHER");
        ArcherAnimations.Add(PlayerAnimations.Buff, "Buff");
        ArcherAnimations.Add(PlayerAnimations.Hurt, "Hurt");
        ArcherAnimations.Add(PlayerAnimations.Special, "Shoot3");
        ArcherAnimations.Add(PlayerAnimations.Death, "Death");

        ElementalistAnimations.Add(PlayerAnimations.Attack1, "Cast1");
        ElementalistAnimations.Add(PlayerAnimations.Attack2, "Cast2");
        ElementalistAnimations.Add(PlayerAnimations.Idle, "Idle");
        ElementalistAnimations.Add(PlayerAnimations.Walk, "Walk");
        ElementalistAnimations.Add(PlayerAnimations.Run, "Fly");
        ElementalistAnimations.Add(PlayerAnimations.FullJump, "Jump");
        ElementalistAnimations.Add(PlayerAnimations.Jump1, "Jump1");
        ElementalistAnimations.Add(PlayerAnimations.Jump2, "Jump2");
        ElementalistAnimations.Add(PlayerAnimations.Jump3, "Jump3");
        ElementalistAnimations.Add(PlayerAnimations.Buff, "Buff");
        ElementalistAnimations.Add(PlayerAnimations.Hurt, "Hurt");
        ElementalistAnimations.Add(PlayerAnimations.Special, "Cast3");
        ElementalistAnimations.Add(PlayerAnimations.Death, "Death");

        DuelistAnimations.Add(PlayerAnimations.Attack1, "Attack 1 DUELIST");
        DuelistAnimations.Add(PlayerAnimations.Attack2, "Attack 2 DUELIST");
        DuelistAnimations.Add(PlayerAnimations.Idle, "Idle");
        DuelistAnimations.Add(PlayerAnimations.Walk, "Walk");
        DuelistAnimations.Add(PlayerAnimations.Run, "Run DUELIST");
        DuelistAnimations.Add(PlayerAnimations.FullJump, "Jump");
        DuelistAnimations.Add(PlayerAnimations.Jump1, "Jump1");
        DuelistAnimations.Add(PlayerAnimations.Jump2, "Jump2");
        DuelistAnimations.Add(PlayerAnimations.Jump3, "Jump3");
        DuelistAnimations.Add(PlayerAnimations.Buff, "Buff");
        DuelistAnimations.Add(PlayerAnimations.Hurt, "Hurt");
        DuelistAnimations.Add(PlayerAnimations.Special, "Attack 3 DUELIST");
        DuelistAnimations.Add(PlayerAnimations.Death, "Death");

        JobsAnimations.Add(Jobs.Archer, ArcherAnimations);
        JobsAnimations.Add(Jobs.Warrior, WarriorAnimations);
        JobsAnimations.Add(Jobs.Elementalist, ElementalistAnimations);
        JobsAnimations.Add(Jobs.Duelist, DuelistAnimations);
    }

    //Function called when the job is changed 
    public void JobChanged(Jobs NewJob)
    {
        Jobs OldJob = MyJob;
        MyJob = NewJob;
        if (JobsAnimations[MyJob][AnimationToPlay] != JobsAnimations[OldJob][AnimationToPlay])
        {
            AnimationManager();
        }

    }

    //Takes a string AnimationString which is the name of the animation and assigns it to AnimationToPlay
    public void ChangeAnimation(string AnimationString)
    {
        AnimationToPlay = (PlayerAnimations)Enum.Parse(typeof(PlayerAnimations), AnimationString);
        AnimationManager();
    }

    //Runs the required animation using SetAnimation spine function
    void AnimationManager()
    {
        bool IsLoop = AnimationToPlay == PlayerAnimations.Death ? false : true;
        characterSkeleton.AnimationState.SetAnimation(0, JobsAnimations[AccGE.Job][AnimationToPlay], IsLoop);
    }

}
public enum PlayerAnimations
{
    Idle, Walk, Attack1, Death, FullJump,Jump1, Jump2, Jump3, Hurt, Run, Attack2, Special, Buff
}
public enum Jobs
{
    Warrior, Archer, Elementalist, Duelist
}
