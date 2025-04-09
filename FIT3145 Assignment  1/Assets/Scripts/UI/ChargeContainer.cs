using System;
using Guns;
using Player;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ChargeContainer : MonoBehaviour
    {
        private Material material;

        private void Awake()
        {
            material = GetComponent<Image>().material;
        }

        private void OnEnable()
        {
            PlayerController.onChangeColor += ChangeChargeColor;
        }
        
        private void OnDisable()
        {
            PlayerController.onChangeColor -= ChangeChargeColor;
        }

        private void ChangeChargeColor(Color newColor)
        {
            material.SetColor("_Color", newColor);
        }
    }
}
