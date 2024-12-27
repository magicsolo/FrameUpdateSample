using System;
using System.Collections.Generic;
using CenterBase;

namespace Game
{
    public static class EventKeys
    {
        public static string LogicMatchUpdate = "LogicMatchUpdate";
        public static string OnOffLine = "OnOffLine";
    }
    
    public class EventRegister
    {
        private Dictionary<String, Action<object>> eventHandlers = new Dictionary<string, Action<object>>();

        public void AddRegister(string eventName, Action<object> handler)
        {
            eventHandlers[eventName] = handler;
        }

        public void RegistAll()
        {
            foreach (var kv in eventHandlers)
            {
                EventManager.instance.RegEvent(kv.Key,kv.Value);
            }
        }

        public void UnregistAll()
        {
            foreach (var kv in eventHandlers)
            {
                EventManager.instance.UnRegEvent(kv.Key,kv.Value);
            }
            eventHandlers.Clear();
        }
    }
    
    public class EventManager: BasicMonoSingle<EventManager>
    {
        struct TmpEventTrigger
        {
            public Action<object> action;
            public object param;

            public void Trigger()
            {
                action(param);
            }
        }
        private Dictionary<string, HashSet<Action<object>>> allActions = new Dictionary<string, HashSet<Action<object>>>();
        private List<TmpEventTrigger> tmpTriggerEventsNextFrame = new List<TmpEventTrigger>();

        private void Update()
        {
            lock (tmpTriggerEventsNextFrame)
            {
                foreach (var trigger in tmpTriggerEventsNextFrame)
                {
                    trigger.Trigger();
                }
                
                tmpTriggerEventsNextFrame.Clear();
            }
        }

        public void RegEvent(string ev, Action<object> action)
        {
            if (!allActions.TryGetValue(ev,out var actions))
            {
                actions = new HashSet<Action<object>>();
                allActions.Add(ev,actions);
            }

            actions.Add(action);
        }

        public void UnRegEvent(string ev, Action<object> action)
        {
            if (!allActions.TryGetValue(ev,out var actions))
            {
                actions = new HashSet<Action<object>>();
                allActions.Add(ev,actions);
            }

            actions.Remove(action);
        }
        
        //延迟到下一帧处理，这里没有对参数进行深拷贝
        public void DispatchEvent(string ev,object param = null)
        {
            if (allActions.TryGetValue(ev,out var actions))
            {
                foreach (var ac in actions)
                {
                    tmpTriggerEventsNextFrame.Add(new TmpEventTrigger() { action = ac, param = param });
                }
            }
        }
        
    }
}