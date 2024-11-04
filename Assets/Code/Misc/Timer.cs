using System;

public class Timer
{
    public float Duration { get; private set; }
    public bool IsStarted { get; private set; }
    public bool IsElapsed { get; private set; }
    public float Passed { get; private set; }

    public event Action<Timer> Elapsed;
    public event Action<Timer> Changed;

    public void Start(float timer)
    {
        if (timer <= 0)
        {
            Logger.LogWarning($"[Timer] started with wrong parameters: {timer}");
            timer = Constants.EPSILON;
        }

        Duration = timer;
        Passed = 0;
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
