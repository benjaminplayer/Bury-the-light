using UnityEngine;
using System.IO;
using System;
using System.Collections;
using TMPro;

public class Collectables : MonoBehaviour, IDataPresistance
{
    [SerializeField] private string id;
    private bool collected = false;

    [ContextMenu("Generate GUID")]
    private void GenerateGUID()
    { 
        id = System.Guid.NewGuid().ToString(); // naredi nov GUID da lahko shrani katere collctables smo shranili
    }

    private void Awake()
    {

    }

    public void SaveData(ref GameData data)
    {

        if (data.fragmentsCollected.ContainsKey(id))
        {
            data.fragmentsCollected.Remove(id);
        }

        data.fragmentsCollected.Add(id,collected);

    }

    public void LoadData(GameData data) 
    {
        if (collected)
        { 
            this.gameObject.SetActive(false);
        }
    }

    public void SetCollected(bool b)
    {
        collected = b;        
    }

}
