using UnityEngine;
using UnityEngine.Events;
using Rlc.Cron;

public class ScheduleManager : MonoBehaviour
{
    public bool m_manualControl = false;
    public int m_cronScheduleMin = 10;

    [Space]
    public UnityEvent onTrigger;

    CronObject m_cronObject;
    bool m_raiseEvent = false;
    int m_lastEditMin = -1;

    void OnDisable()
    {
        m_cronObject?.Stop();
    }

    void Update()
    {
        if (m_raiseEvent)
        {
            m_raiseEvent = false;
            if (m_manualControl == false)
                onTrigger.Invoke();
        }

        try
        {
            if (m_cronScheduleMin == m_lastEditMin) return;

            var config = $"*/{m_cronScheduleMin} * * * *";

            m_cronObject?.Stop();
            m_cronObject = new CronObject(CronSchedule.Parse(config));
            m_cronObject.OnCronTrigger += () => m_raiseEvent = true;
            m_cronObject.Start();

            m_lastEditMin = m_cronScheduleMin;
            Debug.Log("New Cron Schedule");
        }
        catch
        {
            Debug.Log("Cron Invalid expresion");
        }
    }
}
