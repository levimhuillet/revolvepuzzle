using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {
    public static EventManager instance;

    // Define game events below
    public static UnityEvent OnEnterFrost;
    public static UnityEvent OnEnterFlame;
    public static UnityEvent OnExitRealms;
    public static UnityEvent OnInteractEnter;
    public static UnityEvent OnInteractExit;

    private void OnEnable() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (this != instance) {
            Destroy(this.gameObject);
        }

        OnEnterFrost = new UnityEvent();
        OnEnterFlame = new UnityEvent();
        OnExitRealms = new UnityEvent();
        OnInteractEnter = new UnityEvent();
        OnInteractExit = new UnityEvent();
    }
}
