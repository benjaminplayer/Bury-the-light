using UnityEngine;
using System.IO;
using System;
using System.Collections;

public class Collectables : MonoBehaviour
{

    [SerializeField]
    private BoxCollider2D boxCollider;

    private String[] initialData = { "#Total Secrets Found", "f: 0", "", "#Secrets per Chapter:", "Ch1: 0", "Ch2: 0" };
    private void Awake()
    { 
        boxCollider = GetComponent<BoxCollider2D>();

        string path = Application.dataPath + "/Data/Secrets.txt";
        if (!File.Exists(path)) // preveri, ali je bila datoteka izbrisana
        {
            File.Create(path).Dispose();
            
            StreamWriter sw = new StreamWriter(path);
            
            foreach (string s in initialData)
            {
                sw.Write(s);
            }
            sw.Close();
            sw.Dispose();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            HandlePickup();
        }
    }

    private void WriteToFile(ArrayList data) 
    {
        string path = Application.dataPath + "/Data/Secrets.txt";

        StreamWriter sw = new StreamWriter(path);

        foreach (string s in data)
        { 
            sw.WriteLine(s);
        }

        sw.Close();
        sw.Dispose();
        
    }

    private ArrayList ReadFromFile(string path) 
    {
        ArrayList data = new ArrayList();
        if (!File.Exists(path))
        {
            throw new System.Exception("The file \"" +path+ "\" does not exist!");
        }

        StreamReader sr = new StreamReader(path);
        string line;

        while ((line = sr.ReadLine()) != null)
        { 
            data.Add(line);
        }

        sr.Close();
        sr.Dispose();

        return data;
    }

    private void HandlePickup() 
    {
        gameObject.SetActive(false);
        int collected;
        ArrayList data = ReadFromFile(Application.dataPath + "/Data/Secrets.txt");

        for(int i = 0; i < data.Count; i++)
        {
            string s = data[i].ToString();

            if (s.Contains("#")) continue;
            if (s.Contains("f:"))
            {
                Debug.Log(s);
                collected = Int32.Parse(s.Substring(s.IndexOf(':') + 2, 1));
                collected++;

                data[i] = "f: " + collected;
            }
        }

        WriteToFile(data);
    }

}
