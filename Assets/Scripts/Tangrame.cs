using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShapeFight
{
    public class Tangrame : MonoBehaviour
    {
        [SerializeField] private int[] picesToComplite;

        public bool IntegratePice(int idPice, int idPlayer)
        {
            if (picesToComplite[idPice] > 0)
            {
                picesToComplite[idPice]--;
                IsComplite(idPlayer);
                return true;
            }

            return false;
        }

        public void IsComplite(int idPlayer)
        {
            int nb = 0;

            foreach (int item in picesToComplite)
            {
                nb += item;
            }

            if (nb == 0)
            {
                print("Yes");
                GameManager.instance.AddPointsToPlayer(idPlayer);
                // complet
        }
        }
    }
}