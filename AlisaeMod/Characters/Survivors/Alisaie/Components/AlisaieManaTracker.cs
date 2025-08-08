using UnityEngine;
using RoR2;
using System;
using AlisaieMod.Survivors.Alisaie;

namespace AlisaieMod.Characters.Survivors.Alisaie.Components
{
    public class AlisaieManaTracker : MonoBehaviour
    {
        public event Action<bool> OnManaThresholdChanged; // bool: true if threshold crossed, false if left
        public event Action OnManaChanged; // Fires on any mana change

        private int whiteMana;
        private int blackMana;
        private bool inBalance;

        public int WhiteMana => whiteMana;
        public int BlackMana => blackMana;
        public bool InBalance => inBalance;

        public void SetMana(int white, int black)
        {
            int clampedWhite = Mathf.Clamp(white, 0, 100);
            int clampedBlack = Mathf.Clamp(black, 0, 100);
            bool wasInBalance = inBalance;
            whiteMana = clampedWhite;
            blackMana = clampedBlack;
            inBalance = (whiteMana >= 50 && blackMana >= 50);
            if (inBalance != wasInBalance)
            {
                OnManaThresholdChanged?.Invoke(inBalance);
            }
            OnManaChanged?.Invoke();
        }

        public void AddWhiteMana(int amount)
        {
            if (GetComponent < CharacterBody>().HasBuff(AlisaieBuffs.accelerationBuff) )
            {
                amount *= 2; //double mana gain for acceleration buff
            }
            SetMana(whiteMana + amount, blackMana);
        }
        public void AddBlackMana(int amount)
        {
            if (GetComponent<CharacterBody>().HasBuff(AlisaieBuffs.accelerationBuff))
            {
                amount *= 2;
            }
            SetMana(whiteMana, blackMana + amount);
        }
        public void SubtractWhiteMana(int amount)
        {
            SetMana(whiteMana - amount, blackMana);
        }
        public void SubtractBlackMana(int amount)
        {
            SetMana(whiteMana, blackMana - amount);
        }

        public bool ShouldBeEnchanted()
        {
            return whiteMana >= 50 && blackMana >= 50;
        }
    }
}
