using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Rlc.Cron;
using System.Collections.Generic;

namespace U3D.Cron
{
    // Contains an adaptation of Rlc.Cron available at: http://blog.bobcravens.com/2009/10/an-event-based-cron-scheduled-job-in-c/
    public class CronScheduler : MonoBehaviour
    {
        public string config = "* * * * *";
        public bool sendEventsToMainThread = true;

        public UnityEvent onTrigger;

        CronObject m_cronObject;
        void OnEnable()
        {
            if (m_cronObject == null)
            {
                m_cronObject = new CronObject(CronSchedule.Parse(config));
                m_cronObject.OnCronTrigger += OnCronTrigger;
            }
            m_cronObject.Start();
        }
        void OnDisable()
        {
            m_cronObject.Stop();
        }
        void OnCronTrigger()
        {
            if (sendEventsToMainThread)
            {
                m_raiseEvent = true;
            }
            else
            {
                onTrigger.Invoke();
            }
        }
        bool m_raiseEvent = false;
        void Update()
        {
            if (m_raiseEvent)
            {
                m_raiseEvent = false;
                onTrigger.Invoke();
            }
        }
    }
}