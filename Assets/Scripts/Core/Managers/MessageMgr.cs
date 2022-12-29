using System;
using System.Collections.Generic;

public class MessageMgr : Singleton<MessageMgr>
{
    protected MessageMgr() {
        msgMap = new Dictionary<string,List<Action<object[]>>>();
    }

    private Dictionary<string, List<Action<object[]>>> msgMap;

    public void Add(string messageKey,Action<object[]> method,params object[] args)
    {
        var act = GetActions(messageKey, false);
        if (!act.Contains(method))
            act.Add(method);
    }

    public void Remove(string messageKey, Action<object[]> method)
    {
        var act = GetActions(messageKey);
        if (act != null && act.Contains(method))
        {
            act.Remove(method);
            if (act.Count == 0)
                msgMap.Remove(messageKey);
        }
    }

    public void Send(string messageKey, params object[] args)
    {
        var act = GetActions(messageKey);
        if (act!= null)
        {
            foreach (var method in act)
                method.Invoke(args);
        }
    }

    private List<Action<object[]>> GetActions(string messageKey,bool isOKtoNull = true)
    {
        List<Action<object[]>> actions = null;

        if (!msgMap.TryGetValue(messageKey, out actions) && !isOKtoNull)
            msgMap.Add(messageKey, new List<Action<object[]>>());

        return actions;
    }
}
