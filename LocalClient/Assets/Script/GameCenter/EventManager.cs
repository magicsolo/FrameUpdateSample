using System;
using System.Collections.Generic;
using CenterBase;

namespace Game
{
    public static class EventKeys
    {
        public static string LogicMatchUpdate = "LogicMatchUpdate";
    }
    public class EventManager: BasicMonoSingle<EventManager>
    {
        private Dictionary<string, HashSet<Action<object>>> allActions = new Dictionary<string, HashSet<Action<object>>>();

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
        
        public void DispatchEvent(string ev,object param)
        {
            if (allActions.TryGetValue(ev,out var actions))
            {
                foreach (var ac in actions)
                {
                    ac(param);
                }
            }
        }
        
    }
}