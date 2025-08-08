using AlisaieMod.Characters.Survivors.Alisaie.Components;
using AlisaieMod.Survivors.Alisaie;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AlisaieMod.Characters.Survivors.Alisaie.UI
{
    public class AlisaieManaBarController : MonoBehaviour
    {
        public static GameObject manaBarPrefab;
        public static AlisaieManaBarController instance;
        private static HUD hud;

        // Fill images for white and black mana bars
        private Image whiteFillImage;
        private Image blackFillImage;

        // Text components for white and black mana percentages
        private TextMeshProUGUI whiteManaText;
        private TextMeshProUGUI blackManaText;

        // Crystal image and glow GameObject for balanced state
        private Image crystalImage;
        private GameObject crystalGlow;
        private Image manabarBackImage;

        // Colors for the crystal in normal and balanced states
        private readonly Color crystalColorNormal = Color.white;
        private readonly Color crystalColorInBalance = Color.red;

        private CharacterBody trackedBody;
        private AlisaieManaTracker manaTracker;

        public static void Init()
        {
            if (manaBarPrefab == null)
            {
                manaBarPrefab = AlisaieAssets.manaBar;
            }
            HookHUD();
        }

        private static void HookHUD()
        {
            On.RoR2.UI.HUD.Awake += HUD_Awake;
        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;
            if (manaTracker != null)
            {
                manaTracker.OnManaThresholdChanged -= ManaThresholdListener;
                manaTracker.OnManaChanged -= UpdateUI;
            }
        }

        private static void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, HUD self)
        {
            orig(self);
            hud = self;

            // Always create new instance for each HUD
            if (manaBarPrefab != null)
            {
                GameObject manaBarInstance = Instantiate(manaBarPrefab);
                manaBarInstance.transform.SetParent(hud.mainContainer.transform, false);

                RectTransform rect = manaBarInstance.GetComponent<RectTransform>();

                // Anchor to right, 30% up from bottom
                rect.anchorMin = new Vector2(1f, 0.3f);
                rect.anchorMax = new Vector2(1f, 0.3f);

                // Pivot on the right edge (so anchoredPosition moves it left and vertically relative to anchor)
                rect.pivot = new Vector2(0.9f, 0.5f);

                // Position slightly left from right edge, vertically centered on 20% anchor
                rect.anchoredPosition = new Vector2(-50f, 0f);

                instance = manaBarInstance.GetComponent<AlisaieManaBarController>();
                if (instance == null)
                {
                    instance = manaBarInstance.AddComponent<AlisaieManaBarController>();
                }

                instance.Setup();
            }
        }

        private void Setup()
        {
            // Find and cache references to UI elements, log warnings if missing

            whiteFillImage = FindImage("manaBar_white_fill");
            blackFillImage = FindImage("manaBar_black_fill");

            whiteManaText = FindTMP("whiteMana_text");
            blackManaText = FindTMP("blackMana_text");

            crystalImage = FindImage("manaBar_crystal");
            manabarBackImage = FindImage("manaBar_back");

            Transform glowTransform = transform.Find("manaBar_crystalGlow");
            crystalGlow = glowTransform != null ? glowTransform.gameObject : null;

            if (crystalGlow == null)
                Debug.LogWarning("[AlisaieManaBarController] manaBar_crystalGlow not found!");
            else
                crystalGlow.SetActive(false);

            // Find the local player's AlisaieManaTracker
            var body = LocalUserManager.GetFirstLocalUser()?.currentNetworkUser?.GetCurrentBody();
            trackedBody = body;
            if (body != null)
                manaTracker = body.GetComponent<AlisaieManaTracker>();
            if (manaTracker != null)
            {
                manaTracker.OnManaThresholdChanged += ManaThresholdListener;
                manaTracker.OnManaChanged += UpdateUI;
            }

            // Initialize UI to empty mana 
            SetMana(0, 0);
        }

        private Image FindImage(string name)
        {
            Transform t = transform.Find(name);
            if (t != null)
                return t.GetComponent<Image>();

            return null;
        }

        private TextMeshProUGUI FindTMP(string name)
        {
            TextMeshProUGUI[] allTMPs = GetComponentsInChildren<TextMeshProUGUI>(true);
            foreach (var tmp in allTMPs)
            {
                if (tmp.gameObject.name == name)
                    return tmp;
            }
            return null;
        }

        private void ManaThresholdListener(bool inBalance)
        {
            // Play sound when first reaching 50/50 balance (crossing threshold)
            if (inBalance)
            {
                AkSoundEngine.PostEvent(AlisaieStaticValues.Play5050Mana, gameObject);
            }
            UpdateUI();
        }

        // Sets both white and black mana values and updates the UI.
        public void SetMana(int whiteMana, int blackMana)
        {
            if (manaTracker != null)
                manaTracker.SetMana(whiteMana, blackMana);
            UpdateUI();
        }

        // Updates the fill bars, text, and crystal state.
        private void UpdateUI()
        {
            int currentWhiteMana = manaTracker != null ? manaTracker.WhiteMana : 0;
            int currentBlackMana = manaTracker != null ? manaTracker.BlackMana : 0;

            float whiteFillNormalized = currentWhiteMana / 100f;
            float blackFillNormalized = currentBlackMana / 100f;

            if (whiteFillImage != null) whiteFillImage.fillAmount = whiteFillNormalized;
            if (blackFillImage != null) blackFillImage.fillAmount = blackFillNormalized;

            if (whiteManaText != null)
            {
                whiteManaText.text = $"{currentWhiteMana}";
            }
            if (blackManaText != null)
            {
                blackManaText.text = $"{currentBlackMana}";
            }

            bool inBalance = manaTracker != null && manaTracker.InBalance;

            if (crystalImage != null)
                crystalImage.color = inBalance ? crystalColorInBalance : crystalColorNormal;
            if (manabarBackImage != null)
                manabarBackImage.color = inBalance ? Color.white : Color.black;
            if (crystalGlow != null)
                crystalGlow.SetActive(inBalance);
        }

        private void Update()
        {
            var body = LocalUserManager.GetFirstLocalUser()?.currentNetworkUser?.GetCurrentBody();
            bool isAlisaie = body != null && body.GetComponent<AlisaieManaTracker>() != null;
            gameObject.SetActive(isAlisaie);

            if (body != trackedBody)
            {
                // Unsubscribe from previous tracker 
                if (manaTracker != null)
                {
                    manaTracker.OnManaThresholdChanged -= ManaThresholdListener;
                    manaTracker.OnManaChanged -= UpdateUI;
                }
                trackedBody = body;
                manaTracker = trackedBody != null ? trackedBody.GetComponent<AlisaieManaTracker>() : null;
                if (manaTracker != null)
                {
                    manaTracker.OnManaThresholdChanged += ManaThresholdListener;
                    manaTracker.OnManaChanged += UpdateUI;
                }
                UpdateUI();
            }
        }
    }
}
