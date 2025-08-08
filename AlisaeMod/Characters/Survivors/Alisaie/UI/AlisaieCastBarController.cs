using AlisaieMod.Survivors.Alisaie;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace AlisaieMod.Characters.Survivors.Alisaie.UI
{
    public class AlisaieCastBarController : MonoBehaviour
    {
        public static GameObject castBarPrefab;
        public static AlisaieCastBarController instance;
        private static HUD hud;

        private Image fillImage;

        public static void Init()
        {
            if (castBarPrefab == null)
            {
                castBarPrefab = AlisaieAssets.castBar;
            }
            HookHUD();
        }

        private static void HookHUD()
        {
            On.RoR2.UI.HUD.Awake += HUD_Awake;
        }

        private static void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, HUD self)
        {
            orig(self);
            hud = self;

            if (instance == null && castBarPrefab != null)
            {
                GameObject castBarInstance = Instantiate(castBarPrefab);
                castBarInstance.transform.SetParent(hud.mainContainer.transform, false);

                // Optionally adjust position here:
                RectTransform rect = castBarInstance.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0.5f, 0);
                rect.anchorMax = new Vector2(0.5f, 0);
                rect.anchoredPosition = new Vector2(0, 100);

                instance = castBarInstance.GetComponent<AlisaieCastBarController>();
                if (instance == null)
                {
                    instance = castBarInstance.AddComponent<AlisaieCastBarController>();
                }

                instance.Setup();
            }
        }

        private void Setup()
        {
            Transform fillTransform = transform.Find("castBar_fill");
            if (fillTransform != null)
            {
                fillImage = fillTransform.GetComponent<Image>();
            }

            SetFill(0f);
        }

        // Sets the fill amount of the cast bar between 0 and 1.
        public void SetFill(float fillAmount)
        {
            if (fillImage)
            {
                fillImage.fillAmount = Mathf.Clamp01(fillAmount);
            }
        }
    }
}
