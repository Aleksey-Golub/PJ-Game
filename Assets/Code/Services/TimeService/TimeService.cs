using UnityEngine;

namespace Code.Services
{
    public class TimeService : ITimeService
    {
        private bool _stopped;
        private float _savedTimeScale;

        public void StopTime()
        {
            Logger.Log($"[TimeService] StopTime()");

            if (_stopped)
            {
                Logger.LogWarning($"[TimeService] trying to stop when already stopped");
                return;
            }

            _stopped = true;
            _savedTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }

        public void ResumeTime()
        {
            Logger.Log($"[TimeService] ResumeTime()");

            if (!_stopped)
            {
                Logger.LogWarning($"[TimeService] trying to resume when not stopped");
                return;
            }

            _stopped = false;
            Time.timeScale = _savedTimeScale;
        }
    }
}
