using UnityEngine;
using System.Collections.Generic;
public interface IDebugItem // Interface je potreben, saj v List<> ne mores dati generic types (T)
{
    string GetItemTitle();
    string ValueToString();
}

public class DebugItem<T> : IDebugItem
{

    private string ItemTitle;
    private T value;

    public DebugItem(string ItemTitle, T value)
    {
        this.ItemTitle = ItemTitle;
        this.value = value;
        DebugItemRegistry.Register(this);
    }

    public string GetItemTitle()
    {
        return this.ItemTitle;
    }

    public string ValueToString()
    {
        return value?.ToString() ?? "null";
    }

    public T GetValue()
    { 
        return this.value;
    }
}