using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tangrame : MonoBehaviour
{
    [SerializeField] private int[] picesToComplite;

    public bool IntegratePice(int idPice)
    {
        if (picesToComplite[idPice] > 0)
        {
            picesToComplite[idPice]--;
            IsComplite();
            return true;
        }

        return false;
    }

    public void IsComplite()
    {
        int nb = 0;

        foreach (int item in picesToComplite)
        {
            nb += item;
        }

        if (nb==0)
        {
            print("Yes");
            // complet
        }
    }
}
