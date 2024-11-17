using System;

public class Timer
{
    public float Duration { get; private set; }
    public bool IsStarted { get; private set; }
    public bool IsElapsed { get; private set; }
    public float Passed { get; private set; }

    public event Action<Timer> Elapsed;
    public event Action<Timer> Changed;

    public Timer(bool isElapsed = false)
    {
        IsElapsed = isElapsed;
    }

    public void Start(float timer)
    {
        StartAsPartialPassed(timer, 0);
    }

    public void StartAsPartialPassed(float timer, float timerPassed)
    {
        if (timer <= 0)
        {
            Logger.LogWarning($"[Timer] started with wrong parameters: {timer}");
            timer = Constants.EPSILON;
        }

        Duration = timer;
        Passed = timerPassed;
        IsElapsed = false;

        IsStarted = true;
    }

    public void OnUpdate(float deltaTime)
    {
        if (!IsStarted)
            return;

        Passed += deltaTime;
        Changed?.Invoke(this);

        if (Passed >= Duration)
        {
            IsElapsed = true;
            IsStarted = false;

            Elapsed?.Invoke(this);
        }
    }
}
