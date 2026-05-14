using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using TMPro;


public class AddressFieldManager : NetworkBehaviour
{
    private List<TMP_InputField> fields = new();
    private UnityTransport transport;

    private void Start()
    {
        transport = NetworkManager.GetComponent<UnityTransport>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).TryGetComponent(out TMP_InputField field))
            {
                fields.Add(field);
                field.GetComponent<AdressField>().Initialize();
            }
        }

        Load();
    }

    public void Paste()
    {
        string ip = GUIUtility.systemCopyBuffer;
        string[] parts = ip.Split('.');

        if (parts.Length > 4) return;

        for (int i = 0; i < 4; i++)
        {
            if (int.TryParse(parts[i], out _))
            {
                fields[i].text = parts[i];
            }
            else return;
        }
    }

    public void Next(TMP_InputField field)
    {
        int index = fields.IndexOf(field);

        if (index < fields.Count - 1)
        {
            fields[++index].Select();
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("IP1", fields[0].text);
        PlayerPrefs.SetString("IP2", fields[1].text);
        PlayerPrefs.SetString("IP3", fields[2].text);
        PlayerPrefs.SetString("IP4", fields[3].text);
        PlayerPrefs.SetString("Port", fields[4].text);
    }

    private void Load()
    {
        if (PlayerPrefs.HasKey("IP1"))
        {
            fields[0].text = PlayerPrefs.GetString("IP1");
            fields[1].text = PlayerPrefs.GetString("IP2");
            fields[2].text = PlayerPrefs.GetString("IP3");
            fields[3].text = PlayerPrefs.GetString("IP4");
            fields[4].text = PlayerPrefs.GetString("Port");

            ChangeIP();
            ChangePort();
        }
    }

    public void ChangeIP()
    {
        string ip = fields[0].text + "." + fields[1].text + "." + fields[2].text + "." + fields[3].text;
        transport.ConnectionData.Address = ip;
    }

    public void ChangePort()
    {
        transport.ConnectionData.Port = ushort.Parse(fields[4].text);
    }

    public void Block(bool value)
    {
        foreach (TMP_InputField field in fields)
        {
            field.interactable = !value;
        }
    }
}
