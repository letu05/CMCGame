using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public UnityEvent onPause  = new UnityEvent();
    public UnityEvent onResume = new UnityEvent();

    public bool IsPaused { get; private set; } = false;

    // ─── Singleton per-scene ──────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance       = this;
        IsPaused       = false;
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        if (IsPaused) Time.timeScale = 1f;
        if (Instance == this) Instance = null;
    }

    // ─── Public API ───────────────────────────────────────────────────────────

    public void Pause()
    {
        if (IsPaused) return;

        IsPaused       = true;
        Time.timeScale = 0f;
        onPause.Invoke();
    }

    public void Resume()
    {
        bool wasPaused = IsPaused;
        IsPaused       = false;
        Time.timeScale = 1f;

        if (wasPaused) onResume.Invoke();
    }

    public void TogglePause()
    {
        if (IsPaused) Resume();
        else          Pause();
    }
}
