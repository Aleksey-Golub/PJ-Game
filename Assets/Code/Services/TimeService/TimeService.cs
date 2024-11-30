using UnityEngine;

namespace Code.Services
{
    public class TimeService : ITimeService
    {
        private float _savedTimeScale;

        public void StopTime()
        {
            Logger.Log($"[TimeService] StopTime()");

            _savedTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        public void ResumeTime()
        {
            Logger.Log($"[TimeService] ResumeTime()");

            Time.timeScale = _savedTimeScale;
        }
    }
}
