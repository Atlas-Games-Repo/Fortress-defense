using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour, IListener {
	public enum Type{Clk, CClk}
	public Type rotateType;
	public float speed = 0.5f;
	
	// Update is called once per frame
	void Update () {
        if (isStop)
            return;

        transform.Rotate (Vector3.forward, Mathf.Abs (speed) * (rotateType == Type.CClk ? 1 : -1));
	}

    bool isStop = false;
    #region IListener implementation

    public void IPlay()
    {
        //		throw new System.NotImplementedException ();
    }

    public bool IEnabled() {
        return this.enabled;
    }
    public void ISuccess()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IPause()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IUnPause()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IGameOver()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IOnRespawn()
    {
        //		throw new System.NotImplementedException ();
    }

    public void IOnStopMovingOn()
    {
        Debug.Log("IOnStopMovingOn");
        //		anim.enabled = false;
        isStop = true;

    }

    public void IOnStopMovingOff()
    {
        //		anim.enabled = true;
        isStop = false;
    }

    #endregion
}
