using flanne;
using flanne.Core;
using UnityEngine;
using HarmonyLib;
using static DarkerNights.DarkerNights;
using UnityEngine.UI;
using UnityEngine.UIElements.UIR;

namespace DarkerNights
{
    class VisionModifier : DifficultyModifier
    {
        private bool disableSecondaryVision;

        public VisionModifier Init(bool disableSecondaryVision, string name, string description = "An unspecified VisionModifier")
        {
            this.disableSecondaryVision = disableSecondaryVision;
            this.name = name;

            //TODO: Make this mess a library and add support for all languages currently this crashes for any language other than english
            LocalizationSystem.GetDictionaryForEditor().Add(name, description);
            this.descriptionStringID = name;

            return this;
        }

        public override void ModifyGame(GameController gameController)
        {
            if (disableSecondaryVision)
            {
                //this is not a good solution, it's permanent
                //disable outer vision ring
                GameObject playerFogRevealer = gameController.playerFogRevealer;
                GameObject playerVision = playerFogRevealer.transform.Find("PlayerVision").gameObject;
                GameObject outerVisionCircle = playerVision.transform.Find("FogRevealCircleBlue").gameObject;
                outerVisionCircle.transform.localScale = Vector3.zero;

                //this is not a good solution, it's permanent
                //disable muzzle flash
                ShootDetector shootDetector = gameController.shootDetector;
                Gun playerGun = (Gun)Traverse.Create(shootDetector).Field("playerGun").GetValue();
                foreach (Shooter shooter in playerGun.shooters)
                {
                    Traverse.Create(shooter).Field("muzzleFlashPrefab").SetValue(null);
                }

                foreach (ObjectPoolItem poolItem in ObjectPooler.SharedInstance.itemsToPool)
                {
                    if (poolItem.objectToPool.name == "PF_BulletImact")
                    {
                        Log.LogInfo("Patching BulletImpact...");
                        Destroy(poolItem.objectToPool.transform.Find("FogReveal").gameObject);
                        Log.LogInfo("de_Stroyed");

                        //TODO: Projectile OnCollisionEnter2D hook and debug logs
                    }
                }
            }
        }
    }
}