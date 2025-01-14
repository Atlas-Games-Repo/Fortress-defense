using UnityEngine;
using System.Collections;

public interface IListener  {
	void IPlay();
	void ISuccess();
	void IPause ();
	void IUnPause();
	void IGameOver ();
	void IOnRespawn ();
	void IOnStopMovingOn ();
	void IOnStopMovingOff ();
	bool IEnabled();
}
