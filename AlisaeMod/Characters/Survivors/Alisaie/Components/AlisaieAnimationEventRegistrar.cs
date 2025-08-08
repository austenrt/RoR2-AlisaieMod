using UnityEngine;
using RoR2;

namespace AlisaieMod.Characters.Survivors.Alisaie.Components
{
    // Component that registers the CharacterBody with the animation event handler
    // when the model is set up. This bridges the gap between the separate
    // CharacterBody and model GameObjects.
    public class AlisaieAnimationEventRegistrar : MonoBehaviour
    {
        private CharacterBody characterBody;
        private bool hasRegistered = false;

        private void Awake()
        {
            characterBody = GetComponent<CharacterBody>();
        }

        private void Start()
        {
            // Register when the component starts
            TryRegisterModel();
        }

        private void Update()
        {
            // Keep trying to register until successful (in case model loads later)
            if (!hasRegistered)
            {
                TryRegisterModel();
            }
        }

        private void TryRegisterModel()
        {
            if (!characterBody || hasRegistered) return;

            // Get the model locator to find the model GameObject
            var modelLocator = characterBody.modelLocator;
            if (modelLocator && modelLocator.modelTransform)
            {
                GameObject modelObject = modelLocator.modelTransform.gameObject;
                
                // Register this CharacterBody with the model GameObject
                AlisaieAnimationEventHandler.RegisterCharacterBody(modelObject, characterBody);
                hasRegistered = true;
            }
        }

        private void OnDestroy()
        {
            // Unregister when destroyed
            if (hasRegistered && characterBody && characterBody.modelLocator && characterBody.modelLocator.modelTransform)
            {
                GameObject modelObject = characterBody.modelLocator.modelTransform.gameObject;
                AlisaieAnimationEventHandler.UnregisterCharacterBody(modelObject);
            }
        }
    }
}